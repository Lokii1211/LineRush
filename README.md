# 🎯 Line Rush

**An addictive arrow/line puzzle game by [Viya Nexus](https://github.com/Lokii1211)**

Tap the arrows to send them rushing forward — but watch out! Lines that collide will bounce back. Clear all lines without running out of lives to beat each stage.

[![Build LineRush Android](https://github.com/Lokii1211/LineRush/actions/workflows/build-android.yml/badge.svg)](https://github.com/Lokii1211/LineRush/actions/workflows/build-android.yml)

---

## ✨ Features

### Core Gameplay
- 🎮 **30 Hand-Crafted Levels** — Curated puzzles from simple crosses to "The Nexus" boss level
- ♾️ **1000+ Procedural Levels** — Infinite replayability with difficulty scaling
- 🔄 **Smart Line Physics** — Lines rush forward and reverse on collision
- ⚡ **One-Tap Controls** — Simple but deep puzzle mechanics

### Engagement Systems
- 🔥 **Combo System** — Chain line clears for score multipliers (1x → 2x → 3x → 5x)
- ⭐ **Star Ratings** — 1-3 stars per level based on performance
- 💡 **Hint System** — Stuck? Get a hint (1 free per level, more via rewarded ads)
- 🏆 **15 Achievements** — Track milestones from "First Clear" to "Centurion"
- 📅 **Daily Challenges** — New puzzle every day with streak bonuses
- 🗺️ **Level Select** — Replay any level to improve your star rating

### Monetization (AdMob)
- 📢 **Banner Ads** — Non-intrusive bottom banner
- 🎬 **Interstitial Ads** — Every 3 levels completed
- 🎁 **Rewarded Ads** — Watch for extra lives or hints

### Polish
- 🔊 **Procedural Audio** — 13 unique synthesized sound effects
- 💫 **DOTween Animations** — Smooth UI transitions and feedback
- 🎨 **Neon Cyberpunk Theme** — Premium dark UI with glowing accents
- 📱 **Mobile Optimized** — URP rendering, 60fps target

---

## 🏗️ Tech Stack

| Component | Technology |
|-----------|-----------|
| Engine | Unity 6 (6000.0.58f2) |
| Rendering | Universal Render Pipeline (URP) |
| Language | C# |
| Animation | DOTween |
| Ads | Google Mobile Ads (AdMob) |
| CI/CD | GitHub Actions (game-ci/unity-builder) |
| Target | Android (API 23+), iOS |

---

## 🚀 Quick Start

### Prerequisites
- Unity 6 (version 6000.0.58f2)
- Android Build Support module
- Git LFS

### Clone & Open
```bash
git clone https://github.com/Lokii1211/LineRush.git
cd LineRush
```

Open the project in Unity Hub → Add → Browse to cloned folder.

### Build via GitHub Actions (No Unity Required!)
1. Fork or push to your GitHub repository
2. Set up GitHub Secrets (see CI/CD section below)
3. Go to **Actions** → **Build LineRush Android** → **Run workflow**
4. Download the APK/AAB from the workflow artifacts

---

## 📱 AdMob Configuration

| Ad Type | Unit ID |
|---------|---------|
| App ID | `ca-app-pub-2857128148429490~6176198550` |
| Interstitial | `ca-app-pub-2857128148429490/3671614281` |
| Rewarded | `ca-app-pub-2857128148429490/7934831924` |

Ad IDs are configured in [`AdConfig.cs`](Assets/_Game/Scripts/Ads/AdConfig.cs).

---

## 🔄 CI/CD Pipeline

### GitHub Actions Workflows

| Workflow | Purpose |
|----------|---------|
| `activation.yml` | Generate Unity license activation file |
| `build-android.yml` | Build APK + AAB for Android |

### Required GitHub Secrets

| Secret | Description |
|--------|-------------|
| `UNITY_LICENSE` | Contents of your Unity .ulf license file |
| `UNITY_EMAIL` | Unity account email |
| `UNITY_PASSWORD` | Unity account password |

### How to Set Up (First Time)
1. Go to **Actions** → **Acquire Unity License** → **Run workflow**
2. Download the `.alf` file from artifacts
3. Go to [license.unity3d.com/manual](https://license.unity3d.com/manual)
4. Upload `.alf`, download `.ulf`
5. Go to repo **Settings** → **Secrets** → **Actions**
6. Create `UNITY_LICENSE` secret with `.ulf` file contents
7. Create `UNITY_EMAIL` and `UNITY_PASSWORD` secrets
8. Now run **Build LineRush Android** workflow

---

## 📂 Project Structure

```
Assets/
├── _Game/
│   ├── Scripts/
│   │   ├── Ads/          # AdManager, AdConfig
│   │   ├── Audio/        # SoundGenerator, SoundLibrary
│   │   ├── Gameplay/     # HintManager, DailyChallenge, Achievements
│   │   ├── Level/        # ProceduralGenerator, Templates, DifficultyCurve
│   │   ├── Line/         # Line, LineAnimation, LineClick, Collision
│   │   ├── Scoring/      # ScoreManager, ComboDisplay, LevelProgress
│   │   └── UI/           # ThemeConfig, LivesManager, EnergyPanel, LevelSelect
│   └── Data/             # ScriptableObject assets
├── SerapKeremGameKit/     # Core framework
│   └── Scripts/
│       ├── LevelSystem/  # LevelManager, StateManager, GameState
│       └── UI/           # UIRootController, WinPanel, FailPanel, HUDPanel
└── Scenes/               # Game scenes
```

---

## 🎮 Level Difficulty Tiers

| Levels | Tier | Lines | Pattern |
|--------|------|-------|---------|
| 1-10 | Tutorial | 2-4 | Simple prefab levels |
| 11-15 | Intermediate | 4-6 | Cross patterns |
| 16-20 | Advanced | 5-7 | Diagonal mazes |
| 21-25 | Expert | 6-8 | Starburst & web |
| 26-28 | Nightmare | 7-10 | Spiral & clockwork |
| 29-30 | Impossible | 10-12 | Boss levels |
| 31-1030 | Procedural | 2-15 | Algorithm generated |

---

## 📄 License

This project is proprietary software owned by **Viya Nexus**.

---

*Built with ❤️ by Viya Nexus*
