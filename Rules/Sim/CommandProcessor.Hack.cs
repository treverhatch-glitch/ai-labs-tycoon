using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult HoldHackWeek(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab holds a hack week.");
            if (!HackWeek.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no hack week in this game.");
            if (HackWeek.Active(lab)) return Fail(CommandError.InvalidArgument, "A hack week is already on.");
            if (Crunch.Active(lab)) return Fail(CommandError.InvalidArgument, "The team cannot hack for fun while it is in crunch time.");
            if (HackWeek.Researchers(c, lab) < c.Hack.MinResearchers)
                return Fail(CommandError.NotEnoughStaff, "A hack week needs " + c.Hack.MinResearchers + " researchers.");
            if (state.Tick < lab.HackLockedUntil)
                return Fail(CommandError.HackLocked, "The team had a hack week recently and cannot have another before week " + lab.HackLockedUntil + ".");
            if (Crunch.AverageMorale(lab) < c.Hack.MinMorale)
                return Fail(CommandError.TeamTooTired, "The team is too tired to enjoy a hack week (morale " + Crunch.AverageMorale(lab) + ", needs " + c.Hack.MinMorale + ").");
            long cost = HackWeek.Cost(c, lab);
            if (lab.Cash < cost) return NeedCash(lab, cost);
            HackWeek.Hold(c, state, lab, cost);
            return CommandResult.Done(-1);
        }
    }
}
