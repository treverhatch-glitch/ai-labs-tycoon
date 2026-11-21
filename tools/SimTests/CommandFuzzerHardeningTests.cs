using System;
using System.Collections.Generic;
using System.Linq;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Random valid and garbage commands must never throw; refusals leave state unchanged.</summary>
public class CommandFuzzerHardeningTests
{
    const int Seeds = 12;
    const int CommandsPerSeed = 160;

    [Fact]
    public void Random_commands_across_seeds_never_throw_and_balance_the_ledger()
    {
        GameConfig c = TestData.Config();
        for (long seed = 1; seed <= Seeds; seed++)
        {
            var g = new Game(Simulation.NewGame(c, seed));
            g.WithStarterLab(researchers: 2, engineers: 1, gpus: 6);
            var rng = new Rng(seed ^ 0x46555A5F);

            for (int i = 0; i < CommandsPerSeed; i++)
            {
                bool wantValid = rng.Next(4) == 0;
                Command cmd = CommandFuzzerSamples.Make(g, rng, wantValid, out bool usesPlayerLab);
                if (!usesPlayerLab && g.State.PlayerLabId < 0) continue;
                string before = g.Sim.Digest();
                CommandResult result = default;
                Exception thrown = null;
                try { result = g.Try(cmd); }
                catch (Exception ex) { thrown = ex; }

                Assert.Null(thrown);
                if (!result.Ok)
                {
                    Assert.NotEqual(CommandError.None, result.Error);
                    Assert.False(string.IsNullOrEmpty(result.Message));
                    Assert.Equal(before, g.Sim.Digest());
                }

                AssertLedger(c, g.State);
                if (i % 12 == 11)
                {
                    if (g.State.PlayerLabId >= 0 && GameDate.IsMonthStart(c.Calendar, g.Tick))
                        g.Sim.RunPlan(g.State.PlayerLabId, c.Autopilot);
                    g.Sim.Step();
                    AssertLedger(c, g.State);
                }
            }
        }
    }

    static void AssertLedger(GameConfig c, GameState state)
    {
        foreach (LabState lab in state.Labs)
        {
            if (lab.Bankrupt) continue;
            Assert.Equal(lab.Ledger.StartCash + lab.Ledger.Lifetime.Net, lab.Cash);
            Assert.InRange(lab.Reputation, -1000, 1000);
            Assert.True(lab.Cash > -10_000_000_000_000L && lab.Cash < 10_000_000_000_000L);
        }
        Assert.True(state.Tick >= 0);
        Assert.True(state.Events.Count <= c.EventLogMax);
    }
}

static class CommandFuzzerSamples
{
    static LabState Lab(Game g)
    {
        if (g.State.PlayerLabId >= 0) return g.State.Lab(g.State.PlayerLabId);
        return g.State.Labs[0];
    }
    static readonly string[] RoomTypes = { "research-lab", "engineering-bay", "server-closet", "safety-lab", "sales-office", "chip-design-lab", "not-a-room" };
    static readonly string[] Roles = { "researcher", "engineer", "sales", "safety", "chip-architect", "intern" };
    static readonly string[] Sites = { "us-oregon", "us-tennessee", "nowhere" };
    static readonly string[] Lines = { "mini", "standard", "toaster" };
    static readonly string[] ShadyCards = { "liberate-funds", "borrow-weights", "sabotage-run", "bogus-card" };

    public static Command Make(Game g, Rng rng, bool biasValid, out bool usesPlayerLab)
    {
        LabState lab = Lab(g);
        CommandKind[] kinds = (CommandKind[])Enum.GetValues(typeof(CommandKind));
        CommandKind kind = kinds[rng.Next(kinds.Length)];
        int labId = biasValid && g.State.PlayerLabId >= 0 ? g.State.PlayerLabId : PickLabId(g, rng);
        usesPlayerLab = labId == g.State.PlayerLabId;
        if (rng.Next(6) == 0) labId = -1;

        switch (kind)
        {
            case CommandKind.BuildRoom:
            {
                string room = biasValid ? "research-lab" : RoomTypes[rng.Next(RoomTypes.Length)];
                int w = biasValid ? 4 : rng.Next(6) + 1;
                int h = biasValid ? 4 : rng.Next(6) + 1;
                return Command.BuildRoom(labId, room, rng.Next(12), rng.Next(12), w, h);
            }
            case CommandKind.DemolishRoom:
                return Command.DemolishRoom(labId, PickTarget(g, biasValid, lab.Rooms, r => r.Id, rng, 99));
            case CommandKind.BuyLot:
                return Command.BuyLot(labId);
            case CommandKind.Hire:
                return Command.Hire(labId, biasValid ? "researcher" : Roles[rng.Next(Roles.Length)], biasValid ? 1 : rng.Next(5) + 1);
            case CommandKind.Fire:
                return Command.Fire(labId, Roles[rng.Next(Roles.Length)], rng.Next(3) + 1);
            case CommandKind.SetRentedGpus:
                return Command.SetRentedGpus(labId, biasValid ? Math.Min(4, lab.RentedGpus + 1) : rng.Next(40));
            case CommandKind.BuyGpus:
                return Command.BuyGpus(labId, biasValid ? 1 : rng.Next(8), HardwareBatch.Hq);
            case CommandKind.SellGpus:
                return Command.SellGpus(labId, biasValid ? 1 : rng.Next(4), HardwareBatch.Hq);
            case CommandKind.SetTrainingShare:
                return Command.SetTrainingShare(labId, biasValid ? 700 : rng.Next(1500));
            case CommandKind.ResearchTech:
                return Command.ResearchTech(labId, biasValid ? "state-machines" : "moon-math");
            case CommandKind.FilePatent:
                return Command.FilePatent(labId, biasValid ? "state-machines" : "void-tech");
            case CommandKind.StartTraining:
                return Command.StartTraining(labId, biasValid ? "mini" : Lines[rng.Next(Lines.Length)], biasValid ? 200 : rng.Next(1000));
            case CommandKind.StartTrainingRecipe:
                return Command.StartTrainingRecipe(labId, biasValid ? "diligent-mini" : "void-recipe");
            case CommandKind.SetDistillTarget:
                return Command.SetDistillTarget(labId, biasValid && g.State.Labs.Count > 1 ? g.State.Labs[1].Id : rng.Next(8) - 1);
            case CommandKind.ScrapFailedRun:
                return Command.ScrapFailedRun(labId, PickTarget(g, biasValid, lab.Models, m => m.Id, rng, -1));
            case CommandKind.ReleaseModel:
            {
                int modelId = PickTarget(g, biasValid, lab.Models, m => m.Id, rng, -1);
                string sector = biasValid && g.C.Market.Sectors.Count > 0 ? g.C.Market.Sectors[0].Id : "closed-sector";
                return Command.ReleaseModel(labId, modelId, rng.Next(1000), sector);
            }
            case CommandKind.BuyLand:
                return Command.BuyLand(labId, biasValid ? "us-oregon" : Sites[rng.Next(Sites.Length)]);
            case CommandKind.SellLand:
                return Command.SellLand(labId, PickTarget(g, biasValid, lab.Holdings, h => h.Id, rng, -1));
            case CommandKind.LeaseColo:
                return Command.LeaseColo(labId, Sites[rng.Next(Sites.Length)], biasValid ? 100 : rng.Next(5000));
            case CommandKind.BuildHall:
                return Command.BuildHall(labId, PickTarget(g, biasValid, lab.Holdings, h => h.Id, rng, -1), biasValid ? 100 : rng.Next(100_000));
            case CommandKind.AddPowerDeal:
            {
                string deal = g.C.World.PowerDeals.Count > 0 ? g.C.World.PowerDeals[0].Id : "solar";
                return Command.AddPowerDeal(labId, PickTarget(g, biasValid, lab.Holdings, h => h.Id, rng, -1), biasValid ? deal : "void");
            }
            case CommandKind.MoveHardware:
                return Command.MoveHardware(labId, PickTarget(g, biasValid, lab.Fleet, b => b.Id, rng, -1), -1, rng.Next(4) + 1);
            case CommandKind.StartChipDesign:
                return Command.StartChipDesign(labId, biasValid ? "inference" : "toaster", "n28", "small", "fp16", "ddr", "air");
            case CommandKind.CancelChipDesign:
                return Command.CancelChipDesign(labId, PickTarget(g, biasValid, lab.ChipDesigns, d => d.Id, rng, -1));
            case CommandKind.TapeOut:
                return Command.TapeOut(labId, PickTarget(g, biasValid, lab.ChipDesigns, d => d.Id, rng, -1));
            case CommandKind.OrderChips:
                return Command.OrderChips(labId, PickTarget(g, biasValid, lab.ChipDesigns, d => d.Id, rng, -1), PickTarget(g, biasValid, lab.Holdings, h => h.Id, rng, -1), rng.Next(5) + 1);
            case CommandKind.SetChipListing:
                return Command.SetChipListing(labId, PickTarget(g, biasValid, lab.ChipDesigns, d => d.Id, rng, -1), biasValid ? 10_000 : -1, rng.Next(20));
            case CommandKind.BuyListedChips:
            {
                int seller = g.State.PlayerLabId >= 0 ? g.State.PlayerLabId : lab.Id;
                return Command.BuyListedChips(labId, seller, PickTarget(g, false, lab.ChipDesigns, d => d.Id, rng, -1), -1, 1);
            }
            case CommandKind.ResearchFrontier:
                return Command.ResearchFrontier(labId);
            case CommandKind.SellLab:
                return Command.SellLab(labId, rng.Next(3) == 0 ? "spectate" : "found-again");
            case CommandKind.ContinueAfterEnding:
                return Command.ContinueAfterEnding(labId, rng.Next(3), rng.Next(2) == 0 ? "bailout" : "spectate");
            case CommandKind.StartShadyOp:
            {
                int modelId = lab.Models.Count > 0 ? lab.Models[0].Id : -1;
                return Command.StartShadyOp(labId, ShadyCards[rng.Next(ShadyCards.Length)], modelId, -1, -1, "mini");
            }
            case CommandKind.Launder:
                return Command.Launder(labId, biasValid ? (int)Math.Min(1000L, lab.Stash) : rng.Next(50_000));
            case CommandKind.RaiseFunding:
            {
                string round = g.C.Funding.Rounds.Count > 0 ? g.C.Funding.Rounds[0].Id : "angel";
                return Command.RaiseFunding(labId, biasValid ? round : "series-z");
            }
            case CommandKind.SignReservedCompute:
                return Command.SignReservedCompute(labId, biasValid ? 2 : rng.Next(10));
            case CommandKind.QueueGpuPurchase:
                return Command.QueueGpuPurchase(labId, biasValid ? 1 : rng.Next(4), HardwareBatch.Hq);
            case CommandKind.SetOpenWeights:
                return Command.SetOpenWeights(labId, PickTarget(g, biasValid, Lab(g).Models, m => m.Id, rng, 999), biasValid);
            case CommandKind.SetTrainingDataSource:
                return Command.SetTrainingDataSource(labId, biasValid ? "internal" : (rng.Next(3) == 0 ? "licensed" : "bogus"));
            case CommandKind.SetSectorPrice:
            {
                LabState target = g.State.Lab(labId);
                string sector = "game-ai";
                if (target != null)
                {
                    foreach (ModelState m in target.Models)
                    {
                        if (m.Status == ModelStatus.Released && m.Sectors.Count > 0)
                        {
                            sector = m.Sectors[rng.Next(m.Sectors.Count)];
                            break;
                        }
                    }
                }
                int price = biasValid ? MathI.Permille : rng.Next(2500);
                return Command.SetSectorPrice(labId, sector, price);
            }
            default:
                return Command.BuyGpus(labId, 1);
        }
    }

    static int PickLabId(Game g, Rng rng)
    {
        if (g.State.Labs.Count == 0) return -1;
        return g.State.Labs[rng.Next(g.State.Labs.Count)].Id;
    }

    static int PickTarget<T>(Game g, bool biasValid, List<T> list, Func<T, int> id, Rng rng, int garbage)
    {
        if (biasValid && list.Count > 0) return id(list[rng.Next(list.Count)]);
        if (list.Count > 0 && rng.Next(3) == 0) return id(list[rng.Next(list.Count)]);
        return garbage;
    }
}