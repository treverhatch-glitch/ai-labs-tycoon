using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Research leads from lucky runs. See <see cref="LeadsConfig"/>. Only the player's labs find them.</summary>
    public static class ResearchLeads
    {
        public struct Row
        {
            public int LeadId;
            public int ModelId;
            public string ModelName;
            public int Luck;
            public bool Following;
            /// <summary>Weeks until the lead goes cold, or until the investigation ends when it is being followed.</summary>
            public int WeeksLeft;
            public long FollowCost;
        }

        static bool Finds(LabState lab) { return lab.Controller == LabController.Player; }

        public static ResearchLead Lead(LabState lab, int leadId)
        {
            for (int i = 0; i < lab.Leads.Count; i++) if (lab.Leads[i].Id == leadId) return lab.Leads[i];
            return null;
        }

        public static bool AnyFollowed(LabState lab)
        {
            for (int i = 0; i < lab.Leads.Count; i++) if (lab.Leads[i].Following) return true;
            return false;
        }

        /// <summary>What an investigation costs the lab at this date, in research points.</summary>
        public static long FollowCost(GameConfig c, LabState lab, int tick)
        {
            return MathI.Max(c.Leads.MinCost, (long)c.Leads.FollowCostWeeks * TrainingSystem.ResearchPerWeek(c, lab, tick));
        }

        /// <summary>A model has finished training: if it came out well above its aim, there is something to look into.</summary>
        public static void OnTrained(GameConfig c, GameState state, LabState lab, ModelState model, int luckPermille)
        {
            if (!Finds(lab) || luckPermille < c.Leads.MinLuckPermille || lab.Leads.Count >= c.Leads.MaxOpen) return;
            var lead = new ResearchLead();
            lead.Id = model.Id;
            lead.ModelId = model.Id;
            lead.Luck = luckPermille;
            lead.FoundTick = state.Tick;
            lead.ExpiresTick = state.Tick + c.Leads.ExpireWeeks;
            lead.Ref = model.Name;
            lab.Leads.Add(lead);
            Events.Add(state, EventKind.LeadFound, lab.Id, model.Id, luckPermille, model.Name);
        }

        public static void Follow(GameConfig c, GameState state, LabState lab, ResearchLead lead)
        {
            lab.ResearchPoints -= FollowCost(c, lab, state.Tick);
            lead.DoneTick = state.Tick + c.Leads.FollowWeeks;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            for (int i = lab.Leads.Count - 1; i >= 0; i--)
            {
                ResearchLead lead = lab.Leads[i];
                if (lead.Following)
                {
                    if (state.Tick + 1 >= lead.DoneTick) Resolve(c, state, lab, lead);
                }
                else if (state.Tick + 1 >= lead.ExpiresTick)
                {
                    lab.Leads.RemoveAt(i);
                    Events.Add(state, EventKind.LeadWentCold, lab.Id, lead.Id, 0, lead.Ref);
                }
            }
        }

        /// <summary>The outcome of a lead, picked by weight from the lead itself so the same lead always ends the same way.</summary>
        public static LeadOutcomeConfig OutcomeFor(GameConfig c, ResearchLead lead)
        {
            long total = 0;
            for (int i = 0; i < c.Leads.Outcomes.Count; i++) total += c.Leads.Outcomes[i].Weight;
            if (total <= 0) return null;
            long mix = ((long)lead.Id * 2654435761L + (long)lead.FoundTick * 40503L + (long)lead.ModelId * 7919L + lead.Luck) & 0x7FFFFFFFL;
            mix = (mix * 2654435761L & 0xFFFFFFFFL) >> 8;
            long roll = mix % total;
            for (int i = 0; i < c.Leads.Outcomes.Count; i++)
            {
                roll -= c.Leads.Outcomes[i].Weight;
                if (roll < 0) return c.Leads.Outcomes[i];
            }
            return c.Leads.Outcomes[c.Leads.Outcomes.Count - 1];
        }

        static void Resolve(GameConfig c, GameState state, LabState lab, ResearchLead lead)
        {
            LeadOutcomeConfig outcome = OutcomeFor(c, lead);
            long amount = 0;
            if (outcome != null)
            {
                if (outcome.Kind == LeadEffectKind.ResearchWeeks)
                {
                    amount = outcome.Amount * TrainingSystem.ResearchPerWeek(c, lab, state.Tick);
                    lab.ResearchPoints += amount;
                }
                else if (outcome.Kind == LeadEffectKind.TrainingSpeedPermille)
                {
                    int before = lab.LeadSpeedPermille;
                    lab.LeadSpeedPermille = (int)MathI.Min(c.Leads.MaxSpeedPermille, (long)before + outcome.Amount);
                    amount = lab.LeadSpeedPermille - before;
                }
            }
            lab.Leads.Remove(lead);
            GameEvent e = Events.Add(state, EventKind.LeadResolved, lab.Id, lead.Id, amount, outcome == null ? "" : outcome.Id);
            e.Pick = outcome != null && outcome.Kind != LeadEffectKind.Nothing ? 1 : 0;
        }

        public static List<Row> Rows(GameConfig c, LabState lab, int tick)
        {
            var rows = new List<Row>();
            long cost = FollowCost(c, lab, tick);
            for (int i = 0; i < lab.Leads.Count; i++)
            {
                ResearchLead lead = lab.Leads[i];
                var r = new Row();
                r.LeadId = lead.Id;
                r.ModelId = lead.ModelId;
                r.ModelName = lead.Ref;
                r.Luck = lead.Luck;
                r.Following = lead.Following;
                r.WeeksLeft = (int)MathI.Max(0, (lead.Following ? lead.DoneTick : lead.ExpiresTick) - tick);
                r.FollowCost = lead.Following ? 0 : cost;
                rows.Add(r);
            }
            return rows;
        }
    }
}
