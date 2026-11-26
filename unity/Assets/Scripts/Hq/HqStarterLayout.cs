using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.Unity.Sim;

namespace AiLabs.Unity.Hq
{
    /// <summary>Places the same starter desk layout as SimTests <c>WithStarterLab</c> via rules commands.</summary>
    public static class HqStarterLayout
    {
        public static void ApplyIfEmpty(SimulationHost host, bool hireStaff = true, bool rentGpus = true)
        {
            if (host?.Simulation == null) return;
            Simulation sim = host.Simulation;
            LabState player = sim.Player;
            if (player.Rooms.Count > 0) return;

            GameConfig c = sim.Config;
            LotConfig lot = c.Lot(player.LotId);
            BuildRoom(host, lot, player, "research-lab", 4, 4);
            BuildRoom(host, lot, player, "engineering-bay", 4, 4);
            BuildRoom(host, lot, player, "safety-lab", 4, 3);
            BuildRoom(host, lot, player, "sales-office", 4, 3);

            if (hireStaff)
            {
                host.TryApply(Command.Hire(player.Id, "researcher", 2));
                host.TryApply(Command.Hire(player.Id, "engineer", 1));
                host.TryApply(Command.Hire(player.Id, "sales", 1));
            }

            if (rentGpus)
                host.TryApply(Command.SetRentedGpus(player.Id, 6));
        }

        static void BuildRoom(SimulationHost host, LotConfig lot, LabState lab, string typeId, int w, int h)
        {
            if (!LotGrid.FindFreeRect(lot, lab.Rooms, w, h, out int x, out int y))
                return;
            host.TryApply(Command.BuildRoom(lab.Id, typeId, x, y, w, h));
        }
    }
}