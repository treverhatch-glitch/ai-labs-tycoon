using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult StartDrill(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player lab runs drills.");
            if (!Drills.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody runs drills in this game.");
            if (c.Eras.IndexOf(EraSystem.AtTick(c, state.Tick)) < c.Drills.MinEra) return Fail(CommandError.EraLocked, "Drills are not used in this era.");
            if (lab.DrillLeft > 0) return Fail(CommandError.InvalidArgument, "A drill is already running.");
            if (LabQueries.StaffCount(lab, "safety") < c.Drills.Staff) return Fail(CommandError.NotEnoughStaff, "A drill needs " + c.Drills.Staff + " safety people.");
            if (lab.Cash < c.Drills.Cost) return NeedCash(lab, c.Drills.Cost);
            Drills.Start(c, state, lab);
            return CommandResult.Done(-1);
        }
    }
}
