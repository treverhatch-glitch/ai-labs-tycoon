using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>The training-cluster fabric, built in tiers with lead times. See <see cref="FabricConfig"/>. Only the player's lab.</summary>
    public static class Fabric
    {
        public static bool Enabled(GameConfig c) { return c.Fabric.Enabled > 0; }

        public static int EraOf(GameConfig c, int tier) { return tier == 1 ? c.Fabric.EraA : tier == 2 ? c.Fabric.EraB : c.Fabric.EraC; }

        public static int Lead(GameConfig c, int tier) { return tier == 1 ? c.Fabric.LeadA : tier == 2 ? c.Fabric.LeadB : c.Fabric.LeadC; }

        public static int CostPermille(GameConfig c, int tier) { return tier == 1 ? c.Fabric.CostA : tier == 2 ? c.Fabric.CostB : c.Fabric.CostC; }

        public static int Gain(GameConfig c, int tier) { return tier == 0 ? 0 : tier == 1 ? c.Fabric.GainA : tier == 2 ? c.Fabric.GainB : c.Fabric.GainC; }

        public static bool Open(GameConfig c, int tier, int tick) { return c.Eras.IndexOf(EraSystem.AtTick(c, tick)) >= EraOf(c, tier); }

        public static int OwnedGpus(LabState lab)
        {
            int n = 0;
            for (int i = 0; i < lab.Fleet.Count; i++) n += lab.Fleet[i].Count;
            return n;
        }

        public static bool Building(LabState lab) { return lab.FabBuild > 0; }

        /// <summary>The cost of a tier for a number of GPUs at today's prices.</summary>
        public static long Cost(GameConfig c, GameState state, LabState lab, int tier, int gpus)
        {
            GpuGenerationConfig gen = LabQueries.CurrentGeneration(c, state.Tick);
            return MathI.MulPermille(VendorSystem.PricePerGpu(c, state, lab, gen), CostPermille(c, tier)) * gpus;
        }

        public static long Deposit(GameConfig c, long cost) { return MathI.MulPermille(cost, c.Fabric.DepositPermille); }

        /// <summary>True in the last weeks of a build, when the cluster is down.</summary>
        public static bool Cutover(GameConfig c, LabState lab) { return Building(lab) && lab.FabLeft <= c.Fabric.CutoverWeeks; }

        /// <summary>The training speed the fabric gives now, in permille: the finished tier's gain less the cutover loss.</summary>
        public static int Permille(GameConfig c, LabState lab)
        {
            if (!Enabled(c)) return 0;
            return Gain(c, lab.FabTier) - (Cutover(c, lab) ? c.Fabric.CutoverLoss : 0);
        }

        public static void Order(GameConfig c, GameState state, LabState lab)
        {
            int tier = lab.FabTier + 1;
            int gpus = OwnedGpus(lab);
            long deposit = Deposit(c, Cost(c, state, lab, tier, gpus));
            if (deposit > 0) Finance.Spend(lab, LedgerLine.Hardware, deposit);
            lab.FabBuild = tier;
            lab.FabLeft = Lead(c, tier);
            lab.FabGpus = gpus;
            Events.Add(state, EventKind.FabricOrdered, lab.Id, tier, deposit, "");
        }

        public static void Cancel(GameConfig c, GameState state, LabState lab)
        {
            int tier = lab.FabBuild;
            long deposit = Deposit(c, Cost(c, state, lab, tier, lab.FabGpus));
            lab.FabBuild = 0;
            lab.FabLeft = 0;
            lab.FabGpus = 0;
            lab.FabCancelled++;
            Events.Add(state, EventKind.FabricCancelled, lab.Id, tier, deposit, "");
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || !Building(lab)) return;
            if (lab.FabLeft > 0) lab.FabLeft--;
            if (lab.FabLeft > 0) return;
            long cost = Cost(c, state, lab, lab.FabBuild, lab.FabGpus);
            long balance = cost - Deposit(c, cost);
            if (lab.Cash < balance) return;
            if (balance > 0) Finance.Spend(lab, LedgerLine.Hardware, balance);
            int tier = lab.FabBuild;
            lab.FabTier = tier;
            lab.FabBuild = 0;
            lab.FabGpus = 0;
            lab.FabBuilt++;
            Events.Add(state, EventKind.FabricDelivered, lab.Id, tier, balance, "");
        }
    }
}
