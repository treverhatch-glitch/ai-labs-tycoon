using AiLabs.Rules.Config;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Systems
{
    /// <summary>The floors under incident cuts. See <see cref="IncidentFloorConfig"/>. Every cut to a fine or to a chance goes through here.</summary>
    public static class IncidentFloor
    {
        /// <summary>The least the lab pays itself of a gross fine.</summary>
        public static long FineFloor(GameConfig c, long grossFine)
        {
            return MathI.MulPermille(grossFine, MathI.Clamp(c.IncidentFloor.FineFloorPermille, 0, MathI.Permille));
        }

        /// <summary>A fine after a share has been cut, never below the floor and never above the gross fine.</summary>
        public static long CutFine(GameConfig c, long grossFine, long fine)
        {
            return MathI.Clamp(fine, FineFloor(c, grossFine), MathI.Max(0, grossFine));
        }

        /// <summary>How much an insurer may pay of a fine that has already been cut, so that what the lab pays stays at or above the floor.</summary>
        public static long MaxPayout(GameConfig c, long grossFine, long fineAfterCuts)
        {
            return MathI.Max(0, fineAfterCuts - FineFloor(c, grossFine));
        }

        /// <summary>The weekly chance after cuts, never below the floor share of the uncut chance.</summary>
        public static int Chance(GameConfig c, long uncut, long cut)
        {
            long top = MathI.Clamp(uncut, 0, MathI.Permille);
            long floor = MathI.MulPermille(top, MathI.Clamp(c.IncidentFloor.ChanceFloorPermille, 0, MathI.Permille));
            return (int)MathI.Min(MathI.Permille, MathI.Max(floor, cut));
        }
    }
}
