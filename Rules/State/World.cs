using AiLabs.Rules.Core;

namespace AiLabs.Rules.State
{
    /// <summary>What a site is doing right now, for all the labs there. One per site in the config.</summary>
    public sealed class SiteState : ISync
    {
        public string SiteId;
        /// <summary>The first week the site is no longer in a heatwave, or -1 when it never was.</summary>
        public int HeatwaveUntilTick;

        public void Sync(Syncer s)
        {
            s.Str("siteId", ref SiteId);
            s.Int("heatwaveUntilTick", ref HeatwaveUntilTick);
        }
    }

    public enum HoldingKind { Owned, Colo }

    /// <summary>
    /// A lab's foothold at a site: land with a hall on it, or a leased corner of someone else's hall. Hardware batches name
    /// the holding they stand in. Power the hall has, built and promised, is counted against the site's grid.
    /// </summary>
    public sealed class Holding : ISync
    {
        public int Id;
        public string SiteId;
        public HoldingKind Kind;
        /// <summary>Built capacity in kilowatts. For a colo, the kilowatts leased.</summary>
        public int HallKw;
        /// <summary>The extension being built, in kilowatts, or 0 when none. One at a time.</summary>
        public int HallBuildingKw;
        /// <summary>The week the extension is ready, or -1 when none is being built.</summary>
        public int HallDoneTick;
        /// <summary>Power from the holding's own deal that is working now, in kilowatts.</summary>
        public int PrivateKw;
        /// <summary>The power deal bought for this holding, or empty. One per holding.</summary>
        public string DealId;
        /// <summary>The week a bought deal starts delivering, or -1 when it already does or there is none.</summary>
        public int DealDoneTick;

        public void Sync(Syncer s)
        {
            s.Int("id", ref Id);
            s.Str("siteId", ref SiteId);
            s.Enum("kind", ref Kind);
            s.Int("hallKw", ref HallKw);
            s.Int("hallBuildingKw", ref HallBuildingKw);
            s.Int("hallDoneTick", ref HallDoneTick);
            s.Int("privateKw", ref PrivateKw);
            s.Str("dealId", ref DealId);
            s.Int("dealDoneTick", ref DealDoneTick);
        }
    }

    /// <summary>How much one government has noticed one lab, and the step of scrutiny that reached.</summary>
    public sealed class BlocStanding : ISync
    {
        public string BlocId;
        /// <summary>0 to 1000.</summary>
        public int Attention;
        /// <summary>0 none, 1 Letter, 2 Hearing, 3 Audit, 4 Freeze. Stored so each step fires once on the way up.</summary>
        public int Level;

        public const int Letter = 1;
        public const int Hearing = 2;
        public const int Audit = 3;
        public const int Freeze = 4;

        public void Sync(Syncer s)
        {
            s.Str("blocId", ref BlocId);
            s.Int("attention", ref Attention);
            s.Int("level", ref Level);
        }
    }
}
