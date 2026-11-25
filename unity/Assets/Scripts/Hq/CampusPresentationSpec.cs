using AiLabs.Rules.Config;
using UnityEngine;

namespace AiLabs.Unity.Hq
{
    /// <summary>
    /// Shared presentation constants for the cozy campus camera and prop placement.
    /// Graphics batch 1 — rules-neutral; safe to reference from Edit Mode tests.
    /// </summary>
    public static class CampusPresentationSpec
    {
        public const float CameraYawDegrees = 38f;
        public const float CameraPitchDegrees = 32f;
        public const float CameraFieldOfView = 52f;
        public const float CameraDistanceScale = 1.55f;

        public static void ApplyGameplayCamera(Camera cam, int lotWidth, int lotHeight)
        {
            if (cam == null) return;
            float w = lotWidth * HqGridLayout.TileSize;
            float h = lotHeight * HqGridLayout.TileSize;
            Vector3 centre = new Vector3(w * 0.5f, 0f, h * 0.5f);
            float span = Mathf.Max(w, h);
            Quaternion rot = Quaternion.Euler(CameraPitchDegrees, CameraYawDegrees, 0f);
            Vector3 back = rot * Vector3.back;
            cam.transform.rotation = rot;
            cam.transform.position = centre + back * (span * CameraDistanceScale);
            cam.fieldOfView = CameraFieldOfView;
            cam.orthographic = false;
        }

        public static void ApplyCaptureCamera(Camera cam, LotConfig lot)
        {
            if (cam == null || lot == null) return;
            ApplyGameplayCamera(cam, lot.Width, lot.Height);
        }
    }
}