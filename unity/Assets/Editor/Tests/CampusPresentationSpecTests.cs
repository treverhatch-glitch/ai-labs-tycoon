using AiLabs.Unity.Hq;
using NUnit.Framework;
using UnityEngine;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusPresentationSpecTests
    {
        [Test]
        public void Gameplay_camera_sits_outside_the_lot()
        {
            var camGo = new GameObject("Cam");
            var cam = camGo.AddComponent<Camera>();
            CampusPresentationSpec.ApplyGameplayCamera(cam, 14, 10);
            Assert.Greater(cam.transform.position.y, 0f);
            Assert.Greater(Vector3.Distance(cam.transform.position, Vector3.zero), 2f);
            Object.DestroyImmediate(camGo);
        }
    }
}