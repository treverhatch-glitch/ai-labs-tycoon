using System;
using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>One headline on the ticker.</summary>
    public struct Headline
    {
        public int Tick;
        public EventKind Kind;
        /// <summary>The id of the headline or filler line it came from.</summary>
        public string LineId;
        public bool IsFiller;
        public string Text;
    }

    /// <summary>The news ticker. See <see cref="NewsConfig"/>. Read off the event log and the date, never saved, changes no rule.</summary>
    public static class NewsTicker
    {
        public static readonly string[] Placeholders = { "{lab}", "{subject}", "{model}", "{money}", "{amount}", "{ref}", "{sector}", "{era}", "{world}", "{generation}", "{rival}", "{ending}" };

        /// <summary>The newest headlines first. Padded with filler up to the minimum when little has happened.</summary>
        public static List<Headline> Headlines(GameConfig c, GameState state, int max = 0)
        {
            if (max <= 0) max = c.News.DefaultHeadlines;
            var found = new List<Headline>();
            for (int i = state.Events.Count - 1; i >= 0 && found.Count < max; i--)
            {
                Headline h;
                if (About(c, state, state.Events[i], out h)) found.Add(h);
            }
            int want = (int)MathI.Min(max, c.News.MinHeadlines);
            for (int n = 0; found.Count < want && n < c.News.Filler.Count * 2; n++)
            {
                Headline filler;
                if (!PickFiller(c, state, found, n, out filler)) break;
                found.Add(filler);
            }
            return found;
        }

        /// <summary>The headline for one event, or false when the news has nothing to say about it.</summary>
        public static bool About(GameConfig c, GameState state, GameEvent e, out Headline headline)
        {
            headline = new Headline();
            for (int i = 0; i < c.News.Headlines.Count; i++)
            {
                HeadlineConfig line = c.News.Headlines[i];
                if (line.Kind != e.Kind.ToString()) continue;
                if (line.Pick >= 0 && line.Pick != e.Pick) continue;
                headline.Tick = e.Tick;
                headline.Kind = e.Kind;
                headline.LineId = line.IdValue;
                headline.Text = Fill(c, state, line.Text, e).ToUpperInvariant();
                return true;
            }
            return false;
        }

        static bool PickFiller(GameConfig c, GameState state, List<Headline> taken, int attempt, out Headline headline)
        {
            headline = new Headline();
            int year = GameDate.Year(c.Calendar, state.Tick);
            var open = new List<FillerConfig>();
            for (int i = 0; i < c.News.Filler.Count; i++)
            {
                FillerConfig f = c.News.Filler[i];
                if (f.FromYear > year || (f.ToYear > 0 && f.ToYear < year)) continue;
                bool used = false;
                for (int k = 0; k < taken.Count; k++) if (taken[k].IsFiller && taken[k].LineId == f.IdValue) used = true;
                if (!used) open.Add(f);
            }
            if (open.Count == 0) return false;
            long mix = ((long)state.Tick * 31 + taken.Count) * 31 + attempt;
            mix = (mix & 0x7FFFFFFFL) * 2654435761L & 0xFFFFFFFFL;
            FillerConfig chosen = open[(int)((mix >> 8) % open.Count)];
            headline.Tick = state.Tick;
            headline.LineId = chosen.IdValue;
            headline.IsFiller = true;
            headline.Text = chosen.Text.ToUpperInvariant();
            return true;
        }

        static string Fill(GameConfig c, GameState state, string text, GameEvent e)
        {
            LabState lab = state.Lab(e.LabId);
            LabState other = state.Lab(e.SubjectId);
            string first = e.Ref.Split('|')[0];
            ModelState model = lab == null ? null : lab.Model(e.SubjectId);
            SectorConfig sector = c.Sector(first);
            EraConfig era = c.Era(e.Ref);
            WorldEventConfig world = c.WorldEvent(e.Ref);
            GpuGenerationConfig generation = c.Compute.Generation(e.Ref);
            RivalConfig rival = c.Rival(e.Ref);
            EndingDefConfig ending = c.Endings.Def(e.Ref);
            return text
                .Replace("{lab}", lab == null ? "A LAB" : lab.Name)
                .Replace("{subject}", other == null ? "A RIVAL" : other.Name)
                .Replace("{model}", model == null ? e.Ref : model.Name)
                .Replace("{money}", Money(e.Amount))
                .Replace("{amount}", e.Amount.ToString())
                .Replace("{ref}", e.Ref)
                .Replace("{sector}", sector == null ? first : sector.Name)
                .Replace("{era}", era == null ? e.Ref : era.Name)
                .Replace("{world}", world == null ? e.Ref : world.Headline)
                .Replace("{generation}", generation == null ? e.Ref : generation.Name)
                .Replace("{rival}", rival == null ? e.Ref : rival.Name)
                .Replace("{ending}", ending == null ? e.Ref : ending.Name);
        }

        /// <summary>Dollars the way a headline says them: $12, $340K, $1.5M, $40B, $2.2T. Whole-number maths, rounded down.</summary>
        public static string Money(long dollars)
        {
            long a = dollars < 0 ? -dollars : dollars;
            string sign = dollars < 0 ? "-" : "";
            if (a >= 1_000_000_000_000L) return sign + Scaled(a, 1_000_000_000_000L) + "T";
            if (a >= 1_000_000_000L) return sign + Scaled(a, 1_000_000_000L) + "B";
            if (a >= 1_000_000L) return sign + Scaled(a, 1_000_000L) + "M";
            if (a >= 1_000L) return sign + "$" + (a / 1_000L) + "K";
            return sign + "$" + a;
        }

        static string Scaled(long a, long unit)
        {
            long whole = a / unit;
            long tenth = a % unit * 10 / unit;
            return "$" + whole + (tenth > 0 ? "." + tenth : "");
        }
    }
}
