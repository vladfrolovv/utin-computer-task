## Demonstration

https://github.com/user-attachments/assets/1505ec01-ed12-46f7-b7e0-f6a3e54740c0

## Run it

Unity **6000.5.4f1**, open `Assets/UtinComputer/Scenes/Gameplay.unity`, press Play.

## Layout

- `Assets/UtinComputer/Scripts` — gameplay code, split by feature (Spheres, Map, Finish, Cameras)
- `Assets/UtinComputer/Configs` — all tuning values live in these ScriptableObjects
- `Assets/UtinComputer/Prefabs`, `Scenes`, `Materials` — the usual

Zenject for wiring, UniRx for state, DOTween for the juice. Controllers are plain C# and hold the logic; `*View` classes are the only MonoBehaviours.
