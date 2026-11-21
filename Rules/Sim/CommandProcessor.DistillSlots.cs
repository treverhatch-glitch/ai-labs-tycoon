using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult PurchaseDistillSlots(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            if (!DistillSlotSystem.Enabled(c, state, lab))
                return Fail(CommandError.InvalidArgument, "Distill slot packs are not open yet.");
            int rivalId = cmd.TargetId;
            LabState rival = state.Lab(rivalId);
            if (rival == null || rival.Id == lab.Id)
                return Fail(CommandError.InvalidArgument, "Pick another lab to buy distill access from.");
            if (rival.Controller == LabController.Player)
                return Fail(CommandError.InvalidArgument, "You cannot buy slots from yourself.");
            if (LabQueries.StaffByFunction(c, lab, RoleFunction.Research) < c.DistillSlots.MinResearchers)
                return Fail(CommandError.NotEnoughStaff, "Distill slot deals need at least " + c.DistillSlots.MinResearchers + " researchers.");
            string packId = cmd.Id;
            if (string.IsNullOrEmpty(packId)) packId = cmd.Ids != null && cmd.Ids.Count > 0 ? cmd.Ids[0] : null;
            DistillSlotPackConfig pack = string.IsNullOrEmpty(packId) ? DefaultDistillPack(c) : c.DistillSlots.Pack(packId);
            if (pack == null) return Fail(CommandError.UnknownId, "There is no pack \"" + packId + "\".");
            if (lab.Bankrupt) return Fail(CommandError.LabBankrupt, lab.Name + " is bankrupt.");
            if (lab.Cash < pack.CashCost) return NeedCash(lab, pack.CashCost);
            Finance.Spend(lab, LedgerLine.Hiring, pack.CashCost);
            DistillSlotSystem.Purchase(c, state, lab, rivalId, pack);
            return CommandResult.Done(-1);
        }

        static DistillSlotPackConfig DefaultDistillPack(GameConfig c)
        {
            return c.DistillSlots.Packs.Count > 0 ? c.DistillSlots.Packs[0] : null;
        }
    }
}