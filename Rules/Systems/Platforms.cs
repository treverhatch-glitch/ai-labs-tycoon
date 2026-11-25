using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>A distribution deal with a platform. See <see cref="PlatformsConfig"/>. Only the player's lab.</summary>
    public static class Platforms
    {
        public static bool Enabled(GameConfig c) { return c.Platforms.Enabled > 0; }

        public static bool Active(LabState lab) { return lab.PlatPlat > 0; }

        public static int Era(GameConfig c, int plat) { return plat == 1 ? c.Platforms.EraA : plat == 2 ? c.Platforms.EraB : c.Platforms.EraC; }

        public static int LiftPermille(GameConfig c, int plat) { return plat == 1 ? c.Platforms.LiftA : plat == 2 ? c.Platforms.LiftB : c.Platforms.LiftC; }

        public static int FirstCut(GameConfig c, int plat) { return plat == 1 ? c.Platforms.CutA : plat == 2 ? c.Platforms.CutB : c.Platforms.CutC; }

        public static bool InWindow(GameConfig c, LabState lab) { return Active(lab) && lab.PlatLeft <= c.Platforms.WindowWeeks; }

        /// <summary>What the lab loses in reputation if the deal ends now: more the more its users came through the platform.</summary>
        public static int WalkLoss(GameConfig c, LabState lab)
        {
            return (int)MathI.MulPermille(c.Platforms.WalkReputation, lab.PlatReliance);
        }

        /// <summary>The extra revenue the deal brings this week, after the platform's cut. Called from the market with what the lab earned.</summary>
        public static long Lift(GameConfig c, GameState state, LabState lab, long revenue)
        {
            if (!Enabled(c) || !Active(lab) || lab.Id != state.PlayerLabId || revenue <= 0) return 0;
            long lift = MathI.MulPermille(revenue, LiftPermille(c, lab.PlatPlat));
            long cut = MathI.MulPermille(lift, lab.PlatCut);
            lab.PlatPaid += cut;
            lab.PlatKept += lift - cut;
            return lift - cut;
        }

        public static void Sign(GameConfig c, GameState state, LabState lab, int plat)
        {
            lab.PlatPlat = plat;
            lab.PlatLeft = c.Platforms.TermWeeks;
            lab.PlatCut = FirstCut(c, plat);
            lab.PlatRenewals = 0;
            lab.PlatReliance = 0;
            lab.PlatHoldup = 0;
            lab.PlatPaid = 0;
            lab.PlatKept = 0;
            Events.Add(state, EventKind.PlatformSigned, lab.Id, plat, lab.PlatCut, "");
        }

        public static void Renew(GameConfig c, GameState state, LabState lab)
        {
            lab.PlatLeft = c.Platforms.TermWeeks;
            lab.PlatRenewals++;
            lab.PlatHoldup = 0;
            lab.PlatCut = (int)MathI.Min(c.Platforms.MaxCut, lab.PlatCut + c.Platforms.RenewCutStep);
            Events.Add(state, EventKind.PlatformRenewed, lab.Id, lab.PlatPlat, lab.PlatCut, "");
        }

        public static void End(GameConfig c, GameState state, LabState lab, int pick)
        {
            int plat = lab.PlatPlat;
            int loss = PressSystem.Damp(c, lab, WalkLoss(c, lab));
            PressSystem.AddReputation(lab, -loss);
            lab.PlatPlat = 0;
            lab.PlatLeft = 0;
            lab.PlatCut = 0;
            lab.PlatRenewals = 0;
            lab.PlatReliance = 0;
            lab.PlatHoldup = 0;
            lab.PlatPaid = 0;
            lab.PlatKept = 0;
            Events.Add(state, EventKind.PlatformEnded, lab.Id, plat, loss, "").Pick = pick;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || !Active(lab)) return;
            lab.PlatReliance = (int)MathI.Min(MathI.Permille, lab.PlatReliance + c.Platforms.RelianceStep);
            lab.PlatLeft--;
            if (lab.PlatLeft <= 0)
            {
                End(c, state, lab, 1);
                return;
            }
            if (lab.PlatLeft <= c.Platforms.WindowWeeks && lab.PlatHoldup == 0)
            {
                lab.PlatHoldup = 1;
                if (lab.PlatReliance >= c.Platforms.HoldupBar)
                {
                    lab.PlatCut = (int)MathI.Min(c.Platforms.MaxCut, lab.PlatCut + c.Platforms.HoldupCut);
                    Events.Add(state, EventKind.PlatformHoldup, lab.Id, lab.PlatPlat, lab.PlatCut, "");
                }
            }
        }
    }
}
