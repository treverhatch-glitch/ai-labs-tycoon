using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult SetCampaignDeadline(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player lab sets a campaign deadline.");
            if (!CampaignDeadline.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no campaign deadline in this game.");
            if (c.Eras.IndexOf(EraSystem.AtTick(c, state.Tick)) < c.CampaignDeadline.MinEra) return Fail(CommandError.EraLocked, "Campaign deadlines are not used in this era.");
            int tick = cmd.Permille;
            if (tick < 0) return Fail(CommandError.OutOfBounds, "A deadline cannot be negative.");
            if (tick > 0 && tick <= state.Tick) return Fail(CommandError.InvalidArgument, "The deadline must be in the future, or zero to turn it off.");
            CampaignDeadline.Set(c, state, lab, tick);
            return CommandResult.Done(tick);
        }

        static CommandResult ShapeLaw(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player lab shapes law.");
            if (!LawShaping.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no law shaping in this game.");
            if (c.Eras.IndexOf(EraSystem.AtTick(c, state.Tick)) < c.LawShaping.MinEra) return Fail(CommandError.EraLocked, "Law shaping is not used in this era.");
            string goal = cmd.Ids.Count > 0 ? cmd.Ids[0] : "";
            if (!LawShaping.IsGoal(goal)) return Fail(CommandError.UnknownId, "Shape law toward \"strict\" or \"loose\", not \"" + goal + "\".");
            if (LawTarget.Enabled(c) && lab.LawTarget != goal)
                return Fail(CommandError.InvalidArgument, "Set the law target to \"" + goal + "\" before spending on lobby.");
            BlocStanding standing = lab.Bloc(cmd.Id);
            if (standing == null || c.World.Bloc(cmd.Id) == null) return Fail(CommandError.UnknownId, "There is no government \"" + cmd.Id + "\".");
            int left = Lobbying.CooldownLeft(state, standing);
            if (left > 0) return Fail(CommandError.LobbyCooling, "The lab lobbied recently. It can shape law again in " + left + (left == 1 ? " week." : " weeks."));
            long cost = Lobbying.Cost(c, state, lab, goal == LawShaping.Loose ? Lobbying.Subsidy : Lobbying.Quiet);
            if (lab.Cash < cost) return NeedCash(lab, cost);
            Finance.Spend(lab, LedgerLine.Hiring, cost);
            LawShaping.Apply(c, state, lab, standing, goal);
            return CommandResult.Done(-1);
        }

        static CommandResult RespondTakeoff(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player answers a takeoff crisis.");
            if (!OfficeEvents.IsPending(lab)) return Fail(CommandError.NoOfficeEvent, "Nothing is waiting for an answer.");
            OfficeEventConfig e = c.Office.Event(lab.OfficeEventId);
            if (e == null || e.Trigger != OfficeTrigger.Takeoff) return Fail(CommandError.NoOfficeEvent, "There is no takeoff crisis waiting.");
            OfficeChoiceConfig choice = e.Choice(cmd.Id);
            if (choice == null) return Fail(CommandError.UnknownId, "\"" + e.Name + "\" has no answer called \"" + cmd.Id + "\".");
            OfficeEvents.Answer(c, state, lab, e, choice);
            TakeoffCrisis.OnAnswered(c, state, lab);
            EndingSystem.TrySuperintelligence(c, state);
            return CommandResult.Done(-1);
        }

        static CommandResult PickScenarioBaggage(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player picks scenario baggage.");
            if (!ScenarioBaggage.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no scenario baggage in this game.");
            if (state.Tick > lab.PlanStartTick) return Fail(CommandError.InvalidArgument, "Baggage is chosen before the run begins.");
            if (lab.BaggagePicked > 0) return Fail(CommandError.InvalidArgument, "Baggage was already chosen.");
            if (c.ScenarioBaggage.Item(cmd.Id) == null) return Fail(CommandError.UnknownId, "There is no baggage \"" + cmd.Id + "\".");
            ScenarioBaggage.Pick(c, state, lab, cmd.Id);
            return CommandResult.Done(-1);
        }
    }
}