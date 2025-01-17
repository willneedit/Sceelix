==================================
Unity Plugin Installation
Author:Sceelix
==================================

Installation from this Zip:
1. Open or create a new project in Unity.
2. Make sure that under Edit-> Project Settings -> Player -> Other Settings -> Optimization -> Api Compatibility Level is set to ".NET 2.0" (and not just subset).
3. Add the "Unity_Sceelix_Plugin" folder to the "Assets" folder (or in one of its subfolders) of your project.
4. Assuming no build errors occur, a new entry in the top menu, under "Tools", called "Sceelix", should appear. The plugin is now installed.

Installation via the Package Manager:
1. Open the Package Manager using Window -> Package Manager and use its menu for "Add Package from git URL.."
2. Use "https://github.com/willneedit/Unity_Sceelix_Plugin.git" as the git URL
3. Assuming no build errors occur, a new entry in the top menu, under "Tools", called "Sceelix", should appear. The plugin is now installed.

Connecting to the Sceelix Designer:
1. Click the Tools->Sceelix entry at the top menu. A window with several options should appear. You can move it on a corner, but it must remain open.
2. Check the "On" box to enable the connection.

From this moment on, whenever the Sceelix designer is running, the status should change to "CONNECTED". The plugin will automatically connect and disconnect
if the Sceelix Designer is on or off, respectively. It may interrupt the connection in some ocasions (either when it is compiling the project or engaging 
in play mode) but will attempt to reconnect afterwards.