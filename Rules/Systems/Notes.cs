using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>A bridge note that converts to equity. See <see cref="NotesConfig"/>. Only the player's lab.</summary>
    public static class Notes
    {
        public static bool Enabled(GameConfig c) { return c.ConvertibleNotes.Enabled > 0; }

        public static bool Owing(LabState lab) { return lab.NoteOwed > 0; }

        public static int Rate(GameConfig c, int tier) { return tier == 1 ? c.ConvertibleNotes.RateA : tier == 2 ? c.ConvertibleNotes.RateB : c.ConvertibleNotes.RateC; }

        public static int Discount(GameConfig c, int tier) { return tier == 1 ? c.ConvertibleNotes.DiscountA : tier == 2 ? c.ConvertibleNotes.DiscountB : c.ConvertibleNotes.DiscountC; }

        public static int Weeks(GameConfig c, int tier) { return tier == 1 ? c.ConvertibleNotes.WeeksA : tier == 2 ? c.ConvertibleNotes.WeeksB : c.ConvertibleNotes.WeeksC; }

        public static void Issue(GameConfig c, GameState state, LabState lab, int tier, long principal)
        {
            lab.NoteOwed = principal;
            lab.NotePrincipal = principal;
            lab.NoteTier = tier;
            lab.NoteRate = Rate(c, tier);
            lab.NoteDiscount = Discount(c, tier);
            lab.NoteLeft = Weeks(c, tier);
            lab.NoteExtended = 0;
            Finance.Earn(lab, LedgerLine.CapitalIn, principal);
            Events.Add(state, EventKind.NoteIssued, lab.Id, tier, principal, "");
        }

        static void Clear(LabState lab)
        {
            lab.NoteOwed = 0;
            lab.NotePrincipal = 0;
            lab.NoteRate = 0;
            lab.NoteDiscount = 0;
            lab.NoteLeft = 0;
            lab.NoteExtended = 0;
            lab.NoteTier = 0;
        }

        public static void Repay(GameConfig c, GameState state, LabState lab)
        {
            long paid = lab.NoteOwed;
            Finance.Spend(lab, LedgerLine.CapitalIn, paid);
            Clear(lab);
            Events.Add(state, EventKind.NoteRepaid, lab.Id, -1, paid, "");
        }

        public static void Extend(GameConfig c, GameState state, LabState lab)
        {
            lab.NoteLeft += c.ConvertibleNotes.ExtendWeeks;
            lab.NoteRate = (int)MathI.MulPermille(lab.NoteRate, c.ConvertibleNotes.ExtendRate);
            lab.NoteDiscount = (int)MathI.Min(800, lab.NoteDiscount + c.ConvertibleNotes.ExtendDiscount);
            lab.NoteExtended = 1;
            Events.Add(state, EventKind.NoteExtended, lab.Id, -1, lab.NoteRate, "");
        }

        /// <summary>The share of the lab the note becomes: what is owed over the value, which the discount makes smaller.</summary>
        public static int Share(GameConfig c, long owed, long value, int discount)
        {
            long price = MathI.Max(1, MathI.MulPermille(value, MathI.Permille - discount));
            return (int)MathI.Min(c.ConvertibleNotes.MaxConvertPermille, owed * MathI.Permille / price);
        }

        static void Convert(GameConfig c, GameState state, LabState lab, int share, int pick)
        {
            long owed = lab.NoteOwed;
            lab.FounderSharePermille = (int)MathI.MulPermille(lab.FounderSharePermille, MathI.Permille - share);
            StaffEquity.OnRaise(lab, share);
            Clear(lab);
            Events.Add(state, EventKind.NoteConverted, lab.Id, share, owed, "").Pick = pick;
            FundingSystem.TryOust(c, state, lab);
        }

        /// <summary>A round is closing: the note converts at the round's price, less its discount. Returns the dilution for the founders, the round's and the note's together.</summary>
        public static int OnRaise(GameConfig c, GameState state, LabState lab, long cash, int dilution)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || !Owing(lab) || cash <= 0 || dilution <= 0) return dilution;
            long value = cash * MathI.Permille / dilution;
            int share = Share(c, lab.NoteOwed, value, lab.NoteDiscount);
            long owed = lab.NoteOwed;
            Clear(lab);
            Events.Add(state, EventKind.NoteConverted, lab.Id, share, owed, "").Pick = 0;
            return (int)MathI.Min(MathI.Permille - 1, dilution + MathI.MulPermille(MathI.Permille - dilution, share));
        }

        /// <summary>The lab has listed: the note converts at the listing price, less its discount.</summary>
        public static void OnListing(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || !Owing(lab)) return;
            Convert(c, state, lab, Share(c, lab.NoteOwed, Listing.Value(c, lab), lab.NoteDiscount), 1);
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || !Owing(lab)) return;
            lab.NoteOwed += MathI.MulPermille(lab.NoteOwed, lab.NoteRate);
            lab.NoteLeft--;
            if (lab.NoteLeft > 0) return;
            long value = MathI.Max(Listing.Value(c, lab), lab.NoteOwed * 2);
            Convert(c, state, lab, Share(c, lab.NoteOwed, value, c.ConvertibleNotes.MaturityDiscount), 2);
        }
    }
}
