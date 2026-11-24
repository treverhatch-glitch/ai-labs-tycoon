using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// The company chat (bible ch. 14, "a company chat feed where staff react to your decisions in character"). Nothing here is saved: the
    /// feed is read off the event log and the people on the payroll, so a line's speaker is whoever fits it today.
    /// </summary>
    public sealed class ChatConfig : ISync
    {
        /// <summary>How many lines a feed shows unless the screen asks for another number.</summary>
        public int DefaultLines;
        public List<ChatLineConfig> Lines = new List<ChatLineConfig>();

        public ChatLineConfig Line(string id)
        {
            for (int i = 0; i < Lines.Count; i++) if (Lines[i].IdValue == id) return Lines[i];
            return null;
        }

        public void Sync(Syncer s)
        {
            s.Int("defaultLines", ref DefaultLines);
            s.ObjList("lines", ref Lines);
        }
    }

    /// <summary>
    /// One thing a member of staff says about one kind of event. The first line in the list that fits an event and has someone to say it wins,
    /// so put the lines for particular traits before the plain ones.
    /// </summary>
    public sealed class ChatLineConfig : ISync, IHasId
    {
        public string IdValue = "";
        /// <summary>An <c>EventKind</c> name.</summary>
        public string Kind = "";
        /// <summary>The roles that say it. Empty means anyone on the payroll.</summary>
        public List<string> Roles = new List<string>();
        /// <summary>When set, only someone with this trait says it (and the line is skipped when nobody has it).</summary>
        public string Trait = "";
        /// <summary>The event's pick the line is for: -1 for any, or 0 or 1 for events that carry a yes or no.</summary>
        public int Pick = -1;
        /// <summary>"lab" when the event happened to the lab, "subject" when it happened to someone else and the lab is the one it was done to (a poach).</summary>
        public string About = "lab";
        /// <summary>The words. {model}, {amount}, {ref}, {sector}, {role}, {room}, {tech}, {event} and {lab} are filled in.</summary>
        public string Text = "";

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("kind", ref Kind);
            s.StrList("roles", ref Roles);
            s.Str("trait", ref Trait);
            s.Int("pick", ref Pick);
            s.Str("about", ref About);
            s.Str("text", ref Text);
        }
    }

    /// <summary>
    /// The glow (bible ch. 10, "the AI living in the building"): the building shows how risky the worst model is, even when the dashboard
    /// does not. Nothing here is saved either.
    /// </summary>
    public sealed class GlowConfig : ISync
    {
        /// <summary>The combined capability of the lab's models at which the glow is as bright as it gets.</summary>
        public int FullAtCapability;
        public List<GlowStageConfig> Stages = new List<GlowStageConfig>();

        public void Sync(Syncer s)
        {
            s.Int("fullAtCapability", ref FullAtCapability);
            s.ObjList("stages", ref Stages);
        }
    }

    /// <summary>
    /// What the building does at one level of risk. A stage applies to a model when its weekly incident chance reaches <see cref="MinChancePermille"/>
    /// or the rung of the incident it would have reaches <see cref="MinRung"/>; the highest stage that applies wins.
    /// </summary>
    public sealed class GlowStageConfig : ISync, IHasId
    {
        public string IdValue = "";
        public string Name = "";
        /// <summary>The weekly incident chance, in permille, from which this stage applies, or -1 when only the rung counts.</summary>
        public int MinChancePermille = -1;
        /// <summary>The incident rung from which this stage applies, or 0 when only the chance counts.</summary>
        public int MinRung;
        /// <summary>What a person walking through the building sees.</summary>
        public string Sign = "";

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("name", ref Name);
            s.Int("minChancePermille", ref MinChancePermille);
            s.Int("minRung", ref MinRung);
            s.Str("sign", ref Sign);
        }
    }
}
