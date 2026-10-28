# Design
Vietnamese Windows desktop utility for selecting isolated Codex profiles.
Direction: quiet blue workspace, with a navy masthead and individually outlined profile cards.
Segoe UI body 14px, names 17px, heading 27px; spacing 8/12/20/30.
Runtime owner: MainWindow.xaml Window.Resources (Ink #18314B, Accent #245EDD, Danger #A52A39). Shared button and list item templates own hover, selection, keyboard focus, and disabled states.
Native WPF multi-selection, keyboard navigation and scrollbars are intentional.
Destructive confirmation is owned by ConfirmDeletion; cancel receives initial focus.
Deletion removes only validated GUID profile folders, never default Codex data.
No automatic animation. Minimum window size 740x540.

Profile cards bind observable State; labels convey running/stopped/unknown without relying on color. Separate Stop action preserves credentials and confirms interrupted work. Polling interval 4 seconds; errors never imply stopped.

Version 0.6 palette: cloud #F3F5FA, navy #182D46, indigo #4159D1. Status badges: amber running/checking, slate stopped, rose unknown. State is always conveyed through text as well as color.
