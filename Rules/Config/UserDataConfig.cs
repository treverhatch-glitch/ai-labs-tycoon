using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// The flywheel from the lab's own users: a policy (none, opt-in or on by default) turns a share of last week's revenue into units of raw data. A default policy collects more and draws privacy complaints; moving off it deletes most of what was collected. A lab that never sets a policy collects nothing. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class UserDataConfig : ISync
    {
        public int Enabled;
        /// <summary>Weekly revenue that makes one unit of data when the lab collects from every user.</summary>
        public int RevenuePerUnit;
        /// <summary>Share of that collected when users opt in (permille).</summary>
        public int OptInPermille;
        /// <summary>Share collected when collection is on by default (permille).</summary>
        public int DefaultPermille;
        /// <summary>Weeks before the policy can be changed again.</summary>
        public int LockWeeks;
        /// <summary>Weeks between privacy complaints under a default policy.</summary>
        public int ComplaintWeeks;
        /// <summary>Units collected since the last complaint that draw one.</summary>
        public int ComplaintMin;
        /// <summary>Fine for each unit collected since the last complaint.</summary>
        public int ComplaintFine;
        /// <summary>Trust lost in each sector the lab sells in.</summary>
        public int ComplaintTrust;
        /// <summary>Share of the data kept when the lab moves from a default policy to opt-in or none (permille).</summary>
        public int KeepPermille;

        /// <summary>Independent directors who resign over a privacy complaint.</summary>
        public int ComplaintResigns;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("revenuePerUnit", ref RevenuePerUnit);
            s.Int("optInPermille", ref OptInPermille);
            s.Int("defaultPermille", ref DefaultPermille);
            s.Int("lockWeeks", ref LockWeeks);
            s.Int("complaintWeeks", ref ComplaintWeeks);
            s.Int("complaintMin", ref ComplaintMin);
            s.Int("complaintFine", ref ComplaintFine);
            s.Int("complaintTrust", ref ComplaintTrust);
            s.Int("keepPermille", ref KeepPermille);
            s.Int("complaintResigns", ref ComplaintResigns);
        }
    }
}
