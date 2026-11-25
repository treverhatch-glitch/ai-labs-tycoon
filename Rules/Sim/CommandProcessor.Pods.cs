using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

using System.Collections.Generic;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult PodGate(GameConfig c, GameState state, LabState lab)
        {
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab forms pods.");
            if (!Pods.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody forms pods in this game.");
            return CommandResult.Done(-1);
        }

        static CommandResult FormPod(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = PodGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (Pods.Active(lab)) return Fail(CommandError.InvalidArgument, "The lab already has a pod.");
            var ids = new List<int>();
            if (cmd.Ids != null)
                for (int i = 0; i < cmd.Ids.Count; i++)
                {
                    int id;
                    if (!int.TryParse(cmd.Ids[i], out id)) return Fail(CommandError.InvalidArgument, "A staff id is a number.");
                    ids.Add(id);
                }
            if (ids.Count < c.Pods.MinSize || ids.Count > c.Pods.MaxSize) return Fail(CommandError.InvalidArgument, "A pod has " + c.Pods.MinSize + " to " + c.Pods.MaxSize + " people.");
            if (new HashSet<int>(ids).Count != ids.Count) return Fail(CommandError.InvalidArgument, "A person can only be in the pod once.");
            for (int i = 0; i < ids.Count; i++)
                if (Pods.Person(lab, ids[i]) == null) return Fail(CommandError.UnknownId, "The lab has no such person.");
            long cost = ids.Count * (long)c.Pods.KickoffCost;
            if (lab.Cash < cost) return NeedCash(lab, cost);
            Pods.Form(c, state, lab, ids);
            return CommandResult.Done(-1);
        }

        static CommandResult SwapPodMember(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = PodGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (!Pods.Active(lab)) return Fail(CommandError.InvalidArgument, "The lab has no pod.");
            if (state.Tick < lab.PodSwapAfter) return Fail(CommandError.InvalidArgument, "A member was swapped lately; wait until week " + lab.PodSwapAfter + ".");
            if (!lab.PodIds.Contains(cmd.TargetId)) return Fail(CommandError.InvalidArgument, "That person is not in the pod.");
            if (lab.PodIds.Contains(cmd.Count)) return Fail(CommandError.InvalidArgument, "That person is already in the pod.");
            if (Pods.Person(lab, cmd.Count) == null) return Fail(CommandError.UnknownId, "The lab has no such person.");
            Pods.Swap(c, state, lab, cmd.TargetId, cmd.Count);
            return CommandResult.Done(-1);
        }

        static CommandResult DisbandPod(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            CommandResult gate = PodGate(c, state, lab);
            if (!gate.Ok) return gate;
            if (!Pods.Active(lab)) return Fail(CommandError.InvalidArgument, "The lab has no pod.");
            Pods.End(c, state, lab, 1);
            return CommandResult.Done(-1);
        }
    }
}
