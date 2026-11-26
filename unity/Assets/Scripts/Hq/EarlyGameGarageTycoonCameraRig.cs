using UnityEngine;

namespace AiLabs.Unity.Hq
{
    /// <summary>3/4 tycoon framing for Mum's Garage (hero parity with garage-scene-hero-compose.py).</summary>
    public static class EarlyGameGarageTycoonCameraRig
    {
        public static void ApplyGameplayCamera(Camera cam, int lotWidth, int lotHeight)
        {
            if (cam == null) return;
            float w = lotWidth * HqGridLayout.TileSize;
            float h = lotHeight * HqGridLayout.TileSize;
            Vector3 centre = new Vector3(w * 0.5f, 0f, h * 0.5f);
            float span = Mathf.Max(w, h);
            Quaternion rot = Quaternion.Euler(
                EarlyGameGarageUrPresentationSpec.TycoonElevationDegrees,
                EarlyGameGarageUrPresentationSpec.TycoonAzimuthDegrees,
                0f);
            Vector3 back = rot * Vector3.back;
            cam.transform.rotation = rot;
            cam.transform.position = centre + back * (span * EarlyGameGarageUrPresentationSpec.TycoonDistanceScale);
            cam.fieldOfView = EarlyGameGarageUrPresentationSpec.TycoonFieldOfViewDegrees;
            cam.orthographic = false;
            cam.allowHDR = true;
        }
    }
}