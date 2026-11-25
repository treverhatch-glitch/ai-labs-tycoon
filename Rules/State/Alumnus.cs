using AiLabs.Rules.Core;

namespace AiLabs.Rules.State
{
    /// <summary>Someone who left the lab and can be asked back: their old id, role, trait, the week they left and how (1 on good terms, 0 laid off, -1 quit).</summary>
    public sealed class Alumnus : ISync
    {
        public int Id;
        public string RoleId = "";
        public string TraitId = "";
        public int Left;
        public int Terms;

        public void Sync(Syncer s)
        {
            s.Int("id", ref Id);
            s.Str("roleId", ref RoleId);
            s.Str("traitId", ref TraitId);
            s.Int("left", ref Left);
            s.Int("terms", ref Terms);
        }
    }
}
