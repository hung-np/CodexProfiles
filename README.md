# Codex Profiles

A small Windows desktop launcher for opening Codex with separate local profiles.

Create a named profile for each account or workspace, select one or more profiles, and launch them from a single window. Sign in through Codex itself; this launcher does not ask for your password or API key.

> **Compatibility notice:** Profile separation relies on Codex application behavior and is not an officially guaranteed multi-account feature. The initial build was compiled and its main window opened successfully. Independent account sessions still require manual verification on your installed Codex version.

## Features

- Create named profiles with separate local data directories.
- Select and launch multiple profiles.
- Save the profile list between sessions.
- Try to locate the Windows Store Codex installation automatically.
- Choose the Codex executable manually if automatic discovery fails.
- Open the launcher's data folder.
- Remove profiles from the list while retaining their files.

The current interface is in Vietnamese. This README is in English.

## Requirements

### To use a packaged release

- A Windows x64 computer compatible with .NET 10.
- The Codex desktop app installed separately.
- Your own accounts, with access to Codex.
- Internet access when signing in or using online Codex features.

The self-contained package includes the .NET runtime. You do not need Visual Studio, the .NET SDK, Node.js, Python, or Git to run that package.

### To build from source

- Windows.
- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0).
- Internet access to restore required .NET components.
- Optional: [GitHub Desktop](https://desktop.github.com/download/) to manage the repository.

The initial build used .NET SDK 10.0.403 and targeted Windows x64.

## Quick start

1. Download the Windows x64 ZIP from this repository's **Releases** section, when a release has been published.
2. Extract the ZIP into a folder you can keep, such as a folder under Documents.
3. Open the extracted folder in **Windows File Explorer**.
4. Double-click **CodexProfiles.exe**.
5. Enter a profile name and click **Thêm profile**.
6. Select the new profile and click **Mở profile đã chọn**.
7. Sign in in the Codex window that opens.
8. Repeat with another profile and verify that each window shows the intended account.

If you received a local project folder instead of a published release, the packaged application is under **App/**.

Opening a file link inside Codex may display a preview. Use File Explorer to run the executable.

## Using profiles

### Create a profile

Enter a unique, non-empty name and click **Thêm profile**. Examples:

- Personal
- Work
- Account 3

The name is a display label. Internally, each profile has a generated ID used for its data folder.

### Launch one or several profiles

Click a profile to select it. Hold **Ctrl** while clicking to select multiple profiles, then click **Mở profile đã chọn**.

Use **Alt + Tab** to switch between the Codex windows. The launcher sends launch requests; it does not track window state or confirm successful sign-in.

Before using a newly created profile, check the account shown in Codex. If it opens the existing account unexpectedly, do not sign out of that window until you have established whether the sessions are separate.

### Remove a profile

Select the profile and click **Xóa khỏi danh sách**, then confirm.

This removes the list entry only. Its local files are retained. Creating another profile with the same name generates a new ID and does not reconnect the old directory.

Automatic recovery of removed entries and permanent deletion of profile data are not implemented in the current version.

### Button reference

| Interface label | Meaning |
| --- | --- |
| Thêm profile | Add a profile |
| Mở profile đã chọn | Launch selected profiles |
| Mở thư mục dữ liệu | Open the local data folder |
| Xóa khỏi danh sách | Remove selected entries from the list |

## Locating Codex

The launcher first checks a previously saved executable path. If that path is unavailable, it looks for a Windows Store installation matching:

~~~text
C:\Program Files\WindowsApps\OpenAI.Codex_*\app\ChatGPT.exe
~~~

If discovery fails, a file picker asks you to select **ChatGPT.exe** from your Codex installation. Selecting a file saves its path for future launches. Canceling the picker cancels the launch.

The expected executable layout can change after a Codex update. Do not select the command-line backend executable in place of the desktop app.

## How profile separation works

For each launch, the application supplies:

| Setting | Purpose |
| --- | --- |
| CODEX_HOME | Separate Codex configuration and backend state |
| CODEX_ELECTRON_USER_DATA_PATH | Separate desktop application state |
| --user-data-dir | Separate browser-based application state |

A new profile's Codex configuration requests file-based credential storage. Existing configuration files are not overwritten.

These settings apply to the launched process. The launcher does not change global environment variables or copy credentials from your default Codex profile.

Separate profiles are intended to keep application state independent. They are not a security boundary against other programs running as the same Windows user.

## Local data and privacy

The launcher stores its data outside the repository:

~~~text
%LOCALAPPDATA%\CodexProfileManager\
├── profiles.json              Saved profile IDs and names
├── app-path.txt               Manually selected executable path, if any
└── profiles\
    └── <profile-id>\
        ├── codex-home\        Codex configuration and backend state
        └── app-data\          Desktop state, caches, and sign-in-related data
~~~

Paste the following into File Explorer's address bar to open this location:

~~~text
%LOCALAPPDATA%\CodexProfileManager
~~~

Profile folders can contain credentials, conversations, project paths, and other private information. Treat the entire data directory as sensitive.

- Never upload this directory to GitHub.
- Never share auth.json, cookies, tokens, or raw profile archives.
- Do not include private data in screenshots or issue reports.
- Do not rely on a private GitHub repository as a credential vault.
- Review changes before every commit; .gitignore is not a complete security control.

The launcher itself does not need a GitHub token or an OpenAI API key.

## Backup and moving to another PC

There is no built-in backup, export, import, or cloud sync in this version.

### Back up the current PC

1. Finish active Codex work.
2. Close all Codex profile windows and the launcher.
3. Copy the entire CodexProfileManager data directory to a trusted, encrypted backup location.
4. Keep the backup private.

Copying only the application folder does not back up your profiles.

A local backup may help recover the same environment, but restoring sign-in state is not guaranteed. Some state may depend on the Windows account, machine, or application version.

### Move to a new PC

The recommended approach is:

1. Install Codex on the new PC.
2. Download and extract the launcher package.
3. Create the profiles again.
4. Sign in separately to each profile.
5. Restore your projects and other files through their own backup process.

Do not assume copying authentication files will transfer a session. The launcher does not install Codex, transfer subscriptions, or back up external project folders.

## Build from source

Run the following commands in **PowerShell**, from the repository root.

Check the SDK:

~~~powershell
dotnet --list-sdks
~~~

Run the application during development:

~~~powershell
dotnet run --project .\src\CodexProfiles.csproj
~~~

Create a self-contained Windows x64 release:

~~~powershell
dotnet publish .\src\CodexProfiles.csproj -c Release -r win-x64 --self-contained true -o .\App -p:PublishSingleFile=true
~~~

The entry point is:

~~~text
App\CodexProfiles.exe
~~~

Distribute the contents of the entire output folder, including any accompanying files.

For a smaller framework-dependent build:

~~~powershell
dotnet publish .\src\CodexProfiles.csproj -c Release --self-contained false -o .\App-Framework
~~~

That build requires the matching **.NET 10 Desktop Runtime** on the destination machine.

Development and packaged builds currently share the same local data directory. Back up important profile data before testing changes.

## Project layout

~~~text
CodexProfiles/
├── README.md
├── .gitignore
└── src/
    ├── CodexProfiles.csproj
    ├── App.xaml
    ├── App.xaml.cs
    ├── MainWindow.xaml
    └── MainWindow.xaml.cs
~~~

Generated output folders such as App, App-Framework, bin, and obj are excluded from source control.

## Upload the project to GitHub

1. Open **GitHub Desktop**.
2. Use **File → Add local repository** and select the CodexProfiles project folder.
3. If the folder is not a Git repository, use the offered option to create a repository there.
4. Confirm the repository root contains README.md and src.
5. Review the changed files. Include source files, documentation, and .gitignore.
6. Confirm no credentials, profile directories, private data, or release archives are staged.
7. Create the initial commit.
8. Click **Publish repository** and choose the desired visibility.

The generated release ZIP is not excluded by the current .gitignore. Keep it out of commits manually, or add an exclusion before committing it accidentally.

### Publish a downloadable release

1. Build and test the self-contained application.
2. ZIP the contents of App.
3. Open the repository on GitHub.
4. Create a release with a version tag, for example v0.1.0.
5. Attach the ZIP and describe the changes and known limitations.
6. Publish the release.

Automatic updates, release automation, and code signing are not configured in this version. Choose a license before distributing the source for reuse; this project does not currently include a license file.

## Troubleshooting

| Problem | What to do |
| --- | --- |
| A file opens as text or a preview | Open the folder in Windows File Explorer and run CodexProfiles.exe there. |
| No Codex window opens | Select at least one profile and read the status message. Check the selected executable. |
| The executable picker appears | Automatic discovery failed. Select the desktop app's ChatGPT.exe. |
| Codex stopped opening after an update | Verify its new installation path. If a stale saved path still exists, close the launcher and rename app-path.txt so discovery runs again. |
| Two profiles show the same account | Stop account changes and verify isolation. Compatibility may have changed; do not repeatedly sign out of a potentially shared session. |
| A deleted profile's files still exist | This is intentional. Removing an entry retains its data directory. |
| The launcher reports that it cannot read the profile list | Close it, back up the data directory, and inspect or restore profiles.json. It disables the window to avoid overwriting unreadable data. |
| Build fails during restore | Check access to api.nuget.org, your network, and proxy settings. |
| Windows displays an unrecognized-app warning | Verify the source of the package. Builds are not currently code-signed; do not disable Windows protection globally. |

If you need to start a fresh profile list, close the launcher, back up the data directory, and rename profiles.json. This does not reconnect existing profile folders automatically.

## Verification status

Completed for the initial build:

- Release compilation and self-contained Windows x64 publishing.
- Application process started and displayed a window titled Codex Profiles.

Still requires manual testing:

- Profile persistence across restarts.
- Independent account sessions in two Codex windows.
- Multiple-profile launch and repeated launches of the same profile.
- Error handling, file selection, and removal behavior.
- Operation after Codex updates.
- Execution on another Windows PC.

## Scope and limitations

This application is a local profile launcher. It does not implement an account switcher inside Codex, rename profiles, monitor running profiles, manage account usage, or synchronize data between computers.

It is an independent project and is not affiliated with or endorsed by OpenAI.
