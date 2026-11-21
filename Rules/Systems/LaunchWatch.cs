using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>A desk that watches rival launches. See <see cref="LaunchWatchConfig"/>. It only reads rival models and events and never changes what rivals do. Only the player's lab.</summary>
    public static class LaunchWatch
    {
        public static bool Enabled(GameConfig c) { return c.LaunchWatch.Enabled > 0; }

        public static bool On(LabState lab) { return lab.LwOn > 0; }

        public static bool Waiting(LabState lab) { return lab.LwRival > 0; }

        public static long CounterCost(GameConfig c, LabState lab) { return lab.Staff.Count * (long)c.LaunchWatch.CounterPerStaff; }

        public static int Loss(GameConfig c, LabState lab)
        {
            return (int)MathI.MulPermille(c.LaunchWatch.LossReputation, 500 + lab.LwLead);
        }

        public static bool CanOwnSafety(GameConfig c, LabState lab)
        {
            ModelState best = LabQueries.BestReleased(lab);
            return best != null && best.Alignment >= c.LaunchWatch.SafetyBar;
        }

        public static void Open(GameConfig c, GameState state, LabState lab)
        {
            Finance.Spend(lab, LedgerLine.Hiring, c.LaunchWatch.OpenCost);
            lab.LwOn = 1;
            Events.Add(state, EventKind.WatchDeskOpened, lab.Id, 0, c.LaunchWatch.OpenCost, "");
        }

        public static void Close(GameConfig c, GameState state, LabState lab)
        {
            if (Waiting(lab)) Resolve(c, state, lab, 2, 0);
            lab.LwOn = 0;
        }

        public static void Resolve(GameConfig c, GameState state, LabState lab, int answer, int pick)
        {
            int rival = lab.LwRival - 1;
            long change = 0;
            if (answer == 3)
            {
                change = c.LaunchWatch.SafetyGain;
                PressSystem.AddReputation(lab, (int)change);
            }
            else
            {
                int loss = Loss(c, lab);
                if (answer == 1)
                {
                    Finance.Spend(lab, LedgerLine.Hiring, CounterCost(c, lab));
                    loss = (int)MathI.MulPermille(loss, c.LaunchWatch.CounterKeepPermille);
                }
                change = -PressSystem.Damp(c, lab, loss);
                PressSystem.AddReputation(lab, (int)change);
                lab.LwHits++;
            }
            lab.LwAnswers++;
            lab.LwRival = 0;
            lab.LwLead = 0;
            lab.LwWait = 0;
            Events.Add(state, EventKind.LaunchAnswered, lab.Id, rival, change, "").Pick = pick;
        }

        static void Seen(GameConfig c, GameState state, LabState lab, GameEvent launch)
        {
            LabState rival = state.Lab(launch.LabId);
            if (rival == null) return;
            ModelState theirs = null;
            for (int i = 0; i < rival.Models.Count; i++)
                if (rival.Models[i].Id == launch.SubjectId) theirs = rival.Models[i];
            if (theirs == null || theirs.Capability <= 0) return;
            ModelState mine = LabQueries.BestReleased(lab);
            long myCap = mine == null ? 0 : mine.Capability;
            if (theirs.Capability > myCap)
            {
                if (Waiting(lab)) return;
                lab.LwRival = rival.Id + 1;
                lab.LwLead = (int)MathI.Clamp((theirs.Capability - myCap) * MathI.Permille / theirs.Capability, 0, 1000);
                lab.LwWait = 0;
                Events.Add(state, EventKind.RivalLaunchSeen, lab.Id, rival.Id, lab.LwLead, theirs.Name).Pick = 1;
            }
            else
            {
                PressSystem.AddReputation(lab, c.LaunchWatch.LeadGain);
                Events.Add(state, EventKind.RivalLaunchSeen, lab.Id, rival.Id, 0, theirs.Name).Pick = 0;
            }
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || !On(lab)) return;
            Finance.Spend(lab, LedgerLine.Hiring, c.LaunchWatch.WeeklyCost);
            if (Waiting(lab))
            {
                lab.LwWait++;
                if (lab.LwWait >= c.LaunchWatch.WaitWeeks)
                {
                    Resolve(c, state, lab, 2, 0);
                }
            }
            int lastTick = state.Tick - 1;
            for (int i = state.Events.Count - 1; i >= 0; i--)
            {
                GameEvent e = state.Events[i];
                if (e.Tick < lastTick) break;
                if (e.Tick != lastTick || e.Kind != EventKind.ModelReleased || e.LabId == lab.Id) continue;
                Seen(c, state, lab, e);
            }
        }
    }
}
