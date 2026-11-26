using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SalvageMuseumLine(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (string.IsNullOrEmpty(cmd.Id)) return Fail(CommandError.InvalidArgument, "Name the model line to salvage.");
            if (c.Line(cmd.Id) == null) return Fail(CommandError.UnknownId, "There is no model line \"" + cmd.Id + "\".");
            int cap = c.Endings.Museum.PerLine;
            int retiredBefore = MuseumSystem.RetiredOnLine(lab, cmd.Id);
            if (retiredBefore <= cap)
                return Fail(CommandError.InvalidArgument, "That line is not over the museum cap.");
            long cost = c.Endings.Museum.ManualSalvageCashCost;
            if (cost > 0 && lab.Cash < cost) return NeedCash(lab, cost);
            long gained = MuseumSystem.SalvageOneOverCap(lab, cmd.Id, c.Endings.Museum.ResearchPermillePerCapability, cap);
            if (MuseumSystem.RetiredOnLine(lab, cmd.Id) >= retiredBefore)
                return Fail(CommandError.InvalidArgument, "Nothing to salvage on that line.");
            if (cost > 0) Finance.Spend(lab, LedgerLine.Construction, cost);
            lab.ResearchPoints += gained;
            Events.Add(state, EventKind.TechResearched, lab.Id, -1, gained, "museum-salvage-manual");
            return CommandResult.Done(-1);
        }
    }
}