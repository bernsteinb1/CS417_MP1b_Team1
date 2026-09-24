"""
Blender 4.x script: generates higher-detail low-poly engineering props for Steven's office.
Run in Blender Scripting workspace, then File > Export > FBX (Selected Objects).
Objects are separate so Unity can keep interaction/collision logic on scene GameObjects.
"""
import bpy, math
from mathutils import Vector

# Reset
bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)

# ---------- helpers ----------
def mat(name, color, metallic=0.0, roughness=0.5):
    m=bpy.data.materials.new(name)
    m.diffuse_color=(*color,1)
    m.metallic=metallic
    m.roughness=roughness
    return m

def cube(name, loc, scale, material, bevel=0.0):
    bpy.ops.mesh.primitive_cube_add(location=loc)
    o=bpy.context.object; o.name=name; o.scale=(scale[0]/2,scale[1]/2,scale[2]/2)
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    if bevel:
        mod=o.modifiers.new('SoftEdges','BEVEL'); mod.width=bevel; mod.segments=2
    o.data.materials.append(material)
    return o

def cyl(name, loc, radius, depth, material, rot=(0,0,0), verts=16):
    bpy.ops.mesh.primitive_cylinder_add(vertices=verts, radius=radius, depth=depth, location=loc, rotation=rot)
    o=bpy.context.object; o.name=name; o.data.materials.append(material); return o

def beam(name, a, b, radius, material):
    a,b=Vector(a),Vector(b); d=b-a; mid=(a+b)/2
    o=cyl(name,mid,radius,d.length,material)
    o.rotation_mode='QUATERNION'; o.rotation_quaternion=d.to_track_quat('Z','Y')
    return o

metal=mat('Metal',(0.16,0.18,0.21),0.75,0.28)
dark=mat('DarkPlastic',(0.025,0.03,0.04),0.10,0.38)
white=mat('Whiteboard',(0.92,0.93,0.89),0.0,0.33)
red=mat('WarningRed',(0.65,0.025,0.03),0.2,0.3)
blue=mat('AuthBlue',(0.04,0.18,0.70),0.1,0.25)
orange=mat('USBOrange',(0.95,0.22,0.025),0.1,0.25)

# ---------- keyboard ----------
kb=cube('Keyboard_Base',(0,0,0.03),(0.72,0.25,0.045),dark,0.012)
for r in range(4):
    for c in range(12):
        x=-0.31+c*0.056; y=-0.075+r*0.050
        cube(f'Key_{r}_{c}',(x,y,0.065),(0.047,0.038,0.018),metal,0.004)
cube('Spacebar',(0,0.105,0.065),(0.30,0.040,0.018),metal,0.004)

# ---------- framed whiteboard ----------
origin=Vector((0,0.7,0))
cube('Whiteboard_Surface',origin,(1.35,0.045,1.05),white,0.01)
cube('Whiteboard_Frame_T',(0,0.69,0.55),(1.48,0.07,0.07),metal,0.01)
cube('Whiteboard_Frame_B',(0,0.69,-0.55),(1.48,0.07,0.07),metal,0.01)
cube('Whiteboard_Frame_L',(-0.71,0.69,0),(0.07,0.07,1.10),metal,0.01)
cube('Whiteboard_Frame_R',(0.71,0.69,0),(0.07,0.07,1.10),metal,0.01)
cube('MarkerTray',(0,0.62,-0.58),(0.90,0.16,0.05),metal,0.008)

# ---------- terminal guard rail ----------
beam('Guard_Top',(-0.79,1.7,0.6),(0.79,1.7,0.6),0.035,metal)
beam('Guard_Bottom',(-0.79,1.7,-0.6),(0.79,1.7,-0.6),0.035,metal)
beam('Guard_Left',(-0.79,1.7,-0.6),(-0.79,1.7,0.6),0.035,metal)
beam('Guard_Right',(0.79,1.7,-0.6),(0.79,1.7,0.6),0.035,metal)

# ---------- mainframe door ----------
door=cube('Mainframe_Door',(2.1,0,1.12),(0.11,1.16,2.25),dark,0.015)
cube('Door_Warning_Stripe_T',(2.035,0,1.62),(0.015,0.92,0.07),red)
cube('Door_Warning_Stripe_B',(2.035,0,0.62),(0.015,0.92,0.07),red)
cyl('Door_Handle',(2.00,-0.38,1.10),0.035,0.28,metal,rot=(0,math.radians(90),0))

# ---------- mainframe rack ----------
for r in range(3):
    y=1.1+r*0.73
    cube(f'Rack_{r}',(3.2,y,1.15),(0.70,0.60,2.25),dark,0.02)
    for i in range(6):
        cube(f'Rack_{r}_LED_{i}',(2.84,y-0.24,1.82-i*0.22),(0.018,0.07,0.035),red if i%2==0 else blue,0.002)

# ---------- cable bundle ----------
pts=[(-1.0,-0.5,0.1),(-0.6,-0.2,0.1),(-0.2,0.0,0.1),(0.2,0.1,0.2)]
for i in range(len(pts)-1): beam(f'Cable_{i}',pts[i],pts[i+1],0.012,dark)

# Flat shading / names suitable for Unity import
for o in bpy.context.scene.objects:
    if o.type=='MESH':
        for p in o.data.polygons: p.use_smooth=False

print('Generated: keyboard, framed whiteboard, terminal guard rail, mainframe door/racks, cable bundle.')
print('Export selected groups as FBX with Forward=-Z, Up=Y, Apply Transform enabled.')
