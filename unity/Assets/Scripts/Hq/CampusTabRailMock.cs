using UnityEngine;

namespace AiLabs.Unity.Hq
{
    /// <summary>Placeholder for future left tab rail (Q452). No input wiring yet.</summary>
    public sealed class CampusTabRailMock : MonoBehaviour
    {
        public static readonly string[] TabIds = { "hq", "research", "training", "rivals", "world" };

        public void BuildStub(Transform parent)
        {
            var rail = new GameObject("TabRail");
            rail.transform.SetParent(parent, false);
            for (int i = 0; i < TabIds.Length; i++)
            {
                var tab = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tab.name = "Tab_" + TabIds[i];
                tab.transform.SetParent(rail.transform, false);
                tab.transform.localScale = new Vector3(0.12f, 0.12f, 0.02f);
                tab.transform.localPosition = new Vector3(-0.5f, 0.2f - i * 0.15f, 0f);
                tab.GetComponent<Renderer>().material.color = CampusUiTokens.UiSlate;
            }
        }
    }
}