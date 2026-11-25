using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult StandardsGate(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab has a part in the industry standards.");
            if (!Standards.Enabled(c)) return Fail(CommandError.InvalidArgument, "There are no industry standards in this game.");
            return CommandResult.Done(-1);
        }

        static CommandResult TakeStandardsRole(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = StandardsGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (cmd.Count < Standards.Observer || cmd.Count > Standards.Obstructor) return Fail(CommandError.InvalidArgument, "A part is 0 (observer), 1 (member), 2 (sponsor) or 3 (obstructor).");
            if (cmd.Count == lab.StdRole) return Fail(CommandError.InvalidArgument, "The lab already has that part.");
            if (!Standards.Drafting(lab)) return Fail(CommandError.InvalidArgument, "No standard is being drafted.");
            if (cmd.Count == Standards.Sponsor && lab.Cash < c.Standards.SponsorFee) return NeedCash(lab, c.Standards.SponsorFee);
            Standards.TakeRole(c, lab, cmd.Count);
            return CommandResult.Done(-1);
        }

        static CommandResult AdoptStandards(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = StandardsGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (lab.StdLevel <= lab.StdAdopted) return Fail(CommandError.InvalidArgument, "There is no new standard to adopt.");
            long price = Standards.AdoptPrice(c, lab);
            if (lab.Cash < price) return NeedCash(lab, price);
            Standards.Adopt(c, state, lab);
            return CommandResult.Done(-1);
        }

        static CommandResult DropStandards(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = StandardsGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (lab.StdAdopted == 0) return Fail(CommandError.InvalidArgument, "The lab complies with no standard.");
            lab.StdAdopted = 0;
            return CommandResult.Done(-1);
        }
    }
}
