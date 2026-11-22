using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Annotation vendors (bible ch. 6, labelling and human feedback). A lab can contract a vendor for a term: volume (cheap and wide but with a growing risk of a labour scandal), licensed or expert (dearer, smaller jobs, no risk). While the contract runs the fee is paid every week whether it is used or not, and every cleaning job of the data pipeline within the vendor's size yields more. Exposure from the volume vendor builds up unseen, an audit takes most of it away, and a scandal ends the contract with a fine and lost trust. Ending early costs most of what is left. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class AnnotationConfig : ISync
    {
        public int Enabled;
        /// <summary>Weekly fee of the volume vendor.</summary>
        public int Weekly1;
        /// <summary>Weekly fee of the licensed vendor.</summary>
        public int Weekly2;
        /// <summary>Weekly fee of the expert vendor.</summary>
        public int Weekly3;
        /// <summary>Share the volume vendor adds to the yield of a cleaning job (permille).</summary>
        public int Bonus1;
        /// <summary>Share the licensed vendor adds to the yield of a cleaning job (permille).</summary>
        public int Bonus2;
        /// <summary>Share the expert vendor adds to the yield of a cleaning job (permille).</summary>
        public int Bonus3;
        /// <summary>Largest cleaning job, in units, the volume vendor handles; a bigger job gets no bonus.</summary>
        public int Cap1;
        /// <summary>Largest cleaning job, in units, the licensed vendor handles.</summary>
        public int Cap2;
        /// <summary>Largest cleaning job, in units, the expert vendor handles.</summary>
        public int Cap3;
        /// <summary>Exposure the volume vendor adds each week (permille).</summary>
        public int Risk1;
        /// <summary>Exposure the licensed vendor adds each week (permille).</summary>
        public int Risk2;
        /// <summary>Exposure the expert vendor adds each week (permille).</summary>
        public int Risk3;
        /// <summary>Weeks a contract runs.</summary>
        public int Term;
        /// <summary>Share of the fees still to run that ending a contract early costs (permille).</summary>
        public int EarlyPermille;
        /// <summary>What an audit of the vendor costs.</summary>
        public int AuditCost;
        /// <summary>Share of the exposure an audit takes away (permille).</summary>
        public int AuditCut;
        /// <summary>Weeks before the vendor can be audited again.</summary>
        public int AuditWeeks;
        /// <summary>Exposure is divided by this to give the weekly chance of a scandal (permille).</summary>
        public int ExposeDiv;
        /// <summary>Fine when a scandal breaks.</summary>
        public int ExposeFine;
        /// <summary>Trust lost in every sector the lab sells in when a scandal breaks.</summary>
        public int ExposeTrust;
        /// <summary>Exposure left after a scandal.</summary>
        public int ExposeReset;

        /// <summary>Board tension a vendor scandal adds, when the lab holds board meetings.</summary>
        public int ScandalTension;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("weekly1", ref Weekly1);
            s.Int("weekly2", ref Weekly2);
            s.Int("weekly3", ref Weekly3);
            s.Int("bonus1", ref Bonus1);
            s.Int("bonus2", ref Bonus2);
            s.Int("bonus3", ref Bonus3);
            s.Int("cap1", ref Cap1);
            s.Int("cap2", ref Cap2);
            s.Int("cap3", ref Cap3);
            s.Int("risk1", ref Risk1);
            s.Int("risk2", ref Risk2);
            s.Int("risk3", ref Risk3);
            s.Int("term", ref Term);
            s.Int("earlyPermille", ref EarlyPermille);
            s.Int("auditCost", ref AuditCost);
            s.Int("auditCut", ref AuditCut);
            s.Int("auditWeeks", ref AuditWeeks);
            s.Int("exposeDiv", ref ExposeDiv);
            s.Int("exposeFine", ref ExposeFine);
            s.Int("exposeTrust", ref ExposeTrust);
            s.Int("exposeReset", ref ExposeReset);
            s.Int("scandalTension", ref ScandalTension);
        }
    }
}
