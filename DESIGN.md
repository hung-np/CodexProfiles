# Design

Current baseline: Codex Profiles v1.01, a compact Windows profile manager built with C# and WPF on .NET 10. English and Vietnamese are supported; English is the first-run default.

## Visual direction and tokens

Preserve the restrained pearl / petrol / champagne direction, fine outlines and warm selection surfaces. Use no decorative photography or external fonts.
Runtime token owner: `src/MainWindow.xaml`, including `Window.Resources` and shared control templates.

| Role | Current value |
| --- | --- |
| Pearl canvas | #F3F3F0 |
| Ink / deep petrol | #17262B |
| Accent | #20393E |
| Muted text | #5A6972 |
| Lines | #DADDD8 |
| Champagne focus / selection | #927440 |
| Warm selected surface | #F1EDE1 |
| Danger | #A52A39 |

Segoe UI owns interface text: header 22, body 14, profile names 17, badges 12. `InitialConverter` derives avatar initials from the first Unicode text element of the user-supplied name.

## Layout and controls

The profile list owns the remaining star-sized height and remains the dominant surface. Keep the header, one-line creation form, status area and action bar compact. Default window size is 940 × 760; minimum size is 820 × 620.

`BtnBase` owns layered surfaces, inset highlights, hover, pressed, keyboard focus and disabled states. Shared variants are `PrimaryButton` (petrol gradient/champagne text), `SoftButton` (outline), `WarnButton` (champagne) and `DangerButton` (rose outline). Minimum button height is 44 pixels; state changes must not shift layout.

`LanguageSelector` and `LanguageItem` own the WPF ComboBox surface and popup: 44-pixel minimum height, 8-pixel surface radius, petrol gradient, champagne text and chevron, pearl popup and selected checkmark. Preserve WPF selection and keyboard behavior. Keep native WPF list selection and scrollbars.

## State and interaction

`StatusKind` is derived from the internal status and excluded from persistence. State text remains visible alongside color: stopped=slate, running=green, checking/stopping=amber, unknown=rose. A badge reflects the latest process snapshot; it does not guarantee the current state. Refresh approximately every four seconds and show unknown when inspection fails.

There is no default Add button: Enter must not accidentally create profiles while navigating the list. Stop and Delete have separate confirmations, initially focused on the cancel/keep action. Stop retains sign-in data. Delete removes local profile data and must remain blocked when processes are running or inspection fails. Preserve process identity checks, exact path matching and directory validation safeguards.

## Localization

`AppLanguage.English` owns paired messages; dynamic WPF resources refresh static UI. `language.txt` stores `en` or `vi`. Status display is translated independently of status classification. New user-facing content must support both languages. Never translate user-supplied profile names. Native file-picker controls and operating-system error details may follow the Windows display language.

## Delivery and verification

Keep one current self-contained Windows x64 build in App, excluded from Git. Do not generate ZIPs or keep multiple old build folders. Close the launcher before replacing its files; leave users' Codex sessions running.

Build and existing storage/process/translation tests provide partial verification. Full UI flows, keyboard interaction, popup behavior, independent accounts and operation on other computers still require end-to-end verification. Keep version history exclusively in CHANGELOG.md.
