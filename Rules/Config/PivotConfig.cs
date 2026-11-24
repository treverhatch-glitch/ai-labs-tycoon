using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Changing what the lab is good at (bible ch. 5: "speciality points can be re-spent once per era at a cost: a pivot loses some staff and
    /// confuses customers for a year"; ch. 3: "decide whether your lab's identity still fits, or pivot"). The player picks another style. It costs
    /// money and a share of the staff, and every model sells worse for a year. Once per era. Rivals and the autopilot never pivot.
    /// </summary>
    public sealed class PivotConfig : ISync
    {
        /// <summary>The least a week of running the lab is counted at when the retooling is priced.</summary>
        public int MinWeekCost;
        /// <summary>Retooling costs this many weeks of running the lab.</summary>
        public int CostWeeks;
        /// <summary>The share of staff who leave, in permille, rounded down but at least one if there is any staff.</summary>
        public int StaffLossPermille;
        /// <summary>How long customers stay confused, in weeks.</summary>
        public int ConfusionWeeks;
        /// <summary>How much every model's appeal falls while they are confused, in permille.</summary>
        public int ConfusionAppealPermille;

        public void Sync(Syncer s)
        {
            s.Int("minWeekCost", ref MinWeekCost);
            s.Int("costWeeks", ref CostWeeks);
            s.Int("staffLossPermille", ref StaffLossPermille);
            s.Int("confusionWeeks", ref ConfusionWeeks);
            s.Int("confusionAppealPermille", ref ConfusionAppealPermille);
        }
    }
}
