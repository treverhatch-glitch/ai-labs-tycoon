using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Staff equity. See <see cref="EquityConfig"/>. Only the player's lab grants it.</summary>
    public static class StaffEquity
    {
        public static bool Enabled(GameConfig c) { return c.Equity.MaxLevel > 0; }

        public static int Level(GameConfig c, LabState lab) { return (int)MathI.Clamp(lab.EquityLevel, 0, c.Equity.MaxLevel); }

        /// <summary>What every salary is cut by, in permille.</summary>
        public static int SalaryCutPermille(GameConfig c, LabState lab) { return (int)(Level(c, lab) * (long)c.Equity.SalaryCutPermille); }

        public static int MoraleOffset(GameConfig c, LabState lab) { return (int)(Level(c, lab) * (long)c.Equity.MoraleStep); }

        public static int PoachDefencePermille(GameConfig c, LabState lab) { return (int)(Level(c, lab) * (long)c.Equity.PoachStepPermille); }

        /// <summary>What the next level would take off the founder's stake: at least 1 permille.</summary>
        public static int NextGrant(GameConfig c, LabState lab)
        {
            return (int)MathI.Max(1, MathI.MulPermille(lab.FounderSharePermille, c.Equity.DilutionPermille));
        }

        /// <summary>The founder's stake after the next level, which must stay above the funding data's control line.</summary>
        public static int FounderAfterNext(GameConfig c, LabState lab) { return lab.FounderSharePermille - NextGrant(c, lab); }

        /// <summary>What the people own when the lab is sold, out of a price.</summary>
        public static long StaffShare(LabState lab, long price)
        {
            return price <= 0 ? 0 : MathI.MulPermille(price, lab.EquityPermille);
        }

        public static void Grant(GameConfig c, GameState state, LabState lab)
        {
            int grant = NextGrant(c, lab);
            lab.FounderSharePermille -= grant;
            lab.EquityPermille += grant;
            lab.EquityLevel++;
            Events.Add(state, EventKind.EquityGranted, lab.Id, lab.EquityLevel, lab.EquityPermille, "");
        }

        /// <summary>A funding round dilutes the people's stake as it dilutes the founder's.</summary>
        public static void OnRaise(LabState lab, int dilutionPermille)
        {
            if (lab.EquityPermille > 0) lab.EquityPermille = (int)MathI.Max(1, MathI.MulPermille(lab.EquityPermille, MathI.Permille - dilutionPermille));
        }
    }
}
