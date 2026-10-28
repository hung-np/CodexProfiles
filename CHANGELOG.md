# Changelog

## 0.5 — 2026-10-29
- Added a separate Stop selected button; stopping retains all profile data and credentials.
- Added per-profile process states, refreshed approximately every four seconds.
- Show unknown state when Windows process inspection fails instead of claiming stopped.
- Match exact --user-data-dir arguments and include backend descendants, without stopping default Codex.
- Recheck process identity and verify exit before reporting stop completion.
- Added Windows integration tests using a generated fake process, with no real account data.

## 0.4 — 2026-10-28
- Redesigned the Vietnamese interface with a navy header, profile cards, visible selection, shared button styles and an empty state.
- Added explicit permanent-deletion confirmation. Deletion removes local profile data including cached sign-in state, history and configuration.
- Block deletion when selected profiles are detected in running Codex processes or running processes cannot be inspected.
- Validate profile IDs and directory boundaries; refuse linked directories.
- Preserve unrelated profiles and the default Codex installation.
- Consolidated packaged output into App; removed obsolete build folders.
- Local deletion does not revoke remote sessions or securely erase recoverable disk sectors.

## 0.3
- Create profile directories immediately on Add.
- Show application version in the window title.

## 0.2
- Discover registered Codex packages through Get-AppxPackage.
- Retain manual executable selection as an explicit fallback.

## 0.1
- Initial WPF launcher, persisted profile list and multi-profile launching.
- Initial behavior retained data when removing list entries; replaced by permanent deletion in 0.4.


## 0.6 — 2026-10-29
- Refined cloud/navy/indigo palette and profile state badges.
- Updated header copy and hover treatment.
- Distribution uses the App directory; no ZIP is generated.
