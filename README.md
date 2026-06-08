# 🚀⚡ LineRush

**LineRush** is a challenging line-based puzzle game where players strategically activate animated lines to clear the playfield. Avoid collisions, manage your energy, and complete all stages.
Developed with **Unity**, the game features smooth animations, intelligent collision detection, procedural level generation, and a robust, scalable architecture.

---

## 🎮 About the Game

LineRush is a line-based puzzle game where players activate lines by clicking on them.
Each line moves forward with an animation while gradually disappearing from its tail.

The core challenge lies in **timing your clicks correctly** to prevent collisions between lines.

* When two lines collide, you lose an energy charge.
* Clear all lines to win the stage.

---

## ✨ Game Features

### 🎯 Core Mechanics

* ⚡ **Interactive Line System**
  Activate precise, smooth forward animations by clicking on lines.
* 💥 **Smart Collision Detection**
  Advanced head-to-line collision system.
* 🔋 **Energy Management**
  Start with 5 energy charges and track remaining charges via themed UI.
* 🏆 **Win / Lose Conditions**
  Clear all lines to win; lose all energy to fail the stage.
* 📊 **1010 Stages**
  10 hand-crafted stages plus 1000 procedurally generated stages with increasing difficulty.

---

### 🎨 Visual Features

* 🎬 **Smooth Animations**
  DOTween-powered forward and backward line animations with button micro-animations.
* 🎨 **Dynamic Color Feedback**
  Lines change to orange-red on collision, neon green on completion.
* 📹 **Automatic Camera Adjustment**
  Camera automatically frames all lines per stage.
* ✨ **Line Head Tracking**
  A visual "head" object follows the line tip for better visibility.
* 🎨 **Neon Theme**
  Cyan/magenta/dark purple neon-on-dark color scheme.

---

## 🧠 Technical Features

* 🧱 **Component-Based Architecture**
  Modular, SOLID-compliant design with clear separation of responsibilities.
* ⚡ **Vector3 Array Pooling**
  Zero-allocation animation system optimized for performance.
* 🔄 **State Management**
  Centralized game state control via a `StateManager`.
* 🔊 **Procedural Audio**
  Unique procedurally-generated sound effects for every game action.
* 📂 **Hybrid Level System**
  Prefab-based levels + procedural generation for 1000+ stages.
* 🎛️ **Explicit Initialization**
  Clear, deterministic initialization order instead of Unity's implicit lifecycle.
* 🎮 **Styled Buttons**
  Configurable button styles (Pill, Rounded, PlayArrow, Circle) with DOTween animations.

---

## 🛠️ Tools & Packages Used

### 📦 Unity Packages

- ⚙️ **Unity Engine** — 6000.0.58f2 (Unity 6)
- 🔄 **DOTween** — Tween-based animations for line movement and UI
- 🧰 **TriInspector** — Advanced Inspector UI for efficient development
- 🎨 **Universal Render Pipeline (URP)** — Modern and optimized rendering
- 📝 **TextMeshPro** — Advanced text rendering for UI
- ➰ **Line Renderer** — Core system for rendering and animating dynamic lines

---

### 🧩 Custom Framework

**GameKit** – Production-ready Unity infrastructure:

* 📝 Logging and tracing system
* 🔊 Pooling-based audio management with procedural sound support
* 📳 Cross-platform haptic support
* ✨ Auto-recycling particle system
* ♻️ State-driven level system
* 🖼️ Panel-based UI framework
* 🔄 Game state management system
* 💰 Currency / wallet system
* 🧰 Guarded MonoSingleton architecture

---

## 🎨 Custom Systems

### ⚡ Line System

A fully custom-built line architecture including:

* 🎬 **LineAnimation**
  Forward/backward animation using array pooling (zero allocation)
* 👆 **LineClick**
  Input handling and line activation logic
* 💥 **LineHeadCollisionDetector**
  Precise collision detection between line heads and bodies
* 🎨 **LineMaterialHandler**
  Dynamic color management with success/failure feedback
* 🗑️ **LineDestroyer**
  Automatic cleanup after animation completion
* ⚡ **LineRendererHead**
  Visual head object that follows the line's endpoint

---

### 🎛️ Game Systems

* 🔋 **LivesManager** — Singleton-based energy charge management
* 📹 **CameraManager** — Automatic camera adjustment based on level bounds
* 🎯 **Level System** — Hybrid prefab + procedural level loading
* 🔄 **StateManager** — Centralized game states (`Loading`, `OnStart`, `OnWin`, `OnLose`)
* 🔊 **SoundGenerator** — Procedural audio synthesis (7 unique sounds)
* 🎮 **StyledButton** — Configurable button component with micro-animations

---

### 🌀 Procedural Level Generation

* 📊 **LevelDifficultyCurve** — ScriptableObject with AnimationCurves defining difficulty scaling
* 🔧 **ProceduralLevelGenerator** — Creates levels algorithmically at runtime:
  - Seeded randomness for deterministic layouts
  - Configurable line count, grid size, directions, intersections
  - 5 difficulty tiers from Tutorial to Expert

---

## 🎯 How to Play

### 📘 Basic Rules

* 🎯 **Click to Activate**
  Click on any line to activate it.
* 💥 **Avoid Collisions**
  Each collision costs one energy charge.
* ⚡ **Line Completion**
  Lines erase from the tail as they move and are removed after completion.
* 🔋 **Manage Your Energy**
  You start with 5 energy charges.
* 🏆 **Win Condition**
  Complete all lines without collisions.
* 💔 **Lose Condition**
  Lose all 5 energy charges.

---

### 🕹️ Controls

* 🖱️ **Mouse / Touch** — Click or tap a line to activate
* ⏸️ **No Re-activation** — Moving lines cannot be activated again
* 🎯 **Strategy** — Analyze line placement carefully before clicking

---

## 📦 Project Structure

```
Assets/
├── _Game/
│   ├── Scripts/
│   │   ├── Line/        (Line, LineAnimation, LineClick, etc.)
│   │   ├── UI/          (EnergyUI, EnergyPanel, ThemeConfig, StyledButton)
│   │   ├── Audio/       (SoundGenerator, SoundLibrary)
│   │   ├── Level/       (ProceduralLevelGenerator, LevelDifficultyCurve)
│   │   └── Ads/         (AdManager, AdConfig)
│   ├── Resources/
│   │   ├── Levels/      (10 hand-crafted level prefabs)
│   │   └── Line/
│   ├── Scenes/
│   │   └── GameScene.unity
│   └── ...
└── SerapKeremGameKit/   (Framework scripts)
```

---

## 🚀 Getting Started

### 📥 Installation

1. Clone or download the repository
2. Open the project in **Unity Hub** (Unity 6000.0.58f2 or later)
3. Open the main scene: `Assets/_Game/Scenes/GameScene.unity`
4. Press **Play**

---

### 🛠️ Build

1. Go to **File → Build Settings**
2. Select the target platform
3. Click **Build**

---

### 🔧 Configuration

#### Ad Setup
1. Replace placeholder IDs in `Assets/_Game/Scripts/Ads/AdConfig.cs` with your AdMob IDs
2. Import Google Mobile Ads Unity plugin
3. Configure in Assets > Google Mobile Ads > Settings

#### Procedural Levels
1. Create a `LevelDifficultyCurve` asset: Right-click in Project > Create > LineRush > Level Difficulty Curve
2. Assign it to the `ProceduralLevelGenerator` component in the scene
3. Adjust difficulty curves in the Inspector

---

## 📜 **License**

This project is licensed under the MIT License.
