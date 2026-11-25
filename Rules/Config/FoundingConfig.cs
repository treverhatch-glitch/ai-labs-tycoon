using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Who the player is (bible ch. 5): a founder changes the start (money, hype, grants, one powerful enemy) and carries a lasting edge
    /// and a lasting cost. Chosen once when a game is made; parody rivals have none.
    /// </summary>
    public sealed class FounderConfig : ISync, IHasId
    {
        public string IdValue = "";
        public string Name = "";
        public string Blurb = "";
        /// <summary>Starting cash as a share of the era's: 1000 is as usual.</summary>
        public int CashPermille = 1000;
        /// <summary>Added to the starting reputation (hype, or a name in the field); may be negative.</summary>
        public int ReputationDelta;
        /// <summary>The starting grant as a share of the era's: 1000 is as usual.</summary>
        public int GrantPermille = 1000;
        /// <summary>Lasting: how much faster (or slower, if negative) the lab's researchers earn points.</summary>
        public int ResearchPermille;
        /// <summary>Lasting: added to the appeal of every model in every sector; negative for a founder who cannot sell.</summary>
        public int AppealPermille;
        /// <summary>Lasting: added to what every hire costs; positive for a founder staff do not respect.</summary>
        public int HireFeePermille;
        /// <summary>A rival that starts out hating the player (empty for none). A rival that is not in the game is skipped.</summary>
        public string EnemyRivalId = "";
        public int EnemyFeeling;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Str("blurb", ref Blurb);
            s.Int("cashPermille", ref CashPermille);
            s.Int("reputationDelta", ref ReputationDelta);
            s.Int("grantPermille", ref GrantPermille);
            s.Int("researchPermille", ref ResearchPermille);
            s.Int("appealPermille", ref AppealPermille);
            s.Int("hireFeePermille", ref HireFeePermille);
            s.Str("enemyRivalId", ref EnemyRivalId);
            s.Int("enemyFeeling", ref EnemyFeeling);
        }
    }

    /// <summary>
    /// How the lab is good (bible ch. 5, sniper or shotgun): customers in the focus sectors like the lab's models more, customers in every
    /// other sector like them less. Chosen once when a game is made.
    /// </summary>
    public sealed class StyleConfig : ISync, IHasId
    {
        public string IdValue = "";
        public string Name = "";
        public string Blurb = "";
        public List<string> FocusSectors = new List<string>();
        public int FocusAppealPermille;
        /// <summary>Added to the appeal in every sector outside the focus; zero or negative.</summary>
        public int OtherAppealPermille;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Str("blurb", ref Blurb);
            s.StrList("focusSectors", ref FocusSectors);
            s.Int("focusAppealPermille", ref FocusAppealPermille);
            s.Int("otherAppealPermille", ref OtherAppealPermille);
        }
    }
}
