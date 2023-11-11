# joplin-desktop-companion

WPF app made to display my todo Joplin notepad on desktop.

Displays just text with no background based on todo notes and their contents from a pre-defined notebook. Updates automatically on note edits.

Should keep itself attached to the top-left corner of the main display. Should also stay more or less at the desktop level and survive "show desktop command".

Yep, this is quite a makeshift project, with lots of todo comments left in comments but it served me well for the time I've been using it (during last couple of semesters of collage). There won't be any updates, so feel free do do whatever with it.

## Configuration 

1. In `App.config`'s `appSettings` section there is a `dbPath` - make sure it's set to wherever your Joplin database.sqlite lies.
2. The notebook from which the todo notes are taken is hardcoded by ID in `ReadNotes()` function in `MainWindow.xaml.cs`
   ```c#
    string query = "SELECT * from notes where parent_id = '81f63dba745747f2ae31c4b240319db4' and todo_completed = '0' ORDER BY title ASC";
    ```
    You have to change that to the ID of the notebook you want the app to use. Since you will be compiling this project anyway I assume you will be able to figure this out ^^
3. Compile the thing.