# Codex Profiles

A small Windows launcher for separate local Codex profiles. The interface is in Vietnamese.

## Start
Install Codex separately. Extract the Windows x64 release ZIP and run CodexProfiles.exe from Windows File Explorer. The packaged app includes .NET; no SDK is needed to use it.

1. Enter a unique profile name and click **+ Thêm profile**.
2. Select a profile, or hold Ctrl to select several.
3. Click **Mở Codex đã chọn** and sign in through each Codex window.
4. Check the account shown in each window before working.

Profile isolation depends on Codex behavior and is not an officially guaranteed multi-account feature. Verify separate accounts on your installed version.

## Permanently delete a profile
Close the Codex windows belonging to the profiles you want to delete. Select those profiles and click **Xóa profile…**. Read the confirmation and choose **Xóa vĩnh viễn**.

Deletion removes the selected profile directory, including locally stored sign-in data, conversations and configuration. It cannot be undone. Your account itself, other profiles, default Codex data and external project folders are not deleted. Remote sessions are not revoked and disk sectors are not securely wiped.

The app refuses deletion if it detects a selected profile in a running Codex command line, cannot inspect running Codex processes, encounters linked directories, or cannot delete a locked file. Close the relevant Codex windows and retry. A file-system failure may leave a partially deleted profile; its entry remains available for retry. Do not launch that profile before completing deletion.

Profiles removed by older versions may still have orphaned folders. Version 0.4 does not automatically remove orphaned folders because their ownership and use cannot be confirmed from the current list.

## Codex discovery
The launcher queries the current user's OpenAI.Codex package using Windows PowerShell, then checks a previously saved executable path. Manual selection is offered only if discovery fails. Choose the desktop ChatGPT.exe, not the CLI backend. Do not change WindowsApps permissions.

## Local data and security
Data is stored outside the repository:
~~~text
%LOCALAPPDATA%\CodexProfileManager\
  profiles.json
  app-path.txt
  profiles\<generated-id>\
    codex-home\
    app-data\
~~~
Open it with **Thư mục dữ liệu**. These folders can contain access tokens, cookies and private history. Never commit or upload them. The launcher requires no password, API key or GitHub token.

Each launch sets CODEX_HOME, CODEX_ELECTRON_USER_DATA_PATH and --user-data-dir for that process. File-based credential storage is requested for new profiles. This is local separation, not a security boundary from other software running as the same Windows user.

## Backup and move to another PC
There is no built-in export, sync or automatic backup.
Close the launcher and Codex before making a private encrypted backup of the data directory. Do not store it in GitHub.
On a new PC, install Codex, extract the launcher, recreate profiles and sign in again. Copying tokens or machine-bound state is not a reliable migration method. Back up external projects separately.

## Build
Install the [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) on Windows.
Run PowerShell from the repository root:
~~~powershell
dotnet run --project .\src\CodexProfiles.csproj
dotnet publish .\src\CodexProfiles.csproj -c Release -r win-x64 --self-contained true -o .\App -p:PublishSingleFile=true
~~~
Run App\CodexProfiles.exe. ZIP the complete output directory for distribution. Internet is needed for initial package restore.
Development and release builds share local profile data; back up before testing.

Storage safety tests:
~~~powershell
dotnet run --project .\tests\StorageTests.csproj
~~~

## Repository and releases
- src/: WPF application and validated profile-directory deletion.
- tests/: dependency-free storage safety checks using generated fake data.
- DESIGN.md: visual and interaction conventions.
- CHANGELOG.md: consolidated version history.
- App/: generated application, excluded from Git.
- .gitignore: excludes builds, archives and local files.

Use GitHub Desktop to add this project directory, review changed files, commit and publish. Never select the LocalAppData data directory as a repository.
Create a GitHub Release and attach a ZIP of App. No GitHub CLI is required.
No license or signing certificate is included; select a license before allowing public reuse.
Automatic updates are not implemented.

## Troubleshooting
| Issue | Action |
| --- | --- |
| File opens as a preview | Run the EXE from Windows File Explorer. |
| Cannot find Codex | Confirm it opens from Start under the same Windows user. Check the installed desktop executable if prompted. |
| Same account appears in two profiles | Stop account changes and verify profile isolation before signing out. |
| Delete is blocked | Close selected Codex sessions. Do not force-delete live profiles. |
| Profile list is unreadable | Back up the data folder and restore a valid profiles.json; the app disables edits to avoid overwriting it. |
| Restore/build fails | Check access to api.nuget.org and your proxy. |
| Windows warns about the EXE | Verify the release source. Do not disable Windows protection globally; builds are unsigned. |

## Verification
Release build and storage safety tests have been run. Full account isolation, live-session deletion checks, keyboard/accessibility behavior and operation on another PC require end-to-end verification.
See CHANGELOG.md for version history.

Independent project; not affiliated with or endorsed by OpenAI.

## Stop profiles and check status (v0.5)
Select profiles and click **Dừng đã chọn**. Confirm that running work may be interrupted. Stopping terminates only the matched profile processes and their identified descendants; it retains sign-in data. It does not delete the profile.
Each card refreshes approximately every four seconds:
- **Đang chạy**: matched profile processes were detected.
- **Đã dừng**: no matching process was detected in the latest successful scan.
- **Không kiểm tra được**: Windows process inspection failed; the app cannot confirm the profile stopped.
Wait for **Đã dừng** before using the separate Delete button. Default Codex sessions are excluded by exact profile data-directory matching.
A process scan is a snapshot; a profile opened externally can change state between refreshes.
Integration test (creates and stops only a fake test process):
~~~powershell
dotnet run --project .\tests\StorageTests.csproj -- --process-integration
~~~

Version 0.6 distribution: the current build is provided in App/. No release ZIP is generated automatically; archive the folder yourself if needed.
