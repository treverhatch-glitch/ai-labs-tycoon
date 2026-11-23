using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class ScenarioBaggage
    {
        public static bool Enabled(GameConfig c) { return c.ScenarioBaggage.Enabled > 0; }

        public static void ApplyNewGame(GameConfig c, GameState state, LabState lab, string baggageId)
        {
            if (!Enabled(c) || string.IsNullOrEmpty(baggageId)) return;
            BaggageItemConfig item = c.ScenarioBaggage.Item(baggageId);
            if (item == null) return;
            lab.BaggageId = baggageId;
            lab.BaggagePicked = 1;
            ApplyOnce(c, state, lab, item);
        }

        public static void Pick(GameConfig c, GameState state, LabState lab, string baggageId)
        {
            if (!Enabled(c)) return;
            if (lab.BaggagePicked > 0) return;
            BaggageItemConfig item = c.ScenarioBaggage.Item(baggageId);
            if (item == null) return;
            lab.BaggageId = baggageId;
            lab.BaggagePicked = 1;
            ApplyOnce(c, state, lab, item);
            Events.Add(state, EventKind.ScenarioBaggagePicked, lab.Id, -1, 0, baggageId);
        }

        static void ApplyOnce(GameConfig c, GameState state, LabState lab, BaggageItemConfig item)
        {
            if (item.Kind == "debt")
            {
                if (item.LoanPrincipal > 0 && lab.Loans.Count < c.Loans.MaxOpen)
                {
                    long room = Loans.Capacity(c, state, lab);
                    if (room >= item.LoanPrincipal) Loans.Take(c, state, lab, item.LoanPrincipal);
                }
                if (item.NotePrincipal > 0 && Notes.Enabled(c) && !Notes.Owing(lab))
                    Notes.Issue(c, state, lab, 1, item.NotePrincipal);
                if (item.RbfAdvance > 0 && RevenueFinancing.Enabled(c) && !RevenueFinancing.Owing(lab))
                    RevenueFinancing.Take(c, state, lab, 1, item.RbfAdvance);
            }
            else if (item.Kind == "lawsuit")
            {
                if (item.CaseClaim > 0) Litigation.File(c, state, lab, Litigation.Incident, item.CaseClaim, item.CaseStrength, item.Name);
            }
            else if (item.Kind == "leak")
            {
                if (item.ReputationLoss > 0)
                    lab.Reputation = (int)MathI.Max(0, lab.Reputation - item.ReputationLoss);
                if (item.TrustLoss > 0) TrustSystem.OnBaggageLeak(c, state, lab, item.TrustLoss);
            }
        }
    }
}