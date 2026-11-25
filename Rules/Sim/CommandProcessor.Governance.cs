using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult GovernanceGate(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab chooses how it is governed.");
            if (!Governance.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody changes how a company is governed in this game.");
            return CommandResult.Done(-1);
        }

        static CommandResult SetGovernance(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = GovernanceGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (cmd.Count != Governance.Dual && cmd.Count != Governance.Nonprofit) return Fail(CommandError.InvalidArgument, "The structures are dual-class shares (1) and a non-profit parent (2).");
            if (lab.GovForm != 0) return Fail(CommandError.InvalidArgument, "The lab has already changed how it is controlled, and it cannot be undone.");
            int rounds = lab.FundingRounds.Count;
            int most = cmd.Count == Governance.Dual ? c.Governance.DualMaxRounds : c.Governance.NonprofitMaxRounds;
            if (rounds > most) return Fail(CommandError.InvalidArgument, "Investors in " + rounds + " rounds will not agree to it; the most is " + most + ".");
            Governance.Set(c, state, lab, cmd.Count);
            return CommandResult.Done(-1);
        }

        static CommandResult SeatDirector(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = GovernanceGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (lab.GovSearch > 0) return Fail(CommandError.InvalidArgument, "A search is already under way.");
            if (lab.GovDirectors >= c.Governance.MaxDirectors) return Fail(CommandError.InvalidArgument, "The board has all the independent directors it can seat.");
            if (lab.Reputation < c.Governance.DirectorMinRep) return Fail(CommandError.MissingPrerequisite, "Nobody independent will join a lab with a reputation under " + c.Governance.DirectorMinRep + ".");
            if (lab.Cash < c.Governance.DirectorFee) return NeedCash(lab, c.Governance.DirectorFee);
            Governance.StartSearch(c, lab);
            return CommandResult.Done(-1);
        }
    }
}
