using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Unity.Hq;
using AiLabs.Unity.Sim;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AiLabs.Unity.Bootstrap
{
    /// <summary>
    /// Builds the first HQ scene in code: garage lot grid, room blocks, HUD, and a week button.
    /// No rules live here — only Commands (later) and drawing GameState.
    /// </summary>
    public sealed class GameBootstrap : MonoBehaviour
    {
        const long DefaultSeed = 42L;

        SimulationHost _host;
        HqGridView _grid;
        ModelMascotRow _mascots;
        RoomBuildController _roomBuild;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void AutoStart()
        {
            if (Object.FindFirstObjectByType<GameBootstrap>() != null) return;

            var root = new GameObject("GameBootstrap");
            DontDestroyOnLoad(root);
            root.AddComponent<GameBootstrap>();
        }

        void Awake()
        {
            EnsureCameraAndLight();
            EarlyGameGarageUrVolumeBootstrap.EnsureGlobalVolume();
            EarlyGameGarageUrSkyBootstrap.ApplyAmbientFromHdri();
            EarlyGameGarageLighting.Apply();
            _host = new SimulationHost();
            string json = RulesConfigLoader.LoadJsonText();
            GameConfig config = GameConfig.FromJson(json);
            _host.StartNewGame(config, DefaultSeed);
            _host.StateChanged += OnStateChanged;

            var hq = new GameObject("HQ");
            hq.transform.SetParent(transform, false);
            _grid = hq.AddComponent<HqGridView>();
            _grid.Bind(_host.Simulation);

            var mascots = new GameObject("Mascots");
            mascots.transform.SetParent(hq.transform, false);
            _mascots = mascots.AddComponent<ModelMascotRow>();
            _mascots.Bind(_host.Simulation);

            var hud = HqHudView.Create(transform, _host);
            Transform canvas = hud.transform.parent;
            RoomBuildPanel buildPanel = RoomBuildPanel.Create(canvas, _host);
            StaffGpuPanel.Create(canvas, _host);
            TrainingRunPanel.Create(canvas, _host);
            GauntletShowPanel.Create(canvas, _host);
            _roomBuild = hq.AddComponent<RoomBuildController>();
            _roomBuild.Bind(_host, buildPanel);

            var garageArt = EarlyGameArtBootstrap.AttachGarage(hq.transform, _host, hud);
            _host.StateChanged += () =>
            {
                if (garageArt != null && _host?.Simulation != null)
                    garageArt.Bind(_host.Simulation);
            };
        }

        void OnStateChanged()
        {
            if (_grid != null && _host?.Simulation != null)
                _grid.Bind(_host.Simulation);
            if (_mascots != null && _host?.Simulation != null)
                _mascots.Bind(_host.Simulation);
        }

        static void EnsureCameraAndLight()
        {
            if (Camera.main == null)
            {
                var camGo = new GameObject("Main Camera");
                var cam = camGo.AddComponent<Camera>();
                camGo.tag = "MainCamera";
                cam.clearFlags = CameraClearFlags.Skybox;
                cam.backgroundColor = new Color(0.55f, 0.65f, 0.72f);
            }

            if (Camera.main != null)
                EarlyGameGarageTycoonCameraRig.ApplyGameplayCamera(Camera.main, 14, 10);

            if (Object.FindFirstObjectByType<Light>() == null)
            {
                var lightGo = new GameObject("Directional Light");
                var light = lightGo.AddComponent<Light>();
                light.type = LightType.Directional;
                light.intensity = 1.1f;
                light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            }

            if (Object.FindFirstObjectByType<EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<EventSystem>();
                es.AddComponent<StandaloneInputModule>();
            }
        }
    }
}