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
                foreach (StaffMember person in lab.Staff)
                {
                    if (person.Senior < 0 || person.Senior > 1 || (person.Senior == 1 && lab.Id != state.PlayerLabId))
                        p.Add(who + " has a senior person (" + person.Senior + ") that the data does not allow");
                    if (person.LeaveDue < 0 || person.LeaveMorale < 0 || person.LeaveMorale > 1000 || (person.LeaveDue == 0 && person.LeaveMorale != 0) || (person.LeaveDue > 0 && (lab.Id != state.PlayerLabId || person.MentorId != 0)))
                        p.Add(who + " has a person on sabbatical until week " + person.LeaveDue + " (morale " + person.LeaveMorale + ") that the data does not allow");
                    if (person.MentorId < 0 || person.MentorWeeks < 0 || (person.MentorId == 0 && person.MentorWeeks != 0) || (person.MentorId > 0 && (lab.Id != state.PlayerLabId || person.MentorId == person.Id)))
                        p.Add(who + " has a person with mentor " + person.MentorId + " (" + person.MentorWeeks + " weeks), which does not fit the staff");
                }
                if (Sabbatical.Away(lab) > c.Sabbatical.MaxAway) p.Add(who + " has more people on sabbatical than the data allows");
                if (lab.RevenueTargetLevel < 0 || lab.RevenueTargetLevel > c.RevenueTargets.MaxLevel || lab.RevenueTargetEnd < 0 || lab.RevenueTargetGoal < 0 || lab.RevenueTargetSoFar < 0
                    || (lab.RevenueTargetLevel == 0 && (lab.RevenueTargetEnd != 0 || lab.RevenueTargetGoal != 0 || lab.RevenueTargetSoFar != 0))
                    || (lab.RevenueTargetLevel > 0 && (lab.RevenueTargetEnd < 1 || lab.RevenueTargetGoal < 1 || lab.Id != state.PlayerLabId)))
                    p.Add(who + " has a revenue target (level " + lab.RevenueTargetLevel + ", goal " + lab.RevenueTargetGoal + ") that the data does not allow");
                if (lab.SkunkWork < 0 || lab.SkunkPeople < 0 || lab.SkunkStart < 0 || lab.SkunkDue < 0 || lab.SkunkLockedUntil < 0 || (lab.SkunkKind != "" && Skunkworks.FunctionOf(lab.SkunkKind) == null)
                    || (lab.SkunkKind == "" && (lab.SkunkPeople != 0 || lab.SkunkStart != 0 || lab.SkunkDue != 0 || lab.SkunkWork != 0))
                    || (lab.SkunkKind != "" && (lab.SkunkPeople < 1 || lab.SkunkDue <= lab.SkunkStart))
                    || ((lab.SkunkKind != "" || lab.SkunkLockedUntil > 0) && lab.Id != state.PlayerLabId))
                    p.Add(who + " has a skunkworks project (\"" + lab.SkunkKind + "\", " + lab.SkunkPeople + " people, due week " + lab.SkunkDue + ") that the data does not allow");
                if (lab.LabourStage < 0 || lab.LabourStage > 4 || lab.LabourPressure < 0 || lab.LabourPressure > MathI.Permille || lab.LabourDue < 0 || (lab.LabourHard != 0 && lab.LabourHard != 1)
                    || (lab.LabourStage == 0 || lab.LabourStage == 2 ? lab.LabourDue != 0 : lab.LabourDue < 1)
                    || (lab.LabourHard == 1 && lab.LabourStage != 2) || (lab.LabourStage == 2 && lab.LabourPressure != 0)
                    || ((lab.LabourStage != 0 || lab.LabourPressure != 0 || lab.LabourHard != 0) && lab.Id != state.PlayerLabId))
                    p.Add(who + " has a labour stage (" + lab.LabourStage + ", pressure " + lab.LabourPressure + ", due week " + lab.LabourDue + ") that the data does not allow");
                {
                    var seen = new System.Collections.Generic.HashSet<string>();
                    int assigned = 0;
                    foreach (AccountTeam team in lab.AccountTeams)
                    {
                        assigned += team.People;
                        if (c.Sector(team.SectorId) == null || !seen.Add(team.SectorId) || team.People < 1 || team.People > c.Accounts.MaxPeople || team.Weeks < 0 || team.Weeks > c.Accounts.MaxWeeks)
                            p.Add(who + " has an account team (" + team.SectorId + ", " + team.People + " people, " + team.Weeks + " weeks) that the data does not allow");
                    }
                    if (lab.AccountTeams.Count > 0 && lab.Id != state.PlayerLabId) p.Add(who + " has account teams, which only the player's lab can have");
                    if (assigned > c.Accounts.MaxPeople * c.Market.Sectors.Count) p.Add(who + " has more people on account teams than the data allows");
                }
                {
                    foreach (Submission paper in lab.Submissions)
                        if (paper.Tier < 1 || paper.Tier > 3 || paper.Effort < 1 || paper.Effort > 3 || paper.Cost < 1 || paper.Due < 1)
                            p.Add(who + " has a paper out for review (venue " + paper.Tier + ", effort " + paper.Effort + ", due week " + paper.Due + ") that the data does not allow");
                    if (lab.Submissions.Count > c.Conference.MaxPending) p.Add(who + " has more papers out for review than the data allows");
                    if (lab.Submissions.Count > 0 && lab.Id != state.PlayerLabId) p.Add(who + " has papers out for review, which only the player's lab can have");
                }
                {
                    var seenAlumni = new System.Collections.Generic.HashSet<int>();
                    foreach (Alumnus gone in lab.Alumni)
                        if (c.Role(gone.RoleId) == null || gone.Terms < -1 || gone.Terms > 1 || gone.Left < 0 || gone.Left > state.Tick || !seenAlumni.Add(gone.Id))
                            p.Add(who + " has an alumnus (" + gone.Id + ", " + gone.RoleId + ", left week " + gone.Left + ") that the data does not allow");
                    if (lab.Alumni.Count > c.Alumni.MaxSize) p.Add(who + " has more alumni than the data allows");
                    if (lab.Alumni.Count > 0 && lab.Id != state.PlayerLabId) p.Add(who + " has alumni, which only the player's lab can have");
                }
                if (lab.InternCount < 0 || lab.InternStart < 0 || lab.InternDue < 0 || (lab.InternCount > 0) != (lab.InternRole.Length > 0) || (lab.InternCount > 0 && (lab.InternCount > c.Internships.MaxCohort || c.Role(lab.InternRole) == null || lab.InternDue <= lab.InternStart || lab.Id != state.PlayerLabId)) || (lab.InternCount == 0 && (lab.InternStart != 0 || lab.InternDue != 0)))
                    p.Add(who + " has interns (" + lab.InternCount + " in " + lab.InternRole + ", weeks " + lab.InternStart + " to " + lab.InternDue + ") that the data does not allow");
                if (lab.SupportPeople < 0 || lab.SupportBacklog < 0 || lab.SupportPeople > c.Support.MaxPeople || lab.SupportBacklog > c.Support.MaxBacklog || ((lab.SupportPeople > 0 || lab.SupportBacklog > 0) && lab.Id != state.PlayerLabId))
                    p.Add(who + " has a support desk (" + lab.SupportPeople + " people, backlog " + lab.SupportBacklog + ") that the data does not allow");
                {
                    if (lab.Requisitions.Count > c.Requisitions.MaxOpen || (lab.Requisitions.Count > 0 && lab.Id != state.PlayerLabId))
                        p.Add(who + " has more open orders for people than the data allows");
                    foreach (Requisition order in lab.Requisitions)
                        if (c.Role(order.RoleId) == null || order.Count < 1 || order.Count > c.Requisitions.MaxCount || order.Due < 1 || order.Fee < 1)
                            p.Add(who + " has an order for people (" + order.Count + " " + order.RoleId + " due week " + order.Due + ") that the data does not allow");
                    if (lab.ScholarCount < 0 || lab.ScholarStart < 0 || lab.ScholarDue < 0 || lab.ScholarCount > c.Scholars.MaxScholars || (lab.ScholarCount > 0) != (lab.ScholarDue > 0) || (lab.ScholarCount > 0 && (lab.ScholarDue <= lab.ScholarStart || lab.Id != state.PlayerLabId)) || (lab.ScholarCount == 0 && lab.ScholarStart != 0))
                        p.Add(who + " has visiting scholars (" + lab.ScholarCount + ", weeks " + lab.ScholarStart + " to " + lab.ScholarDue + ") that the data does not allow");
                }
                {
                    if (lab.Cases.Count > 0 && (lab.Id != state.PlayerLabId && state.PlayerLabId >= 0)) p.Add(who + " has claims against it but is not the player's lab");
                    if (lab.Cases.Count > c.Litigation.MaxOpen) p.Add(who + " has more claims open than the data allows");
                    foreach (LawCase lawCase in lab.Cases)
                        if (lawCase.Id < 1 || lawCase.Id > lab.CaseSeq || lawCase.Kind < 0 || lawCase.Kind > 4 || lawCase.Claim < 1 || lawCase.Strength < 1 || lawCase.Strength > MathI.Permille || lawCase.Stage < 0 || lawCase.Stage > 1 || lawCase.Counsel < 0 || lawCase.Counsel > 1 || lawCase.Appealed < 0 || lawCase.Appealed > 1 || lawCase.Due < 1 || (lawCase.Appealed == 1 && lawCase.Stage != 1))
                            p.Add(who + " has a claim (" + lawCase.Id + ") that the data does not allow");
                    if (lab.CaseSeq < 0 || lab.Cases.Count > lab.CaseSeq) p.Add(who + " has a claim count that does not add up");
                    if (lab.SecurityTier < 0 || lab.SecurityTier > c.Security.MaxTier || lab.SecurityBuilding < 0 || lab.SecurityBuilding > c.Security.MaxTier || lab.SecurityDue < 0 || (lab.SecurityBuilding > 0) != (lab.SecurityDue > 0) || (lab.SecurityBuilding > 0 && lab.SecurityBuilding != lab.SecurityTier + 1))
                        p.Add(who + " has security (tier " + lab.SecurityTier + ", building " + lab.SecurityBuilding + ", week " + lab.SecurityDue + ") that the data does not allow");
                    if (lab.Exposure < 0 || lab.BreachDue < 0 || lab.Ransoms < 0 || ((lab.SecurityTier > 0 || lab.SecurityBuilding > 0 || lab.Exposure > 0 || lab.BreachDue > 0 || lab.Ransoms > 0) && (lab.Id != state.PlayerLabId && state.PlayerLabId >= 0)))
                        p.Add(who + " has security exposure (" + lab.Exposure + ", breach week " + lab.BreachDue + ", ransoms " + lab.Ransoms + ") that the data does not allow");
                }
                {
                    bool contract = lab.ContractKind.Length > 0;
                    bool offer = lab.OfferKind.Length > 0;
                    if (contract && (c.GovContracts.Kind(lab.ContractKind) == null || lab.ContractDue <= lab.ContractStart || lab.ContractPaid < 0 || lab.ContractMissed < 0 || lab.ContractPaid + lab.ContractMissed > c.GovContracts.Installments || lab.ContractMissed >= c.GovContracts.MissLimit))
                        p.Add(who + " has a government contract (" + lab.ContractKind + ", weeks " + lab.ContractStart + " to " + lab.ContractDue + ") that the data does not allow");
                    if (!contract && (lab.ContractStart != 0 || lab.ContractDue != 0 || lab.ContractPaid != 0 || lab.ContractMissed != 0))
                        p.Add(who + " has contract dates but no contract");
                    if (offer && (contract || c.GovContracts.Kind(lab.OfferKind) == null || lab.OfferDue < 1)) p.Add(who + " has an offer (" + lab.OfferKind + ") that the data does not allow");
                    if (!offer && lab.OfferDue != 0) p.Add(who + " has an offer date but no offer");
                    if (lab.OfferSeq < 0 || lab.ContractNext < 0 || ((contract || offer || lab.OfferSeq > 0 || lab.ContractNext > 0) && (lab.Id != state.PlayerLabId && state.PlayerLabId >= 0)))
                        p.Add(who + " has government contract state that the data does not allow");
                }
                if (lab.Charter < 0 || lab.Charter > 2 || lab.CharterTo < 0 || lab.CharterTo > 3 || lab.CharterDue < 0 || (lab.CharterTo > 0) != (lab.CharterDue > 0) || (lab.CharterTo > 0 && lab.CharterTo - 1 == lab.Charter) || lab.CapPaid < 0 || ((lab.Charter != 0 || lab.CharterTo != 0 || lab.CapPaid != 0) && (lab.Id != state.PlayerLabId && state.PlayerLabId >= 0)))
                    p.Add(who + " has a charter (" + lab.Charter + ", under review " + lab.CharterTo + ", week " + lab.CharterDue + ", investors paid " + lab.CapPaid + ") that the data does not allow");
                if (lab.PolicyLevel < 0 || lab.PolicyLevel > c.Scaling.MaxLevel || lab.PolicyLockedUntil < 0 || lab.PolicyHold < 0 || lab.PolicyHold > lab.PolicyLevel || lab.PolicyBreaks < 0 || ((lab.PolicyLevel != 0 || lab.PolicyLockedUntil != 0 || lab.PolicyBreaks != 0) && (lab.Id != state.PlayerLabId && state.PlayerLabId >= 0)))
                    p.Add(who + " has a scaling policy (level " + lab.PolicyLevel + ", hold " + lab.PolicyHold + ", locked until " + lab.PolicyLockedUntil + ") that the data does not allow");
                if (lab.TaxStructure < 0 || lab.TaxStructure > 2 || lab.TaxBuilding < 0 || lab.TaxBuilding > 2 || (lab.TaxBuilding != 0 && lab.TaxBuilding <= lab.TaxStructure) || (lab.TaxBuilding == 0) != (lab.TaxDue == 0) || lab.TaxAccrued < 0 || lab.TaxSaved < 0 || lab.TaxResearch < 0 || lab.TaxCredit < 0 || lab.TaxClaimed < 0 || lab.TaxExposure < 0 || lab.TaxAuditDue < 0 || lab.TaxAuditOwed < 0 || (lab.TaxAuditDue == 0) != (lab.TaxAuditOwed == 0)
                    || ((lab.TaxStructure != 0 || lab.TaxBuilding != 0 || lab.TaxAccrued != 0 || lab.TaxSaved != 0 || lab.TaxResearch != 0 || lab.TaxCredit != 0 || lab.TaxClaimed != 0 || lab.TaxExposure != 0 || lab.TaxAuditDue != 0) && (lab.Id != state.PlayerLabId && state.PlayerLabId >= 0)))
                    p.Add(who + " keeps tax books (structure " + lab.TaxStructure + ", building " + lab.TaxBuilding + ", audit due " + lab.TaxAuditDue + ") that the data does not allow");
                if (lab.StdLevel < 0 || lab.StdLevel > c.Standards.MaxLevel || lab.StdDraft < 0 || (lab.StdDraft != 0 && lab.StdDraft != lab.StdLevel + 1) || lab.StdDraft > c.Standards.MaxLevel || lab.StdSupport < 0 || (lab.StdDraft == 0 && lab.StdSupport != 0) || lab.StdRole < 0 || lab.StdRole > 3 || lab.StdNext < 0 || lab.StdAdopted < 0 || lab.StdAdopted > lab.StdLevel || lab.StdLapses < 0
                    || ((lab.StdLevel != 0 || lab.StdDraft != 0 || lab.StdRole != 0 || lab.StdNext != 0 || lab.StdLapses != 0) && (lab.Id != state.PlayerLabId && state.PlayerLabId >= 0)))
                    p.Add(who + " has a part in the industry standards (level " + lab.StdLevel + ", draft " + lab.StdDraft + ", adopted " + lab.StdAdopted + ") that the data does not allow");
                if (lab.WarrantyTier < 0 || lab.WarrantyTier > 2 || lab.Spares < 0 || lab.Spares > c.Reliability.MaxSpares || lab.FailCarry < 0 || lab.FailCarry >= 1000000 || lab.RmaUnits < 0 || (lab.RmaUnits == 0) != (lab.RmaDue == 0) || lab.RmaDue < 0 || lab.GpuFailures < 0
                    || ((lab.WarrantyTier != 0 || lab.Spares != 0 || lab.FailCarry != 0 || lab.RmaUnits != 0 || lab.GpuFailures != 0) && (lab.Id != state.PlayerLabId && state.PlayerLabId >= 0)))
                    p.Add(who + " keeps a warranty or spares (tier " + lab.WarrantyTier + ", spares " + lab.Spares + ", waiting " + lab.RmaUnits + ") that the data does not allow");
                if (lab.CovBreach < 0 || lab.CovWaiverUntil < 0 || lab.CovWaivers < 0 || lab.CovWaivers > c.Covenants.MaxWaivers || lab.CovCalls < 0 || (lab.CovBreach > 0 && lab.Loans.Count == 0)
                    || ((lab.CovBreach != 0 || lab.CovWaiverUntil != 0 || lab.CovWaivers != 0 || lab.CovCalls != 0) && (lab.Id != state.PlayerLabId && state.PlayerLabId >= 0)))
                    p.Add(who + " is in breach of a loan covenant or has a waiver (breach " + lab.CovBreach + ", waivers " + lab.CovWaivers + ") that the data does not allow");
                if (lab.CapPolicy < 0 || lab.CapPolicy > 2 || lab.CapPool < 0 || lab.CapSwitchTick < 0 || lab.CapWritten < 0
                    || ((lab.CapPolicy != 0 || lab.CapPool != 0 || lab.CapSwitchTick != 0 || lab.CapWritten != 0) && (lab.Id != state.PlayerLabId && state.PlayerLabId >= 0)))
                    p.Add(who + " has a capital allowance policy or pool (policy " + lab.CapPolicy + ", pool " + lab.CapPool + ") that the data does not allow");
                if (lab.Listed < 0 || lab.Listed > 1 || lab.ListPrice < 0 || lab.ListGuidance < 0 || lab.ListGuidance > 3 || lab.ListTarget < 0 || lab.ListQuarter < 0 || lab.ListBase < 0 || lab.ListNext < 0 || lab.ListKeep < 0 || lab.ListKeep > 1000 || lab.ListStreak < 0 || lab.ListFollowOn < 0 || lab.ListActivist < 0 || lab.ListActivist > c.Listing.MaxActivist
                    || (lab.Listed == 1 && (lab.ListPrice < c.Listing.PriceFloor || lab.ListPrice > c.Listing.PriceCap || lab.ListKeep < 1 || lab.ListNext < 1))
                    || (lab.Listed == 0 && (lab.ListPrice != 0 || lab.ListGuidance != 0 || lab.ListTarget != 0 || lab.ListQuarter != 0 || lab.ListNext != 0 || lab.ListKeep != 0 || lab.ListStreak != 0 || lab.ListActivist != 0))
                    || ((lab.Listed != 0 || lab.ListBase != 0 || lab.ListFollowOn != 0) && lab.Id != state.PlayerLabId && state.PlayerLabId >= 0))
                    p.Add(who + " is listed (price " + lab.ListPrice + ", guidance " + lab.ListGuidance + ", activist " + lab.ListActivist + ") in a way the data does not allow");
                if (lab.RbfAdvance < 0 || lab.RbfOwed < 0 || lab.RbfBase < 0 || lab.RbfShare < 0 || lab.RbfShare > 1000 || lab.RbfTier < 0 || lab.RbfTier > 3 || lab.RbfWeeks < 0 || lab.RbfSlump < 0
                    || (lab.RbfOwed > 0 && (lab.RbfAdvance < 1 || lab.RbfTier < 1 || lab.RbfShare < 1))
                    || (lab.RbfOwed == 0 && (lab.RbfAdvance != 0 || lab.RbfBase != 0 || lab.RbfShare != 0 || lab.RbfTier != 0 || lab.RbfWeeks != 0 || lab.RbfSlump != 0))
                    || (lab.RbfOwed != 0 && lab.Id != state.PlayerLabId && state.PlayerLabId >= 0))
                    p.Add(who + " owes " + lab.RbfOwed + " on a revenue advance (tier " + lab.RbfTier + ") in a way the data does not allow");
                if (lab.FoundFund < 0 || lab.FoundGiven < 0 || lab.FoundFocus < 0 || lab.FoundFocus > 3 || lab.FoundPayout < 0 || lab.FoundPayout > 3 || lab.FoundGoodwill < 0 || lab.FoundGoodwill > c.Foundation.MaxGoodwill || lab.FoundSwitch < 0 || lab.FoundDrip < 0
                    || (lab.FoundFund > 0 && lab.FoundFocus < 1)
                    || (lab.FoundFund == 0 && (lab.FoundGiven != 0 || lab.FoundFocus != 0 || lab.FoundPayout != 0 || lab.FoundGoodwill != 0 || lab.FoundSwitch != 0 || lab.FoundDrip != 0))
                    || (lab.FoundFund != 0 && lab.Id != state.PlayerLabId && state.PlayerLabId >= 0))
                    p.Add(who + " has a foundation of " + lab.FoundFund + " (focus " + lab.FoundFocus + ", goodwill " + lab.FoundGoodwill + ") in a way the data does not allow");
                if (lab.LeaseUnits < 0 || lab.LeaseValue < 0 || lab.LeaseRent < 0 || lab.LeaseBuyback < 0 || lab.LeaseLeft < 0 || lab.LeaseMissed < 0 || lab.LeaseExtended < 0 || lab.LeaseExtended > 1
                    || (lab.LeaseUnits > 0 && (lab.LeaseLeft < 1 || lab.LeaseValue < 1))
                    || (lab.LeaseUnits == 0 && (lab.LeaseValue != 0 || lab.LeaseRent != 0 || lab.LeaseBuyback != 0 || lab.LeaseLeft != 0 || lab.LeaseMissed != 0 || lab.LeaseExtended != 0))
                    || (lab.LeaseUnits != 0 && lab.Id != state.PlayerLabId && state.PlayerLabId >= 0))
                    p.Add(who + " has " + lab.LeaseUnits + " GPUs leased back (value " + lab.LeaseValue + ", " + lab.LeaseLeft + " weeks left) in a way the data does not allow");
                if (lab.NoteOwed < 0 || lab.NotePrincipal < 0 || lab.NoteRate < 0 || lab.NoteDiscount < 0 || lab.NoteDiscount > 1000 || lab.NoteLeft < 0 || lab.NoteExtended < 0 || lab.NoteExtended > 1 || lab.NoteTier < 0 || lab.NoteTier > 3
                    || (lab.NoteOwed > 0 && (lab.NotePrincipal < 1 || lab.NoteLeft < 1 || lab.NoteTier < 1))
                    || (lab.NoteOwed == 0 && (lab.NotePrincipal != 0 || lab.NoteRate != 0 || lab.NoteDiscount != 0 || lab.NoteLeft != 0 || lab.NoteExtended != 0 || lab.NoteTier != 0))
                    || (lab.NoteOwed != 0 && lab.Id != state.PlayerLabId && state.PlayerLabId >= 0))
                    p.Add(who + " owes " + lab.NoteOwed + " on a convertible note (tier " + lab.NoteTier + ", " + lab.NoteLeft + " weeks left) in a way the data does not allow");
                if (lab.DevLine < 0 || lab.DevLine > 3 || lab.DevStage < 0 || lab.DevStage > 2 || lab.DevWeeks < 0 || lab.DevQuality < 0 || lab.DevQuality > 1000 || lab.DevTier < 0 || lab.DevTier > 3 || lab.DevSold < 0 || lab.DevBase < 0
                    || (lab.DevLine > 0 && (lab.DevStage < 1 || lab.DevTier < 1))
                    || (lab.DevLine == 0 && (lab.DevStage != 0 || lab.DevWeeks != 0 || lab.DevQuality != 0 || lab.DevTier != 0 || lab.DevSold != 0 || lab.DevBase != 0))
                    || (lab.DevLine != 0 && lab.Id != state.PlayerLabId && state.PlayerLabId >= 0))
                    p.Add(who + " has a device line " + lab.DevLine + " (stage " + lab.DevStage + ", quality " + lab.DevQuality + ") in a way the data does not allow");
                if (lab.PlatPlat < 0 || lab.PlatPlat > 3 || lab.PlatLeft < 0 || lab.PlatCut < 0 || lab.PlatRenewals < 0 || lab.PlatReliance < 0 || lab.PlatReliance > MathI.Permille || lab.PlatHoldup < 0 || lab.PlatHoldup > 1 || lab.PlatPaid < 0 || lab.PlatKept < 0
                    || (lab.PlatPlat > 0 && lab.PlatLeft < 1)
                    || (lab.PlatPlat == 0 && (lab.PlatLeft != 0 || lab.PlatCut != 0 || lab.PlatRenewals != 0 || lab.PlatReliance != 0 || lab.PlatHoldup != 0 || lab.PlatPaid != 0 || lab.PlatKept != 0))
                    || (lab.PlatPlat != 0 && lab.Id != state.PlayerLabId && state.PlayerLabId >= 0))
                    p.Add(who + " has a deal with platform " + lab.PlatPlat + " (" + lab.PlatLeft + " weeks left, cut " + lab.PlatCut + ") in a way the data does not allow");
                if (lab.CreedA < 0 || lab.CreedA > 5 || lab.CreedB < 0 || lab.CreedB > 5 || lab.CreedIntegrity < 0 || lab.CreedIntegrity > MathI.Permille || lab.CreedBand < 0 || lab.CreedBand > 2 || lab.CreedLock < 0 || lab.CreedTown < 0 || lab.CreedKept < 0 || lab.CreedBroken < 0
                    || (lab.CreedA > 0 && (lab.CreedB < 1 || lab.CreedB == lab.CreedA))
                    || (lab.CreedA == 0 && (lab.CreedB != 0 || lab.CreedIntegrity != 0 || lab.CreedBand != 0 || lab.CreedLock != 0 || lab.CreedTown != 0 || lab.CreedKept != 0 || lab.CreedBroken != 0))
                    || (lab.CreedA != 0 && lab.Id != state.PlayerLabId && state.PlayerLabId >= 0))
                    p.Add(who + " has a creed of " + lab.CreedA + " and " + lab.CreedB + " (integrity " + lab.CreedIntegrity + ") in a way the data does not allow");
                if (lab.ShockKind < 0 || lab.ShockKind > 3 || lab.ShockRef < 0 || lab.ShockRef > c.WorldEvents.Count || lab.ShockLeft < 0 || lab.ShockBraced < 0 || lab.ShockRode < 0 || lab.ShockCountered < 0
                    || (lab.ShockKind > 0 && (lab.ShockLeft < 1 || lab.ShockRef < 1))
                    || (lab.ShockKind == 0 && (lab.ShockRef != 0 || lab.ShockLeft != 0))
                    || ((lab.ShockKind != 0 || lab.ShockBraced != 0 || lab.ShockRode != 0 || lab.ShockCountered != 0) && lab.Id != state.PlayerLabId && state.PlayerLabId >= 0))
                    p.Add(who + " has a shock " + lab.ShockKind + " waiting (" + lab.ShockLeft + " weeks left) in a way the data does not allow");
                if (lab.ReplPending < 0 || lab.ReplWait < 0 || lab.ReplLeft < 0 || lab.ReplRigor < 0 || lab.ReplRigor > MathI.Permille || lab.ReplWins < 0 || lab.ReplFails < 0
                    || (lab.ReplPending == 0 && lab.ReplWait != 0)
                    || (lab.ReplLeft == 0 && lab.ReplRigor != 0)
                    || ((lab.ReplPending != 0 || lab.ReplLeft != 0 || lab.ReplWins != 0 || lab.ReplFails != 0) && lab.Id != state.PlayerLabId && state.PlayerLabId >= 0))
                    p.Add(who + " has " + lab.ReplPending + " papers waiting on their code and one being replicated (" + lab.ReplLeft + " weeks) in a way the data does not allow");
                if (lab.TbRef < 0 || lab.TbRef > c.Research.Techs.Count || lab.TbLeft < 0 || lab.TbWins < 0 || lab.TbLosses < 0
                    || (lab.TbRef > 0 && lab.TbLeft < 1) || (lab.TbRef == 0 && lab.TbLeft != 0)
                    || ((lab.TbRef != 0 || lab.TbWins != 0 || lab.TbLosses != 0) && lab.Id != state.PlayerLabId && state.PlayerLabId >= 0))
                    p.Add(who + " has a public tech bet " + lab.TbRef + " (" + lab.TbLeft + " weeks left) in a way the data does not allow");
                if (lab.WlModel < 0 || lab.WlHype < 0 || lab.WlHype > 1000 || lab.WlLeft < 0 || lab.WlWins < 0 || lab.WlFlops < 0
                    || (lab.WlModel > 0 && lab.WlLeft < 1) || (lab.WlModel == 0 && (lab.WlLeft != 0 || lab.WlHype != 0))
                    || ((lab.WlModel != 0 || lab.WlWins != 0 || lab.WlFlops != 0) && lab.Id != state.PlayerLabId && state.PlayerLabId >= 0))
                    p.Add(who + " has a waitlist " + lab.WlModel + " (hype " + lab.WlHype + ", " + lab.WlLeft + " weeks left) in a way the data does not allow");
                if (lab.BrandOn < 0 || lab.BrandOn > 1 || lab.BrandScore < 0 || lab.BrandScore > 1000 || lab.BrandReview < 0 || lab.BrandLow < 0 || lab.BrandLow > 1 || lab.BrandReviews < 0
                    || (lab.BrandOn == 0 && (lab.BrandScore != 0 || lab.BrandReview != 0 || lab.BrandLow != 0))
                    || (lab.BrandReview > 0 && lab.BrandLow != 1)
                    || ((lab.BrandOn != 0 || lab.BrandReviews != 0) && lab.Id != state.PlayerLabId && state.PlayerLabId >= 0))
                    p.Add(who + " has an employer brand (on " + lab.BrandOn + ", score " + lab.BrandScore + ", review " + lab.BrandReview + ") in a way the data does not allow");
                if (lab.RoadmapLevel < 0 || lab.RoadmapLevel > c.Roadmap.MaxLevel
                    || (lab.RoadmapLevel == 0 && (lab.RoadmapStart != 0 || lab.RoadmapDue != 0 || lab.RoadmapGoal != 0))
                    || (lab.RoadmapLevel > 0 && (lab.RoadmapStart < 0 || lab.RoadmapDue <= lab.RoadmapStart || lab.RoadmapGoal < 1 || lab.Id != state.PlayerLabId)))
                    p.Add(who + " has a roadmap (level " + lab.RoadmapLevel + ", due week " + lab.RoadmapDue + ") that the data does not allow");
                if (lab.PledgeLevel < 0 || lab.PledgeLevel > c.Pledge.MaxLevel || lab.PledgeLockedUntil < 0 || ((lab.PledgeLevel > 0 || lab.PledgeLockedUntil > 0) && lab.Id != state.PlayerLabId))
                    p.Add(who + " has a safety pledge at level " + lab.PledgeLevel + " (locked until " + lab.PledgeLockedUntil + "), which the data does not allow");
                if (lab.Reserve < 0 || (lab.Reserve > 0 && lab.Id != state.PlayerLabId))
                    p.Add(who + " keeps a reserve of $" + lab.Reserve + ", which the data does not allow");
                if (lab.BountyLevel < 0 || lab.BountyLevel > c.BugBounty.MaxLevel || lab.BountyLockedUntil < 0 || ((lab.BountyLevel > 0 || lab.BountyLockedUntil > 0) && lab.Id != state.PlayerLabId))
                    p.Add(who + " runs a bug bounty at level " + lab.BountyLevel + " (locked until " + lab.BountyLockedUntil + "), which the data does not allow");
                if (lab.WhistleWeeks < 0 || lab.WhistleModel < 0 || lab.WhistleDue < 0 || lab.Silenced < 0
                    || (lab.WhistleDue == 0 && lab.WhistleModel != 0)
                    || (lab.WhistleDue > 0 && lab.Model(lab.WhistleModel) == null)
                    || ((lab.WhistleWeeks > 0 || lab.WhistleDue > 0 || lab.Silenced > 0) && lab.Id != state.PlayerLabId))
                    p.Add(who + " has a whistleblower's concern (weeks " + lab.WhistleWeeks + ", due " + lab.WhistleDue + ", silenced " + lab.Silenced + ") that the data does not allow");
                if (lab.Chairs < 0 || lab.Chairs > c.Chairs.MaxChairs || (lab.Chairs > 0 && lab.Id != state.PlayerLabId)) p.Add(who + " holds " + lab.Chairs + " university chairs, which the data does not allow");
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
                    if (model.SunsetStart < 0 || model.SunsetTick < 0 || (model.SunsetTick == 0 && model.SunsetStart != 0)
                        || (model.SunsetTick > 0 && (model.SunsetTick <= model.SunsetStart || lab.Id != state.PlayerLabId)))
                        p.Add(model.Name + " has an end-of-life notice (given week " + model.SunsetStart + ", leaves week " + model.SunsetTick + ") that the data does not allow");
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
