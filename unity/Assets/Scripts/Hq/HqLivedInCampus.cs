using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Unity.Sim;

namespace AiLabs.Unity.Hq
{
    /// <summary>Starter layout, staff, a live training run, and time steps for screenshots.</summary>
    public static class HqLivedInCampus
    {
        public const int DefaultTrainWeeks = 16;

        public static void Apply(SimulationHost host, int trainWeeks = DefaultTrainWeeks)
        {
            if (host?.Simulation == null) return;
            HqStarterLayout.ApplyIfEmpty(host);
            LabState player = host.Simulation.Player;
            host.TryApply(Command.StartTraining(player.Id, "mini", 500));
            if (trainWeeks > 0) host.StepWeeks(trainWeeks);
        }
    }
}