using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SetPlaybook(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player lab sets the runaway playbook.");
            if (!RunawayPlaybook.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no runaway playbook in this game.");
            if (c.Eras.IndexOf(EraSystem.AtTick(c, state.Tick)) < c.RunawayPlaybook.MinEra) return Fail(CommandError.EraLocked, "The runaway playbook is not used in this era.");
            if (cmd.Count < 0 || cmd.Count > c.RunawayPlaybook.MaxLevel) return Fail(CommandError.InvalidArgument, "The playbook goes from level 0 to " + c.RunawayPlaybook.MaxLevel + ".");
            if (cmd.Count == RunawayPlaybook.Level(c, lab)) return Fail(CommandError.InvalidArgument, "The playbook is already at level " + cmd.Count + ".");
            int added = (int)MathI.Max(0, cmd.Count - RunawayPlaybook.Level(c, lab));
            long cost = added * (long)c.RunawayPlaybook.SetupPerLevel;
            if (cost > 0 && lab.Cash < cost) return NeedCash(lab, cost);
            RunawayPlaybook.Set(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }

        static CommandResult ArmPlaybook(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player lab arms the runaway playbook.");
            if (!RunawayPlaybook.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no runaway playbook in this game.");
            if (c.Eras.IndexOf(EraSystem.AtTick(c, state.Tick)) < c.RunawayPlaybook.MinEra) return Fail(CommandError.EraLocked, "The runaway playbook is not used in this era.");
            if (RunawayPlaybook.Level(c, lab) <= 0) return Fail(CommandError.InvalidArgument, "Set a playbook level first.");
            if (RunawayPlaybook.Armed(lab)) return Fail(CommandError.InvalidArgument, "The playbook is already armed.");
            if (RunawayPlaybook.ArmLocked(state, lab)) return Fail(CommandError.BountyLocked, "The playbook was armed recently and holds until week " + lab.PlayArmUntil + ".");
            if (LabQueries.StaffCount(lab, "safety") < c.RunawayPlaybook.Staff) return Fail(CommandError.NotEnoughStaff, "Arming the playbook needs " + c.RunawayPlaybook.Staff + " safety people.");
            RunawayPlaybook.Arm(c, state, lab);
            return CommandResult.Done(-1);
        }
    }
}