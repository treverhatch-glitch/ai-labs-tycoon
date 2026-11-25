using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult BranchSpecialist(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player lab branches specialists.");
            if (!SpecialistLines.Enabled(c)) return Fail(CommandError.InvalidArgument, "There are no specialist lines in this game.");
            if (c.Eras.IndexOf(EraSystem.AtTick(c, state.Tick)) < c.SpecialistLines.MinEra) return Fail(CommandError.EraLocked, "Specialist lines are not used in this era.");
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.Status != ModelStatus.Trained && model.Status != ModelStatus.Released) return Fail(CommandError.WrongModelStatus, model.Name + " must be trained first.");
            if (model.SpecKind > 0) return Fail(CommandError.InvalidArgument, model.Name + " is already a specialist.");
            if (SpecialistLines.FindKind(c, cmd.Id) == null) return Fail(CommandError.UnknownId, "There is no specialist kind \"" + cmd.Id + "\".");
            if (lab.Cash < c.SpecialistLines.Cost) return NeedCash(lab, c.SpecialistLines.Cost);
            SpecialistLines.Branch(c, state, lab, model, cmd.Id);
            return CommandResult.Done(model.Id);
        }

        static CommandResult AcquireDataSource(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player lab acquires data sources.");
            if (!DataSources.Enabled(c)) return Fail(CommandError.InvalidArgument, "There are no data sources in this game.");
            if (DataSources.Index(c, cmd.Id) < 0) return Fail(CommandError.UnknownId, "There is no data source \"" + cmd.Id + "\".");
            int i = DataSources.Index(c, cmd.Id);
            if (i >= 4) return Fail(CommandError.InvalidArgument, "Too many data sources.");
            int left = i == 0 ? lab.SrcWeeks0 : i == 1 ? lab.SrcWeeks1 : i == 2 ? lab.SrcWeeks2 : lab.SrcWeeks3;
            if (left > 0) return Fail(CommandError.InvalidArgument, "That source is already active.");
            DataSourceItemConfig src = c.DataSources.Sources[i];
            if (lab.Cash < src.WeeklyCost) return NeedCash(lab, src.WeeklyCost);
            DataSources.Acquire(c, state, lab, cmd.Id);
            return CommandResult.Done(i);
        }

        static CommandResult RenewDataSource(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player lab renews data sources.");
            if (!DataSources.Enabled(c)) return Fail(CommandError.InvalidArgument, "There are no data sources in this game.");
            if (DataSources.Index(c, cmd.Id) < 0) return Fail(CommandError.UnknownId, "There is no data source \"" + cmd.Id + "\".");
            DataSourceItemConfig src = c.DataSources.Sources[DataSources.Index(c, cmd.Id)];
            if (lab.Cash < src.WeeklyCost) return NeedCash(lab, src.WeeklyCost);
            DataSources.Renew(c, state, lab, cmd.Id);
            return CommandResult.Done(DataSources.Index(c, cmd.Id));
        }

        static CommandResult SetWeightsPolicy(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player lab sets weights policy.");
            if (!OpenWeights.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no open-weights choice in this game.");
            ModelState model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.Status != ModelStatus.Released) return Fail(CommandError.WrongModelStatus, model.Name + " must be released.");
            OpenWeights.SetPolicy(c, state, lab, model, cmd.Permille);
            return CommandResult.Done(model.Id);
        }

        static CommandResult SetSectorPrice(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player lab sets sector prices.");
            if (!SectorPricing.Enabled(c)) return Fail(CommandError.InvalidArgument, "There is no sector pricing desk in this game.");
            if (c.Sector(cmd.Id) == null) return Fail(CommandError.UnknownId, "There is no sector \"" + cmd.Id + "\".");
            SectorPricing.SetTier(c, state, lab, cmd.Id, cmd.Permille);
            return CommandResult.Done(cmd.Permille);
        }
    }
}