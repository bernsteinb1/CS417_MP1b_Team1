Steven rubric extras patch
==========================

Added only to StevenScene_LogicReady through a single StevenRubricExtras bootstrap component.
No XR/OpenXR/ProjectSettings files were modified.
No existing Steven scripts were edited.

Runtime-created features:
- Grabbable hand mirror with horizontally reversed RenderTexture view.
- Grabbable magnifying glass with narrow-FOV RenderTexture view.
- Grabbable blacklight/UV flashlight.
- Invisible-writing quad using Steven/BlacklightWriting; visible only inside the blacklight cone/range.
- Two-button keycard puzzle; IDCard stays hidden until 1 -> 2 is entered.
- Two-button USB puzzle parented to Drawer; USBDrive stays hidden until A -> B is entered.
- Existing six-bit state-machine puzzle remains unchanged for the OverrideToken.

Safety:
- The bootstrap runs only when the active scene name is exactly StevenScene_LogicReady.
- StevenScene_LogicReadySolved is untouched.
- Existing lock sockets, OfficeProgressManager, timer, inventory, XR rig, and ProjectSettings are untouched.

New files live under StevenAssets/Scripts, StevenAssets/Shaders, and StevenAssets/Resources.
