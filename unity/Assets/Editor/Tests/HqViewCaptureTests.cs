using System.IO;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Unity.Hq;
using AiLabs.Unity.Sim;
using NUnit.Framework;
using UnityEngine;

namespace AiLabs.Unity.Editor.Tests
{
    /// <summary>Rebuilds the procedural HQ view and writes a PNG for CI artifacts.</summary>
    public sealed class HqViewCaptureTests
    {
        const int CaptureWidth = 960;
        const int CaptureHeight = 540;

        [Test]
        public void Captures_hq_grid_for_ci_artifact()
        {
            string configPath = Path.GetFullPath(Path.Combine(
                Directory.GetParent(Application.dataPath).FullName, "..", "Rules", "Data", "default-config.json"));
            GameConfig config = GameConfig.FromJson(File.ReadAllText(configPath));

            var simGo = new GameObject("SimHarness");
            var host = new SimulationHost();
            host.StartNewGame(config, 42L);

            var camGo = new GameObject("CaptureCamera");
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.55f, 0.65f, 0.72f);
            LotConfig lot = config.Lot(host.Simulation.Player.LotId);
            CampusPresentationSpec.ApplyCaptureCamera(cam, lot);

            var lightGo = new GameObject("Light");
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.1f;
            light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            var hqGo = new GameObject("HQ");
            var grid = hqGo.AddComponent<HqGridView>();
            grid.Bind(host.Simulation);

            var rt = new RenderTexture(CaptureWidth, CaptureHeight, 24);
            cam.targetTexture = rt;
            cam.Render();
            RenderTexture.active = rt;

            var tex = new Texture2D(CaptureWidth, CaptureHeight, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, CaptureWidth, CaptureHeight), 0, 0);
            tex.Apply();

            string artifactsDir = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "artifacts");
            Directory.CreateDirectory(artifactsDir);
            string capturePath = Path.Combine(artifactsDir, "hq-view.png");
            File.WriteAllBytes(capturePath, tex.EncodeToPNG());
            Assert.True(File.Exists(capturePath));
            Assert.Greater(new FileInfo(capturePath).Length, 4096);

            RenderTexture.active = null;
            cam.targetTexture = null;
            Object.DestroyImmediate(rt);
            Object.DestroyImmediate(tex);
            Object.DestroyImmediate(camGo);
            Object.DestroyImmediate(lightGo);
            Object.DestroyImmediate(hqGo);
            Object.DestroyImmediate(simGo);
        }
    }
}