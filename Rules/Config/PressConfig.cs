using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// The press (bible ch. 14, the press room): news beats (a launch, an incident, a hearing, the Gauntlet crown) bring a journalist to the
    /// player's desk, the player answers with a line of their own, and a lab can leak a rival's problems. The press room and the auditorium
    /// dampen a scandal and sharpen a launch (see the room perks of kind ScandalDamping and LaunchBuzz).
    /// </summary>
    public sealed class PressConfig : ISync
    {
        /// <summary>Weeks a journalist waits for an answer. After that the story runs without the lab's side and nothing changes.</summary>
        public int InquiryWeeks;
        /// <summary>The most questions waiting at one time; a beat that arrives with the desk full goes unasked.</summary>
        public int MaxOpenInquiries;
        /// <summary>Reputation a launch earns at full buzz; the lab's launch buzz share of this is what it earns.</summary>
        public int LaunchBuzzPoints;
        /// <summary>Caps on what all press rooms together can add up to, so no room ever makes a lab immune.</summary>
        public int MaxDampingPermille;
        public int MaxBuzzPermille;
        /// <summary>No answer is ever less likely than this.</summary>
        public int MinSuccessPermille;

        /// <summary>A leak only works on a rival that had an incident this recently.</summary>
        public int LeakWindowWeeks;
        /// <summary>What a leak costs, in hiring fees of a sales rep of the era, paid win or lose.</summary>
        public int LeakFeeHireFees;
        public int LeakBaseSuccessPermille;
        public int LeakReputationSlopePermille;
        /// <summary>Per permille of scandal damping the lab has, the chance of a leak moves by this share (a lab used to the press knows how to place a story).</summary>
        public int LeakRoomSlopePermille;
        /// <summary>Reputation the rival loses when the leak runs.</summary>
        public int LeakHitPoints;
        /// <summary>Reputation the leaker loses when it is traced back to them (damped by the leaker's own press rooms).</summary>
        public int LeakBackfirePoints;
        /// <summary>How much more the rival dislikes the leaker after a leak that ran, and after one that did not.</summary>
        public int LeakFeelingHit;
        public int LeakFailFeelingHit;

        public List<BeatConfig> Beats = new List<BeatConfig>();
        public List<JournalistConfig> Journalists = new List<JournalistConfig>();
        public List<PressResponseConfig> Responses = new List<PressResponseConfig>();

        public BeatConfig Beat(string id)
        {
            for (int i = 0; i < Beats.Count; i++) if (Beats[i].Id == id) return Beats[i];
            return null;
        }

        public JournalistConfig Journalist(string id)
        {
            for (int i = 0; i < Journalists.Count; i++) if (Journalists[i].Id == id) return Journalists[i];
            return null;
        }

        public PressResponseConfig Response(string id)
        {
            for (int i = 0; i < Responses.Count; i++) if (Responses[i].Id == id) return Responses[i];
            return null;
        }

        public void Sync(Syncer s)
        {
            s.Int("inquiryWeeks", ref InquiryWeeks);
            s.Int("maxOpenInquiries", ref MaxOpenInquiries);
            s.Int("launchBuzzPoints", ref LaunchBuzzPoints);
            s.Int("maxDampingPermille", ref MaxDampingPermille);
            s.Int("maxBuzzPermille", ref MaxBuzzPermille);
            s.Int("minSuccessPermille", ref MinSuccessPermille);
            s.Int("leakWindowWeeks", ref LeakWindowWeeks);
            s.Int("leakFeeHireFees", ref LeakFeeHireFees);
            s.Int("leakBaseSuccessPermille", ref LeakBaseSuccessPermille);
            s.Int("leakReputationSlopePermille", ref LeakReputationSlopePermille);
            s.Int("leakRoomSlopePermille", ref LeakRoomSlopePermille);
            s.Int("leakHitPoints", ref LeakHitPoints);
            s.Int("leakBackfirePoints", ref LeakBackfirePoints);
            s.Int("leakFeelingHit", ref LeakFeelingHit);
            s.Int("leakFailFeelingHit", ref LeakFailFeelingHit);
            s.ObjList("beats", ref Beats);
            s.ObjList("journalists", ref Journalists);
            s.ObjList("responses", ref Responses);
        }
    }

    /// <summary>
    /// A kind of news. A good beat (a launch) is a chance to win reputation; a bad beat (an incident, a hearing) is reputation already lost,
    /// part of which a good answer wins back. The stake is the reputation at play.
    /// </summary>
    public sealed class BeatConfig : ISync, IHasId
    {
        public string IdValue = "";
        public string Name = "";
        /// <summary>True for good news: the stake is <see cref="StakePoints"/>, and a failed answer is a gaffe on top.</summary>
        public bool Good;
        /// <summary>The stake of a good beat. A bad beat (and the crown) stakes what the beat actually moved, and ignores this.</summary>
        public int StakePoints;
        public string Blurb = "";

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Bool("good", ref Good);
            s.Int("stakePoints", ref StakePoints);
            s.Str("blurb", ref Blurb);
        }
    }

    /// <summary>A journalist, who asks about some beats and takes some answers better than others.</summary>
    public sealed class JournalistConfig : ISync, IHasId
    {
        public string IdValue = "";
        public string Name = "";
        public string Outlet = "";
        public List<string> Beats = new List<string>();
        /// <summary>Added to the chance of every answer: a friendly trade writer is easier than a tabloid.</summary>
        public int ChancePermille;
        /// <summary>Scales what a failed answer costs: a tabloid makes a gaffe bigger.</summary>
        public int BackfirePermille;
        public string Blurb = "";

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Str("outlet", ref Outlet);
            s.StrList("beats", ref Beats);
            s.Int("chancePermille", ref ChancePermille);
            s.Int("backfirePermille", ref BackfirePermille);
            s.Str("blurb", ref Blurb);
        }
    }

    /// <summary>One way to answer a journalist: what it costs, how likely it is to work, how much of the stake it wins, and how much it loses if it does not.</summary>
    public sealed class PressResponseConfig : ISync, IHasId
    {
        public string IdValue = "";
        public string Name = "";
        public string Blurb = "";
        /// <summary>The cost in hiring fees of a sales rep of the era, paid whether or not it works.</summary>
        public int CostHireFees;
        public int BaseSuccessPermille;
        /// <summary>Per permille of scandal damping the lab has, the chance moves by this share.</summary>
        public int RoomSlopePermille;
        /// <summary>Per permille the lab's reputation is above 500, the chance moves by this share.</summary>
        public int ReputationSlopePermille;
        /// <summary>The share of the stake won as reputation when it works.</summary>
        public int RecoverPermille;
        /// <summary>The share of the stake lost as reputation when it does not (before the journalist's own scale).</summary>
        public int BackfirePermille;

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Str("blurb", ref Blurb);
            s.Int("costHireFees", ref CostHireFees);
            s.Int("baseSuccessPermille", ref BaseSuccessPermille);
            s.Int("roomSlopePermille", ref RoomSlopePermille);
            s.Int("reputationSlopePermille", ref ReputationSlopePermille);
            s.Int("recoverPermille", ref RecoverPermille);
            s.Int("backfirePermille", ref BackfirePermille);
        }
    }
}
