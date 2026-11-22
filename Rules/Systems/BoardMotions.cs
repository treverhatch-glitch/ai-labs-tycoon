using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>Board meetings, motions and tension. See <see cref="BoardAgendaConfig"/>. Only the player's lab.</summary>
    public static class BoardMotions
    {
        public const int Calm = 1;
        public const int Runway = 2;
        public const int SafetyStaff = 3;

        public static bool Enabled(GameConfig c) { return c.BoardAgenda.Enabled > 0; }

        public static int Incidents(LabState lab)
        {
            int n = 0;
            for (int i = 0; i < lab.Models.Count; i++) n += lab.Models[i].Incidents;
            return n;
        }

        /// <summary>What the board would ask of the lab for a motion of this kind now.</summary>
        public static long GoalFor(GameConfig c, LabState lab, int kind)
        {
            if (kind == Calm) return Incidents(lab);
            if (kind == Runway)
                return lab.Cash < c.BoardAgenda.RunwayFloor ? c.BoardAgenda.RunwayFloor : MathI.MulPermille(lab.Cash, MathI.Permille + c.BoardAgenda.RunwayStretch);
            return LabQueries.StaffCount(lab, "safety") + c.BoardAgenda.SafetyAdd;
        }

        public static bool Met(GameConfig c, LabState lab, int kind, long goal)
        {
            if (kind == Calm) return Incidents(lab) <= goal;
            if (kind == Runway) return lab.Cash >= goal;
            return LabQueries.StaffCount(lab, "safety") >= goal;
        }

        /// <summary>What the board adds to the control the founders need to keep, once the tension is high.</summary>
        public static int OustMargin(GameConfig c, LabState lab)
        {
            if (!Enabled(c) || lab.MeetingsOn == 0 || lab.BoardTension < c.BoardAgenda.OustAt) return 0;
            return (int)MathI.MulPermille(c.BoardAgenda.OustMargin, lab.BoardTension);
        }

        public static long RaiseCash(GameConfig c, LabState lab, long cash)
        {
            if (!Enabled(c) || lab.MeetingsOn == 0 || lab.MotionsKept == 0 || lab.BoardTension > c.BoardAgenda.CalmBelow) return cash;
            return MathI.MulPermille(cash, MathI.Permille + c.BoardAgenda.RaiseBonus);
        }

        static int Gain(GameConfig c, LabState lab, int amount)
        {
            long cut = MathI.Min(800, (long)lab.GovDirectors * c.BoardAgenda.DirectorCut);
            return (int)MathI.MulPermille(amount, MathI.Permille - cut);
        }

        static void Raise(GameConfig c, LabState lab, int amount)
        {
            lab.BoardTension = (int)MathI.Min(MathI.Permille, lab.BoardTension + Gain(c, lab, amount));
        }

        /// <summary>Adds tension from outside the motions (a scandal, a blow-up), when the lab holds meetings.</summary>
        public static void AddTension(GameConfig c, LabState lab, int amount)
        {
            if (!Enabled(c) || lab.MeetingsOn == 0 || amount <= 0) return;
            Raise(c, lab, amount);
        }

        static void Ease(LabState lab, int amount)
        {
            lab.BoardTension = (int)MathI.Max(0, lab.BoardTension - amount);
        }

        public static void Hold(GameConfig c, GameState state, LabState lab, bool on)
        {
            lab.MeetingsOn = on ? 1 : 0;
            lab.MotionNext = on ? state.Tick + c.BoardAgenda.MeetingWeeks : 0;
        }

        public static void Answer(GameConfig c, GameState state, LabState lab, bool accept)
        {
            int kind = lab.MotionPend;
            long goal = lab.MotionPendGoal;
            lab.MotionPend = 0;
            lab.MotionPendGoal = 0;
            lab.MotionAnswerBy = 0;
            if (accept)
            {
                lab.MotionKind = kind;
                lab.MotionGoal = kind == Calm ? Incidents(lab) : goal;
                lab.MotionDue = state.Tick + c.BoardAgenda.CommitWeeks;
                Ease(lab, c.BoardAgenda.AcceptRelief);
            }
            else
            {
                lab.MotionsDeclined++;
                Raise(c, lab, c.BoardAgenda.DeclineTension);
            }
            Events.Add(state, EventKind.MotionAnswered, lab.Id, kind, 0, "").Pick = accept ? 1 : 0;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || lab.MeetingsOn == 0) return;
            int now = state.Tick + 1;
            if (lab.MotionPend != 0 && now >= lab.MotionAnswerBy)
            {
                int kind = lab.MotionPend;
                lab.MotionPend = 0;
                lab.MotionPendGoal = 0;
                lab.MotionAnswerBy = 0;
                lab.MotionsDeclined++;
                Raise(c, lab, c.BoardAgenda.IgnoreTension);
                Events.Add(state, EventKind.MotionIgnored, lab.Id, kind, 0, "");
            }
            if (lab.MotionKind != 0 && now >= lab.MotionDue)
            {
                int kind = lab.MotionKind;
                bool kept = Met(c, lab, kind, lab.MotionGoal);
                lab.MotionKind = 0;
                lab.MotionGoal = 0;
                lab.MotionDue = 0;
                if (kept)
                {
                    lab.MotionsKept++;
                    Ease(lab, c.BoardAgenda.KeptRelief);
                }
                else
                {
                    lab.MotionsBroken++;
                    Raise(c, lab, c.BoardAgenda.BrokenTension);
                }
                Events.Add(state, EventKind.MotionJudged, lab.Id, kind, 0, "").Pick = kept ? 1 : 0;
            }
            if (lab.MotionPend == 0 && lab.MotionKind == 0 && now >= lab.MotionNext)
            {
                int kind = 1 + lab.MotionsTabled % 3;
                lab.MotionsTabled++;
                lab.MotionPend = kind;
                lab.MotionPendGoal = GoalFor(c, lab, kind);
                lab.MotionAnswerBy = now + c.BoardAgenda.AnswerWeeks;
                lab.MotionNext = now + c.BoardAgenda.MeetingWeeks;
                Events.Add(state, EventKind.MotionTabled, lab.Id, kind, lab.MotionPendGoal, "");
            }
            if (lab.BoardTension >= c.BoardAgenda.OustAt && Governance.ControlShare(c, lab) <= c.Funding.ControlPermille + OustMargin(c, lab))
            {
                Events.Add(state, EventKind.BoardForcedVote, lab.Id, -1, lab.BoardTension, "");
                lab.MeetingsOn = 0;
                lab.BoardTension = 0;
                lab.MotionPend = 0;
                lab.MotionPendGoal = 0;
                lab.MotionAnswerBy = 0;
                lab.MotionKind = 0;
                lab.MotionGoal = 0;
                lab.MotionDue = 0;
                lab.MotionNext = 0;
                FundingSystem.TryOustForced(c, state, lab);
            }
        }
    }
}
