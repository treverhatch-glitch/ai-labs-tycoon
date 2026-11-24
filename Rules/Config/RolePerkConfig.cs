using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>What a role does beyond its function. Each kind is one rule read from the staff a lab has today; none of it is saved.</summary>
    public enum RolePerkKind
    {
        /// <summary>Takes a share off every hiring fee.</summary>
        HireDiscount,
        /// <summary>Catches a share of incidents before they reach customers.</summary>
        IncidentCut,
        /// <summary>Makes every unit of compute spent on alignment do more.</summary>
        AlignmentWork,
        /// <summary>Raises the appeal of the lab's models in the perk's sectors (every sector when it lists none).</summary>
        SectorAppeal
    }

    /// <summary>
    /// A perk of a role: every person in the role adds <see cref="PerStaffPermille"/> to the perk, up to <see cref="CapPermille"/>.
    /// Perks of the same kind add up. The effect is worked out from the staff list each time it is asked, so nothing is stored.
    /// </summary>
    public sealed class RolePerkConfig : ISync, IHasId
    {
        public string IdValue = "";
        public string RoleId = "";
        public RolePerkKind Kind;
        public int PerStaffPermille;
        public int CapPermille;
        /// <summary>The sectors a sector perk applies to; empty for perks that are not about a sector.</summary>
        public List<string> Sectors = new List<string>();
        /// <summary>A line a screen can show beside the role.</summary>
        public string Blurb = "";

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("roleId", ref RoleId);
            s.Enum("kind", ref Kind);
            s.Int("perStaffPermille", ref PerStaffPermille);
            s.Int("capPermille", ref CapPermille);
            s.StrList("sectors", ref Sectors);
            s.Str("blurb", ref Blurb);
        }
    }
}
