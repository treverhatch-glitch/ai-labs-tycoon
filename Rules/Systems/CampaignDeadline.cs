using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    public static class CampaignDeadline
    {
        public static bool Enabled(GameConfig c) { return c.CampaignDeadline.Enabled > 0; }

        public static void Set(GameConfig c, GameState state, LabState lab, int deadlineTick)
        {
            lab.CampaignDeadlineTick = deadlineTick;
            Events.Add(state, EventKind.CampaignDeadlineSet, lab.Id, deadlineTick, 0, deadlineTick <= 0 ? "off" : "on");
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || lab.Bankrupt) return;
            int tick = lab.CampaignDeadlineTick;
            if (tick < 1 || state.Tick + 1 < tick) return;
            if (EndingSystem.HasOpenEnding(state)) return;
            string eraId = EraSystem.AtTick(c, state.Tick).Id;
            if (Campaign.Recorded(lab, eraId) == null)
            {
                EraScore score = Campaign.Mark(c, state, lab, eraId);
                if (score.Of > 0)
                {
                    lab.EraScores.Add(score);
                    Events.Add(state, EventKind.EraScored, lab.Id, score.Met, score.Points, eraId);
                }
            }
            int runScore = Campaign.RunScore(c, state, lab);
            int bonus = (int)MathI.MulPermille(runScore, c.CampaignDeadline.ScoreBonusPermille);
            Events.Add(state, EventKind.CampaignDeadlineHit, lab.Id, runScore, bonus, eraId);
            EndingSystem.RecordCampaignStop(c, state, lab, runScore, bonus);
        }
    }
}