using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult LicenceGate(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab holds a licence.");
            if (!Licence.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody licenses labs in this game.");
            return CommandResult.Done(-1);
        }

        static CommandResult ApplyLicence(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = LicenceGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (Licence.Active(lab)) return Fail(CommandError.InvalidArgument, "The lab already holds or has applied for a licence.");
            if (cmd.Count < 1 || cmd.Count > 3) return Fail(CommandError.InvalidArgument, "The regime is 1 registry, 2 licence or 3 audit regime.");
            if (!Licence.Open(c, cmd.Count, state.Tick)) return Fail(CommandError.EraLocked, "That regime is not open yet.");
            if (state.Tick < lab.LicCooldown) return Fail(CommandError.InvalidArgument, "The lab cannot apply again until week " + lab.LicCooldown + ".");
            long fee = Licence.Fee(c, cmd.Count);
            if (lab.Cash < fee) return NeedCash(lab, fee);
            Licence.Apply(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }

        static CommandResult RenewLicence(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = LicenceGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (!Licence.Held(lab)) return Fail(CommandError.InvalidArgument, "The lab holds no licence.");
            if (lab.LicLeft > c.Licence.RenewWindow) return Fail(CommandError.InvalidArgument, "A licence can only be renewed in its last " + c.Licence.RenewWindow + " weeks.");
            if (!Licence.MeetsBar(c, lab, lab.LicRegime)) return Fail(CommandError.MissingPrerequisite, "The lab's best released model is not aligned enough to renew.");
            long fee = Licence.Fee(c, lab.LicRegime);
            if (lab.Cash < fee) return NeedCash(lab, fee);
            Licence.Renew(c, state, lab);
            return CommandResult.Done(-1);
        }

        static CommandResult SurrenderLicence(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = LicenceGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (!Licence.Active(lab)) return Fail(CommandError.InvalidArgument, "The lab holds no licence and has no application.");
            Licence.Surrender(c, state, lab);
            return CommandResult.Done(-1);
        }
    }
}
