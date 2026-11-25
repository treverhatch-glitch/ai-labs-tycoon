using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>The knobs an era can turn. Each is an era field in the config, read from the calendar; none is stored in a save.</summary>
    public enum EraRuleKind
    {
        TrainingCost, ResearchSpeed, IncidentLoad, HireFee, ChipPrice, Adoption, Fine, ReleaseBar
    }

    /// <summary>One rule of the era: what it is now, what it was the era before, and what it was in the first era.</summary>
    public struct EraRuleLine
    {
        public EraRuleKind Kind;
        public int Value;
        public int Previous;
        public int Baseline;
        /// <summary>True when the era just changed this rule from the one before it.</summary>
        public bool Shifted { get { return Value != Previous; } }
    }

    /// <summary>
    /// "The rules shift" with every era. The era table carries a handful of multipliers and one law (a floor on the
    /// alignment of any model put on sale). Every system that is touched asks here, so one place says what the
    /// current era does and the screens can list it. Nothing is stored: the year picks the era.
    /// </summary>
    public static class EraRules
    {
        public static int Value(EraConfig era, EraRuleKind kind)
        {
            switch (kind)
            {
                case EraRuleKind.TrainingCost: return era.TrainingCostPermille;
                case EraRuleKind.ResearchSpeed: return era.ResearchPermille;
                case EraRuleKind.IncidentLoad: return era.IncidentLoadPermille;
                case EraRuleKind.HireFee: return era.HireFeePermille;
                case EraRuleKind.ChipPrice: return era.ChipPricePermille;
                case EraRuleKind.Adoption: return era.AdoptionPermille;
                case EraRuleKind.Fine: return era.FinePermille;
                default: return era.MinReleaseAlignment;
            }
        }

        public static int Value(GameConfig c, int tick, EraRuleKind kind) { return Value(EraSystem.AtTick(c, tick), kind); }

        /// <summary>The one-off fee to hire this many people of a role in this era.</summary>
        public static long HireCost(GameConfig c, RoleConfig role, int count, int tick)
        {
            return HireFee(c, role, tick) * count;
        }

        public static long HireFee(GameConfig c, RoleConfig role, int tick)
        {
            return MathI.MulPermille(role.HireCost, EraSystem.AtTick(c, tick).HireFeePermille);
        }

        /// <summary>A price list figure for chips, scaled by the era's shortage or glut.</summary>
        public static long ChipPrice(GameConfig c, long price, int tick)
        {
            return MathI.MulPermille(price, EraSystem.AtTick(c, tick).ChipPricePermille);
        }

        /// <summary>An incident fine, scaled by how hard the era's law bites.</summary>
        public static long Fine(GameConfig c, long fine, int tick)
        {
            return MathI.MulPermille(fine, EraSystem.AtTick(c, tick).FinePermille);
        }

        /// <summary>A sector's adoption after the era's appetite, capped at the whole sector.</summary>
        public static int Adoption(GameConfig c, long rawPermille, int tick)
        {
            return (int)MathI.Min(MathI.Permille, MathI.MulPermille(rawPermille, EraSystem.AtTick(c, tick).AdoptionPermille));
        }

        /// <summary>The alignment a model must show before it can go on sale, or 0 when the era has no such law.</summary>
        public static int ReleaseBar(GameConfig c, int tick) { return EraSystem.AtTick(c, tick).MinReleaseAlignment; }

        public static int ReleaseBar(GameConfig c, GameState state, LabState lab, int tick)
        {
            int bar = ReleaseBar(c, tick);
            if (lab != null && state != null && lab.Id == state.PlayerLabId)
                bar = (int)MathI.Clamp(bar + LawShaping.ReleaseBarDelta(c, lab, tick), 0, MathI.Permille);
            return bar;
        }

        /// <summary>True when the model clears the era's law. What counts is the alignment the lab can show, so a lab that sandbags passes on paper.</summary>
        public static bool MeetsReleaseBar(GameConfig c, ModelState model, int tick)
        {
            return SafetySystem.DisplayedAlignment(model) >= ReleaseBar(c, tick);
        }

        public static bool MeetsReleaseBar(GameConfig c, GameState state, LabState lab, ModelState model, int tick)
        {
            return SafetySystem.DisplayedAlignment(model) >= ReleaseBar(c, state, lab, tick);
        }

        /// <summary>Every rule of the era at this tick, in a fixed order, with the era before it and the first era for comparison.</summary>
        public static List<EraRuleLine> Briefing(GameConfig c, int tick)
        {
            int at = EraSystem.IndexAtYear(c, GameDate.Year(c.Calendar, tick));
            EraConfig era = c.Eras[at];
            EraConfig before = c.Eras[at > 0 ? at - 1 : 0];
            EraConfig first = c.Eras[0];
            var lines = new List<EraRuleLine>();
            for (int k = 0; k <= (int)EraRuleKind.ReleaseBar; k++)
            {
                var kind = (EraRuleKind)k;
                var line = new EraRuleLine();
                line.Kind = kind;
                line.Value = Value(era, kind);
                line.Previous = Value(before, kind);
                line.Baseline = Value(first, kind);
                lines.Add(line);
            }
            return lines;
        }

        /// <summary>Only the rules the era changed from the era before it: the "what's new" list for the era banner.</summary>
        public static List<EraRuleLine> Shifts(GameConfig c, int tick)
        {
            var shifts = new List<EraRuleLine>();
            List<EraRuleLine> all = Briefing(c, tick);
            for (int i = 0; i < all.Count; i++) if (all[i].Shifted) shifts.Add(all[i]);
            return shifts;
        }

        public static string Label(EraRuleKind kind)
        {
            switch (kind)
            {
                case EraRuleKind.TrainingCost: return "Training runs cost";
                case EraRuleKind.ResearchSpeed: return "Research speed";
                case EraRuleKind.IncidentLoad: return "Incident chance";
                case EraRuleKind.HireFee: return "Hiring fees";
                case EraRuleKind.ChipPrice: return "Chip prices";
                case EraRuleKind.Adoption: return "Customer appetite";
                case EraRuleKind.Fine: return "Fines";
                default: return "Alignment needed to go on sale";
            }
        }

        /// <summary>A rule as a short phrase: "x1.5" for a multiplier, "at least 80" for the law's floor.</summary>
        public static string Text(EraRuleKind kind, int value)
        {
            if (kind == EraRuleKind.ReleaseBar) return value <= 0 ? "no rule" : "at least " + value;
            int tenths = (value % 1000) / 100;
            int hundredths = (value % 100) / 10;
            string text = "x" + (value / 1000);
            if (tenths > 0 || hundredths > 0) text += "." + tenths + (hundredths > 0 ? hundredths.ToString() : "");
            return text;
        }
    }
}
