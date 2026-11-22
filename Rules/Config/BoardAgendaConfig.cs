using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// The board agenda (bible ch. 5, the board and its confidence). A lab with investors on its board can choose to hold board meetings. Every quarter the board tables a motion in turn: stay free of incidents, keep cash up, or add safety staff. The lab accepts it (a commitment judged at its deadline) or declines. Tension builds when motions are declined, ignored or broken and is eased when commitments are kept; independent directors take some of it off. From a high tension the board can force a vote on the founders: the founders then need more of the control than usual, so dual-class shares or a non-profit parent protect them. A calm board that has been kept its word lets the lab raise more. Only the player's lab, and nothing happens until the lab holds meetings. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class BoardAgendaConfig : ISync
    {
        public int Enabled;
        /// <summary>Investor seats the board needs before the lab can hold board meetings.</summary>
        public int MinSeats;
        /// <summary>Weeks between meetings that table a motion.</summary>
        public int MeetingWeeks;
        /// <summary>Weeks the lab has to answer a motion; no answer counts against it.</summary>
        public int AnswerWeeks;
        /// <summary>Weeks the lab has to keep a commitment it accepted.</summary>
        public int CommitWeeks;
        /// <summary>Tension an accepted motion takes away at once.</summary>
        public int AcceptRelief;
        /// <summary>Tension a kept commitment takes away.</summary>
        public int KeptRelief;
        /// <summary>Tension a declined motion adds.</summary>
        public int DeclineTension;
        /// <summary>Tension a motion nobody answered adds.</summary>
        public int IgnoreTension;
        /// <summary>Tension a broken commitment adds.</summary>
        public int BrokenTension;
        /// <summary>Share of added tension each independent director takes away (permille), at most 800 in all.</summary>
        public int DirectorCut;
        /// <summary>Tension from which the board can force a vote on the founders.</summary>
        public int OustAt;
        /// <summary>Control share, in permille, that full tension adds to what the founders need to keep.</summary>
        public int OustMargin;
        /// <summary>Tension at or below which the board is calm.</summary>
        public int CalmBelow;
        /// <summary>Cash a calm board adds to a funding round once the lab has kept a commitment (permille).</summary>
        public int RaiseBonus;
        /// <summary>How much more cash than now a runway motion asks for (permille).</summary>
        public int RunwayStretch;
        /// <summary>The cash a runway motion asks for when the lab has less than this.</summary>
        public int RunwayFloor;
        /// <summary>Safety staff a safety motion asks the lab to add.</summary>
        public int SafetyAdd;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("minSeats", ref MinSeats);
            s.Int("meetingWeeks", ref MeetingWeeks);
            s.Int("answerWeeks", ref AnswerWeeks);
            s.Int("commitWeeks", ref CommitWeeks);
            s.Int("acceptRelief", ref AcceptRelief);
            s.Int("keptRelief", ref KeptRelief);
            s.Int("declineTension", ref DeclineTension);
            s.Int("ignoreTension", ref IgnoreTension);
            s.Int("brokenTension", ref BrokenTension);
            s.Int("directorCut", ref DirectorCut);
            s.Int("oustAt", ref OustAt);
            s.Int("oustMargin", ref OustMargin);
            s.Int("calmBelow", ref CalmBelow);
            s.Int("raiseBonus", ref RaiseBonus);
            s.Int("runwayStretch", ref RunwayStretch);
            s.Int("runwayFloor", ref RunwayFloor);
            s.Int("safetyAdd", ref SafetyAdd);
        }
    }
}
