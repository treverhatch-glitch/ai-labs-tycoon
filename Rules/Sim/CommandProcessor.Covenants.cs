using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult CovenantGate(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab has a loan covenant.");
            if (!Covenants.Enabled(c)) return Fail(CommandError.InvalidArgument, "Loans carry no covenant in this game.");
            if (!Covenants.InBreach(c, state, lab)) return Fail(CommandError.InvalidArgument, "The lab is not in breach of a covenant.");
            return CommandResult.Done(-1);
        }

        static CommandResult WaiveCovenant(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = CovenantGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (Covenants.Waived(state, lab)) return Fail(CommandError.InvalidArgument, "A waiver is already in force.");
            if (lab.CovWaivers >= c.Covenants.MaxWaivers) return Fail(CommandError.InvalidArgument, "The lender will not waive it again.");
            long fee = Covenants.WaiverFee(c, lab);
            if (lab.Cash < fee) return NeedCash(lab, fee);
            Covenants.Waive(c, state, lab);
            return CommandResult.Done(-1);
        }

        static CommandResult CureCovenant(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = CovenantGate(c, state, lab);
            if (!gate.Ok) return gate;
            long pay = Covenants.CureAmount(c, state, lab);
            if (lab.Cash < pay) return NeedCash(lab, pay);
            Covenants.Cure(c, state, lab);
            return CommandResult.Done(-1);
        }
    }
}
