Rubric Extras v3

Based on rubric extras v2. Adds two rubric-focused scoreboards without changing XR/OpenXR settings or teammate scenes.

Run once in Unity with StevenScene_LogicReady open and NOT in Play Mode:
Tools > Steven > Bake Rubric Extras v3 + Scoreboards (One Time)

The bake recreates StevenRubricExtras and includes:
- two grabbable hand mirrors (mirror camera direction fixed)
- magnifying glass
- blacklight + ceiling writing DONT LOOK DOWN
- blue keycard math panel
- orange USB CS panel
- Progress Scoreboard: live Keys Discovered/Remaining and Locks Solved/Unsolved
- Puzzle Scoreboard: explicitly lists one clue for each of the three puzzles and total 3 clues

Progress logic:
- A key is counted as discovered when its releasing puzzle is solved.
- A lock is counted solved from OfficeProgressManager's three lock booleans.

Nothing is regenerated at Play Mode startup. The scoreboard has a small runtime component only to update the text as progression changes.
