using System;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;

namespace AiLabs.Unity.Sim
{
    /// <summary>Wires the rules simulation into the Unity presentation layer. Game rules stay in Rules/.</summary>
    public sealed class SimulationHost
    {
        public Simulation Simulation { get; private set; }

        public event Action StateChanged;

        public void StartNewGame(GameConfig config, long seed)
        {
            Simulation = Simulation.NewGame(config, seed);
            StateChanged?.Invoke();
        }

        public void StepWeek()
        {
            if (Simulation == null) return;
            Simulation.Step();
            StateChanged?.Invoke();
        }
    }
}