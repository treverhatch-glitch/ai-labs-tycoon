using System.Collections.Generic;
using System.Text;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>
    /// The decision logs: what a rival (or the vendor) did and the numbers it did it on. Entries are data; the sentence
    /// is written by <see cref="DecisionText"/> from templates in the config.
    /// </summary>
    public static class DecisionLog
    {
        /// <summary>Only labs run by a plan keep a log: the player's own lab (or a bot driving it) is not written about, so a replay of its journal gives the same game.</summary>
        public static void Add(GameConfig c, LabState lab, int tick, DecisionAction action, string reference, long amount, params Reason[] reasons)
        {
            Add(c, null, lab, tick, action, reference, amount, reasons);
        }

        public static void Add(GameConfig c, GameState state, LabState lab, int tick, DecisionAction action, string reference, long amount, params Reason[] reasons)
        {
            if (lab.Controller != LabController.Plan) return;
            lab.Log.Add(Entry(tick, action, reference, amount, reasons));
            if (state != null) RivalIntentNews.OnDecision(c, state, lab, tick, action, reference);
            int extra = lab.Log.Count - c.Rivalry.LogMax;
            if (extra > 0) lab.Log.RemoveRange(0, extra);
        }

        public static void AddVendor(GameConfig c, GameState state, DecisionAction action, string reference, long amount, params Reason[] reasons)
        {
            List<DecisionEntry> log = state.Vendor.Log;
            log.Add(Entry(state.Tick, action, reference, amount, reasons));
            int extra = log.Count - c.Vendor.LogMax;
            if (extra > 0) log.RemoveRange(0, extra);
        }

        static DecisionEntry Entry(int tick, DecisionAction action, string reference, long amount, Reason[] reasons)
        {
            var e = new DecisionEntry();
            e.Tick = tick;
            e.Action = action;
            e.Ref = reference ?? "";
            e.Amount = amount;
            if (reasons != null) e.Reasons.AddRange(reasons);
            return e;
        }

        /// <summary>The quarter a tick falls in, counted from the first week of the game.</summary>
        public static int QuarterOf(GameConfig c, int tick) { return tick / (c.Calendar.WeeksPerMonth * 3); }

        /// <summary>Whether the lab already has a line for this action and thing in the quarter. "Skipped" and "held back" are written at most once per line per quarter.</summary>
        public static bool LoggedThisQuarter(GameConfig c, LabState lab, DecisionAction action, string reference, int tick)
        {
            int quarter = QuarterOf(c, tick);
            for (int i = lab.Log.Count - 1; i >= 0; i--)
            {
                DecisionEntry e = lab.Log[i];
                if (QuarterOf(c, e.Tick) != quarter) return false;
                if (e.Action == action && e.Ref == reference) return true;
            }
            return false;
        }

        /// <summary>The newest entry of an action, or null.</summary>
        public static DecisionEntry Last(LabState lab, DecisionAction action)
        {
            for (int i = lab.Log.Count - 1; i >= 0; i--) if (lab.Log[i].Action == action) return lab.Log[i];
            return null;
        }
    }

    /// <summary>Writes a decision as a sentence, from the templates in the config's rivalry section.</summary>
    public static class DecisionText
    {
        /// <summary>One line, dated: "2024 M3 W1: ChatGBT Labs started training Mid 4 (forecast gain 12%; runway 14 weeks)". <paramref name="lab"/> is null for the vendor's log.</summary>
        public static string Line(GameConfig c, GameState state, LabState lab, DecisionEntry e)
        {
            string who = lab == null ? c.Compute.VendorName : lab.Name;
            string action = c.Rivalry.ActionTemplate(e.Action.ToString()) ?? "{lab}: " + e.Action;
            string text = action
                .Replace("{lab}", who)
                .Replace("{ref}", RefName(c, state, e))
                .Replace("{amount}", e.Amount.ToString());

            var reasons = new StringBuilder();
            for (int i = 0; i < e.Reasons.Count; i++)
            {
                Reason r = e.Reasons[i];
                string phrase = c.Rivalry.ReasonTemplate(r.Code.ToString()) ?? r.Code.ToString() + " {value}";
                if (reasons.Length > 0) reasons.Append("; ");
                reasons.Append(phrase.Replace("{value}", r.Value.ToString()).Replace("{pct}", Percent(r.Value)));
            }
            string line = GameDate.Label(c.Calendar, e.Tick) + ": " + text;
            if (reasons.Length > 0) line += " (" + reasons + ")";
            return line;
        }

        /// <summary>Permille as a percentage with one decimal when needed: 125 is "12.5", 120 is "12", -30 is "-3".</summary>
        public static string Percent(int permille)
        {
            int abs = permille < 0 ? -permille : permille;
            string sign = permille < 0 ? "-" : "";
            return abs % 10 == 0 ? sign + (abs / 10) : sign + (abs / 10) + "." + (abs % 10);
        }

        /// <summary>The thing an entry is about, as a name when the reference is a config id or a lab id.</summary>
        public static string RefName(GameConfig c, GameState state, DecisionEntry e)
        {
            string r = e.Ref;
            switch (e.Action)
            {
                case DecisionAction.BoughtLand:
                case DecisionAction.BuiltHall:
                    { SiteConfig site = c.World.Site(r); return site == null ? r : site.Name; }
                case DecisionAction.Hired:
                    { RoleConfig role = c.Role(r); return role == null ? r : role.Name; }
                case DecisionAction.Built:
                    {
                        RoomTypeConfig type = c.RoomType(r);
                        if (type != null) return type.Name;
                        LotConfig lot = c.Lot(r);
                        return lot == null ? r : lot.Name;
                    }
                case DecisionAction.StartedRun:
                case DecisionAction.SkippedRun:
                    { ModelLineConfig line = c.Line(r); return line == null ? r : line.Name; }
                case DecisionAction.Published:
                    { TechConfig tech = c.Tech(r); return tech == null ? r : tech.Name; }
                case DecisionAction.BoughtGpus:
                case DecisionAction.Replaced:
                    { GpuGenerationConfig gen = c.Compute.Generation(r); return gen == null ? r : gen.Name; }
                case DecisionAction.Lobbied:
                    { BlocConfig bloc = c.World.Bloc(r); return bloc == null ? r : bloc.Name; }
                case DecisionAction.ShiftedAllocation:
                case DecisionAction.Courted:
                    {
                        int id;
                        if (int.TryParse(r, out id))
                        {
                            LabState lab = state.Lab(id);
                            if (lab != null) return lab.Name;
                        }
                        return r;
                    }
                default: return r;
            }
        }
    }
}
