using System;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;

namespace AiLabs.Unity.Sim
{
    /// <summary>Wires the rules simulation into the Unity presentation layer. Game rules stay in Rules/.</summary>
    public sealed class SimulationHost
    {
        public Simulation Simulation { get; private set; }
        public CommandResult LastResult { get; private set; }
        public string StatusMessage { get; private set; }
        GameConfig _config;

        public event Action StateChanged;

        public void StartNewGame(GameConfig config, long seed)
        {
            _config = config;
            Simulation = Simulation.NewGame(config, seed);
            StatusMessage = "";
            StateChanged?.Invoke();
        }

        public string SaveJson()
        {
            return Simulation == null ? "" : Simulation.Save();
        }

        public bool LoadJson(string saveJson)
        {
            if (_config == null || string.IsNullOrEmpty(saveJson)) return false;
            Simulation = Simulation.Load(_config, saveJson);
            StatusMessage = "";
            StateChanged?.Invoke();
            return true;
        }

        public bool TryApply(Command command)
        {
            if (Simulation == null) return false;
            LastResult = Simulation.Apply(command);
            StatusMessage = LastResult.Ok ? "" : LastResult.Message;
            StateChanged?.Invoke();
            return LastResult.Ok;
        }

        public void StepWeek()
        {
            if (Simulation == null) return;
            Simulation.Step();
            StatusMessage = "";
            StateChanged?.Invoke();
        }

        public void StepWeeks(int weeks)
        {
            if (Simulation == null || weeks <= 0) return;
            Simulation.StepMany(weeks);
            StatusMessage = "";
            StateChanged?.Invoke();
        }
    }
}