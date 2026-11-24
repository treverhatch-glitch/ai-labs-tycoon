using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    public enum OfficeEffectKind
    {
        /// <summary>Points of reputation (0 to 1000).</summary>
        Reputation,
        /// <summary>Points of safety culture.</summary>
        Culture,
        /// <summary>Points of morale for everyone on the payroll.</summary>
        Morale,
        /// <summary>Weeks of the lab's research output, as research points. Negative loses them (never below none).</summary>
        ResearchWeeks,
        /// <summary>Weeks of what the lab pays every week to stand still, as cash. Negative spends it (never more than the lab has).</summary>
        CashWeeks
    }

    /// <summary>What the lab needs before an office event can happen to it.</summary>
    public enum OfficeRequires
    {
        Nothing,
        /// <summary>A model on sale.</summary>
        ReleasedModel,
        /// <summary>Somebody in the safety function.</summary>
        SafetyStaff,
        /// <summary>At least one person on the payroll.</summary>
        AnyStaff
    }

    /// <summary>
    /// Office events (bible ch. 14, "event ideas"): now and then something happens to the player's lab that has no right answer, and the player picks
    /// a response. An event left unanswered lapses and changes nothing. Only the player's labs have them.
    /// </summary>
    public sealed class OfficeConfig : ISync
    {
        /// <summary>The weekly chance that an event happens when none is waiting, in permille. Worked out from the date and the game's seed, not from a dice stream.</summary>
        public int ChancePermille;
        /// <summary>No event happens before this week of the game's calendar (a game that starts in a later era is already past it).</summary>
        public int FirstAfterWeeks;
        /// <summary>Weeks an event waits for an answer before it lapses.</summary>
        public int ExpireWeeks;
        /// <summary>Cash effects are never worth less than this many dollars a week, so a lab with no running costs still feels them.</summary>
        public long MinWeekCost;
        public List<OfficeEventConfig> Events = new List<OfficeEventConfig>();

        public OfficeEventConfig Event(string id)
        {
            for (int i = 0; i < Events.Count; i++) if (Events[i].IdValue == id) return Events[i];
            return null;
        }

        public void Sync(Syncer s)
        {
            s.Int("chancePermille", ref ChancePermille);
            s.Int("firstAfterWeeks", ref FirstAfterWeeks);
            s.Int("expireWeeks", ref ExpireWeeks);
            s.Long("minWeekCost", ref MinWeekCost);
            s.ObjList("events", ref Events);
        }
    }

    public sealed class OfficeEventConfig : ISync, IHasId
    {
        public string IdValue = "";
        public string Name = "";
        /// <summary>What happened, in a sentence or two.</summary>
        public string Text = "";
        public OfficeRequires Requires;
        public List<OfficeChoiceConfig> Choices = new List<OfficeChoiceConfig>();

        public string Id { get { return IdValue; } }

        public OfficeChoiceConfig Choice(string id)
        {
            for (int i = 0; i < Choices.Count; i++) if (Choices[i].IdValue == id) return Choices[i];
            return null;
        }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Str("text", ref Text);
            s.Enum("requires", ref Requires);
            s.ObjList("choices", ref Choices);
        }
    }

    public sealed class OfficeChoiceConfig : ISync, IHasId
    {
        public string IdValue = "";
        /// <summary>The button.</summary>
        public string Text = "";
        /// <summary>What came of it, in a sentence.</summary>
        public string Result = "";
        public List<OfficeEffectConfig> Effects = new List<OfficeEffectConfig>();

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("text", ref Text);
            s.Str("result", ref Result);
            s.ObjList("effects", ref Effects);
        }
    }

    public sealed class OfficeEffectConfig : ISync
    {
        public OfficeEffectKind Kind;
        public int Amount;

        public void Sync(Syncer s)
        {
            s.Enum("kind", ref Kind);
            s.Int("amount", ref Amount);
        }
    }
}
