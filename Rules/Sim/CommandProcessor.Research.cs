using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult ResearchTech(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (WorldQueries.IsFrozen(lab, lab.HomeBlocId))
                return Fail(CommandError.PermitFrozen, c.World.Bloc(lab.HomeBlocId).Agency + " has frozen " + lab.Name + "'s permits at home.");
            TechConfig tech = c.Tech(cmd.Id);
            if (tech == null) return Fail(CommandError.UnknownId, "There is no tech \"" + cmd.Id + "\".");
            if (LabQueries.HasTech(lab, tech.Id)) return Fail(CommandError.AlreadyResearched, tech.Name + " is already researched.");
            if (ResearchQueries.IsBlocked(lab, tech.Id))
                return Fail(CommandError.TechForkBlocked, tech.Name + " is down a branch " + lab.Name + " did not choose.");
            if (ResearchQueries.PatentedElsewhere(c, state, lab.Id, tech.Id, out LabState holder))
                return Fail(CommandError.PatentBlocked, holder.Name + " holds a patent on " + tech.Name + ".");
            if (!EraSystem.TechOpen(c, tech, state.Tick))
                return Fail(CommandError.EraLocked, tech.Name + " is not invented yet: it arrives with " + c.Era(tech.Era).Name + " (" + c.Era(tech.Era).StartYear + ").");
            for (int i = 0; i < tech.Requires.Count; i++)
                if (!LabQueries.HasTech(lab, tech.Requires[i]))
                    return Fail(CommandError.MissingPrerequisite, tech.Name + " needs " + c.Tech(tech.Requires[i]).Name + " first.");
            long cost = LabQueries.TechCost(c, state, lab, tech);
            if (lab.ResearchPoints < cost)
                return Fail(CommandError.NotEnoughResearch, tech.Name + " costs " + cost + " research points and the lab has " + lab.ResearchPoints + ".");

            lab.ResearchPoints -= cost;
            lab.Techs.Add(tech.Id);
            ResearchQueries.ApplyFork(c, lab, tech.Id);
            Events.Add(state, EventKind.TechResearched, lab.Id, -1, cost, tech.Id);
            return CommandResult.Done(-1);
        }

        static CommandResult FilePatent(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Controller == LabController.Plan)
                return Fail(CommandError.InvalidArgument, "Rival labs file patents through their own counsel.");
            if (WorldQueries.IsFrozen(lab, lab.HomeBlocId))
                return Fail(CommandError.PermitFrozen, c.World.Bloc(lab.HomeBlocId).Agency + " has frozen " + lab.Name + "'s permits at home.");
            TechConfig tech = c.Tech(cmd.Id);
            if (tech == null) return Fail(CommandError.UnknownId, "There is no tech \"" + cmd.Id + "\".");
            if (!LabQueries.HasTech(lab, tech.Id))
                return Fail(CommandError.MissingPrerequisite, "Research " + tech.Name + " before filing.");
            if (!tech.Patentable)
                return Fail(CommandError.NotPatentable, tech.Name + " is prior art, not a product.");
            if (ResearchQueries.ActivePatent(lab, tech.Id, state.Tick) != null)
                return Fail(CommandError.PatentAlreadyFiled, lab.Name + " already holds a patent on " + tech.Name + ".");
            if (c.Research.Patents == null || c.Research.Patents.DurationWeeks < 1)
                return Fail(CommandError.InvalidArgument, "Patent filings are not configured.");
            long cost = ResearchQueries.PatentFilingCost(c, tech);
            if (lab.Cash < cost) return NeedCash(lab, cost);
            int until = state.Tick + c.Research.Patents.DurationWeeks;
            var p = new PatentState { TechId = tech.Id, FiledTick = state.Tick, ExpiresTick = until };
            lab.Patents.Add(p);
            Finance.Spend(lab, LedgerLine.Fines, cost);
            Events.Add(state, EventKind.PatentFiled, lab.Id, -1, cost, tech.Id);
            return CommandResult.Done(-1);
        }

        static CommandResult RenouncePatent(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Controller == LabController.Plan)
                return Fail(CommandError.InvalidArgument, "Rival labs file patents through their own counsel.");
            if (WorldQueries.IsFrozen(lab, lab.HomeBlocId))
                return Fail(CommandError.PermitFrozen, c.World.Bloc(lab.HomeBlocId).Agency + " has frozen " + lab.Name + "'s permits at home.");
            TechConfig tech = c.Tech(cmd.Id);
            if (tech == null) return Fail(CommandError.UnknownId, "There is no tech \"" + cmd.Id + "\".");
            PatentState patent = ResearchQueries.ActivePatent(lab, tech.Id, state.Tick);
            if (patent == null) return Fail(CommandError.InvalidArgument, lab.Name + " holds no active patent on " + tech.Name + ".");
            lab.Patents.Remove(patent);
            Events.Add(state, EventKind.PatentExpired, lab.Id, -1, 0, tech.Id);
            return CommandResult.Done(-1);
        }
    }
}