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

## 0.7 — 2026-10-29
- Integrated user-supplied XAML and Unicode initial avatars.
- Refined ink/pearl/champagne palette, profile selection and shared controls.
- Added derived status categories for badge styling; removed global default Add action.

## 0.8 — 2026-10-29
- Prioritized profile list height, compacted header and inline creation form.
- Removed decorative header shapes that could inflate layout measurements.
- Reduced footer spacing and card padding; increased default window height.

## 0.9 — 2026-10-29
- Refined pearl/petrol/champagne visual system, fine selection outlines and profile avatars.
- Retained compact header and profile-first layout. No functional changes.

## 0.10 — 2026-10-29
- Rebuilt button surfaces with layered borders, inset highlights, subtle depth and visible keyboard rings.
- Distinct primary launch, outlined add/folder, champagne stop and restrained danger actions.
- Added pressed/hover/disabled treatments without shifting layout.

## 1.00 — 2026-10-29
- Started the 1.00 stable version series; binary assembly version 1.0.0.0.
- Added English/Vietnamese language selection, English by default, saved per Windows user.
- Localized controls, statuses, confirmation dialogs, accessible labels and application errors.
- Added translation tests and preserved existing profile data.

## 1.01 — 2026-10-29
- Restyled language selector and dropdown to match primary controls: petrol surface, champagne border and chevron, inset highlight, selected checkmark and keyboard focus ring.
