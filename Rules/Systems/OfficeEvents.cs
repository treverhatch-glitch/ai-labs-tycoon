using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Office events with a choice. See <see cref="OfficeConfig"/>. Only the player's labs have them.</summary>
    public static class OfficeEvents
    {
        public struct Pending
        {
            public OfficeEventConfig Event;
            public int WeeksLeft;
        }

        static bool Has(LabState lab) { return lab.Controller == LabController.Player; }

        public static bool IsPending(LabState lab) { return !string.IsNullOrEmpty(lab.OfficeEventId); }

        /// <summary>The event waiting for an answer and how long it will wait, or a default value (Event null) when none is.</summary>
        public static Pending Waiting(GameConfig c, LabState lab, int tick)
        {
            var p = new Pending();
            if (!IsPending(lab)) return p;
            p.Event = c.Office.Event(lab.OfficeEventId);
            p.WeeksLeft = (int)MathI.Max(0, lab.OfficeEventExpires - tick);
            return p;
        }

        public static bool MeetsRequirement(GameConfig c, LabState lab, OfficeRequires need)
        {
            switch (need)
            {
                case OfficeRequires.ReleasedModel:
                    for (int i = 0; i < lab.Models.Count; i++) if (lab.Models[i].Status == ModelStatus.Released) return true;
                    return false;
                case OfficeRequires.SafetyStaff: return LabQueries.StaffByFunction(c, lab, RoleFunction.Safety) > 0;
                case OfficeRequires.AnyStaff: return lab.Staff.Count > 0;
                default: return true;
            }
        }

        static long Mix(GameState state, LabState lab, int salt)
        {
            long mix = (state.Seed * 31 + state.Tick) * 31 + lab.Id * 17 + salt;
            mix = (mix & 0x7FFFFFFFL) * 2654435761L & 0xFFFFFFFFL;
            return mix >> 8;
        }

        /// <summary>The events that could happen now: their needs are met and the lab has not just seen them. Once all have been seen the list starts again.</summary>
        public static List<OfficeEventConfig> Eligible(GameConfig c, LabState lab, out bool startsAgain)
        {
            var fresh = new List<OfficeEventConfig>();
            var any = new List<OfficeEventConfig>();
            for (int i = 0; i < c.Office.Events.Count; i++)
            {
                OfficeEventConfig e = c.Office.Events[i];
                if (!MeetsRequirement(c, lab, e.Requires)) continue;
                any.Add(e);
                if (!lab.OfficeSeen.Contains(e.IdValue)) fresh.Add(e);
            }
            startsAgain = fresh.Count == 0;
            return startsAgain ? any : fresh;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Has(lab) || c.Office.Events.Count == 0) return;
            if (IsPending(lab))
            {
                if (state.Tick + 1 >= lab.OfficeEventExpires) Lapse(state, lab);
                return;
            }
            if (state.Tick < c.Office.FirstAfterWeeks) return;
            if (Mix(state, lab, 1) % MathI.Permille >= c.Office.ChancePermille) return;
            bool startsAgain;
            List<OfficeEventConfig> options = Eligible(c, lab, out startsAgain);
            if (options.Count == 0) return;
            OfficeEventConfig chosen = options[(int)(Mix(state, lab, 2) % options.Count)];
            lab.OfficeEventId = chosen.IdValue;
            lab.OfficeEventExpires = state.Tick + c.Office.ExpireWeeks;
            if (startsAgain) lab.OfficeSeen.Clear();
            lab.OfficeSeen.Add(chosen.IdValue);
            Events.Add(state, EventKind.OfficeEventPosed, lab.Id, -1, 0, chosen.IdValue);
        }

        static void Lapse(GameState state, LabState lab)
        {
            Events.Add(state, EventKind.OfficeEventLapsed, lab.Id, -1, 0, lab.OfficeEventId);
            lab.OfficeEventId = "";
            lab.OfficeEventExpires = 0;
        }

        public static long WeekCost(GameConfig c, GameState state, LabState lab)
        {
            return MathI.Max(c.Office.MinWeekCost, LabQueries.WeeklyRunningCost(c, state, lab));
        }

        /// <summary>Applies the answer, clears the event and writes it to the log.</summary>
        public static void Answer(GameConfig c, GameState state, LabState lab, OfficeEventConfig e, OfficeChoiceConfig choice)
        {
            for (int i = 0; i < choice.Effects.Count; i++) Apply(c, state, lab, choice.Effects[i]);
            lab.OfficeEventId = "";
            lab.OfficeEventExpires = 0;
            Events.Add(state, EventKind.OfficeEventAnswered, lab.Id, -1, 0, e.IdValue + "|" + choice.IdValue);
        }

        static void Apply(GameConfig c, GameState state, LabState lab, OfficeEffectConfig effect)
        {
            switch (effect.Kind)
            {
                case OfficeEffectKind.Reputation:
                    lab.Reputation = (int)MathI.Clamp((long)lab.Reputation + effect.Amount, 0, MathI.Permille);
                    break;
                case OfficeEffectKind.Culture:
                    SafetyCulture.Move(c, lab, effect.Amount);
                    break;
                case OfficeEffectKind.Morale:
                    for (int i = 0; i < lab.Staff.Count; i++)
                        lab.Staff[i].Morale = (int)MathI.Clamp((long)lab.Staff[i].Morale + effect.Amount, 0, MathI.Permille);
                    break;
                case OfficeEffectKind.ResearchWeeks:
                    lab.ResearchPoints = MathI.Max(0, lab.ResearchPoints + effect.Amount * TrainingSystem.ResearchPerWeek(c, lab, state.Tick));
                    break;
                case OfficeEffectKind.CashWeeks:
                    long cash = effect.Amount * WeekCost(c, state, lab);
                    if (cash >= 0) Finance.Earn(lab, LedgerLine.Grants, cash);
                    else Finance.Spend(lab, LedgerLine.Hiring, MathI.Min(-cash, MathI.Max(0, lab.Cash)));
                    break;
            }
        }
    }
}
