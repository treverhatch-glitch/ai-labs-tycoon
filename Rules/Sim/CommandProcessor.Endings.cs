using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult ResearchFrontier(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (!EndingSystem.AllOpenTechsDone(c, lab, state.Tick))
                return Fail(CommandError.FrontierUnavailable, "Frontier research opens once every tech of this year is researched.");
            long cost = EndingSystem.FrontierCost(c, lab);
            if (cost < 0) return Fail(CommandError.FrontierUnavailable, "The frontier is capped at level " + c.Endings.Frontier.MaxLevel + ".");
            if (lab.ResearchPoints < cost)
                return Fail(CommandError.NotEnoughResearch, "The next frontier level costs " + cost + " research points.");
            lab.ResearchPoints -= cost;
            lab.FrontierLevel++;
            Events.Add(state, EventKind.TechResearched, lab.Id, -1, cost, "frontier");
            return CommandResult.Done(-1);
        }

        static CommandResult SellLab(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab can be sold.");
            if (lab.Bankrupt) return Fail(CommandError.LabBankrupt, lab.Name + " is bankrupt.");
            string mode = cmd.Id ?? "";
            if (mode != "found-again" && mode != "spectate") return Fail(CommandError.InvalidArgument, "Sell as found-again or spectate.");
            long rawDebt = ExitDebt.Total(c, lab);
            long adjDebt = ExitDebtEraHaircut.AdjustDebt(c, state, rawDebt);
            if (adjDebt < rawDebt) ExitDebtEraHaircut.OnExit(c, state, lab, rawDebt, adjDebt);
            long price = EndingSystem.SellPrice(c, state, lab);
            if (price <= lab.Cash) return Fail(CommandError.SellRefused, "Nothing to sell: the price is at most the cash already in the lab.");

            RivalConfig successor = c.Rival("successor");
            if (successor == null) return Fail(CommandError.UnknownId, "There is no successor plan.");

            lab.EquityLevel = 0;
            lab.EquityPermille = 0;
            string soldName = lab.Name;
            string founderId = lab.FounderId, styleId = lab.StyleId;
            lab.FounderId = "";
            lab.StyleId = "";
            lab.Controller = LabController.Plan;
            lab.RivalId = "successor";
            if (!lab.Name.Contains("(new owners)")) lab.Name += " (new owners)";

            if (mode == "found-again")
            {
                LabState fresh = WorldSetup.FoundAgain(c, state, soldName, lab.Leader, lab.FamilyName, price, founderId, styleId);
                state.Labs.Insert(0, fresh);
                state.PlayerLabId = fresh.Id;
            }
            else
            {
                state.PlayerLabId = -1;
            }

            EndingSystem.RecordExit(c, state, lab.Id, mode);
            return CommandResult.Done(freshId(state, mode));
        }

        static int freshId(GameState state, string mode)
        {
            if (mode == "found-again" && state.Labs.Count > 0) return state.Labs[0].Id;
            return -1;
        }

        static CommandResult ContinueAfterEnding(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (cmd.TargetId < 0 || cmd.TargetId >= state.Endings.Count)
                return Fail(CommandError.EndingUnavailable, "There is no ending record " + cmd.TargetId + ".");
            EndingRecord record = state.Endings[cmd.TargetId];
            if (record.LabId != lab.Id) return Fail(CommandError.EndingUnavailable, "That ending belongs to another lab.");
            if (record.Choice != "") return Fail(CommandError.AlreadyChose, "That ending choice was already made.");
            EndingDefConfig def = c.Endings.Def(record.EndingId);
            if (def == null) return Fail(CommandError.UnknownId, "Unknown ending.");
            bool allowed = false;
            for (int i = 0; i < def.ContinueModes.Count; i++) if (def.ContinueModes[i] == cmd.Id) { allowed = true; break; }
            if (!allowed) return Fail(CommandError.EndingUnavailable, "That mode is not offered for " + def.Name + ".");

            if (record.EndingId == EndingSystem.BankruptId && cmd.Id == "bailout")
                EndingSystem.ApplyBailout(c, state, lab);
            else
                EndingSystem.ApplyContinue(c, state, lab, record, cmd.Id);

            record.Choice = cmd.Id;
            record.ChoiceTick = state.Tick;
            return CommandResult.Done(-1);
        }
    }
}