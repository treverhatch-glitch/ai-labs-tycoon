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
                case OfficeRequires.OwnChip:
                    for (int i = 0; i < lab.ChipDesigns.Count; i++) if (lab.ChipDesigns[i].Status == ChipDesignStatus.Ready) return true;
                    return false;
                case OfficeRequires.RentedCompute: return lab.RentedGpus > 0;
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
        public static List<OfficeEventConfig> Eligible(GameConfig c, LabState lab, out bool startsAgain, int tick = -1)
        {
            var fresh = new List<OfficeEventConfig>();
            var any = new List<OfficeEventConfig>();
            for (int i = 0; i < c.Office.Events.Count; i++)
            {
                OfficeEventConfig e = c.Office.Events[i];
                if (e.Trigger != OfficeTrigger.Random) continue;
                if (!MeetsRequirement(c, lab, e.Requires)) continue;
                if (tick >= 0 && !InYears(c, e, tick)) continue;
                any.Add(e);
                if (!lab.OfficeSeen.Contains(e.IdValue)) fresh.Add(e);
            }
            startsAgain = fresh.Count == 0;
            return startsAgain ? any : fresh;
        }

        /// <summary>True when the event's years (if it has any) take in the calendar year of this tick.</summary>
        public static bool InYears(GameConfig c, OfficeEventConfig e, int tick)
        {
            int year = GameDate.Year(c.Calendar, tick);
            return (e.FromYear <= 0 || year >= e.FromYear) && (e.ToYear <= 0 || year <= e.ToYear);
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Has(lab) || c.Office.Events.Count == 0) return;
            if (IsPending(lab))
            {
                if (state.Tick + 1 >= lab.OfficeEventExpires) Lapse(c, state, lab);
                return;
            }
            if (state.Tick < c.Office.FirstAfterWeeks) return;
            if (Mix(state, lab, 1) % MathI.Permille >= c.Office.ChancePermille) return;
            bool startsAgain;
            List<OfficeEventConfig> options = Eligible(c, lab, out startsAgain, state.Tick);
            if (options.Count == 0) return;
            OfficeEventConfig chosen = options[(int)(Mix(state, lab, 2) % options.Count)];
            lab.OfficeEventId = chosen.IdValue;
            lab.OfficeEventExpires = state.Tick + c.Office.ExpireWeeks;
            if (startsAgain) lab.OfficeSeen.Clear();
            lab.OfficeSeen.Add(chosen.IdValue);
            Events.Add(state, EventKind.OfficeEventPosed, lab.Id, -1, 0, chosen.IdValue);
        }

        /// <summary>The hearing questions: the events a hearing at home can pose.</summary>
        public static List<OfficeEventConfig> HearingQuestions(GameConfig c) { return WithTrigger(c, OfficeTrigger.Hearing); }

        static List<OfficeEventConfig> WithTrigger(GameConfig c, OfficeTrigger trigger)
        {
            var list = new List<OfficeEventConfig>();
            for (int i = 0; i < c.Office.Events.Count; i++)
                if (c.Office.Events[i].Trigger == trigger) list.Add(c.Office.Events[i]);
            return list;
        }

        /// <summary>
        /// A hearing at the player's home government has begun: the committee asks a question, taking the place of whatever was waiting
        /// (that lapses). Rotates through the questions so the same one is not asked twice in a row. Does nothing for rivals, for a hearing
        /// abroad, or when the data has no hearing questions.
        /// </summary>
        public static void OnHearing(GameConfig c, GameState state, LabState lab, string blocId)
        {
            if (!Has(lab) || blocId != lab.HomeBlocId) return;
            PoseTriggered(c, state, lab, OfficeTrigger.Hearing);
        }

        /// <summary>One of the player's models tried to escape: the crisis takes the place of whatever was waiting. Does nothing for rivals or when the data has no escape events.</summary>
        public static void OnEscape(GameConfig c, GameState state, LabState lab)
        {
            if (!Has(lab)) return;
            PoseTriggered(c, state, lab, OfficeTrigger.Escape);
        }

        /// <summary>Released capability and corruption crossed the takeoff bar. Rivals have none.</summary>
        public static void OnTakeoff(GameConfig c, GameState state, LabState lab)
        {
            if (!Has(lab)) return;
            int baseWeeks = c.Office.ExpireWeeks;
            int expire = TakeoffCrisisEraGrace.AnswerWeeks(c, state);
            PoseTriggered(c, state, lab, OfficeTrigger.Takeoff, expire);
            TakeoffCrisisEraGrace.OnPosed(c, state, lab, expire - baseWeeks);
        }

        public static void OnRungSix(GameConfig c, GameState state, LabState lab)
        {
            if (!Has(lab)) return;
            List<OfficeEventConfig> all = WithTrigger(c, OfficeTrigger.RungSix);
            if (all.Count == 0) all = WithTrigger(c, OfficeTrigger.ShutdownCompute);
            if (all.Count == 0) return;
            OfficeEventConfig chosen = all[(int)(Mix(state, lab, 7) % all.Count)];
            if (IsPending(lab)) Lapse(c, state, lab);
            lab.OfficeEventId = chosen.IdValue;
            lab.OfficeEventExpires = state.Tick + c.Office.ExpireWeeks;
            lab.OfficeSeen.Add(chosen.IdValue);
            Events.Add(state, EventKind.OfficeEventPosed, lab.Id, -1, 0, chosen.IdValue);
        }

        /// <summary>A model went on sale for the first time: the release notes need an answer, unless something else is already waiting. Rivals have none.</summary>
        public static void OnRelease(GameConfig c, GameState state, LabState lab)
        {
            if (!Has(lab) || IsPending(lab)) return;
            int baseWeeks = c.Office.ExpireWeeks;
            int expire = OfficeReleaseEraGrace.AnswerWeeks(c, state);
            PoseTriggered(c, state, lab, OfficeTrigger.Release, expire);
            OfficeReleaseEraGrace.OnPosed(c, state, lab, expire - baseWeeks);
        }

        static void PoseTriggered(GameConfig c, GameState state, LabState lab, OfficeTrigger trigger, int expireWeeks = -1)
        {
            List<OfficeEventConfig> all = WithTrigger(c, trigger);
            if (all.Count == 0) return;
            var fresh = new List<OfficeEventConfig>();
            for (int i = 0; i < all.Count; i++) if (!lab.OfficeSeen.Contains(all[i].IdValue)) fresh.Add(all[i]);
            if (fresh.Count == 0)
            {
                for (int i = 0; i < all.Count; i++) lab.OfficeSeen.Remove(all[i].IdValue);
                fresh = all;
            }
            OfficeEventConfig chosen = fresh[(int)(Mix(state, lab, 3) % fresh.Count)];
            if (IsPending(lab)) Lapse(c, state, lab);
            lab.OfficeEventId = chosen.IdValue;
            int weeks = expireWeeks >= 0 ? expireWeeks : c.Office.ExpireWeeks;
            lab.OfficeEventExpires = state.Tick + weeks;
            lab.OfficeSeen.Add(chosen.IdValue);
            Events.Add(state, EventKind.OfficeEventPosed, lab.Id, -1, 0, chosen.IdValue);
        }

        static void Lapse(GameConfig c, GameState state, LabState lab)
        {
            OfficeEventConfig e = c.Office.Event(lab.OfficeEventId);
            OfficeChoiceConfig fallback = e == null || string.IsNullOrEmpty(e.LapseChoice) ? null : e.Choice(e.LapseChoice);
            Events.Add(state, EventKind.OfficeEventLapsed, lab.Id, -1, 0, fallback == null ? lab.OfficeEventId : lab.OfficeEventId + "|" + fallback.IdValue);
            if (fallback != null) for (int i = 0; i < fallback.Effects.Count; i++) Apply(c, state, lab, fallback.Effects[i]);
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
            HearingSharePrice.OnAnswered(c, state, lab, e, choice);
            if (e.Trigger == OfficeTrigger.Escape || e.Trigger == OfficeTrigger.Takeoff)
                RungSixCrisis.OnArcAnswered(c, state, lab, e.Trigger);
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
                case OfficeEffectKind.Attention:
                    BlocStanding home = lab.Bloc(lab.HomeBlocId);
                    if (home != null) home.Attention = (int)MathI.Clamp((long)home.Attention + effect.Amount, 0, MathI.Permille);
                    break;
                case OfficeEffectKind.Corruption:
                    for (int i = 0; i < lab.Models.Count; i++)
                        if (lab.Models[i].Status == ModelStatus.Released)
                            lab.Models[i].Corruption = (int)MathI.Clamp((long)lab.Models[i].Corruption + effect.Amount, 0, MathI.Permille);
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
