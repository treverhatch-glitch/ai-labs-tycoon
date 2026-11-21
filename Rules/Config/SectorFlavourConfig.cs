using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// How a sector fails. The support bot promises a refund policy you do not have; the legal tool cites cases that do not exist. A flavour
    /// says how hard an incident hits in its sector and what it costs beyond reputation: refunds to customers, a longer or deeper glitch, and a
    /// sharper fall in customer trust. A sector with no flavour fails the plain way. Nothing here is saved: it is all read from the data.
    /// </summary>
    public sealed class SectorFlavourConfig : ISync, IHasId
    {
        public string IdValue = "";
        /// <summary>The sector this flavour belongs to.</summary>
        public string Sector = "";
        /// <summary>One line on what goes wrong there, for the incident screen.</summary>
        public string Failure = "";
        /// <summary>Multiplies the incident fine (1000 = unchanged). When a model sells in several flavoured sectors, the one with the biggest takings sets the fine and the reputation lost.</summary>
        public int FinePermille;
        /// <summary>Multiplies the reputation lost (1000 = unchanged).</summary>
        public int ReputationPermille;
        /// <summary>Multiplies the customer trust lost in this sector (1000 = unchanged).</summary>
        public int TrustPermille;
        /// <summary>Refunds and credits paid to customers: this many weeks of the model's takings in this sector, on top of the fine. 0 = none.</summary>
        public int MakeGoodWeeks;
        /// <summary>Weeks the model's quality here stays down after an incident. 0 = the lab-wide <c>safety.glitchWeeks</c>.</summary>
        public int GlitchWeeks;
        /// <summary>The share of quality kept while glitching here (1000 = no loss). 0 = the lab-wide <c>safety.glitchQualityPermille</c>.</summary>
        public int GlitchQualityPermille;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("sector", ref Sector);
            s.Str("failure", ref Failure);
            s.Int("finePermille", ref FinePermille);
            s.Int("reputationPermille", ref ReputationPermille);
            s.Int("trustPermille", ref TrustPermille);
            s.Int("makeGoodWeeks", ref MakeGoodWeeks);
            s.Int("glitchWeeks", ref GlitchWeeks);
            s.Int("glitchQualityPermille", ref GlitchQualityPermille);
        }
    }
}
