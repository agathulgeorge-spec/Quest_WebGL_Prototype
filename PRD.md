# Cave - Product Requirements Document (PRD)

## 1. Project Overview

**Game Title:** Cave  
**Version:** 0.0.1  
**Platform:** Unity 2D  
**Genre:** 2D Adventure/Puzzle  
**Development Status:** Pre-Production  

### Vision Statement
Cave is a 2D adventure game featuring light-based mechanics and environmental interactions. Players control a hero exploring cave systems using a magical beacon to navigate dark environments while dealing with water physics, reflection mechanics, and light-absorbing obstacles that create strategic lighting challenges.

---

## 2. Core Gameplay Features

### 2.1 Hero Character

#### Movement & Controls
- **Basic Movement**: WASD or Arrow Keys for horizontal movement
- **Jump Mechanic**: Spacebar or dedicated jump button
- **Combat System**: Integrated sword attack (sword is part of the sprite, not separate weapon)

#### Character Specifications
- 2D sprite-based character
- Responsive jump physics with gravity
- Attack animation with sword included in sprite design
- Collision detection for environment interaction

### 2.2 Beacon System

#### Fixed Beacon Locations
- **Beacon Placement**: Beacons are fixed objects placed strategically throughout levels
- **Proximity Control**: Hero can only activate beacons when within interaction range
- **Visual Feedback**: Beacons change color to indicate availability and activation state
- **Strategic Gameplay**: Players must find and reach beacons to control lighting

#### Light Ray Mechanics
- **Activation**: Press and hold `F` to emit light rays (only when near a beacon)
- **Directional Control**: 
  - Hold `F`: Emit light in 360-degree radius from beacon position
  - `F` + directional input: Focused light direction (clockwise) from beacon
  - `G` + directional input: Focused light direction (counter-clockwise) from beacon
- **Light Properties**:
  - Visible light rays extending from beacon position (not hero)
  - Dynamic lighting affects environment visibility
  - Rays interact with reflective surfaces

#### Light Reflection System
- Light rays reflect off water surfaces at appropriate angles
- Reflected rays can illuminate areas indirectly
- Multiple reflection bounces possible
- Ray intensity may decrease with each reflection

### 2.3 Water Physics

#### Surface Interaction
- **Light Reflection**: Water surfaces act as mirrors for light rays
- **Surface Tension**: Light rays do not pierce through water surface
- **Reflection Angle**: Follows realistic physics (angle of incidence = angle of reflection)

#### Hero-Water Interaction
- **Sinking Mechanic**: Hero gradually sinks when entering water
- **Movement Penalty**: Reduced movement speed while in water
- **Buoyancy**: Potential for floating/swimming mechanics (future consideration)

### 2.4 Light-Absorbing Obstacles

#### Obstacle Properties
- **Light Dissipation**: Any light ray (direct or reflected) that touches an obstacle is completely absorbed
- **No Reflection**: Obstacles do not reflect light rays like water surfaces
- **Total Absorption**: Light rays stop entirely upon contact, creating shadow zones
- **Strategic Placement**: Used to create lighting puzzles and block certain light paths

#### Interaction Mechanics
- **Ray Termination**: Light rays end immediately when hitting obstacle surface
- **Shadow Creation**: Obstacles cast shadows, blocking light from reaching areas behind them
- **Puzzle Elements**: Players must navigate light around obstacles to reach targets
- **Destructible Options**: Some obstacles may be destroyable with sword attacks (future consideration)

---

## 3. Technical Requirements

### 3.1 Unity Setup
- **Version**: Unity 2D project
- **Rendering Pipeline**: Universal Render Pipeline (URP) for 2D lighting
- **Physics**: Unity 2D Physics system for collision and water interaction

### 3.2 Placeholder Assets

#### Visual Representations (Temporary)
- **Hero**: Colored rectangle/capsule with basic animation frames
- **Beacon**: Circular/diamond shape that glows when activated
- **Water**: Blue rectangular areas with transparency
- **Cave Walls**: Gray/brown rectangles for boundaries
- **Light-Absorbing Obstacles**: Dark/black rectangular or circular shapes
- **Light Rays**: Line renderers or particle effects

#### Collision Systems
- **Hero**: Capsule Collider 2D with Rigidbody 2D
- **Water**: Trigger Colliders for entry detection
- **Walls**: Static Colliders for boundaries
- **Light-Absorbing Obstacles**: Static Colliders that stop light rays
- **Beacon**: Interaction trigger zone

---

## 4. Game Mechanics Specifications

### 4.1 Light System Implementation

#### Ray Casting
- Use Unity's Raycast2D for light ray calculations
- Calculate reflection angles using Vector2.Reflect()
- Implement ray length limitations for performance
- Visual representation through LineRenderer components
- **Light Dissipation**: Raycast detection for light-absorbing obstacles
- **Ray Termination**: Stop ray rendering when hitting obstacle colliders
- **Layer-Based Detection**: Use collision layers to differentiate between reflective (water) and absorbing (obstacles) surfaces

#### Lighting Effects
- Dynamic 2D lighting system using URP
- Light intensity falloff over distance
- Color temperature variations possible
- Shadow casting from solid objects

### 4.2 Physics Implementation

#### Jump Mechanics
- Configurable jump force and gravity
- Ground detection for jump availability
- Jump height variations based on input duration
- Landing animation states

#### Water Physics
- Trigger-based water entry detection
- Gradual sinking using Transform.Translate or Rigidbody velocity
- Buoyancy force calculations
- Splash effects (particle systems)

### 4.3 Combat System

#### Sword Attack
- Melee attack with limited range
- Attack direction based on hero facing
- Hit detection using OverlapCircle or trigger colliders
- Attack cooldown/timing mechanics
- Damage dealing to interactive objects

### 4.4 Light Absorption System

#### Obstacle Detection
- Layer-based collision detection to identify light-absorbing materials
- Immediate ray termination upon contact with obstacle colliders
- No light penetration or partial absorption - complete dissipation only
- Shadow zone calculation for areas behind obstacles

#### Implementation Details
- **Raycast Filtering**: Use LayerMask to separate reflective vs absorbing surfaces  
- **Ray Termination Logic**: Stop LineRenderer and light calculations at obstacle hit point
- **Performance Optimization**: Early termination prevents unnecessary reflection calculations
- **Visual Effects**: Optional absorption particle effects when light hits obstacles

---

## 5. Level Design Framework

### 5.1 Cave Environment

#### Layout Components
- **Entry/Exit Points**: Level progression markers
- **Water Bodies**: Various sizes and depths
- **Beacon Placement**: Strategic lighting puzzle locations
- **Light-Absorbing Obstacles**: Blocks and barriers that dissipate light rays
- **Hidden Areas**: Discoverable through light manipulation

#### Interactive Elements
- **Reflective Surfaces**: Mirrors, water, crystals
- **Light-Absorbing Obstacles**: Blocks that completely dissipate light rays
- **Physical Obstacles**: Barriers requiring light or combat to overcome
- **Switches/Mechanisms**: Activated by light beams
- **Collectibles**: Items revealed by proper lighting

### 5.2 Puzzle Design

#### Light-Based Puzzles
- Redirect light to activate switches
- Use multiple reflections to reach distant targets
- Navigate light around absorbing obstacles to reach targets
- Illuminate hidden passages or platforms
- Coordinate beacon positioning with movement
- Create complex light paths avoiding obstacle shadow zones

---

## 6. User Interface Requirements

### 6.1 Control Indicators
- Visual feedback for beacon activation state
- Light ray direction indicators
- Water depth/danger warnings
- Attack readiness indicators

### 6.2 HUD Elements
- Health/energy bars (if applicable)
- Beacon charge/cooldown timers
- Environmental interaction prompts
- Objective/quest tracking

---

## 7. Performance Considerations

### 7.1 Optimization Targets
- **Frame Rate**: Maintain 60 FPS on target platforms
- **Draw Calls**: Minimize through sprite batching
- **Light Calculations**: Optimize ray casting frequency
- **Particle Effects**: Limit simultaneous water/light effects

### 7.2 Scalability
- Configurable graphics quality settings
- Dynamic lighting complexity adjustment
- Object pooling for projectiles and effects
- LOD systems for distant objects

---

## 8. Development Phases

### Phase 1: Core Mechanics (Current)
- [ ] Hero movement and jump implementation
- [ ] Basic beacon light system
- [ ] Water collision and sinking
- [ ] Light-absorbing obstacle system
- [ ] Placeholder asset integration

### Phase 2: Advanced Systems
- [ ] Light reflection mechanics
- [ ] Combat system with sword
- [ ] Enhanced water physics
- [ ] Basic level creation tools

### Phase 3: Polish & Content
- [ ] Replace placeholder assets with final art
- [ ] Level design and puzzle creation
- [ ] UI/UX implementation
- [ ] Performance optimization

---

## 9. Success Metrics

### 9.1 Technical Milestones
- Stable 60 FPS with full lighting system active
- Accurate light reflection calculations
- Proper light dissipation on obstacle contact
- Responsive hero controls with smooth jumping
- Realistic water interaction feedback

### 9.2 Gameplay Goals
- Intuitive beacon control system
- Engaging light-based puzzle mechanics
- Satisfying combat with visual feedback
- Immersive cave exploration experience

---

## 10. Risks and Mitigation

### 10.1 Technical Risks
- **Performance Issues**: Complex lighting calculations may impact frame rate
  - *Mitigation*: Implement LOD and optimization early
- **Physics Complexity**: Water and reflection systems may conflict
  - *Mitigation*: Modular system design with clear separation

### 10.2 Design Risks
- **Control Complexity**: F/G beacon controls may be unintuitive
  - *Mitigation*: Extensive playtesting and control customization
- **Difficulty Curve**: Light puzzles may be too complex initially
  - *Mitigation*: Progressive tutorial integration

---

*Document Version: 1.2*  
*Last Updated: January 22, 2026*  
*Major Change: Beacon system updated to proximity-based control*  
*Next Review: Phase 1 Completion*