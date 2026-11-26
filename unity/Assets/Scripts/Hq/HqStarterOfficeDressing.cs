using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using UnityEngine;

namespace AiLabs.Unity.Hq
{
    /// <summary>Procedural desk / monitor / staff props on starter office rooms (placeholder until art thread assets land).</summary>
    public static class HqStarterOfficeDressing
    {
        public static void DressRoom(Transform roomRoot, RoomState room, RoomTypeConfig type, int staffInRoom, Material litTemplate)
        {
            if (roomRoot == null || room == null) return;
            float floorY = HqGridLayout.FloorY;
            float w = roomRoot.localScale.x;
            float d = roomRoot.localScale.z;
            float roomTop = roomRoot.localPosition.y + roomRoot.localScale.y * 0.5f;

            var props = new GameObject("Props");
            props.transform.SetParent(roomRoot, false);
            props.transform.localPosition = Vector3.zero;

            int desks = Mathf.Clamp(room.Width, 1, 3);
            for (int i = 0; i < desks; i++)
            {
                float t = desks == 1 ? 0.5f : i / (float)(desks - 1);
                float lx = Mathf.Lerp(-w * 0.32f, w * 0.32f, t);
                float lz = d * 0.1f;
                SpawnDesk(props.transform, litTemplate, lx, roomTop, lz);
                SpawnComputer(props.transform, litTemplate, lx, roomTop + 0.08f, lz - 0.05f,
                    type != null && type.Purpose == RoomPurpose.Compute);
            }

            int figures = Mathf.Clamp(staffInRoom, 0, 4);
            for (int s = 0; s < figures; s++)
            {
                float angle = s * Mathf.PI * 2f / Mathf.Max(figures, 1);
                float rx = Mathf.Cos(angle) * w * 0.22f;
                float rz = Mathf.Sin(angle) * d * 0.22f;
                SpawnStaff(props.transform, litTemplate, rx, roomTop, rz);
            }
        }

        public static int StaffForRoom(LabState lab, RoomState room, RoomTypeConfig type)
        {
            if (lab.Staff == null || lab.Staff.Count == 0) return 0;
            if (type == null) return 1;
            int basis = Mathf.Max(1, lab.Staff.Count / Mathf.Max(1, lab.Rooms.Count));
            if (type.Purpose == RoomPurpose.Research || type.Purpose == RoomPurpose.Compute)
                return Mathf.Min(lab.Staff.Count, basis + 1);
            return Mathf.Min(lab.Staff.Count, basis);
        }

        static void SpawnDesk(Transform parent, Material lit, float x, float y, float z)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "Desk";
            go.transform.SetParent(parent, false);
            go.transform.localScale = new Vector3(0.42f, 0.06f, 0.28f);
            go.transform.localPosition = new Vector3(x, y + 0.03f, z);
            var col = new Color(0.45f, 0.38f, 0.32f);
            go.GetComponent<Renderer>().material = lit != null ? new Material(lit) { color = col } : new Material(HqShaders.Lit()) { color = col };
            RemoveCollider(go);
        }

        static void SpawnComputer(Transform parent, Material lit, float x, float y, float z, bool bright)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "Computer";
            go.transform.SetParent(parent, false);
            go.transform.localScale = new Vector3(0.22f, 0.14f, 0.08f);
            go.transform.localPosition = new Vector3(x, y + 0.07f, z);
            var mat = lit != null ? new Material(lit) : new Material(HqShaders.Lit());
            mat.EnableKeyword("_EMISSION");
            Color screen = bright ? new Color(0.2f, 0.75f, 1f) : new Color(0.15f, 0.35f, 0.55f);
            mat.color = new Color(0.12f, 0.12f, 0.14f);
            mat.SetColor("_EmissionColor", screen * (bright ? 0.6f : 0.35f));
            go.GetComponent<Renderer>().material = mat;
            RemoveCollider(go);
        }

        static void SpawnStaff(Transform parent, Material lit, float x, float y, float z)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = "Staff";
            go.transform.SetParent(parent, false);
            go.transform.localScale = new Vector3(0.12f, 0.2f, 0.12f);
            go.transform.localPosition = new Vector3(x, y + 0.22f, z);
            var col = new Color(0.85f, 0.72f, 0.58f);
            go.GetComponent<Renderer>().material = lit != null ? new Material(lit) { color = col } : new Material(HqShaders.Lit()) { color = col };
            RemoveCollider(go);
        }

        static void RemoveCollider(GameObject go)
        {
            var col = go.GetComponent<Collider>();
            if (col != null) Object.Destroy(col);
        }
    }
}