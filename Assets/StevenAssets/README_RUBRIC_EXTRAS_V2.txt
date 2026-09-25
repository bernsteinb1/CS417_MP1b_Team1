STEVEN RUBRIC EXTRAS V2
=======================

This version does NOT use a runtime extras bootstrap.

One-time bake:
1. Open Scenes/StevenScene_LogicReady.unity.
2. Make sure Unity is NOT in Play Mode.
3. Run: Tools > Steven > Bake Rubric Extras v2 (One Time)
4. Save/commit the scene and StevenAssets changes.
5. Test the complete progression in Play Mode / Quest.

Baked features:
- Two grabbable hand mirrors with different handle colors.
- One grabbable magnifying glass.
- One grabbable blacklight.
- Invisible ceiling writing: DONT LOOK DOWN.
- Blue left-wall math puzzle: 2 + 2 = ?; correct answer opens the EXISTING hidden keycard compartment using its existing EasedMover.
- Orange left-wall CS puzzle: FIFO = QUEUE. It refuses to operate until the blue keycard has been swiped. After card authorization, the correct answer opens the EXISTING USB drawer using its existing EasedMover.

Progression remains:
Math panel -> compartment opens -> take ID card -> swipe ID card -> CS panel unlocks -> solve CS panel -> drawer opens -> take USB -> insert USB -> existing 6-bit puzzle -> override token -> door.

Important:
- IDCard remains in the hidden compartment at game start.
- USBDrive remains in the drawer at game start.
- The CardTrigger remains the real IDCard lock and still updates OfficeProgressManager.
- CardTrigger no longer directly opens the drawer; it authorizes the orange panel instead.
- The old compartment handle interaction is disabled so it cannot bypass the math puzzle, but the existing compartment EasedMover itself is unchanged.
- No OpenXR / ProjectSettings / teammate scenes are modified by this patch.
