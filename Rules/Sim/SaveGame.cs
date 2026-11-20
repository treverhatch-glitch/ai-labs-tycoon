using System;
using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public sealed class SaveException : Exception
    {
        public SaveException(string message) : base(message) { }
    }

    /// <summary>
    /// Saves are the game state as JSON text. The rules never touch a disk: the host (Unity, a test,
    /// the console tool) decides where the text goes.
    /// </summary>
    public static class SaveGame
    {
        public static string Write(GameState state) { return Json.Write(Syncer.Write(state), false); }

        public static string Digest(GameState state) { return Core.Digest.Sha256Hex(Write(state)); }

        public static GameState Read(GameConfig c, string json)
        {
            GameState state;
            try
            {
                JsonValue root = Json.Parse(json);
                JsonValue schema = root.Kind == JsonKind.Object ? root.Get("schema") : null;
                if (schema == null || schema.Kind != JsonKind.Int) throw new SaveException("This is not an AI Labs Tycoon save: it has no schema number.");
                if (schema.Int > GameState.CurrentSchema)
                    throw new SaveException("This save was made by a newer version of the game (save schema " + schema.Int + ", this game reads up to " + GameState.CurrentSchema + ").");
                root = SaveMigrations.Run(c, root, (int)schema.Int);
                state = Syncer.Read<GameState>(root, "save");
            }
            catch (JsonException e) { throw new SaveException("The save is not readable: " + e.Message); }
            catch (SyncException e) { throw new SaveException("The save is not readable: " + e.Message); }

            Reconcile(c, state);
            List<string> problems = Problems(c, state);
            if (problems.Count > 0)
                throw new SaveException("The save does not fit this version of the game:\n- " + string.Join("\n- ", problems));
            return state;
        }

        /// <summary>Content added to the config after the save was made (a new sector) joins the saved world.</summary>
        static void Reconcile(GameConfig c, GameState state)
        {
            for (int i = 0; i < c.Market.Sectors.Count; i++)
                if (state.Sector(c.Market.Sectors[i].Id) == null) state.Sectors.Add(WorldSetup.NewSector(c.Market.Sectors[i], GameDate.Year(c.Calendar, state.Tick)));
            for (int i = 0; i < c.World.Sites.Count; i++)
                if (state.Site(c.World.Sites[i].Id) == null) state.Sites.Add(WorldSetup.NewSite(c.World.Sites[i].Id));
            for (int i = 0; i < state.Labs.Count; i++)
                for (int k = 0; k < c.World.Blocs.Count; k++)
                    if (state.Labs[i].Bloc(c.World.Blocs[k].Id) == null) state.Labs[i].Blocs.Add(WorldSetup.NewStanding(c.World.Blocs[k].Id));
            state.ConfigVersion = c.Version;
        }

        /// <summary>Every id a save uses must still exist in the config, and every cross-reference must point at something.</summary>
        public static List<string> Problems(GameConfig c, GameState state)
        {
            var p = new List<string>();
            if (state.Labs.Count == 0) { p.Add("there are no labs"); return p; }
            if (state.Labs[0].Controller != LabController.Player) p.Add("the first lab is not the player's");

            for (int i = 0; i < state.Labs.Count; i++)
            {
                LabState lab = state.Labs[i];
                string who = "lab " + lab.Name;
                for (int k = 0; k < i; k++) if (state.Labs[k].Id == lab.Id) p.Add("two labs share the id " + lab.Id);
                if (c.Lot(lab.LotId) == null) p.Add(who + " stands on the unknown lot \"" + lab.LotId + "\"");
                if (lab.Controller == LabController.Plan && c.Rival(lab.RivalId) == null) p.Add(who + " follows the unknown rival plan \"" + lab.RivalId + "\"");
                foreach (RoomState room in lab.Rooms) if (c.RoomType(room.TypeId) == null) p.Add(who + " has a room of the unknown type \"" + room.TypeId + "\"");
                foreach (StaffPool pool in lab.Staff) if (c.Role(pool.RoleId) == null) p.Add(who + " employs the unknown role \"" + pool.RoleId + "\"");
                if (c.World.Bloc(lab.HomeBlocId) == null) p.Add(who + " is based in the unknown bloc \"" + lab.HomeBlocId + "\"");
                foreach (BlocStanding standing in lab.Blocs) if (c.World.Bloc(standing.BlocId) == null) p.Add(who + " has a standing with the unknown bloc \"" + standing.BlocId + "\"");
                foreach (Holding holding in lab.Holdings)
                {
                    if (c.World.Site(holding.SiteId) == null) p.Add(who + " holds ground at the unknown site \"" + holding.SiteId + "\"");
                    if (!string.IsNullOrEmpty(holding.DealId) && c.World.PowerDeal(holding.DealId) == null) p.Add(who + " has the unknown power deal \"" + holding.DealId + "\"");
                }
                foreach (HardwareBatch batch in lab.Fleet)
                {
                    if (batch.HoldingId != HardwareBatch.Hq && lab.Holding(batch.HoldingId) == null) p.Add(who + " has hardware in the holding " + batch.HoldingId + ", which does not exist");
                    if (!string.IsNullOrEmpty(batch.GpuGenId) && c.Compute.Generation(batch.GpuGenId) == null) p.Add(who + " has hardware of the unknown generation \"" + batch.GpuGenId + "\"");
                }
                foreach (string tech in lab.Techs) if (c.Tech(tech) == null) p.Add(who + " knows the unknown tech \"" + tech + "\"");
                foreach (ModelState model in lab.Models)
                {
                    if (c.Line(model.LineId) == null) p.Add(who + " has a model of the unknown line \"" + model.LineId + "\"");
                    foreach (string sector in model.Sectors) if (c.Sector(sector) == null) p.Add(model.Name + " sells into the unknown sector \"" + sector + "\"");
                }
            }

            foreach (SiteState site in state.Sites) if (c.World.Site(site.SiteId) == null) p.Add("the unknown site \"" + site.SiteId + "\"");

            foreach (SectorState sector in state.Sectors)
            {
                if (c.Sector(sector.Id) == null) { p.Add("the unknown sector \"" + sector.Id + "\""); continue; }
                foreach (SectorEntry entry in sector.Entries)
                {
                    LabState lab = state.Lab(entry.LabId);
                    if (lab == null || lab.Model(entry.ModelId) == null) p.Add("sector " + sector.Id + " lists a model that does not exist");
                }
            }
            return p;
        }
    }
}
