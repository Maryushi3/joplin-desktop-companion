using Microsoft.Data.Sqlite;
using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Documents;

namespace joplin_dc
{
    public partial class MainWindow : Window
    {
        FlowDocument contentDoc = new FlowDocument();
        string previousQueryResult = "";
        public MainWindow()
        {
            
            InitializeComponent();

            // watching db file for changes
            var fileSystemWatcher = new FileSystemWatcher(System.IO.Path.GetDirectoryName(ConfigurationManager.AppSettings.Get("dbPath")));
            fileSystemWatcher.Filter = (System.IO.Path.GetFileName(ConfigurationManager.AppSettings.Get("dbPath")));
            fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
            fileSystemWatcher.Changed += this.OnFileChanged;
            fileSystemWatcher.EnableRaisingEvents = true;
            // refresher
            Loaded += OnLoaded;

            // keeps window attached to top-left of main display
            LocationChanged += OnLocationChanged;
        }

        // keeps window attached to top-left of main display
        private void OnLocationChanged(object sender, EventArgs e)
        {
            this.Left = 0;
            this.Top = 0;
        }

        // loads notes on start, duh
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            ReadNotes();
        }

        // reloads notes when something changes
        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                ReadNotes();
            });
        }

        // reads sqlite db, and puts it in the text box, slightly "formatted"
        private void ReadNotes()
        {
            contentDoc.Blocks.Clear();
            ContentTextBox.Text = "";

            string connectionString = @"Data Source=" + ConfigurationManager.AppSettings.Get("dbPath") + ";";
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                // todo: add logic for extracting notebook ID by name (and config)
                string query = "SELECT * from notes where parent_id = '81f63dba745747f2ae31c4b240319db4' and todo_completed = '0' ORDER BY title ASC";
                SqliteCommand command = new SqliteCommand(query, conn);
                SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ContentTextBox.Text = ContentTextBox.Text + "# " + reader["title"].ToString() + "\n";
                    var run = new Bold(new Run("[ ] " + reader["title"].ToString()));
                    Paragraph noteTitle = new Paragraph(run);
                    noteTitle.FontSize = 16;
                    noteTitle.Margin = new Thickness(0,10,0,2);
                    contentDoc.Blocks.Add(noteTitle);
                    if (reader["body"].ToString().Length>0)
                    {
                        // todo: add support for at least some of markdown formatting options (italics, bold, strikethrough)
                        // todo: add nicer markers for markdown lists
                        // todo: unify spacing for tabs and spaces
                        ContentTextBox.Text = ContentTextBox.Text + reader["body"].ToString() + "\n\n";

                        Paragraph noteBody = new Paragraph(new Run(reader["body"].ToString()));
                        noteBody.FontSize = 11;
                        noteBody.Margin = new Thickness(0);
                        contentDoc.Blocks.Add(noteBody);
                    }
                    else
                    {
                        ContentTextBox.Text = ContentTextBox.Text + "\n";

                        Paragraph noteBody = new Paragraph(new Run(" "));
                        noteBody.FontSize = 11;
                        noteBody.Margin = new Thickness(0);
                        //contentDoc.Blocks.Add(noteBody);
                    }
                }

                reader.Close();
                ContentRTB.Document = contentDoc;
            }
        }

        // keeps window not-minimized (interferes with tray icon)
        private async void Window_StateChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Statee changed");
            await RestoreWindow(this);
        }

        public Task RestoreWindow(Window window)
        {
            return Task.Factory.StartNew(() =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    Thread.Sleep(100);
                    window.WindowState = System.Windows.WindowState.Normal;
                }));
            });
        }
    }


}
