using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>A public waitlist for a model not yet out. See <see cref="WaitlistConfig"/>. Only the player's lab.</summary>
    public static class Waitlist
    {
        public static bool Enabled(GameConfig c) { return c.Waitlist.Enabled > 0; }

        public static bool Open(LabState lab) { return lab.WlModel > 0; }

        public static ModelState Model(LabState lab)
        {
            for (int i = 0; i < lab.Models.Count; i++)
                if (lab.Models[i].Id == lab.WlModel) return lab.Models[i];
            return null;
        }

        static void End(GameConfig c, GameState state, LabState lab, int pick)
        {
            int id = lab.WlModel;
            long rep;
            if (pick == 1)
            {
                lab.WlWins++;
                rep = MathI.MulPermille(lab.WlHype, c.Waitlist.WinPerThousand);
                PressSystem.AddReputation(lab, (int)rep);
            }
            else
            {
                if (pick == 0) lab.WlFlops++;
                rep = -PressSystem.Damp(c, lab, (int)MathI.MulPermille(lab.WlHype, pick == 0 ? c.Waitlist.LossPerThousand : c.Waitlist.ClosePerThousand));
                PressSystem.AddReputation(lab, (int)rep);
            }
            lab.WlModel = 0;
            lab.WlHype = 0;
            lab.WlLeft = 0;
            Events.Add(state, EventKind.WaitlistEnded, lab.Id, id, rep, "").Pick = pick;
        }

        public static void Begin(GameConfig c, GameState state, LabState lab, ModelState model, int weeks)
        {
            lab.WlModel = model.Id;
            lab.WlHype = 0;
            lab.WlLeft = weeks;
            Events.Add(state, EventKind.WaitlistOpened, lab.Id, model.Id, weeks, model.Name);
        }

        public static void Close(GameConfig c, GameState state, LabState lab) { End(c, state, lab, 2); }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || !Open(lab)) return;
            ModelState m = Model(lab);
            if (m == null || m.Status == ModelStatus.Retired)
            {
                End(c, state, lab, 0);
                return;
            }
            if (m.Status == ModelStatus.Released)
            {
                End(c, state, lab, 1);
                return;
            }
            lab.WlHype = (int)MathI.Min(1000, lab.WlHype + c.Waitlist.HypeStep);
            lab.WlLeft--;
            if (lab.WlLeft <= 0) End(c, state, lab, 0);
        }
    }
}
