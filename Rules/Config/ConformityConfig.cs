using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// A staged conformity assessment for one model. The lab compiles a dossier, then submits evidence when the model has a live red-team result (and for a broad scope a live external audit), the assessor reviews it, and a model whose true alignment meets the bar and whose claim gap is small is certified for a term. Findings send the lab back to gather evidence after a remediation delay; a surveillance check partway through the term can suspend the certificate. A certified model has a lower weekly incident chance and its sectors trust it more. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class ConformityConfig : ISync
    {
        public int Enabled;
        /// <summary>Models that can be in the pipeline or certified at once.</summary>
        public int MaxActive;
        /// <summary>Weeks to compile the dossier (stage 1).</summary>
        public int DossierWeeks;
        /// <summary>Research points a dossier costs; a broad scope costs this plus the extra.</summary>
        public int DossierPoints;
        /// <summary>More research points a broad-scope dossier costs.</summary>
        public int BroadExtraPoints;
        /// <summary>Weeks the assessor takes over the evidence (stage 3).</summary>
        public int ReviewWeeks;
        /// <summary>Weeks a certificate lasts.</summary>
        public int TermWeeks;
        /// <summary>Weeks into the term when the assessor checks again.</summary>
        public int SurveillanceAt;
        /// <summary>Weeks of remediation before evidence can be submitted again after findings.</summary>
        public int ResubmitWeeks;
        /// <summary>True alignment a narrow scope needs (0 to 1000).</summary>
        public int BarNarrow;
        /// <summary>True alignment a broad scope needs.</summary>
        public int BarBroad;
        /// <summary>Most the shown alignment may exceed the true one.</summary>
        public int MaxGap;
        /// <summary>Share a narrow certificate takes off the model weekly incident chance (permille).</summary>
        public int CutNarrow;
        /// <summary>Same for a broad one.</summary>
        public int CutBroad;
        /// <summary>Trust gained in each sector of the model when a certificate is granted.</summary>
        public int TrustGain;
        /// <summary>Trust lost in each sector of the model when a certificate is suspended.</summary>
        public int SuspendTrust;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("maxActive", ref MaxActive);
            s.Int("dossierWeeks", ref DossierWeeks);
            s.Int("dossierPoints", ref DossierPoints);
            s.Int("broadExtraPoints", ref BroadExtraPoints);
            s.Int("reviewWeeks", ref ReviewWeeks);
            s.Int("termWeeks", ref TermWeeks);
            s.Int("surveillanceAt", ref SurveillanceAt);
            s.Int("resubmitWeeks", ref ResubmitWeeks);
            s.Int("barNarrow", ref BarNarrow);
            s.Int("barBroad", ref BarBroad);
            s.Int("maxGap", ref MaxGap);
            s.Int("cutNarrow", ref CutNarrow);
            s.Int("cutBroad", ref CutBroad);
            s.Int("trustGain", ref TrustGain);
            s.Int("suspendTrust", ref SuspendTrust);
        }
    }
}
