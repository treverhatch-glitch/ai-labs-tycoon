using AiLabs.Unity.Hq;
using NUnit.Framework;
using UnityEngine;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class EarlyGameGarageUrPresentationSpecTests
    {
        [Test]
        public void Tycoon_camera_matches_hero_compose_angles()
        {
            Assert.AreEqual(42f, EarlyGameGarageUrPresentationSpec.TycoonAzimuthDegrees);
            Assert.AreEqual(30f, EarlyGameGarageUrPresentationSpec.TycoonElevationDegrees);
            Assert.Greater(EarlyGameGarageUrPresentationSpec.BloomIntensity, 0.1f);
            Assert.IsTrue(EarlyGameGarageUrPresentationSpec.HdriEnvironmentPath.Contains("brown_photostudio"));
        }

        [Test]
        public void Tycoon_camera_frames_garage_lot()
        {
            var camGo = new GameObject("Cam");
            var cam = camGo.AddComponent<Camera>();
            EarlyGameGarageTycoonCameraRig.ApplyGameplayCamera(cam, 14, 10);
            Assert.Greater(cam.transform.position.y, 0.5f);
            Assert.Greater(Vector3.Dot(cam.transform.forward, Vector3.down), 0f);
            Object.DestroyImmediate(camGo);
        }
    }
}