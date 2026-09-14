# Cave - 2D Unity Game

A 2D adventure game featuring dynamic light mechanics, water physics, and environmental interactions.

## 🎮 Game Overview

Cave is an innovative 2D game where players control a hero character equipped with a magical beacon that casts light rays. The game features:

- **Hero Character**: Movement, jumping, and sword combat
- **Dynamic Lighting**: 360° and directional light emission with realistic physics
- **Water Physics**: Reflective surfaces and hero sinking mechanics  
- **Light-Absorbing Obstacles**: Strategic barriers that dissipate light rays
- **Environmental Puzzles**: Navigate using light reflection and absorption

## 🚀 Quick Start

1. **Setup Unity Scene**: Follow the [SETUP_GUIDE.md](SETUP_GUIDE.md) for complete scene configuration
2. **Configure Layers**: Create required layers (Water, Obstacle, Player, Ground)
3. **Add Game Objects**: Set up Hero, Water, Obstacles, and GameManager
4. **Test Controls**: Use WASD, Spacebar, F, and G keys to test functionality

## 🎯 Controls

| Input | Action |
|-------|--------|
| **WASD** | Move hero |
| **Spacebar** | Jump |
| **E Key** | Sword Attack |
| **F** | Activate beacon (360° light) |
| **F + Arrows** | Directional light (clockwise) |
| **G + Arrows** | Directional light (counter-clockwise) |
| **F1** | Toggle debug info |

## 📁 Project Structure

```
Assets/
├── Scripts/
│   ├── Player/
│   │   └── HeroController.cs          # Hero movement, jumping, combat
│   ├── Lighting/  
│   │   └── BeaconController.cs        # Light ray system and controls
│   ├── Environment/
│   │   ├── Water.cs                   # Water physics and reflection
│   │   └── LightAbsorbingObstacle.cs  # Light-dissipating obstacles
│   └── GameManager.cs                 # Game coordination and debugging
├── Scenes/
│   └── SampleScene.unity              # Main game scene
└── Settings/                          # URP and rendering settings
```

## 🔧 Core Systems

### Hero Character System
- **Physics-based movement** with WASD controls
- **Jump mechanics** with ground detection
- **Sword combat** system with attack ranges
- **Water interaction** with sinking and movement penalties

### Beacon Light System  
- **360° Light Emission**: Press F for omnidirectional light
- **Directional Control**: F/G + arrow keys for focused beams
- **Ray Casting**: Real-time light ray calculations
- **Visual Feedback**: Line renderers show light paths

### Water Physics System
- **Reflective Surfaces**: Light rays bounce at realistic angles
- **Hero Sinking**: Gradual submersion with physics drag
- **Surface Detection**: Proper collision and trigger handling
- **Visual Effects**: Splash and bubble systems

### Light Absorption System
- **Ray Termination**: Complete light dissipation on contact
- **Obstacle Types**: Destructible and indestructible variants
- **Visual Effects**: Absorption animations and feedback
- **Strategic Gameplay**: Creates shadows and puzzle elements

## 🎨 Visual Design (Placeholder)

Current implementation uses simple colored shapes:
- **Hero**: Blue rectangle with sprite renderer
- **Water**: Transparent blue rectangles  
- **Obstacles**: Black squares/rectangles
- **Ground/Walls**: Brown/gray rectangles
- **Light Rays**: Yellow line renderers

## 🔬 Technical Features

### Advanced Lighting
- **Multi-Reflection**: Light bounces multiple times off surfaces
- **Layer-Based Detection**: Separate handling for reflective vs absorbing materials
- **Performance Optimization**: Ray distance limits and early termination
- **Real-Time Calculations**: Dynamic light path updates

### Physics Integration
- **Unity 2D Physics**: Rigidbody and Collider components
- **Collision Layers**: Organized interaction between different object types
- **Trigger Systems**: Water detection and environmental interactions
- **Debug Visualization**: Gizmos and ray drawing for development

### Audio System (Ready)
- **Modular Sound Effects**: Placeholder audio sources configured
- **Event-Driven**: Sound triggered by game actions
- **Volume Management**: Configurable audio levels

## 🐛 Debug Features

### Development Tools
- **F1 Debug Toggle**: Real-time game state information
- **Visual Gizmos**: Scene view debugging for colliders and rays
- **Console Logging**: Detailed event tracking and system status
- **Component Validation**: Automatic setup verification

### Debug Information
- Hero position and velocity
- Beacon activation state and direction
- Light ray collision data
- Water interaction status
- System initialization validation

## 📋 Requirements

### Unity Setup
- **Unity Version**: 2022.3 LTS or newer
- **Render Pipeline**: Universal Render Pipeline (URP) 
- **Input System**: New Unity Input System package
- **2D Physics**: Unity 2D physics system

### Layer Configuration
| Layer | Name | Purpose |
|-------|------|---------|
| 6 | Water | Reflective surfaces |
| 7 | Obstacle | Light-absorbing barriers |
| 8 | Player | Hero character |
| 9 | Ground | Collision surfaces |

## 🚧 Development Status

### ✅ Completed Features
- Hero character controller with movement and combat
- Beacon light system with F/G controls  
- Water physics with reflection and sinking
- Light-absorbing obstacles with dissipation
- Multi-reflection light bouncing system
- Comprehensive debug and validation tools

### 🔄 Future Enhancements
- Replace placeholder sprites with final artwork
- Advanced particle effects for water and light
- Audio implementation with sound effects
- Level design tools and prefab system
- Save/load game state functionality
- Performance optimization for mobile platforms

## 🎯 Game Design Goals

### Core Mechanics
- **Light-Based Puzzles**: Use reflection and absorption strategically
- **Environmental Navigation**: Water as both obstacle and tool
- **Combat Integration**: Sword attacks on destructible obstacles
- **Physics Realism**: Believable light and water behavior

### Player Experience
- **Intuitive Controls**: Easy to learn, hard to master
- **Visual Clarity**: Clear feedback for all interactions  
- **Progressive Complexity**: Layered challenge introduction
- **Exploration Focus**: Reward creative light manipulation

## 📖 Documentation

- **[PRD.md](PRD.md)**: Complete Product Requirements Document
- **[SETUP_GUIDE.md](SETUP_GUIDE.md)**: Step-by-step Unity configuration
- **Script Documentation**: Inline code comments and method descriptions

## 🤝 Contributing

This is a prototype implementation following the PRD specifications. The codebase is structured for easy extension and modification:

1. **Modular Design**: Independent system components
2. **Clear Interfaces**: Public methods for cross-system communication  
3. **Configurable Parameters**: Inspector-exposed settings
4. **Debug Support**: Built-in development tools

---

**Version**: 0.0.1  
**Last Updated**: January 22, 2026  
**Unity Version**: 2022.3+ LTS  
**Platform**: PC (Windows/Mac/Linux)