using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Human escalation queue (bible ch. 6, when agents need a person). Agent actions pile work on a queue; if it grows past what the support desk can handle, agent revenue is delayed and incidents rise. Support staff drain the queue each week, or the lab can pay to clear items. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class HumanQueueConfig : ISync
    {
        public int Enabled;
        /// <summary>First era for the human escalation queue.</summary>
        public int MinEra;
        /// <summary>Support people needed before the desk can drain the queue each week.</summary>
        public int StaffNeed;
        /// <summary>Largest queue the desk handles without delaying agent revenue.</summary>
        public int Capacity;
        /// <summary>Share of agent revenue lost for each item over capacity (permille).</summary>
        public int LiftCut;
        /// <summary>Weekly incident permille per queued item on agent models.</summary>
        public int IncidentPer;
        /// <summary>Cash to clear one item now.</summary>
        public int ClearCost;
        /// <summary>Weeks a staffed clear keeps the desk busy.</summary>
        public int BusyWeeks;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minEra", ref MinEra);
            s.Int("staffNeed", ref StaffNeed);
            s.Int("capacity", ref Capacity);
            s.Int("liftCut", ref LiftCut);
            s.Int("incidentPer", ref IncidentPer);
            s.Int("clearCost", ref ClearCost);
            s.Int("busyWeeks", ref BusyWeeks);
        }
    }
}
