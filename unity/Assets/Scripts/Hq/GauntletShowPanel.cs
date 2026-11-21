using AiLabs.Rules.State;
using AiLabs.Unity.Sim;
using AiLabs.Unity.UI;
using UnityEngine;
using UnityEngine.UI;

namespace AiLabs.Unity.Hq
{
    /// <summary>TV-style overlay for the yearly Gauntlet results (reads state only).</summary>
    public sealed class GauntletShowPanel : MonoBehaviour
    {
        SimulationHost _host;
        Text _body;
        int _lastGauntletCount;

        public static GauntletShowPanel Create(Transform canvasParent, SimulationHost host)
        {
            var panelRoot = HqUiFactory.Panel(canvasParent, "GauntletShowPanel",
                new Vector2(0.25f, 0.35f), new Vector2(0.75f, 0.75f), new Vector2(0f, 0f),
                new Color(0.05f, 0.06f, 0.1f, 0.96f));
            panelRoot.SetActive(false);

            var panel = panelRoot.AddComponent<GauntletShowPanel>();
            panel._host = host;
            panel.BuildUi(panelRoot.transform);
            panel._lastGauntletCount = host.Simulation?.State.Gauntlets.Count ?? 0;
            host.StateChanged += panel.OnStateChanged;
            return panel;
        }

        void OnDestroy()
        {
            if (_host != null) _host.StateChanged -= OnStateChanged;
        }

        void BuildUi(Transform root)
        {
            HqUiFactory.Label(root, "Title", "The Gauntlet", 22, TextAnchor.UpperCenter)
                .fontStyle = FontStyle.Bold;

            _body = HqUiFactory.Label(root, "Body", "", 14, TextAnchor.UpperLeft);
            _body.rectTransform.offsetMin = new Vector2(16f, 48f);
            _body.rectTransform.offsetMax = new Vector2(-16f, -16f);
            _body.alignment = TextAnchor.UpperLeft;

            HqUiFactory.SmallButton(root, "Dismiss", () => gameObject.SetActive(false));
        }

        void OnStateChanged()
        {
            if (_host?.Simulation == null) return;
            var gauntlets = _host.Simulation.State.Gauntlets;
            if (gauntlets.Count > _lastGauntletCount && gauntlets.Count > 0)
            {
                GauntletRecord latest = gauntlets[gauntlets.Count - 1];
                _body.text = GauntletShowFormatter.Summary(_host.Simulation.Config, _host.Simulation.State, latest);
                gameObject.SetActive(true);
            }
            _lastGauntletCount = gauntlets.Count;
        }
    }
}