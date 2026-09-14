# Cave Game - Unity Setup Guide

## Prerequisites
- Unity 6.3 LTS (or newer)
- Universal Render Pipeline (URP) configured *(enables 2D lighting effects)*
- Input System package installed *(modern input handling)*

## Layer Setup

**Why we need layers**: Unity uses layers to control which objects can interact with each other. Think of layers like "categories" - water objects only reflect light, obstacles absorb light, etc. This prevents unwanted interactions.

Before setting up game objects, create these layers in **Project Settings > Tags and Layers**:

1. **Layer 6**: "Water" - *Why: Tells our light system "this surface reflects light rays"*
2. **Layer 7**: "Obstacle" - *Why: Tells our light system "this surface absorbs/stops light rays"*  
3. **Layer 8**: "Player" - *Why: Separates hero from environment for proper collision detection*
4. **Layer 9**: "Ground" - *Why: Defines what surfaces the hero can jump from and walk on*
5. **Layer 10**: "Beacon" - *Why: Allows hero to detect nearby beacons for light control*

## Scene Setup Instructions

### 1. Create Hero Character

1. **Create Hero GameObject**:
   - Right-click in Hierarchy → Create Empty → Name it "Hero"
   - Position: (0, 0, 0) *Why: Center of our game world, starting point*

2. **Add Components to Hero** *(Each component adds specific functionality)*:

   **Rigidbody2D** *(Physics simulation - makes hero affected by gravity, forces)*:
   - Add Component → Rigidbody2D
     - **Linear Damping**: 0 *Why: No air resistance - keeps movement responsive*
     - **Angular Damping**: 0 *Why: No rotation resistance*  
     - **Freeze Rotation Z**: ✓ *Why: Prevents hero from spinning/rotating when hit by forces*

   **Box Collider 2D** *(Physical boundaries - defines hero's shape for collisions)*:
   - Add Component → Box Collider 2D
     - Size: (0.8, 1.6) *Why: Slightly smaller than sprite for better game feel - prevents getting stuck on edges*

   **Sprite Renderer** *(Visual representation - what player sees on screen)*:
   - Add Component → Sprite Renderer
     - Color: Blue (#0080FF) *Why: Temporary visual so we can see the hero before getting actual artwork*

   **HeroController** *(Game logic - movement, jumping, combat, beacon detection)*:
   - Add Component → **HeroController** (our script)
   - *Why: Contains all the code that makes the hero move, jump, attack, and detect nearby beacons*

3. **Configure Hero Settings**:
   - Set Layer to "Player" (Layer 8) *Why: Tells other systems "this is the player character"*
   - Add Tag "Player" (create if needed) *Why: Easy way to find player in code using GameObject.FindWithTag()*

### 2. Create Ground/Walls *(Physical boundaries that define the playable area)*

1. **Create Ground** *(Platform for hero to walk and jump from)*:
   - Right-click in Hierarchy → 2D Object → Sprites → Square
   - Name it "Ground" *Why: Clear naming helps organization*
   - Position: (0, -3, 0) *Why: Below hero's starting position so hero lands on it*
   - Scale: (10, 1, 1) *Why: Wide (10 units) so hero can move around, thin (1 unit) like a platform*
   - Color: Brown (#8B4513) *Why: Visual indicator - brown looks like dirt/ground*
   - Layer: "Ground" (Layer 9) *Why: Tells our ground detection system "this is something to stand on"*
   - Add Component → Box Collider 2D (should auto-add) *Why: Physical collision - stops hero from falling through*

2. **Create Walls** *(Boundaries to prevent hero from leaving play area)*:
   - Create → 2D Object → Sprites → Square
   - Name: "Wall_Left" / "Wall_Right" *Why: Clear naming for organization*
   - Position: (-5, 0, 0) / (5, 0, 0) *Why: Left and right edges of our play area*
   - Scale: (1, 6, 1) *Why: Tall (6 units) to block jumping, thin (1 unit) like walls*
   - Color: Gray (#808080) *Why: Visual indicator - gray looks like stone walls*
   - Layer: "Ground" (Layer 9) *Why: Same collision behavior as ground - solid barriers*

### 3. Create Water Objects *(Interactive environment that reflects light and affects hero movement)*

1. **Create Water**:
   - Right-click in Hierarchy → Create Empty → Name it "Water_01"
   - Position: (2, -2, 0) *Why: Positioned near ground level, offset from center for variety*

2. **Add Components to Water** *(Each component serves a specific purpose)*:

   **Sprite Renderer** *(Visual representation)*:
   - Add Component → Sprite Renderer
     - Create → 2D Object → Sprites → Square (or use existing)
     - Color: Blue with transparency (#0080FF80) *Why: Looks like water, transparency shows depth*
     - Sorting Order: -1 *Why: Renders behind hero but in front of background*

   **Box Collider 2D** *(Detection zone - NOT physical barrier)*:
   - Add Component → Box Collider 2D  
     - Is Trigger: ✓ *Why: CRITICAL - allows hero to enter water instead of bouncing off*
     - Size: (3, 1) *Why: Matches visual size of water sprite*

   **Water Script** *(Game logic for water behavior)*:
   - Add Component → **Water** (our script)
   - *Why: Handles hero sinking, light reflection calculations, splash effects*

3. **Configure Water**:
   - Set Layer to "Water" (Layer 6) *Why: Tells light system "reflect rays off this surface"*
   - Add Tag "Water" (create if needed) *Why: Easy identification in collision detection code*

### 4. Create Light-Absorbing Obstacles *(Strategic barriers that create lighting puzzles)*

1. **Create Obstacle**:
   - Right-click in Hierarchy → Create Empty → Name it "Obstacle_01" 
   - Position: (1, -1, 0) *Why: Positioned to block some light paths, creating puzzle opportunities*

2. **Add Components to Obstacle** *(Different behavior from water - blocks instead of reflects)*:

   **Sprite Renderer** *(Visual representation)*:
   - Add Component → Sprite Renderer
     - Create → 2D Object → Sprites → Square
     - Color: Black (#000000) *Why: Black absorbs light visually - matches game mechanic*
     - Sorting Order: 0 *Why: Visible layer, same as hero*

   **Box Collider 2D** *(Physical barrier - BLOCKS movement unlike water)*:
   - Add Component → Box Collider 2D
     - Is Trigger: ✗ (solid collider) *Why: CRITICAL - hero can't walk through, must go around*
     - Size: (1, 1) *Why: Standard unit size for easy level design*

   **LightAbsorbingObstacle Script** *(Game logic for light absorption)*:
   - Add Component → **LightAbsorbingObstacle** (our script)
   - *Why: Detects light ray collisions and completely stops them (no reflection)*

3. **Configure Obstacle**:
   - Set Layer to "Obstacle" (Layer 7) *Why: Tells light system "absorb/stop rays that hit this"*
   - Add Tag "Obstacle" (create if needed) *Why: Allows sword attacks to identify targets*

### 5. Create Beacon Objects *(Light sources that hero can control when nearby)*

1. **Create Beacon**:
   - Right-click in Hierarchy → Create Empty → Name it "Beacon_01"
   - Position: (-2, -1, 0) *Why: Strategic location for hero to find and use*

2. **Add Components to Beacon** *(Standalone light source with proximity control)*:

   **BeaconController Script** *(Light system - generates and controls light rays)*:
   - Add Component → **BeaconController** (our script)
   - *Why: Handles light ray casting, reflection calculations, and responds to hero input*

   **Circle Collider 2D** *(Detection zone for hero proximity)*:
   - Add Component → Circle Collider 2D (may auto-add from script)
     - Is Trigger: ✓ *Why: Allows hero to enter detection zone without collision*
     - Radius: 0.5 *Why: Small detection area around beacon object*

   **Sprite Renderer** *(Visual representation - created automatically by script)*:
   - *Why: BeaconController automatically creates a diamond-shaped beacon sprite*

3. **Configure Beacon**:
   - Set Layer to "Beacon" (Layer 10) *Why: Allows hero to detect this as a controllable beacon*
   - Add Tag "Beacon" (create if needed) *Why: Easy identification for proximity systems*

4. **Create Additional Beacons** *(Repeat for multiple beacon locations)*:
   - Duplicate Beacon_01 → Name them "Beacon_02", "Beacon_03", etc.
   - Position them strategically around your level *Why: Creates puzzle opportunities and light management*

### 6. Create Game Manager *(Central coordinator for all game systems)*

1. **Create GameManager**:
   - Right-click in Hierarchy → Create Empty → Name it "GameManager"
   - *Why: Invisible object that manages game state, debug info, and system coordination*
   - Add Component → **GameManager** (our script)
   - *Why: Validates setup, provides debug tools, coordinates between systems*

2. **Configure GameManager** *(Tell it where to find other systems)*:
   - Assign Hero reference in inspector *Why: GameManager needs to monitor hero's state*
   - Leave Beacon reference empty *Why: Hero now detects beacons automatically, no single beacon*
   - Configure layer masks *(Tell GameManager which layers do what)*:
     - Ground Layer: "Ground" *Why: Knows what surfaces are walkable*
     - Water Layer: "Water" *Why: Knows what surfaces reflect light*
     - Obstacle Layer: "Obstacle" *Why: Knows what surfaces absorb light*
     - Player Layer: "Player" *Why: Knows which object is the hero*

## Camera Setup *(Player's view into the game world)*

1. **Configure Main Camera**:
   - Position: (0, 0, -10) *Why: Z=-10 puts camera behind 2D objects (Z=0), centered on action*
   - Size: 8 (for 2D orthographic) *Why: Shows ~16 units of game world - good overview without being too zoomed out*
   - Background: Dark gray (#2F2F2F) *Why: Dark cave atmosphere, makes light rays more visible*

## Input Configuration *(How player controls translate to game actions)*

The game uses Unity's Input System. Ensure these inputs are configured:

- **Movement**: WASD or Arrow Keys → Horizontal/Vertical axes *Why: Standard game movement*
- **Jump**: Spacebar → "Jump" action *Why: Universal jump button*
- **Attack**: Spacebar → Custom handling in HeroController *Why: Same key as jump, context-dependent*
- **Beacon**: F key → Custom handling in BeaconController *Why: Easy to reach, memorable (F for "Fire/Flash")*
- **Counter-Clockwise**: G key → Custom handling in BeaconController *Why: Next to F key, easy combo*

## Testing the Setup *(Verify each game mechanic works correctly)*

### Controls:
- **WASD**: Move hero *Tests: Physics movement, collision with walls*
- **Spacebar**: Jump *Tests: Ground detection, gravity, jump force*
- **E Key**: Sword attack *Tests: Attack range, collision detection*
- **Move near beacon, then F**: Activate beacon (360° light) *Tests: Proximity detection, light ray creation*
- **Near beacon, F + Arrow Keys**: Directional light (clockwise) *Tests: Input relay to beacon*
- **Near beacon, G + Arrow Keys**: Directional light (counter-clockwise) *Tests: Alternative direction control*
- **F1**: Toggle debug information *Tests: GameManager functionality, system monitoring*

### Expected Behavior *(What each test proves about your game systems)*:

1. **Hero should move and jump with physics**
   - *Tests*: Rigidbody2D integration, collider setup, ground detection
   - *Why Important*: Core movement must feel responsive and predictable

2. **Hero should sink when entering water**  
   - *Tests*: Trigger colliders, water physics, movement penalties
   - *Why Important*: Water interaction creates strategic decisions

3. **Light rays should emit from beacon only when hero is nearby and presses F**
   - *Tests*: Proximity detection, input relay, line renderer creation from beacon position
   - *Why Important*: Creates strategic beacon placement gameplay

4. **Light rays should reflect off water surfaces**
   - *Tests*: Raycast collision, reflection calculations, multi-bounce system  
   - *Why Important*: Puzzle mechanics depend on accurate light physics

5. **Light rays should be absorbed (stop) when hitting obstacles**
   - *Tests*: Layer detection, ray termination, obstacle interaction
   - *Why Important*: Creates strategic light management challenges

6. **Hero should be able to attack obstacles with sword**
   - *Tests*: Combat system, obstacle destruction, attack range
   - *Why Important*: Provides player agency to modify environment

## Troubleshooting

### No Light Rays Visible:
- Check that BeaconController is attached to Hero
- Verify line renderers are being created (check children of Hero in scene)
- Ensure raycast layers are configured correctly

### Hero Not Moving:
- Verify Rigidbody2D is attached and not frozen
- Check that ground layer is configured for ground detection
- Ensure HeroController script is attached and enabled

### Water Not Working:
- Verify water collider is set as Trigger
- Check that Water script is attached
- Ensure proper layer and tag assignment

### Light Not Reflecting:
- Verify water objects are on correct layer (Water)
- Check BeaconController reflection settings
- Ensure water colliders are not set as triggers for physics

## Sample Scene Layout

```
Hero (0, 0) - Blue square with HeroController
├── Ground (-2 to 2, -3) - Brown rectangle
├── Water (2, -2) - Transparent blue rectangle  
├── Obstacle (1, -1) - Black square
├── Beacon_01 (-2, -1) - Diamond shape with BeaconController
├── Beacon_02 (3, 0) - Diamond shape with BeaconController
├── Wall_Left (-5, 0) - Gray rectangle
└── Wall_Right (5, 0) - Gray rectangle
```

This creates a simple test environment where you can:
- Move the hero around and find beacons
- Walk near beacons to activate light control
- Test beacon proximity detection
- Jump on platforms  
- Test water physics and light reflection from beacon position
- Test light absorption with obstacles
- Verify beacon control only works when hero is nearby

## Next Steps

Once basic functionality is verified:
1. Add more complex level layouts
2. Create prefabs for reusable objects
3. Add particle effects and audio
4. Implement more advanced lighting features
5. Create actual sprite artwork to replace placeholders