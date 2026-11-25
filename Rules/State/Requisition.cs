using AiLabs.Rules.Core;

namespace AiLabs.Rules.State
{
    /// <summary>An order for people in a role: how many, the week they arrive and the price per person locked on the day of the order.</summary>
    public sealed class Requisition : ISync
    {
        public string RoleId = "";
        public int Count;
        public int Due;
        public long Fee;

        public void Sync(Syncer s)
        {
            s.Str("roleId", ref RoleId);
            s.Int("count", ref Count);
            s.Int("due", ref Due);
            s.Long("fee", ref Fee);
        }
    }
}
