using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult ProgrammeGate(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab runs research programmes.");
            if (!Programmes.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody runs research programmes in this game.");
            return CommandResult.Done(-1);
        }

        static CommandResult StartProgramme(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = ProgrammeGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (Programmes.Active(lab)) return Fail(CommandError.InvalidArgument, "The lab already has a research programme.");
            TechConfig tech = c.Tech(cmd.Id);
            if (tech == null) return Fail(CommandError.UnknownId, "There is no tech \"" + cmd.Id + "\".");
            if (LabQueries.HasTech(lab, tech.Id)) return Fail(CommandError.AlreadyResearched, tech.Name + " is already researched.");
            if (!EraSystem.TechOpen(c, tech, state.Tick)) return Fail(CommandError.EraLocked, tech.Name + " is not invented yet.");
            for (int i = 0; i < tech.Requires.Count; i++)
                if (!LabQueries.HasTech(lab, tech.Requires[i])) return Fail(CommandError.MissingPrerequisite, tech.Name + " needs " + c.Tech(tech.Requires[i]).Name + " first.");
            if (tech.Cost < c.Programmes.MinCost) return Fail(CommandError.InvalidArgument, tech.Name + " is too small for a programme.");
            Programmes.Start(c, state, lab, c.Research.Techs.FindIndex(t => t.Id == tech.Id));
            return CommandResult.Done(-1);
        }

        static CommandResult ChooseApproach(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = ProgrammeGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (!Programmes.Active(lab) || lab.PgPhase != Programmes.Choosing) return Fail(CommandError.InvalidArgument, "No stage is waiting for an approach.");
            if (cmd.Count < 1 || cmd.Count > 2) return Fail(CommandError.InvalidArgument, "The approach is 1 careful or 2 bold.");
            long cost = Programmes.StageCost(c, lab, cmd.Count == 2);
            if (lab.ResearchPoints < cost) return Fail(CommandError.NotEnoughResearch, "The stage costs " + cost + " research points and the lab has " + lab.ResearchPoints + ".");
            Programmes.Run(c, lab, cmd.Count == 2);
            return CommandResult.Done(-1);
        }

        static CommandResult RecoverStage(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = ProgrammeGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (!Programmes.Active(lab) || lab.PgPhase != Programmes.Failed) return Fail(CommandError.InvalidArgument, "No failed stage is waiting for a decision.");
            if (cmd.Count < 1 || cmd.Count > 3) return Fail(CommandError.InvalidArgument, "The choice is 1 retry boldly, 2 fall back to careful or 3 give up.");
            if (cmd.Count == 3)
            {
                Programmes.Abandon(c, state, lab);
                return CommandResult.Done(-1);
            }
            long cost = Programmes.StageCost(c, lab, cmd.Count == 1);
            if (lab.ResearchPoints < cost) return Fail(CommandError.NotEnoughResearch, "The stage costs " + cost + " research points and the lab has " + lab.ResearchPoints + ".");
            Programmes.Run(c, lab, cmd.Count == 1);
            return CommandResult.Done(-1);
        }

        static CommandResult AbandonProgramme(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = ProgrammeGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (!Programmes.Active(lab)) return Fail(CommandError.InvalidArgument, "The lab has no research programme.");
            Programmes.Abandon(c, state, lab);
            return CommandResult.Done(-1);
        }
    }
}
