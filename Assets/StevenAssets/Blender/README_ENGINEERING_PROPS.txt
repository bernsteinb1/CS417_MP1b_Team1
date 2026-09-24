Run generate_engineering_props.py in Blender's Scripting workspace.
It creates higher-detail low-poly versions of:
- keyboard
- framed whiteboard + marker tray
- terminal guard rail
- metal mainframe restricted door
- mainframe racks
- cable bundle

Unity scene baking does NOT require these models; the one-click Unity baker creates functional primitive equivalents immediately. The Blender objects are optional visual replacements once gameplay is stable.

Recommended FBX export:
- Selected Objects
- Apply Transform
- Forward: -Z Forward
- Up: Y Up
- Apply Scalings: FBX Units Scale
