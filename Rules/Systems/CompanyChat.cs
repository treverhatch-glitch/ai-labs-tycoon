using System;
using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>One line in the company chat.</summary>
    public struct ChatMessage
    {
        public int Tick;
        public EventKind Kind;
        public string LineId;
        public int StaffId;
        public string RoleId;
        public string TraitId;
        /// <summary>The speaker as a screen shows it: the role and the person's number.</summary>
        public string Speaker;
        public string Text;
    }

    /// <summary>The company chat: staff react to what happened to the lab, in character. Read off the event log, never saved.</summary>
    public static class CompanyChat
    {
        public static List<ChatMessage> Feed(GameConfig c, GameState state, LabState lab, int max = 0)
        {
            if (max <= 0) max = c.Chat.DefaultLines;
            var found = new List<ChatMessage>();
            for (int i = state.Events.Count - 1; i >= 0 && found.Count < max; i--)
            {
                ChatMessage m;
                if (Say(c, state, lab, state.Events[i], out m)) found.Add(m);
            }
            found.Reverse();
            return found;
        }

        /// <summary>What the lab's people would say about one event, or false when nobody has anything to say.</summary>
        public static bool Say(GameConfig c, GameState state, LabState lab, GameEvent e, out ChatMessage message)
        {
            message = new ChatMessage();
            for (int i = 0; i < c.Chat.Lines.Count; i++)
            {
                ChatLineConfig line = c.Chat.Lines[i];
                if (!Fits(c, line, lab, e)) continue;
                StaffMember speaker = Speaker(c, lab, line, e);
                if (speaker == null) continue;
                RoleConfig role = c.Role(speaker.RoleId);
                message.Tick = e.Tick;
                message.Kind = e.Kind;
                message.LineId = line.IdValue;
                message.StaffId = speaker.Id;
                message.RoleId = speaker.RoleId;
                message.TraitId = speaker.TraitId;
                message.Speaker = (role == null ? speaker.RoleId : role.Name) + " " + speaker.Id;
                message.Text = Fill(c, state, lab, line.Text, e);
                return true;
            }
            return false;
        }

        static bool Fits(GameConfig c, ChatLineConfig line, LabState lab, GameEvent e)
        {
            if (line.Kind != e.Kind.ToString()) return false;
            if (line.FromYear > 0 || line.ToYear > 0)
            {
                int year = GameDate.Year(c.Calendar, e.Tick);
                if (line.FromYear > 0 && year < line.FromYear) return false;
                if (line.ToYear > 0 && year > line.ToYear) return false;
            }
            if (line.Pick >= 0 && line.Pick != e.Pick) return false;
            if (line.About == "subject") return e.SubjectId == lab.Id;
            return e.LabId == lab.Id;
        }

        static StaffMember Speaker(GameConfig c, LabState lab, ChatLineConfig line, GameEvent e)
        {
            var fits = new List<StaffMember>();
            for (int i = 0; i < lab.Staff.Count; i++)
            {
                StaffMember person = lab.Staff[i];
                if (line.Roles.Count > 0 && !line.Roles.Contains(person.RoleId)) continue;
                if (line.Trait.Length > 0 && person.TraitId != line.Trait) continue;
                fits.Add(person);
            }
            if (fits.Count == 0) return null;
            return fits[Pick(e, fits.Count)];
        }

        static int Pick(GameEvent e, int count)
        {
            long mix = (((long)e.Tick * 31 + (int)e.Kind) * 31 + e.SubjectId) * 31 + e.Amount;
            mix = (mix & 0x7FFFFFFFL) * 2654435761L & 0xFFFFFFFFL;
            return (int)((mix >> 8) % count);
        }

        static string Fill(GameConfig c, GameState state, LabState lab, string text, GameEvent e)
        {
            string model = e.Ref;
            ModelState named = e.SubjectId >= 0 ? lab.Model(e.SubjectId) : null;
            if (named != null) model = named.Name;
            string first = e.Ref.Split('|')[0];
            SectorConfig sector = c.Sector(first);
            RoleConfig role = c.Role(e.Ref);
            RoomTypeConfig roomType = c.RoomType(e.Ref);
            TechConfig techConfig = c.Tech(e.Ref);
            OfficeEventConfig officeEvent = c.Office.Event(first);
            return text
                .Replace("{model}", model)
                .Replace("{amount}", e.Amount.ToString())
                .Replace("{ref}", e.Ref)
                .Replace("{sector}", sector == null ? first : sector.Name)
                .Replace("{role}", role == null ? e.Ref : role.Name)
                .Replace("{room}", roomType == null ? e.Ref : roomType.Name)
                .Replace("{tech}", techConfig == null ? e.Ref : techConfig.Name)
                .Replace("{event}", officeEvent == null ? first : officeEvent.Name)
                .Replace("{lab}", lab.Name);
        }

        public static bool IsEventKind(string name)
        {
            EventKind ignored;
            return Enum.TryParse(name, out ignored);
        }
    }
}
