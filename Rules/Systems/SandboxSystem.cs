using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>
    /// The sandbox wing lets red-teamers rehearse failures in private: some incidents never reach the newspapers.
    /// </summary>
    public static class SandboxSystem
    {
        public static int RoomTiles(LabState lab, string roomTypeId)
        {
            int tiles = 0;
            for (int i = 0; i < lab.Rooms.Count; i++)
            {
                RoomState room = lab.Rooms[i];
                if (room.TypeId != roomTypeId) continue;
                tiles += room.Width * room.Height;
            }
            return tiles;
        }

        public static int CatchPermille(GameConfig c, GameState state, LabState lab)
        {
            SandboxCatchConfig cfg = c.Safety.SandboxCatch;
            if (cfg == null || !EraSystem.RoomOpen(c, c.RoomType(cfg.RoomTypeId), state.Tick)) return 0;
            int tiles = RoomTiles(lab, cfg.RoomTypeId);
            if (tiles < 1) return 0;
            int staff = LabQueries.StaffCount(lab, cfg.RoleId);
            if (staff < 1) return 0;
            int permille = tiles * cfg.CatchPermillePerTile + staff * cfg.CatchPermillePerStaff;
            return (int)MathI.Min(cfg.CatchCapPermille, permille);
        }

        public static bool MayCatchRung(GameConfig c, int rung)
        {
            return rung > 0 && rung <= c.Safety.SandboxCatch.MaxRungCaught;
        }

        public static void ApplyCaught(GameConfig c, GameState state, LabState lab, ModelState model, int rung, IncidentRungConfig def)
        {
            SandboxCatchConfig cfg = c.Safety.SandboxCatch;
            long fine = MathI.Max(c.Safety.IncidentFineMin, MarketSystem.ModelRevenue(state, lab.Id, model.Id) * c.Safety.IncidentFineRevenueWeeks);
            if (def != null) fine = MathI.MulPermille(fine, def.FinePermille);
            fine = MathI.Max(c.Safety.IncidentFineMin, MathI.MulPermille(fine, cfg.CaughtFinePermille));

            int repLoss = def == null ? c.Safety.IncidentReputationLoss : def.ReputationLoss;
            repLoss = (int)MathI.MulPermille(repLoss, cfg.CaughtReputationPermille);

            model.Incidents++;
            model.LastIncidentRung = rung;
            model.LastIncidentTick = state.Tick;
            lab.SandboxCaughtTotal++;
            lab.Reputation = (int)MathI.Max(0, lab.Reputation - repLoss);
            Finance.Spend(lab, LedgerLine.Fines, fine);
            StaffSystem.OnIncident(c, lab);
            Events.Add(state, EventKind.SandboxCaught, lab.Id, model.Id, fine, model.Name);
        }
    }
}