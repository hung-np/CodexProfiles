# Design
Vietnamese WPF profile launcher. Version 0.7 builds on the user's supplied XAML.
Direction: restrained luxury through ink, pearl, champagne details and generous spacing; no decorative photography or external fonts.
Runtime token owner: MainWindow.xaml Window.Resources.
Ink #182831; primary #223E4B; canvas #F1F2F4; muted #5A6972; lines #DDE2E4; champagne focus/selection detail #A58B59.
Segoe UI for all interface text; headings 26–28, body 14, profile names 17, badges 12.
User-supplied InitialConverter owns Unicode grapheme initials. Native WPF list selection and scrollbars retained.
Shared BtnBase owns hover, pressed, focus and disabled states. Semantic primary/soft/warn/danger variants are intentional.
StatusKind is derived from current status and excluded from persistence; state text remains visible alongside color.
Stopped=slate, running=green, checking/stopping=amber, unknown=rose. A green badge alone never proves a state.
No default Add button: Enter must not accidentally create profiles while navigating the list.
Profile deletion/stop behavior unchanged; cancel-first deletion confirmation retained.
No ZIP generation. App directory is the packaged deliverable.

Version 0.8: profile list owns remaining star-sized height. Compact header without measured decorative shapes, one-line creation form, reduced footer and card spacing. Default height 760.

Version 0.9: understated premium workspace. Pearl #F3F3F0, deep petrol #17262B, champagne #927440. Fine card outlines, restrained corners, warm selection surface and contained gold header rail. Keep profile list as dominant surface.

Version 0.10 button architecture: shared BtnBase with layered surface/inset highlight/hover/focus ring; PrimaryButton petrol gradient and champagne text; SoftButton outlined; WarnButton champagne; DangerButton rose outline. 44px minimum height, stable layout across states.

Version 1.00 localization: AppLanguage.English owns paired messages; dynamic WPF resources refresh static UI. English is the first-run default; language.txt stores en/vi. Header has a native language ComboBox; status display is translated independently of status classification. Profile names are never translated.

LanguageSelector and LanguageItem own the authored WPF ComboBox surface and popup. Use 44px height, 8px radius, petrol gradient/champagne text, pearl popup and selected checkmark. Native ComboBox selection/keyboard behavior remains.
