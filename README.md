# Project Supers Client

Unity game client for the **Supers Fight Game** platform. It connects to the game server backend (REST API + WebSocket) for player accounts, inventories, customization, friends, matchmaking and store, and runs real-time 1v1 combat over WebRTC peer-to-peer connections.

## Features

- **Authentication & account** — guest login, signup, login, refresh tokens, account update, change password.
- **Character customization** — race, hair, eyes, mouth, skin color; fully equipable accessory set (hat, mask, neck, chest, back, shoulders, gloves, hip, leg, boots) with color palettes.
- **Emblem editor** — build a custom emblem from layered shapes, rendered to a transparent texture and shown on the main menu.
- **Store** — browse accessories by type and equip items.
- **Friend system** — send/respond/remove friend requests, list friends and recently played players.
- **Matchmaking & challenges** — find/cancel matchmaking, send/respond to direct challenges.
- **Real-time combat** — host-authoritative 1v1 matches over WebRTC P2P (reliable + unreliable data channels), with reconnect logic.
- **Character systems** — movement (run, sprint, dash, fly, jump), attacks (normal, strike, fly), block/deflect, hitboxes/hurtboxes, debuffs, take-damage, knock-back, target lock.
- **Destructible environment** — voxel-based destructible walls, floors, and obstacles with custom physics.
- **Training area** — practice against a configurable dummy.
- **Match result** — post-match result screen.
- **Settings** — keybinds, mouse sensitivity, master volume.
- **Realtime presence** — WebSocket connection for presence and server-pushed events (e.g. player banned).
- **Audio & VFX** — attack/dash/fly/hit/movement SFX scaled by damage, speed-lines, hurt overlay, dash cooldown bar.

## Tech Stack

- Unity 2022.3.62f3 (LTS)
- C#
- WebGL (deployment target) + Unity Editor (development)
- NativeWebSocket (WebSocket client)
- Unity WebRTC + UnityPeerJS (peer-to-peer match networking)
- Newtonsoft Json.NET (serialization)
- UGUI + Text Mesh Pro (UI)
- UnityGLTF (GLTF model loading)
- SimpleVoxelDestructionForUnity (destructible environment)

## Architecture

The client is a scene-based Unity application. Scripts are organized by feature domain:

| Folder | Responsibilities |
|--------|------------------|
| `Network/` | REST API + WebSocket services, request/response DTOs, value objects, P2P manager |
| `Player/` | `PlayerData` singleton, audio manager, player input handlers |
| `Character/` | Character data (stats, states, accessories), controllers, customization SOs, manager, SFX |
| `Match/` | Match data/DTOs, packet senders/receivers, match managers, VFX |
| `MatchMaking/` | Matchmaking controller and peer connection flow |
| `MainMenu/` | Main menu controller, character model, challenges, emblem |
| `GUI/` | UI controllers for every window (login, account, customize, store, settings, match, ...) |
| `Map/` | Map data, destructible obstacles with custom physics |
| `Utils/` | Scene loading, cookies, tab visibility, number formatting, UI generation, types |

The network layer is split into **development** and **deployment** profiles (`NetworkDataSO`) so the client can target different servers in-editor vs. in a release build.

## Project Structure

```
├── Assets/
│   ├── Scenes/                 # StartScene, LoadingScene, BattleScene, MatchResultScene, TrainingAreaScene
│   ├── Scripts/
│   │   ├── Network/            # RestApiService, WebSocketService, P2PManager, service classes, DTOs
│   │   │   ├── Dto/            # Request / Response / ValueObject / WsMessage
│   │   │   └── Service/        # Auth, account, inventory, friend, match, store, config, PeerJS
│   │   ├── Player/             # PlayerData, AudioManager, input controllers
│   │   ├── Character/          # Controllers, services, data, customization SOs, SFX, manager
│   │   ├── Match/              # MatchData, packets, host/client senders+receivers, managers
│   │   ├── MatchMaking/        # MatchMakingController
│   │   ├── MainMenu/           # MainMenuController, ChallengeController, emblem, store data
│   │   ├── GUI/                # UI controllers (windows, market, match, settings, training)
│   │   ├── Map/                # MapData, MapObstaclesController/SO
│   │   └── Utils/              # Scene loading, cookies, tab visibility, number formatting
│   ├── StreamingAssets/
│   │   └── config.json         # Server base URL (editable after build, no rebuild needed)
│   └── WebGLTemplates/         # Custom WebGL template
├── Builds/                     # development_builds / deployment_builds
├── Packages/                   # Unity package manifest
└── ProjectSettings/            # Unity project settings
```

## Prerequisites

- Unity Hub with **Unity 2022.3.62f3 (LTS)**
- Linux x64 toolchain (`com.unity.toolchain.linux-x86_64`) for WebGL builds
- The client-server backend (see the `project_supers_server` repo)

## Getting Started

1. Clone the repository.
2. Open it with Unity Hub → **Add project from disk** using Unity 2022.3.62f3.
3. Let Unity import the project (packages are restored from `Packages/manifest.json`).
4. Open the `StartScene` scene and press **Play**.

In the Unity Editor the client automatically uses the **development** network profile (`NetworkDataSO`). Point it at your local client server, e.g.:

```json
{
    "baseUrl": "localhost:8080",
    "baseRestSchema": "http://",
    "baseWebSocketSchema": "ws://"
}
```

## Configuration

The server base URL is resolved in this order:

1. `StreamingAssets/config.json` (see `ConfigLoader`) — editable **directly after a WebGL build** without rebuilding.
2. `NetworkDataSO` asset — `DevelopmentNetworkData` for editor/debug builds, `DeploymentNetworkData` for release builds.

| Setting | Location | Description |
|---------|----------|-------------|
| `baseUrl` | `config.json` / `NetworkDataSO` | Client server host:port |
| `baseRestSchema` | `config.json` / `NetworkDataSO` | REST scheme (`https://` deployment, `http://` development) |
| `baseWebSocketSchema` | `config.json` / `NetworkDataSO` | WebSocket scheme (`wss://` deployment, `ws://` development) |

## Networking & Match Flow

1. **REST API** (`RestApiService`) — all CRUD via `UnityWebRequest`: auth, account, inventory, accessories, keyboard config, friends, store, matchmaking, challenges, finish match.
2. **WebSocket** (`WebSocketService`) — presence + server-pushed events (`/ws/{username}`), e.g. player banned or incoming challenges.
3. **P2P match** (`P2PManager`) — after matchmaking succeeds, peers exchange signaling via the server and connect over **WebRTC** using a host-authoritative model:
   - **Reliable channel** — game events, match start, animations, flying interruption, etc.
   - **Unreliable channel** — high-frequency character states (positions, rotations).
   - `HostPacketSender/Receiver` and `ClientPacketSender/Receiver` mirror the same packet set on each side.
   - `MatchConnectionManager` handles disconnects and reconnects the host/client peer.

## Build

### WebGL (deployment)

Open the project in Unity, then **File → Build Settings → WebGL → Build**. Release builds (Development Build unchecked) use the **deployment** network profile (`baseUrl: project-supers-client-server.cloudflared.com:8080`).

### Development build

Debug builds (Development Build checked) use the **development** network profile and read `StreamingAssets/config.json` for the server URL. Set the WebGL template to the project's custom template before building.

## License

Proprietary project. Internal use.
