using AiLabs.Unity.Sim;
using UnityEngine;
using UnityEngine.UI;

namespace AiLabs.Unity.Hq
{
    public sealed class HqMascotStripHud : MonoBehaviour
    {
        SimulationHost _host;
        Text _line;
        public static HqMascotStripHud Create(Transform parent, SimulationHost host)
        {
            var go = new GameObject("MascotStrip");
            go.transform.SetParent(parent, false);
            var h = go.AddComponent<HqMascotStripHud>();
            h._host = host;
            h._line = go.AddComponent<Text>();
            host.StateChanged += h.Refresh;
            return h;
        }
        void OnDestroy() { if (_host != null) _host.StateChanged -= Refresh; }
        void Refresh()
        {
            if (_host?.Simulation == null) return;
            var sim = _host.Simulation;
            string lastWeekLand = HqDisplayQueriesHud.LastWeekLandHardwareSummaryLine(sim.Player);
            string lastWeekLandPart = string.IsNullOrEmpty(lastWeekLand) ? "" : " · " + lastWeekLand;
            string thisWeekChipRevenue = HqDisplayQueriesHud.ThisWeekChipRevenueSummaryLine(sim.Player);
            string thisWeekChipRevenuePart = string.IsNullOrEmpty(thisWeekChipRevenue) ? "" : " · " + thisWeekChipRevenue;
            string lastWeekConstructionPatent = HqDisplayQueriesHud.LastWeekConstructionPatentSummaryLine(sim.Player);
            string lastWeekConstructionPatentPart = string.IsNullOrEmpty(lastWeekConstructionPatent) ? "" : " · " + lastWeekConstructionPatent;
            string lastWeekHiringPatent = HqDisplayQueriesHud.LastWeekHiringPatentSummaryLine(sim.Player);
            string lastWeekHiringPatentPart = string.IsNullOrEmpty(lastWeekHiringPatent) ? "" : " · " + lastWeekHiringPatent;
            string thisWeekBuild = HqDisplayQueriesHud.ThisWeekConstructionSalariesSummaryLine(sim.Player);
            string thisWeekBuildPart = string.IsNullOrEmpty(thisWeekBuild) ? "" : " · " + thisWeekBuild;
            _line.text = "Mascots" + lastWeekLandPart;
        }
    }
}
