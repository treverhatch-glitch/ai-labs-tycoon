using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Unity.Sim;
using UnityEngine;
using UnityEngine.UI;

namespace AiLabs.Unity.Hq
{
    /// <summary>Cash, calendar date, era label, and the advance-week control.</summary>
    public sealed class HqHudView : MonoBehaviour
    {
        SimulationHost _host;
        Image _barImage;
        Text _cashLabel;
        Text _dateLabel;
        Text _eraLabel;
        Text _phaseLabel;
        Text _researchLabel;
        Text _statusLabel;

        public void Bind(SimulationHost host)
        {
            _host = host;
            _host.StateChanged += OnStateChanged;
            OnStateChanged();
        }

        void OnDestroy()
        {
            if (_host != null) _host.StateChanged -= OnStateChanged;
        }

        void OnStateChanged()
        {
            if (_host?.Simulation == null) return;
            Simulation sim = _host.Simulation;
            LabState lab = sim.Player;
            if (_cashLabel != null)
            {
                _cashLabel.text = "Cash: $" + lab.Cash.ToString("N0");
                _cashLabel.color = HqFinancialHud.CashColor(lab);
            }
            if (_dateLabel != null) _dateLabel.text = sim.DateLabel;
            if (_researchLabel != null)
                _researchLabel.text = "Research: " + lab.ResearchPoints.ToString("N0") + " pts · Staff: " + lab.Staff.Count;
            if (_eraLabel != null) _eraLabel.text = sim.Era.Name;
            if (_phaseLabel != null)
                _phaseLabel.text = HqCampusPhase.LabelForEra(sim.Config, sim.Era.Id, lab);
            if (_statusLabel != null)
            {
                string status = HqFinancialHud.StatusLine(lab);
                string lastWeekLand = HqDisplayQueriesHud.LastWeekLandHardwareSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekLand))
                    status = string.IsNullOrEmpty(status) ? lastWeekLand : status + " · " + lastWeekLand;
                string lastWeekSiteCapital = HqDisplayQueriesHud.LastWeekSiteCapitalSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekSiteCapital))
                    status = string.IsNullOrEmpty(status) ? lastWeekSiteCapital : status + " · " + lastWeekSiteCapital;
                string lifetimeGrantsNet = HqDisplayQueriesHud.LifetimeGrantsNetSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGrantsNet))
                    status = string.IsNullOrEmpty(status) ? lifetimeGrantsNet : status + " · " + lifetimeGrantsNet;
                string thisWeekChipRevenue = HqDisplayQueriesHud.ThisWeekChipRevenueSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekChipRevenue))
                    status = string.IsNullOrEmpty(status) ? thisWeekChipRevenue : status + " · " + thisWeekChipRevenue;
                string lastWeekConstructionPatent = HqDisplayQueriesHud.LastWeekConstructionPatentSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekConstructionPatent))
                    status = string.IsNullOrEmpty(status) ? lastWeekConstructionPatent : status + " · " + lastWeekConstructionPatent;
                string lastWeekHiringPatent = HqDisplayQueriesHud.LastWeekHiringPatentSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekHiringPatent))
                    status = string.IsNullOrEmpty(status) ? lastWeekHiringPatent : status + " · " + lastWeekHiringPatent;
                string thisWeekBuild = HqDisplayQueriesHud.ThisWeekConstructionSalariesSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekBuild))
                    status = string.IsNullOrEmpty(status) ? thisWeekBuild : status + " · " + thisWeekBuild;
                string thisWeekFinesPower = HqDisplayQueriesHud.ThisWeekFinesPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekFinesPower))
                    status = string.IsNullOrEmpty(status) ? thisWeekFinesPower : status + " · " + thisWeekFinesPower;
                string lastWeekChipDevLand = HqDisplayQueriesHud.LastWeekChipDevLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekChipDevLand))
                    status = string.IsNullOrEmpty(status) ? lastWeekChipDevLand : status + " · " + lastWeekChipDevLand;
                string thisWeekSalariesGpu = HqDisplayQueriesHud.ThisWeekSalariesGpuSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekSalariesGpu))
                    status = string.IsNullOrEmpty(status) ? thisWeekSalariesGpu : status + " · " + thisWeekSalariesGpu;
                string lastWeekPatentSite = HqDisplayQueriesHud.LastWeekPatentSiteSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekPatentSite))
                    status = string.IsNullOrEmpty(status) ? lastWeekPatentSite : status + " · " + lastWeekPatentSite;
                string lastWeekCapitalGrey = HqDisplayQueriesHud.LastWeekCapitalGreySummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekCapitalGrey))
                    status = string.IsNullOrEmpty(status) ? lastWeekCapitalGrey : status + " · " + lastWeekCapitalGrey;
                string lastWeekGrantsRevenue = HqDisplayQueriesHud.LastWeekGrantsRevenueSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGrantsRevenue))
                    status = string.IsNullOrEmpty(status) ? lastWeekGrantsRevenue : status + " · " + lastWeekGrantsRevenue;
                string thisWeekGpuFinesGrey = HqDisplayQueriesHud.ThisWeekGpuFinesGreySummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGpuFinesGrey))
                    status = string.IsNullOrEmpty(status) ? thisWeekGpuFinesGrey : status + " · " + thisWeekGpuFinesGrey;
                string lastWeekSalariesRoom = HqDisplayQueriesHud.LastWeekSalariesRoomSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekSalariesRoom))
                    status = string.IsNullOrEmpty(status) ? lastWeekSalariesRoom : status + " · " + lastWeekSalariesRoom;
                string lifetimeRevenueChip = HqDisplayQueriesHud.LifetimeRevenueChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeRevenueChip))
                    status = string.IsNullOrEmpty(status) ? lifetimeRevenueChip : status + " · " + lifetimeRevenueChip;
                string lastWeekGpuUpkeepRent = HqDisplayQueriesHud.LastWeekGpuUpkeepRentSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGpuUpkeepRent))
                    status = string.IsNullOrEmpty(status) ? lastWeekGpuUpkeepRent : status + " · " + lastWeekGpuUpkeepRent;
                string lastWeekRevenueNet = HqDisplayQueriesHud.LastWeekRevenueNetSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekRevenueNet))
                    status = string.IsNullOrEmpty(status) ? lastWeekRevenueNet : status + " · " + lastWeekRevenueNet;
                string thisWeekNetTraining = HqDisplayQueriesHud.ThisWeekNetTrainingSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekNetTraining))
                    status = string.IsNullOrEmpty(status) ? thisWeekNetTraining : status + " · " + thisWeekNetTraining;
                string thisWeekGrantsFines = HqDisplayQueriesHud.ThisWeekGrantsFinesSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGrantsFines))
                    status = string.IsNullOrEmpty(status) ? thisWeekGrantsFines : status + " · " + thisWeekGrantsFines;
                string lifetimePowerFines = HqDisplayQueriesHud.LifetimePowerFinesSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimePowerFines))
                    status = string.IsNullOrEmpty(status) ? lifetimePowerFines : status + " · " + lifetimePowerFines;
                string lastWeekGreyLaunder = HqDisplayQueriesHud.LastWeekGreyLaunderSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGreyLaunder))
                    status = string.IsNullOrEmpty(status) ? lastWeekGreyLaunder : status + " · " + lastWeekGreyLaunder;
                string vendorCourt = HqDisplayQueriesHud.LatestVendorCourtLine(sim.State, sim.Tick);
                if (!string.IsNullOrEmpty(vendorCourt))
                    status = string.IsNullOrEmpty(status) ? vendorCourt : status + " · " + vendorCourt;
                _statusLabel.text = status;
                _statusLabel.gameObject.SetActive(!string.IsNullOrEmpty(status));
            }
            if (_barImage != null) _barImage.color = HqFinancialHud.BarTint(lab);
        }

        public static HqHudView Create(Transform parent, SimulationHost host)
        {
            var canvasGo = new GameObject("HUD");
            canvasGo.transform.SetParent(parent, false);
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGo.AddComponent<GraphicRaycaster>();

            var panel = new GameObject("TopBar");
            panel.transform.SetParent(canvasGo.transform, false);
            var panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0.08f, 0.09f, 0.12f, 0.92f);
            var panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0f, 1f);
            panelRect.anchorMax = new Vector2(1f, 1f);
            panelRect.pivot = new Vector2(0.5f, 1f);
            panelRect.sizeDelta = new Vector2(0f, 96f);
            panelRect.anchoredPosition = Vector2.zero;

            var hud = panel.AddComponent<HqHudView>();
            hud._barImage = panelImage;
            hud._cashLabel = CreateLabel(panel.transform, "CashLabel", new Vector2(16f, -12f), TextAnchor.UpperLeft);
            hud._dateLabel = CreateLabel(panel.transform, "DateLabel", new Vector2(220f, -12f), TextAnchor.UpperLeft);
            hud._researchLabel = CreateLabel(panel.transform, "ResearchLabel", new Vector2(16f, -40f), TextAnchor.UpperLeft);
            hud._researchLabel.fontSize = 18;
            hud._statusLabel = CreateLabel(panel.transform, "StatusLabel", new Vector2(16f, -68f), TextAnchor.UpperLeft);
            hud._statusLabel.fontSize = 16;
            hud._statusLabel.color = new Color(1f, 0.7f, 0.45f);
            hud._phaseLabel = CreateLabel(panel.transform, "PhaseLabel", new Vector2(16f, -40f), TextAnchor.UpperRight);
            var phaseRect = hud._phaseLabel.GetComponent<RectTransform>();
            phaseRect.anchorMin = new Vector2(1f, 1f);
            phaseRect.anchorMax = new Vector2(1f, 1f);
            phaseRect.pivot = new Vector2(1f, 1f);
            phaseRect.anchoredPosition = new Vector2(-16f, -12f);
            phaseRect.sizeDelta = new Vector2(280f, 28f);
            hud._phaseLabel.alignment = TextAnchor.UpperRight;

            hud._eraLabel = CreateLabel(panel.transform, "EraLabel", new Vector2(-16f, -40f), TextAnchor.UpperRight);

            CreateTimeControls(canvasGo.transform, hud);
            CreateSaveLoadControls(canvasGo.transform, hud);
            hud.Bind(host);
            return hud;
        }

        static void CreateSaveLoadControls(Transform parent, HqHudView hud)
        {
            AddHudButton(parent, "Save game", new Vector2(24f, 24f), new Vector2(0f, 0f), () =>
                HqSaveLoad.SaveToFile(hud._host, HqSaveLoad.DefaultPath));
            AddHudButton(parent, "Load game", new Vector2(184f, 24f), new Vector2(0f, 0f), () =>
                HqSaveLoad.LoadFromFile(hud._host, HqSaveLoad.DefaultPath));
        }

        static void AddHudButton(Transform parent, string caption, Vector2 anchoredPos, Vector2 anchor, UnityEngine.Events.UnityAction onClick)
        {
            var go = new GameObject(caption);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0f, 0f);
            rect.sizeDelta = new Vector2(148f, 44f);
            rect.anchoredPosition = anchoredPos;

            var image = go.AddComponent<Image>();
            image.color = new Color(0.2f, 0.45f, 0.35f, 1f);
            var button = go.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(onClick);

            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(go.transform, false);
            var labelRect = labelGo.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            var label = labelGo.AddComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = 15;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Color.white;
            label.text = caption;
        }

        static Text CreateLabel(Transform parent, string name, Vector2 anchoredPos, TextAnchor align)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = align == TextAnchor.UpperRight ? new Vector2(1f, 1f) : new Vector2(0f, 1f);
            rect.anchorMax = align == TextAnchor.UpperRight ? new Vector2(1f, 1f) : new Vector2(0f, 1f);
            rect.pivot = align == TextAnchor.UpperRight ? new Vector2(1f, 1f) : new Vector2(0f, 1f);
            rect.sizeDelta = new Vector2(420f, 28f);
            rect.anchoredPosition = anchoredPos;
            var text = go.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 20;
            text.color = Color.white;
            text.alignment = align;
            return text;
        }

        static void CreateTimeControls(Transform parent, HqHudView hud)
        {
            float y = 24f;
            AddTimeButton(parent, hud, "Advance week", new Vector2(-24f, y), () => hud._host.StepWeek());
            AddTimeButton(parent, hud, "Advance month", new Vector2(-240f, y), () =>
            {
                int weeks = hud._host.Simulation.Config.Calendar.WeeksPerMonth;
                hud._host.StepWeeks(weeks);
            });
            AddTimeButton(parent, hud, "Advance year", new Vector2(-456f, y), () =>
            {
                int weeks = hud._host.Simulation.Config.Calendar.WeeksPerYear;
                hud._host.StepWeeks(weeks);
            });
        }

        static void AddTimeButton(Transform parent, HqHudView hud, string caption, Vector2 anchoredPos, UnityEngine.Events.UnityAction onClick)
        {
            var go = new GameObject(caption);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(1f, 0f);
            rect.sizeDelta = new Vector2(200f, 48f);
            rect.anchoredPosition = anchoredPos;

            var image = go.AddComponent<Image>();
            image.color = new Color(0.25f, 0.55f, 0.85f, 1f);
            var button = go.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(onClick);

            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(go.transform, false);
            var labelRect = labelGo.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            var label = labelGo.AddComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = 16;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Color.white;
            label.text = caption;
        }
    }
}