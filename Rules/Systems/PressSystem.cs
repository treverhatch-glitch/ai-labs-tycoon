using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>
    /// The press (bible ch. 14). Press rooms dampen what a scandal costs and sharpen a launch (both read from the rooms each time, nothing saved).
    /// A beat in the player's story (a launch, an incident, a hearing, the crown) brings a journalist with a question; an answer costs money, works
    /// with a chance and wins back some of the stake or loses a share of it; a question nobody answers lapses without changing anything.
    /// A lab can also leak a rival's recent incident to the press.
    /// </summary>
    public static class PressSystem
    {
        /// <summary>A chance taken apart: parts that add up to the total, the last part being whatever the floor or the ceiling changed.</summary>
        public struct ChanceParts
        {
            public int Base;
            public int JournalistPart;
            public int RoomPart;
            public int ReputationPart;
            public int Clamp;

            public int Total { get { return Base + JournalistPart + RoomPart + ReputationPart + Clamp; } }
        }

        public static int Damping(GameConfig c, LabState lab)
        {
            return (int)MathI.Min(RoomPerks.Permille(c, lab, RoomPerkKind.ScandalDamping), c.Press.MaxDampingPermille) + ScalingPolicy.DampPermille(c, lab) + Foundation.Damping(c, lab);
        }

        public static int Buzz(GameConfig c, LabState lab)
        {
            return (int)MathI.Min(RoomPerks.Permille(c, lab, RoomPerkKind.LaunchBuzz), c.Press.MaxBuzzPermille) + Traits.Report(c, lab).BuzzPermille;
        }

        /// <summary>A reputation loss after the press room has had its say.</summary>
        public static int Damp(GameConfig c, LabState lab, int loss)
        {
            if (loss <= 0) return loss;
            return loss - (int)MathI.MulPermille(loss, Damping(c, lab)) + (int)MathI.MulPermille(loss, Traits.Report(c, lab).ScandalPermille);
        }

        public static int LaunchPoints(GameConfig c, LabState lab)
        {
            return (int)MathI.MulPermille(c.Press.LaunchBuzzPoints, Buzz(c, lab));
        }

        public static long AnswerCost(GameConfig c, int tick, PressResponseConfig response)
        {
            return HireFees(c, tick, response.CostHireFees);
        }

        public static long LeakFee(GameConfig c, int tick)
        {
            return HireFees(c, tick, c.Press.LeakFeeHireFees);
        }

        static long HireFees(GameConfig c, int tick, int count)
        {
            RoleConfig sales = c.Role("sales");
            return sales == null ? 0 : EraRules.HireFee(c, sales, tick) * count;
        }

        public static ChanceParts AnswerChance(GameConfig c, LabState lab, PressInquiry inquiry, PressResponseConfig response)
        {
            JournalistConfig journalist = c.Press.Journalist(inquiry.JournalistId);
            var parts = new ChanceParts();
            parts.Base = response.BaseSuccessPermille;
            parts.JournalistPart = journalist == null ? 0 : journalist.ChancePermille;
            parts.RoomPart = (int)((long)Damping(c, lab) * response.RoomSlopePermille / MathI.Permille);
            parts.ReputationPart = (int)(((long)lab.Reputation - 500) * response.ReputationSlopePermille / MathI.Permille);
            return Bounded(c, parts);
        }

        public static ChanceParts LeakChance(GameConfig c, LabState lab)
        {
            PressConfig p = c.Press;
            var parts = new ChanceParts();
            parts.Base = p.LeakBaseSuccessPermille;
            parts.RoomPart = (int)((long)Damping(c, lab) * p.LeakRoomSlopePermille / MathI.Permille);
            parts.ReputationPart = (int)(((long)lab.Reputation - 500) * p.LeakReputationSlopePermille / MathI.Permille);
            return Bounded(c, parts);
        }

        static ChanceParts Bounded(GameConfig c, ChanceParts parts)
        {
            int raw = parts.Base + parts.JournalistPart + parts.RoomPart + parts.ReputationPart;
            int bounded = (int)MathI.Clamp(raw, c.Press.MinSuccessPermille, MathI.Permille);
            parts.Clamp = bounded - raw;
            return parts;
        }

        /// <summary>Reputation a working answer wins.</summary>
        public static int RecoverPoints(PressInquiry inquiry, PressResponseConfig response)
        {
            return (int)MathI.MulPermille(inquiry.Stake, response.RecoverPermille);
        }

        /// <summary>Reputation a failed answer loses: a share of the stake, scaled by how the journalist plays it.</summary>
        public static int BackfirePoints(GameConfig c, PressInquiry inquiry, PressResponseConfig response)
        {
            JournalistConfig journalist = c.Press.Journalist(inquiry.JournalistId);
            long loss = MathI.MulPermille(inquiry.Stake, response.BackfirePermille);
            if (journalist != null) loss = MathI.MulPermille(loss, journalist.BackfirePermille);
            return (int)loss;
        }

        public static PressInquiry Inquiry(LabState lab, int inquiryId)
        {
            for (int i = 0; i < lab.PressInquiries.Count; i++) if (lab.PressInquiries[i].Id == inquiryId) return lab.PressInquiries[i];
            return null;
        }

        public static void AddReputation(LabState lab, int delta)
        {
            lab.Reputation = (int)MathI.Clamp(lab.Reputation + delta, 0, MathI.Permille);
        }

        /// <summary>
        /// Something newsworthy happened to a lab. Only the player's lab is asked about it. <paramref name="points"/> is the reputation the beat
        /// moved (what a bad beat cost, what the crown paid); a good beat with a fixed stake in the config uses that instead.
        /// </summary>
        public static void OnBeat(GameConfig c, GameState state, LabState lab, string beatId, int points, string what)
        {
            if (lab.Id != state.PlayerLabId || lab.Bankrupt) return;
            BeatConfig beat = c.Press.Beat(beatId);
            if (beat == null) return;
            int stake = beat.Good && beat.StakePoints > 0 ? beat.StakePoints : points;
            if (stake <= 0) return;
            if (lab.PressInquiries.Count >= c.Press.MaxOpenInquiries) return;
            JournalistConfig journalist = PickJournalist(c, state, beatId);
            if (journalist == null) return;

            var inquiry = new PressInquiry();
            inquiry.Id = state.TakeId();
            inquiry.JournalistId = journalist.Id;
            inquiry.BeatId = beatId;
            inquiry.Stake = stake;
            inquiry.ExpiresTick = state.Tick + c.Press.InquiryWeeks;
            inquiry.Ref = what;
            lab.PressInquiries.Add(inquiry);
            Events.Add(state, EventKind.PressInquiry, lab.Id, inquiry.Id, stake, beatId + "|" + journalist.Id + "|" + what);
        }

        /// <summary>The journalists who cover a beat take it in turns, by the week.</summary>
        static JournalistConfig PickJournalist(GameConfig c, GameState state, string beatId)
        {
            int n = 0;
            for (int i = 0; i < c.Press.Journalists.Count; i++) if (c.Press.Journalists[i].Beats.Contains(beatId)) n++;
            if (n == 0) return null;
            int pick = state.Tick % n;
            for (int i = 0; i < c.Press.Journalists.Count; i++)
            {
                if (!c.Press.Journalists[i].Beats.Contains(beatId)) continue;
                if (pick-- == 0) return c.Press.Journalists[i];
            }
            return null;
        }

        /// <summary>A model went on sale: the press room sharpens it, and a journalist may ask.</summary>
        public static void OnLaunch(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            int points = LaunchPoints(c, lab);
            if (points > 0)
            {
                AddReputation(lab, points);
                Events.Add(state, EventKind.LaunchBuzz, lab.Id, model.Id, points, model.Name);
            }
            OnBeat(c, state, lab, "launch", 0, model.Name);
        }

        /// <summary>Each week: questions nobody answered lapse.</summary>
        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            for (int i = lab.PressInquiries.Count - 1; i >= 0; i--)
                if (state.Tick >= lab.PressInquiries[i].ExpiresTick) lab.PressInquiries.RemoveAt(i);
        }

        /// <summary>Whether the rival had an incident recently enough for a leak to be news.</summary>
        public static bool IsLeakable(GameConfig c, GameState state, LabState rival)
        {
            for (int i = 0; i < rival.Models.Count; i++)
            {
                ModelState m = rival.Models[i];
                if (m.Incidents > 0 && state.Tick - m.LastIncidentTick <= c.Press.LeakWindowWeeks) return true;
            }
            return false;
        }

        public static string LastIncidentModelName(GameConfig c, GameState state, LabState rival)
        {
            ModelState best = null;
            for (int i = 0; i < rival.Models.Count; i++)
            {
                ModelState m = rival.Models[i];
                if (m.Incidents > 0 && state.Tick - m.LastIncidentTick <= c.Press.LeakWindowWeeks && (best == null || m.LastIncidentTick > best.LastIncidentTick)) best = m;
            }
            return best == null ? "" : best.Name;
        }
    }
}
