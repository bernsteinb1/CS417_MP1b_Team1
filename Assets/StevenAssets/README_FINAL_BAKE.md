# Steven Office Final Bake

This patch intentionally does **not** modify `ProjectSettings`, OpenXR settings, controller profiles, or VersionControlSettings.

## One-click finalization
1. Back up / commit your current branch.
2. Copy this patch into the same Unity project.
3. Open `StevenScene_LogicReady.unity` and wait for compilation to finish.
4. Stay **out of Play Mode**.
5. Run **Tools > Steven > Bake Final Office + Mainframe (Remove Bootstrap)**.
6. Unity saves the active scene, removes the `StevenOfficeBootstrap` component, then removes the old bootstrap/editor-generator source files and recompiles.
7. Save once more with `Ctrl+S` after recompilation.
8. Test in Quest Link.

## Resulting office flow
- Find/open the lower hidden bookshelf compartment and obtain the **blue ID card**.
- Hold the blue card near the **blue NFC-shaped card pad**. The card is not consumed/snapped; proximity authorizes the reader.
- A recessed secure media drawer eases outward and reveals the **orange USB drive**.
- Insert the orange drive into the **orange USB port**. This powers the mounted state terminal.
- Read the framed handwritten whiteboard and solve the six-bit state puzzle.
- Puzzle success slides the bookshelf and raises the bars, revealing the secret chamber and **green Override Token**.
- Insert the green token into the **green final panel**.
- All three locks complete -> office exit opens toward the classroom and `GameStateManager` marks `Room.Office` solved.

## Final puzzle whiteboard
The whiteboard is a real textured board, not a TMP rules panel. The source PNG is:
- `StevenAssets/Textures/StatePuzzleWhiteboard.png`

It explains every button with exact transformations and jokes. You can open/edit that PNG in Aseprite if you want different handwriting or wording.

## Mainframe endgame nook
A second room is built through a doorway in the office's right wall. It contains:
- heavy hinged metal `MAINFRAME // RESTRICTED` door
- multi-room authorization status panel
- handwritten room-completion checklist whiteboard
- mainframe racks + root console

`MainframeAccessController` automatically reads the team's existing `GameStateManager` every frame. The door opens only when these four states are solved:
- `Room.Office`
- `Room.Classroom`
- `Room.Bathroom`
- `Room.UtilityCloset`

No teammate-specific Steven wiring is required as long as each room marks its normal `GameStateManager` flag.

For local testing, use the component context command **DEBUG - Mark All Rooms Solved** on `MainframeNook > MainframeAccessController` while in Play Mode.

## Side content retained
- loss timer, restyled as neon red and pulsing below one minute
- 3 collectible circuit chips
- red-herring office props
- visible lock progress
- eased reveal motions

## Optional Blender replacements
`StevenAssets/Blender/generate_engineering_props.py` generates higher-detail low-poly versions of:
- keyboard
- framed whiteboard + marker tray
- terminal guard rail
- mainframe metal door
- mainframe racks
- cable bundle

The baked Unity primitives are already functional, so Blender is optional polish rather than a dependency.

## Do not commit accidental shared settings
If GitHub Desktop shows unrelated changes to `OpenXR Package Settings.asset`, `VersionControlSettings.asset`, or other XR/ProjectSettings files, discard those unless the team intentionally changed them.
