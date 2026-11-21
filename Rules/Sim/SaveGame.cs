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
            for (int i = 0; i < state.Labs.Count; i++) VendorSystem.EnsureStanding(c, state, state.Labs[i].Id);
            state.ConfigVersion = c.Version;
        }

        /// <summary>Every id a save uses must still exist in the config, and every cross-reference must point at something.</summary>
        public static List<string> Problems(GameConfig c, GameState state)
        {
            var p = new List<string>();
            if (state.Labs.Count == 0) { p.Add("there are no labs"); return p; }
            if (state.PlayerLabId < 0)
            {
                for (int i = 0; i < state.Labs.Count; i++)
                {
                    LabState lab = state.Labs[i];
                    if (lab.Controller == LabController.Player && !lab.Bankrupt)
                        p.Add("playerLabId is -1 but a player lab still exists");
                }
            }
            else
            {
                LabState player = state.Player;
                if (player == null) p.Add("playerLabId " + state.PlayerLabId + " does not match any lab");
                else if (player.Controller != LabController.Player) p.Add("the player's lab is not under player control");
            }

            for (int i = 0; i < state.Labs.Count; i++)
            {
                LabState lab = state.Labs[i];
                string who = "lab " + lab.Name;
                for (int k = 0; k < i; k++) if (state.Labs[k].Id == lab.Id) p.Add("two labs share the id " + lab.Id);
                if (c.Lot(lab.LotId) == null) p.Add(who + " stands on the unknown lot \"" + lab.LotId + "\"");
                if (lab.Controller == LabController.Plan && c.Rival(lab.RivalId) == null) p.Add(who + " follows the unknown rival plan \"" + lab.RivalId + "\"");
                if (lab.ResearchSharePermille < 0 || lab.ResearchSharePermille > c.Research.MaxComputeSharePermille) p.Add(who + " has " + lab.ResearchSharePermille + " permille of compute set aside for research");
                if (lab.InsuranceLevel < 0 || lab.InsuranceLevel > c.Finance.InsuranceMaxLevel || lab.InsuranceClaims < 0 || lab.InsuranceLockedUntil < 0)
                    p.Add(who + " has incident insurance at level " + lab.InsuranceLevel + " with " + lab.InsuranceClaims + " claims");
                if (lab.PayLevel < c.StaffPeople.PayMinLevel || lab.PayLevel > c.StaffPeople.PayMaxLevel || lab.PayLockedUntil < 0) p.Add(who + " pays at level " + lab.PayLevel + ", which the data does not allow");
                if (lab.InterpLevel < 0 || lab.InterpLevel > c.Interp.MaxLevel) p.Add(who + " runs an interpretability programme at level " + lab.InterpLevel);
                if (lab.ComplianceLevel < 0 || lab.ComplianceLevel > c.Compliance.MaxLevel) p.Add(who + " runs a compliance office at level " + lab.ComplianceLevel);
                if (lab.Prestige < 0 || lab.Prestige > c.Prestige.Max) p.Add(who + " has " + lab.Prestige + " academic prestige");
                if (lab.SubletPermille < 0 || lab.SubletPermille > c.Sublet.MaxPermille || lab.SubletLockedUntil < 0) p.Add(who + " lets " + lab.SubletPermille + " permille of its compute to outsiders, which the data does not allow");
                if (lab.FocusLockedUntil < 0 || !string.IsNullOrEmpty(lab.ResearchFocus) && c.Focus.Option(lab.ResearchFocus) == null) p.Add(who + " has the research focus \"" + lab.ResearchFocus + "\", which the data does not have");
                if (lab.EquityLevel < 0 || lab.EquityLevel > c.Equity.MaxLevel || lab.EquityPermille < 0 || lab.EquityPermille > MathI.Permille || lab.EquityLevel == 0 != (lab.EquityPermille == 0) || lab.EquityPermille + lab.FounderSharePermille > MathI.Permille)
                    p.Add(who + " has given its team equity level " + lab.EquityLevel + " worth " + lab.EquityPermille + " permille, which does not fit with its founder share");
                if (lab.ConsultantLevel < 0 || lab.ConsultantLevel > c.Consultants.MaxLevel || lab.ConsultantWeeks < 0 || lab.ConsultantWeeks > c.Consultants.Weeks || lab.ConsultantLockedUntil < 0 || (lab.ConsultantWeeks > 0) != (lab.ConsultantLevel > 0))
                    p.Add(who + " has policy consultants (level " + lab.ConsultantLevel + ", " + lab.ConsultantWeeks + " weeks left), which the data does not allow");
                if (lab.TechDebt < 0 || lab.TechDebt > MathI.Permille || (lab.TechDebt > 0 && lab.Id != state.PlayerLabId)) p.Add(who + " carries " + lab.TechDebt + " points of technical debt, which the data does not allow");
                if (lab.ResearchGrantStage < 0 || lab.ResearchGrantStage > ResearchGrants.Owed || lab.ResearchGrantTier < 0 || lab.ResearchGrantTier > c.ResearchGrants.MaxTier || lab.ResearchGrantTick < 0 || lab.ResearchGrantOwed < 0 || lab.ResearchGrantLockedUntil < 0
                    || (lab.ResearchGrantStage == ResearchGrants.None && (lab.ResearchGrantTier != 0 || lab.ResearchGrantOwed != 0))
                    || (lab.ResearchGrantStage != ResearchGrants.None && lab.ResearchGrantTier < 1)
                    || (lab.ResearchGrantStage == ResearchGrants.InReview && lab.ResearchGrantOwed != 0)
                    || (lab.ResearchGrantStage == ResearchGrants.Owed && (lab.ResearchGrantOwed < 1 || lab.ResearchGrantOwed > lab.ResearchGrantTier)))
                    p.Add(who + " has a research grant (stage " + lab.ResearchGrantStage + ", tier " + lab.ResearchGrantTier + ", " + lab.ResearchGrantOwed + " papers owed) that the data does not allow");
                if (lab.SlaLevel < 0 || lab.SlaLevel > c.Sla.MaxLevel || lab.SlaLockedUntil < 0 || lab.SlaStrikes < 0 || (c.Sla.StrikeWeeks > 0 && lab.SlaStrikes >= c.Sla.StrikeWeeks)) p.Add(who + " has a service-level pledge (level " + lab.SlaLevel + ", " + lab.SlaStrikes + " broken weeks) that the data does not allow");
                if (lab.OnCallLevel < 0 || lab.OnCallLevel > c.OnCall.MaxLevel || lab.OnCallLockedUntil < 0) p.Add(who + " has an on-call rota of level " + lab.OnCallLevel + ", which the data does not allow");
                if (lab.CampaignLevel < 0 || lab.CampaignLevel > c.Marketing.MaxLevel || lab.CampaignWeeks < 0 || lab.CampaignWeeks > c.Marketing.Weeks || lab.HangoverWeeks < 0 || lab.HangoverWeeks > c.Marketing.HangoverWeeks || lab.CampaignLockedUntil < 0
                    || (lab.CampaignWeeks > 0 || lab.HangoverWeeks > 0) != (lab.CampaignLevel > 0 && !string.IsNullOrEmpty(lab.CampaignSector) && c.Sector(lab.CampaignSector) != null))
                    p.Add(who + " has a marketing campaign (level " + lab.CampaignLevel + ", " + lab.CampaignWeeks + " weeks, hangover " + lab.HangoverWeeks + ") that does not fit the data");
                if (lab.OptimiseLevel < 0 || lab.OptimiseLevel > c.Optimise.MaxLevel) p.Add(who + " optimises its inference at level " + lab.OptimiseLevel + ", which the data does not allow");
                if (lab.EthicsLevel < 0 || lab.EthicsLevel > c.Board.MaxLevel || lab.EthicsLockedUntil < 0) p.Add(who + " has an ethics board of level " + lab.EthicsLevel + ", which the data does not allow");
                if (lab.ChiefScientistId < -1 || lab.ChiefSafetyId < -1) p.Add(who + " has a chief with the id " + (lab.ChiefScientistId < -1 ? lab.ChiefScientistId : lab.ChiefSafetyId));
                if (lab.HackWeeks < 0 || lab.HackWeeks > c.Hack.Weeks || lab.HackLockedUntil < 0) p.Add(who + " has " + lab.HackWeeks + " weeks of hack week left, which the data does not allow");
                if (lab.CrunchWeeks < 0 || lab.CrunchWeeks > c.StaffPeople.CrunchMaxWeeks) p.Add(who + " has been in crunch time for " + lab.CrunchWeeks + " weeks");
                foreach (Loan loan in lab.Loans)
                    if (loan.Balance <= 0 || loan.Payment <= 0 || loan.Misses < 0) p.Add(who + " has a loan (" + loan.Id + ") with nothing owed, no payment or a negative miss count");
                foreach (RoomState room in lab.Rooms) if (c.RoomType(room.TypeId) == null) p.Add(who + " has a room of the unknown type \"" + room.TypeId + "\"");
                foreach (StaffMember person in lab.Staff)
                {
                    if (c.Role(person.RoleId) == null) p.Add(who + " employs the unknown role \"" + person.RoleId + "\"");
                    if (c.StaffPeople.Trait(person.TraitId) == null) p.Add(who + " has staff with unknown trait \"" + person.TraitId + "\"");
                }
                if (!string.IsNullOrEmpty(lab.FounderId) && c.Founder(lab.FounderId) == null) p.Add(who + " has the unknown founder \"" + lab.FounderId + "\"");
                if (!string.IsNullOrEmpty(lab.StyleId) && c.Style(lab.StyleId) == null) p.Add(who + " has the unknown style \"" + lab.StyleId + "\"");
                foreach (PoachOffer offer in lab.PoachOffers)
                {
                    if (TalentSystem.Person(lab, offer.StaffId) == null) p.Add(who + " has an offer for staff " + offer.StaffId + ", who is not on the payroll");
                    if (state.Lab(offer.RivalLabId) == null) p.Add(who + " has an offer from the unknown lab " + offer.RivalLabId);
                }
                foreach (PressInquiry q in lab.PressInquiries)
                {
                    if (c.Press.Beat(q.BeatId) == null) p.Add(who + " has a question about the unknown beat \"" + q.BeatId + "\"");
                    if (c.Press.Journalist(q.JournalistId) == null) p.Add(who + " has a question from the unknown journalist \"" + q.JournalistId + "\"");
                }
                if (lab.PressInquiries.Count > c.Press.MaxOpenInquiries) p.Add(who + " has more open questions than the press desk allows");
                var trustSeen = new System.Collections.Generic.HashSet<string>();
                foreach (SectorTrust trust in lab.SectorTrust)
                {
                    if (c.Sector(trust.SectorId) == null) p.Add(who + " has trust in the unknown sector \"" + trust.SectorId + "\"");
                    if (!trustSeen.Add(trust.SectorId)) p.Add(who + " has two trust records for \"" + trust.SectorId + "\"");
                    if (trust.Trust < 0 || trust.Trust > 1000) p.Add(who + " has a trust of " + trust.Trust + " in " + trust.SectorId + ", outside 0 to 1000");
                }
                foreach (Complaint complaint in lab.Complaints)
                    if (c.Sector(complaint.SectorId) == null) p.Add(who + " has a complaint from the unknown sector \"" + complaint.SectorId + "\"");
                if (lab.Complaints.Count > c.Trust.MaxOpenComplaints) p.Add(who + " has more open complaints than the customer desk allows");
                if (lab.GauntletTuning < 0 || lab.GauntletTuning > c.Gauntlet.MaxTuningPermille) p.Add(who + " has a Gauntlet tuning of " + lab.GauntletTuning + ", outside 0 to " + c.Gauntlet.MaxTuningPermille);
                if (c.Culture.Start + lab.CultureShift < 0 || c.Culture.Start + lab.CultureShift > MathI.Permille) p.Add(who + " has a culture shift of " + lab.CultureShift + ", which leaves 0 to 1000");
                if (lab.ReleaseGate < 0 || lab.ReleaseGate > c.Culture.MaxGate) p.Add(who + " has a release gate of " + lab.ReleaseGate + ", outside 0 to " + c.Culture.MaxGate);
                foreach (RoomState room in lab.Rooms)
                    if (room.EraAge < 0 || room.EraAge > c.Eras.Count) p.Add(who + " has a room " + room.Id + " that is " + room.EraAge + " eras old, outside 0 to " + c.Eras.Count);
                if (lab.NamingMess < 0 || lab.NamingMess > c.Naming.MaxMess) p.Add(who + " has a naming mess of " + lab.NamingMess + ", outside 0 to " + c.Naming.MaxMess);
                if (!string.IsNullOrEmpty(lab.OfficeEventId) && c.Office.Event(lab.OfficeEventId) == null) p.Add(who + " has an office event \"" + lab.OfficeEventId + "\" that is not in the data");
                foreach (string seen in lab.OfficeSeen) if (c.Office.Event(seen) == null) p.Add(who + " has seen the unknown office event \"" + seen + "\"");
                var leadIds = new System.Collections.Generic.HashSet<int>();
                foreach (ResearchLead lead in lab.Leads)
                {
                    if (!leadIds.Add(lead.Id)) p.Add(who + " has two leads with id " + lead.Id);
                    if (lab.Model(lead.ModelId) == null) p.Add(who + " has a lead about the missing model " + lead.ModelId);
                }
                if (lab.Leads.Count > c.Leads.MaxOpen) p.Add(who + " has more leads than the desk allows");
                if (lab.LeadSpeedPermille < 0 || lab.LeadSpeedPermille > c.Leads.MaxSpeedPermille) p.Add(who + " has a lead speed of " + lab.LeadSpeedPermille + ", outside 0 to " + c.Leads.MaxSpeedPermille);
                if (lab.GateSetTick < 0) p.Add(who + " has a gate set at tick " + lab.GateSetTick);
                if (lab.PoachOffers.Count > c.Talent.MaxOpenOffers && c.Talent.OfferWeeks > 0) p.Add(who + " has more open offers than the talent market allows");
                if (c.World.Bloc(lab.HomeBlocId) == null) p.Add(who + " is based in the unknown bloc \"" + lab.HomeBlocId + "\"");
                foreach (BlocStanding standing in lab.Blocs) if (c.World.Bloc(standing.BlocId) == null) p.Add(who + " has a standing with the unknown bloc \"" + standing.BlocId + "\"");
                foreach (Holding holding in lab.Holdings)
                {
                    if (c.World.Site(holding.SiteId) == null) p.Add(who + " holds ground at the unknown site \"" + holding.SiteId + "\"");
                    if (!string.IsNullOrEmpty(holding.DealId) && c.World.PowerDeal(holding.DealId) == null) p.Add(who + " has the unknown power deal \"" + holding.DealId + "\"");
                }
                foreach (HardwareBatch batch in lab.Fleet)
                {
                    if (batch.HoldingId != HardwareBatch.Hq && batch.HoldingId != HardwareBatch.Cloud && lab.Holding(batch.HoldingId) == null) p.Add(who + " has hardware in the holding " + batch.HoldingId + ", which does not exist");
                    if (!string.IsNullOrEmpty(batch.GpuGenId) && c.Compute.Generation(batch.GpuGenId) == null) p.Add(who + " has hardware of the unknown generation \"" + batch.GpuGenId + "\"");
                }
                foreach (ChipDesign design in lab.ChipDesigns)
                {
                    if (c.Chips.Target(design.Target) == null || c.Chips.Node(design.NodeId) == null
                        || c.Chips.Option(ChipOptionGroup.Die, design.DieId) == null || c.Chips.Option(ChipOptionGroup.Format, design.FormatId) == null
                        || c.Chips.Option(ChipOptionGroup.Memory, design.MemoryId) == null || c.Chips.Option(ChipOptionGroup.Cooling, design.CoolingId) == null)
                        p.Add(who + " has the chip design " + design.Name + " made of something the game does not know");
                }
                foreach (ChipOrder order in lab.ChipOrders)
                {
                    LabState designer = state.Lab(order.DesignLabId);
                    if (designer == null || designer.Design(order.DesignId) == null) p.Add(who + " has chips on order of a design that does not exist");
                    if (order.HoldingId != HardwareBatch.Cloud && lab.Holding(order.HoldingId) == null) p.Add(who + " has chips on order for the holding " + order.HoldingId + ", which does not exist");
                    if (c.Chips.Node(order.NodeId) == null) p.Add(who + " has chips on order at the unknown process \"" + order.NodeId + "\"");
                }
                foreach (ChipListing listing in lab.ChipListings)
                    if (lab.Design(listing.DesignId) == null) p.Add(who + " lists chips of a design that does not exist");
                foreach (HardwareBatch batch in lab.Fleet)
                {
                    if (batch.ChipDesignId < 0) continue;
                    LabState designer = state.Lab(batch.DesignLabId);
                    if (designer == null || designer.Design(batch.ChipDesignId) == null) p.Add(who + " has chips of a design that does not exist");
                }
                foreach (string tech in lab.Techs) if (c.Tech(tech) == null) p.Add(who + " knows the unknown tech \"" + tech + "\"");
                foreach (ModelState model in lab.Models)
                {
                    if (!string.IsNullOrEmpty(model.Objective) && c.Objectives.Objective(model.Objective) == null) p.Add(model.Name + " was trained for the unknown objective \"" + model.Objective + "\"");
                    if (model.RedTeamUntil < 0 || model.RedTeamUntil > state.Tick + c.RedTeam.Weeks) p.Add(model.Name + " is safer from a red team until week " + model.RedTeamUntil);
                    if (model.IncidentRepLoss < 0 || model.PostMortemDone < 0 || model.PostMortemDone > PostMortem.Full) p.Add(model.Name + " has a post-mortem record (loss " + model.IncidentRepLoss + ", depth " + model.PostMortemDone + ") that does not fit");
                    if (model.AuditedUntil < 0 || model.AuditedUntil > state.Tick + c.Audit.Weeks) p.Add(model.Name + " holds an audit certificate until week " + model.AuditedUntil);
                    if (c.Line(model.LineId) == null) p.Add(who + " has a model of the unknown line \"" + model.LineId + "\"");
                    foreach (string sector in model.Sectors) if (c.Sector(sector) == null) p.Add(model.Name + " sells into the unknown sector \"" + sector + "\"");
                }
            }

            foreach (VendorStanding standing in state.Vendor.Standings)
                if (state.Lab(standing.LabId) == null) p.Add("the vendor holds a standing for lab " + standing.LabId + ", which does not exist");
            foreach (string id in state.PendingRivals) if (c.Rival(id) == null) p.Add("the unknown rival \"" + id + "\" is due to join");

            foreach (FoundryMonth month in state.Foundry) if (c.Chips.Node(month.NodeId) == null) p.Add("the foundry has a booking at the unknown process \"" + month.NodeId + "\"");

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
