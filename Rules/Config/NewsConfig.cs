using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// The news ticker (bible ch. 14, "ticker headlines", and ch. 17, "company chat and news ticker"): what the world is saying, in capitals, read off
    /// the event log. It covers every lab, not just the player's. When little has happened it is padded with the filler the news always has.
    /// Placeholders: {lab} the lab the event is about, {subject} the other lab, {model}, {money} the event's amount as $1.5B, {amount}, {ref},
    /// {sector}, {era}, {world}, {generation}, {rival} and {ending}.
    /// </summary>
    public sealed class NewsConfig : ISync
    {
        /// <summary>How many headlines a ticker shows unless the screen asks for another number.</summary>
        public int DefaultHeadlines;
        /// <summary>The ticker is padded with filler to at least this many headlines.</summary>
        public int MinHeadlines;
        public List<HeadlineConfig> Headlines = new List<HeadlineConfig>();
        public List<FillerConfig> Filler = new List<FillerConfig>();

        public void Sync(Syncer s)
        {
            s.Int("defaultHeadlines", ref DefaultHeadlines);
            s.Int("minHeadlines", ref MinHeadlines);
            s.ObjList("headlines", ref Headlines);
            s.ObjList("filler", ref Filler);
        }
    }

    /// <summary>The headline for one kind of event. The first that fits wins.</summary>
    public sealed class HeadlineConfig : ISync, IHasId
    {
        public string IdValue = "";
        /// <summary>An <c>EventKind</c> name.</summary>
        public string Kind = "";
        /// <summary>When 0 or 1, only events with that pick; -1 for any.</summary>
        public int Pick = -1;
        public string Text = "";

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Str("kind", ref Kind);
            s.Int("pick", ref Pick);
            s.Str("text", ref Text);
        }
    }

    /// <summary>A headline the news always has, whatever happened this week.</summary>
    public sealed class FillerConfig : ISync, IHasId
    {
        public string IdValue = "";
        /// <summary>The first year it can run.</summary>
        public int FromYear;
        /// <summary>The last year it can run; 0 for no end.</summary>
        public int ToYear;
        public string Text = "";

        public string Id { get { return IdValue; } }

        public void Sync(Syncer s)
        {
            s.Str("id", ref IdValue);
            s.Int("fromYear", ref FromYear);
            s.Int("toYear", ref ToYear);
            s.Str("text", ref Text);
        }
    }
}
