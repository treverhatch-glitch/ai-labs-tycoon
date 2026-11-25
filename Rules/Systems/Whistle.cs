using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>The whistleblower. See <see cref="WhistleConfig"/>. Only the player's lab has one, and nothing happens while its models show what they are.</summary>
    public static class Whistle
    {
        public const int Clean = 0;
        public const int Silence = 1;

        public static bool Enabled(GameConfig c) { return c.Whistle.Enabled > 0; }

        public static bool Open(LabState lab) { return lab.WhistleDue > 0; }

        /// <summary>The released model whose shown alignment stands furthest above the real one, if any is far enough above to worry anybody.</summary>
        public static ModelState Worst(GameConfig c, LabState lab)
        {
            ModelState pick = null;
            int best = c.Whistle.GapPermille - 1;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.Status != ModelStatus.Released) continue;
                int gap = SafetySystem.AlignmentGap(c, m);
                if (gap > best) { best = gap; pick = m; }
            }
            return pick;
        }

        public static bool Unhappy(GameConfig c, LabState lab)
        {
            for (int i = 0; i < lab.Staff.Count; i++)
                if (lab.Staff[i].RoleId == c.Whistle.RoleId && lab.Staff[i].Morale <= c.Whistle.MoraleBelow) return true;
            return false;
        }

        public static long SilenceCost(GameConfig c, LabState lab) { return c.Whistle.SilenceFee * (lab.Silenced + 1L); }

        public static int LeakReputationLoss(GameConfig c, LabState lab)
        {
            return c.Whistle.LeakReputation + lab.Silenced * c.Whistle.LeakPerSilence;
        }

        static void MoveMorale(GameConfig c, LabState lab, int delta)
        {
            for (int i = 0; i < lab.Staff.Count; i++)
                if (lab.Staff[i].RoleId == c.Whistle.RoleId) lab.Staff[i].Morale = (int)MathI.Clamp((long)lab.Staff[i].Morale + delta, 0, MathI.Permille);
        }

        static void Lose(GameConfig c, LabState lab, int loss)
        {
            lab.Reputation = (int)MathI.Max(0, lab.Reputation - loss);
        }

        static void Close(LabState lab)
        {
            lab.WhistleWeeks = 0;
            lab.WhistleModel = 0;
            lab.WhistleDue = 0;
        }

        public static void Answer(GameConfig c, GameState state, LabState lab, int choice)
        {
            ModelState model = lab.Model(lab.WhistleModel);
            string name = model == null ? "" : model.Name;
            long cost = 0;
            if (choice == Clean)
            {
                if (model != null) model.DisplayedAlignment = model.Alignment;
                Lose(c, lab, PressSystem.Damp(c, lab, c.Whistle.CleanReputation));
                MoveMorale(c, lab, c.Whistle.CleanMorale);
            }
            else
            {
                cost = SilenceCost(c, lab);
                Finance.Spend(lab, LedgerLine.Fines, cost);
                lab.Silenced++;
                MoveMorale(c, lab, -c.Whistle.SilenceMorale);
            }
            GameEvent e = Events.Add(state, EventKind.WhistleAnswered, lab.Id, lab.WhistleModel, cost, name);
            e.Pick = choice;
            Close(lab);
        }

        static void Leak(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            int loss = PressSystem.Damp(c, lab, LeakReputationLoss(c, lab));
            int before = lab.Reputation;
            Lose(c, lab, loss);
            for (int i = 0; i < model.Sectors.Count; i++) TrustSystem.OnExposed(c, state, lab, model.Sectors[i]);
            model.DisplayedAlignment = model.Alignment;
            int people = LabQueries.StaffCount(lab, c.Whistle.RoleId);
            if (people > 0)
            {
                StaffSystem.Fire(lab, c.Whistle.RoleId, 1);
                ControlStale.OnFired(c, lab, c.Whistle.RoleId, 1);
                TalentSystem.DropGoneOffers(lab);
            }
            MoveMorale(c, lab, -c.Whistle.LeakMorale);
            GameEvent e = Events.Add(state, EventKind.WhistleLeaked, lab.Id, model.Id, before - lab.Reputation, model.Name);
            e.Pick = people > 0 ? 1 : 0;
            lab.Silenced = 0;
            Close(lab);
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            if (Open(lab))
            {
                ModelState open = lab.Model(lab.WhistleModel);
                if (open == null || open.Status != ModelStatus.Released || SafetySystem.AlignmentGap(c, open) < c.Whistle.GapPermille)
                {
                    Close(lab);
                    return;
                }
                if (state.Tick + 1 >= lab.WhistleDue) Leak(c, state, lab, open);
                return;
            }
            ModelState worst = Worst(c, lab);
            if (worst != null && Unhappy(c, lab)) lab.WhistleWeeks++;
            else if (lab.WhistleWeeks > 0) lab.WhistleWeeks--;
            if (lab.WhistleWeeks < c.Whistle.WeeksToConcern || worst == null) return;
            lab.WhistleModel = worst.Id;
            lab.WhistleDue = state.Tick + 1 + c.Whistle.AnswerWeeks;
            Events.Add(state, EventKind.WhistleRaised, lab.Id, worst.Id, SafetySystem.AlignmentGap(c, worst), worst.Name);
        }
    }
}
