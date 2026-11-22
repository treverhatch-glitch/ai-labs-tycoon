using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using UnityEngine;

namespace AiLabs.Unity.Hq
{
    /// <summary>HUD colours and warnings from rules lab cash / bankruptcy flags only.</summary>
    public static class HqFinancialHud
    {
        public static Color CashColor(LabState lab)
        {
            if (lab.Bankrupt) return new Color(0.95f, 0.35f, 0.35f);
            if (lab.Cash <= 0) return new Color(1f, 0.65f, 0.35f);
            return Color.white;
        }

        public static string StatusLine(LabState lab)
        {
            if (lab.Bankrupt) return "Bankrupt — retrench or restart.";
            if (lab.Cash <= 0) return "Out of cash.";
            if (lab.WeeksInDebt > 0) return "In debt (" + lab.WeeksInDebt + " wk).";
            return "";
        }

        public static Color BarTint(LabState lab)
        {
            if (lab.Bankrupt) return new Color(0.22f, 0.08f, 0.08f, 0.94f);
            if (lab.Cash <= 0) return new Color(0.2f, 0.12f, 0.08f, 0.94f);
            return new Color(0.08f, 0.09f, 0.12f, 0.92f);
        }

        public static float FloorPolishPermille(GameConfig config, LabState lab, string eraId)
        {
            long start = config.PlayerStart != null ? config.PlayerStart.Cash : 1;
            if (start < 1) start = 1;
            int eraBoost = HqCampusPhase.EraIndex(config, eraId) * 80;
            int cashBoost = (int)Mathf.Clamp(lab.Cash * 1000L / start, 0, 400);
            return eraBoost + cashBoost;
        }
    }
}