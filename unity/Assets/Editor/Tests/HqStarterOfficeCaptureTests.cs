using System.IO;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Unity.Hq;
using AiLabs.Unity.Sim;
using NUnit.Framework;
using UnityEngine;

namespace AiLabs.Unity.Editor.Tests
{
  /// <summary>Starter office with desks/staff/computers — screenshot for playable milestone.</summary>
  public sealed class HqStarterOfficeCaptureTests
  {
    const int CaptureWidth = 1280;
    const int CaptureHeight = 720;

    [Test]
    public void Captures_starter_office_for_playable_milestone()
    {
      string configPath = Path.GetFullPath(Path.Combine(
        Directory.GetParent(Application.dataPath).FullName, "..", "Rules", "Data", "default-config.json"));
      GameConfig config = GameConfig.FromJson(File.ReadAllText(configPath));

      var host = new SimulationHost();
      host.StartNewGame(config, 77L);
      HqStarterLayout.ApplyIfEmpty(host);

      var camGo = new GameObject("CaptureCamera");
      var cam = camGo.AddComponent<Camera>();
      cam.clearFlags = CameraClearFlags.SolidColor;
      cam.backgroundColor = new Color(0.52f, 0.62f, 0.7f);
      cam.fieldOfView = 50f;

      var lightGo = new GameObject("Light");
      var light = lightGo.AddComponent<Light>();
      light.type = LightType.Directional;
      light.intensity = 1.15f;
      light.transform.rotation = Quaternion.Euler(48f, -25f, 0f);

      var hqGo = new GameObject("HQ");
      var grid = hqGo.AddComponent<HqGridView>();
      grid.Bind(host.Simulation);
      HqGridView.AlignMainCamera(config.Lot(host.Simulation.Player.LotId), cam);

      var rt = new RenderTexture(CaptureWidth, CaptureHeight, 24);
      cam.targetTexture = rt;
      cam.Render();
      RenderTexture.active = rt;
      var tex = new Texture2D(CaptureWidth, CaptureHeight, TextureFormat.RGB24, false);
      tex.ReadPixels(new Rect(0, 0, CaptureWidth, CaptureHeight), 0, 0);
      tex.Apply();

      string artifactsDir = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "artifacts");
      Directory.CreateDirectory(artifactsDir);
      string path = Path.Combine(artifactsDir, "hq-starter-office.png");
      File.WriteAllBytes(path, tex.EncodeToPNG());
      Assert.True(File.Exists(path));
      Assert.Greater(new FileInfo(path).Length, 4096);

      RenderTexture.active = null;
      cam.targetTexture = null;
      Object.DestroyImmediate(rt);
      Object.DestroyImmediate(tex);
      Object.DestroyImmediate(camGo);
      Object.DestroyImmediate(lightGo);
      Object.DestroyImmediate(hqGo);
    }
  }
}