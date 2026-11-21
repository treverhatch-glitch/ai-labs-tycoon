using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Pulling a model out of a sector. See <see cref="PulloutConfig"/>. Only the player's lab does it.</summary>
    public static class Pullout
    {
        public static bool Enabled(GameConfig c) { return c.Pullout.Enabled > 0; }

        /// <summary>The lab's entry in the sector when the model is the one on sale there, otherwise null.</summary>
        public static SectorEntry EntryOf(GameState state, LabState lab, ModelState model, string sectorId)
        {
            SectorState sector = state.Sector(sectorId);
            SectorEntry entry = sector == null ? null : sector.Entry(lab.Id);
            return entry != null && entry.ModelId == model.Id ? entry : null;
        }

        /// <summary>What the break fee is: the weeks asked for times what the model last earned there in a week.</summary>
        public static long BreakFee(GameConfig c, SectorEntry entry) { return MathI.Max(0, entry.LastRevenue) * c.Pullout.FeeWeeks; }

        public static void Run(GameConfig c, GameState state, LabState lab, ModelState model, string sectorId)
        {
            SectorState sector = state.Sector(sectorId);
            SectorEntry entry = sector.Entry(lab.Id);
            long fee = BreakFee(c, entry);
            if (fee > 0) Finance.Spend(lab, LedgerLine.Fines, fee);
            sector.Entries.Remove(entry);
            model.Sectors.Remove(sectorId);
            TrustSystem.Shift(c, state, lab, sectorId, -c.Pullout.TrustDrop, false);
            lab.Reputation = (int)MathI.Max(0, lab.Reputation - PressSystem.Damp(c, lab, c.Pullout.ReputationLoss));
            SectorConfig config = c.Sector(sectorId);
            bool retired = model.Sectors.Count == 0;
            GameEvent e = Events.Add(state, EventKind.SectorPulledOut, lab.Id, model.Id, fee, config == null ? sectorId : config.Name);
            e.Pick = retired ? 1 : 0;
            if (retired)
            {
                model.Status = ModelStatus.Retired;
                Events.Add(state, EventKind.ModelRetired, lab.Id, model.Id, 0, model.Name);
            }
        }
    }
}
