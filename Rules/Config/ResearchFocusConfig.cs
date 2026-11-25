using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// A research focus: the lab picks one direction and techs in it cost less research while techs in the other directions cost more.
    /// Changing it costs cash and cannot be done again for a while. With no focus, or no options, every tech costs what it says.
    /// Only the player's lab picks one.
    /// </summary>
    public sealed class ResearchFocusConfig : ISync
    {
        /// <summary>The directions a lab can pick. An empty list turns focus off.</summary>
        public List<FocusOptionConfig> Options = new List<FocusOptionConfig>();
        /// <summary>What a tech in the chosen direction takes off its research cost, in permille.</summary>
        public int DiscountPermille;
        /// <summary>What a tech in a direction the lab did not pick adds to its research cost, in permille.</summary>
        public int PenaltyPermille;
        /// <summary>The cash it costs to change the focus (including dropping it).</summary>
        public long SwitchCost;
        /// <summary>Weeks after a change before the focus can be changed again.</summary>
        public int LockWeeks;

        public bool Enabled { get { return Options.Count > 0; } }

        public FocusOptionConfig Option(string id)
        {
            for (int i = 0; i < Options.Count; i++) if (Options[i].Id == id) return Options[i];
            return null;
        }

        public void Sync(Syncer s)
        {
            s.ObjList("options", ref Options);
            s.Int("discountPermille", ref DiscountPermille);
            s.Int("penaltyPermille", ref PenaltyPermille);
            s.Long("switchCost", ref SwitchCost);
            s.Int("lockWeeks", ref LockWeeks);
        }
    }

    public sealed class FocusOptionConfig : ISync
    {
        public string Id;
        public string Name;
        /// <summary>One funny line for the research screen.</summary>
        public string Blurb;
        /// <summary>Names of <see cref="TechEffectKind"/> values. A tech with any of them is in this direction.</summary>
        public List<string> Kinds = new List<string>();

        public bool Covers(TechEffectKind kind)
        {
            string name = kind.ToString();
            for (int i = 0; i < Kinds.Count; i++) if (Kinds[i] == name) return true;
            return false;
        }

        public void Sync(Syncer s)
        {
            s.Str("id", ref Id);
            s.Str("name", ref Name);
            s.Str("blurb", ref Blurb);
            s.StrList("kinds", ref Kinds);
        }
    }
}
