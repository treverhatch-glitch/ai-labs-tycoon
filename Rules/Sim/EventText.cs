using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    /// <summary>
    /// Plain English for an event, for the console tool, test output and a first news ticker.
    /// The game's real screens are free to write their own: events are data.
    /// </summary>
    public static class EventText
    {
        public static string Describe(GameConfig c, GameState state, GameEvent e)
        {
            LabState lab = state.Lab(e.LabId);
            string who = lab == null ? "A lab" : lab.Name;
            return GameDate.Label(c.Calendar, e.Tick) + ": " + Body(c, state, e, who);
        }

        static string IncidentHeadline(GameConfig c, GameEvent e)
        {
            if (!string.IsNullOrEmpty(e.RiskClass))
            {
                foreach (RiskClassConfig risk in c.Safety.RiskClasses)
                    if (risk.Id == e.RiskClass) return risk.Headlines[e.Pick];
            }
            return EraSystem.AtTick(c, e.Tick).IncidentHeadlines[e.Pick];
        }

        static string NewRisks(GameConfig c, GameEvent e)
        {
            List<RiskClassConfig> opening = RiskClasses.OpeningIn(c, c.Era(e.Ref));
            if (opening.Count == 0) return "";
            var names = new List<string>();
            for (int i = 0; i < opening.Count; i++) names.Add(opening[i].Name.ToLowerInvariant());
            return " A new kind of trouble: " + string.Join(", ", names) + ".";
        }

        static string EraShifts(GameConfig c, GameEvent e)
        {
            List<EraRuleLine> shifts = EraRules.Shifts(c, e.Tick);
            if (shifts.Count == 0) return "";
            var parts = new List<string>();
            for (int i = 0; i < shifts.Count; i++) parts.Add(EraRules.Label(shifts[i].Kind).ToLowerInvariant() + " " + EraRules.Text(shifts[i].Kind, shifts[i].Value));
            return " The rules shift: " + string.Join(", ", parts) + ".";
        }

        static string Body(GameConfig c, GameState state, GameEvent e, string who)
        {
            switch (e.Kind)
            {
                case EventKind.RoomBuilt: return who + " built " + WithArticle(c.RoomType(e.Ref).Name) + " for $" + e.Amount + ".";
                case EventKind.RoomDemolished: return who + " knocked down " + WithArticle(c.RoomType(e.Ref).Name) + ".";
                case EventKind.LotBought: return who + " moved up to the " + c.Lot(e.Ref).Name + " for $" + e.Amount + ".";
                case EventKind.Hired: return who + " hired " + e.Amount + " x " + c.Role(e.Ref).Name + ".";
                case EventKind.Fired: return who + " let go of " + e.Amount + " x " + c.Role(e.Ref).Name + ".";
                case EventKind.GpusRented: return who + " now rents " + e.Amount + " GPUs.";
                case EventKind.GpusBought: return who + " bought " + e.Amount + " GPUs from " + c.Compute.VendorName + ".";
                case EventKind.GpusSold: return who + " sold " + e.Amount + " GPUs.";
                case EventKind.TechResearched:
                    if (e.Ref == "frontier") return who + " pushed the research frontier.";
                    TechConfig tech = c.Tech(e.Ref);
                    return who + " researched " + (tech == null ? e.Ref : tech.Name) + ".";
                case EventKind.TrainingStarted: return who + " started training " + e.Ref + ".";
                case EventKind.ObjectiveSet:
                    {
                        ObjectiveConfig objective = c.Objectives.Objective(string.IsNullOrEmpty(e.Ref) ? c.Objectives.DefaultId : e.Ref);
                        return who + " set " + (objective == null ? e.Ref : objective.Name) + " as what the next version is trained for.";
                    }
                case EventKind.SuccessorDesigned:
                    {
                        ModelState helper = state.Lab(e.LabId) == null ? null : state.Lab(e.LabId).Model((int)e.Amount);
                        return who + " put " + (helper == null ? "an older model" : helper.Name) + " to work on designing " + e.Ref + ".";
                    }
                case EventKind.ModelRushed:
                    return who + " rushed " + e.Ref + " out with " + e.Amount + (e.Amount == 1 ? " week" : " weeks") + " of testing skipped.";
                case EventKind.Lobbied:
                    {
                        string[] parts = e.Ref.Split('|');
                        BlocConfig bloc = parts.Length == 3 ? c.World.Bloc(parts[0]) : null;
                        if (bloc == null) return who + " lobbied a government.";
                        string aim = parts[1] == Lobbying.Subsidy ? "a subsidy" : "a little less attention";
                        if (parts[2] == "backfired") return who + " lobbied " + bloc.Name + " for " + aim + " and it became a scandal.";
                        if (parts[2] == "missed") return who + " lobbied " + bloc.Name + " for " + aim + ". Nothing came of it.";
                        return who + " lobbied " + bloc.Name + " and got " + aim + ".";
                    }
                case EventKind.StylePivoted:
                    {
                        string[] parts = e.Ref.Split('|');
                        StyleConfig to = parts.Length == 2 ? c.Style(parts[1]) : null;
                        return who + " pivoted to " + (to == null ? "a new style" : to.Name) + ". " + e.SubjectId + (e.SubjectId == 1 ? " person" : " people") + " left.";
                    }
                case EventKind.EraScored:
                    {
                        EraConfig era = c.Era(e.Ref);
                        return who + " finished " + (era == null ? e.Ref : era.Name) + " with " + e.SubjectId + (e.SubjectId == 1 ? " goal" : " goals") + " met, scoring " + e.Amount + ".";
                    }
                case EventKind.TrainingFailed:
                    return who + "'s run of " + e.Ref + " diverged and lost " + e.Amount + " compute of progress.";
                case EventKind.RunSetUp:
                    {
                        string[] parts = e.Ref.Split('|');
                        if (parts.Length == 2 && parts[0] == "approach")
                        {
                            ApproachConfig a = c.Runs.Approach(string.IsNullOrEmpty(parts[1]) ? c.Runs.DefaultApproachId : parts[1]);
                            return who + " chose " + (a == null ? parts[1] : a.Name) + " for a run.";
                        }
                        CheckpointConfig k = parts.Length == 2 ? c.Runs.Checkpoint(string.IsNullOrEmpty(parts[1]) ? c.Runs.DefaultCheckpointId : parts[1]) : null;
                        return who + " chose \"" + (k == null ? e.Ref : k.Name) + "\" for a run.";
                    }
                case EventKind.OffsiteHeld: return who + " took the team on an offsite for $" + e.Amount + ". " + e.SubjectId + (e.SubjectId == 1 ? " person" : " people") + " came back happier.";
                case EventKind.InsuranceClaimPaid: return "The insurer paid $" + e.Amount + " of " + who + "'s fine for " + e.Ref + ".";
                case EventKind.InsuranceDropped: return "The insurer ended " + who + "'s policy after " + e.Amount + " claims.";
                case EventKind.CrunchEnded:
                    return who + "'s crunch time ended after " + e.Amount + (e.Amount == 1 ? " week" : " weeks") + (e.Ref == "tired" ? ": the team was worn out." : e.Ref == "spent" ? ": it ran as long as it can." : e.Ref == "empty" ? ": there was no one left." : ".");
                case EventKind.ChipSlotSnubbed: return "Foundry snub. " + who + "'s chip slot went to a bigger customer and the delivery slips " + e.Amount + " weeks.";
                case EventKind.LoanTaken: return who + " borrowed $" + e.Amount + " against its chips.";
                case EventKind.LoanRepaid:
                    return e.Ref == "early" ? who + " paid off a loan early: $" + e.Amount + "." : who + " paid off a $" + e.Amount + " loan.";
                case EventKind.LoanMissed: return who + " missed a $" + e.Amount + " loan payment.";
                case EventKind.LoanDefaulted:
                    return who + " defaulted: the lender took " + e.SubjectId + (e.SubjectId == 1 ? " GPU" : " GPUs") + (e.Amount > 0 ? " and wrote off $" + e.Amount + "." : ".");
                case EventKind.StaffQuit:
                    {
                        string[] parts = e.Ref.Split('|');
                        RoleConfig role = c.Role(parts[0]);
                        return "A " + (role == null ? parts[0] : role.Name.ToLowerInvariant()) + " quit " + who + " by blog post. It cost " + e.Amount + " reputation.";
                    }
                case EventKind.TrainingFinished: return who + " finished training " + e.Ref + " (capability " + e.Amount + ").";
                case EventKind.ModelReleased: return who + " released " + e.Ref + " into " + e.Amount + (e.Amount == 1 ? " sector." : " sectors.");
                case EventKind.ModelRetired: return who + " retired " + e.Ref + ".";
                case EventKind.Incident:
                    {
                        LabState incidentLab = state.Lab(e.LabId);
                        ModelState incidentModel = incidentLab == null ? null : incidentLab.Model(e.SubjectId);
                        string modelName = incidentModel == null ? e.Ref : incidentModel.Name;
                        return "INCIDENT at " + who + ": " + IncidentHeadline(c, e).Replace("{model}", modelName) + " Fine: $" + e.Amount + ".";
                    }
                case EventKind.GauntletCrown: return e.Ref + " by " + who + " takes the crown at " + c.Gauntlet.Name + ".";
                case EventKind.GauntletBracket: return who + " wins the " + c.Sector(e.Ref).Name + " bracket at " + c.Gauntlet.Name + ".";
                case EventKind.GauntletTuned:
                    return e.Pick == 1
                        ? who + " paid $" + e.Amount + " to tune its entries for " + c.Gauntlet.Name + "."
                        : who + " could not find the $" + e.Amount + " to tune its entries for " + c.Gauntlet.Name + ", so they went in as they were.";
                case EventKind.GauntletExposed:
                    return "A rival exposed " + who + "'s tuning at " + c.Gauntlet.Name + ": " + e.SubjectId + " entries lose their lift and " + who + " loses " + e.Amount + " reputation.";
                case EventKind.GauntletDemo:
                    return e.Pick == 1
                        ? who + "'s " + e.Ref + " nailed the live demo at " + c.Gauntlet.Name + ": reputation +" + e.Amount + "."
                        : who + "'s " + e.Ref + " fell over on stage at " + c.Gauntlet.Name + " and became a meme: reputation " + e.Amount + ".";
                case EventKind.GauntletAward:
                    return e.Ref == GauntletSystem.AwardConfident
                        ? who + " wins Most Confident While Wrong at " + c.Gauntlet.Name + ". Reputation " + (-e.Amount) + "."
                        : who + " wins Best Newcomer at " + c.Gauntlet.Name + ". The staff are thrilled: morale +" + e.Amount + ".";
                case EventKind.TrustLost:
                    return who + " lost " + e.Amount + " trust with " + c.Sector(e.Ref).Buyers + " (" + c.Sector(e.Ref).Name + ").";
                case EventKind.CustomerComplaint:
                    {
                        string[] bits = e.Ref.Split('|');
                        SectorConfig sector = c.Sector(bits[0]);
                        return "A customer in " + (sector == null ? bits[0] : sector.Name) + " wrote to " + who + " about " + (bits.Length > 1 ? bits[1] : "an incident") + " (" + e.Amount + " trust at stake).";
                    }
                case EventKind.ComplaintAnswered:
                    {
                        string[] bits = e.Ref.Split('|');
                        ComplaintResponseConfig response = c.Trust.Response(bits[0]);
                        SectorConfig sector = bits.Length > 1 ? c.Sector(bits[1]) : null;
                        string where = sector == null ? "a sector" : sector.Name;
                        string how = response == null ? bits[0] : response.Name.ToLowerInvariant();
                        return e.Pick == 1
                            ? who + " answered the complaint from " + where + " with a " + how + ": trust +" + e.Amount + "."
                            : who + "'s " + how + " went down badly in " + where + ": trust " + e.Amount + ".";
                    }
                case EventKind.RoomDated:
                    {
                        RoomTypeConfig dated = c.RoomType(e.Ref);
                        return who + "'s " + (dated == null ? e.Ref : dated.Name) + " is now " + RoomAge.Stage(c, (int)e.Amount).Name.ToLowerInvariant() + ", " + e.Amount + (e.Amount == 1 ? " era" : " eras") + " behind. Staff are complaining.";
                    }
                case EventKind.OfficeEventPosed:
                    {
                        OfficeEventConfig posed = c.Office.Event(e.Ref);
                        return posed == null ? who + " has something on the desk." : who + ": " + posed.Text;
                    }
                case EventKind.OfficeEventAnswered:
                    {
                        string[] bits = e.Ref.Split('|');
                        OfficeEventConfig asked = c.Office.Event(bits[0]);
                        OfficeChoiceConfig chose = asked == null || bits.Length < 2 ? null : asked.Choice(bits[1]);
                        return chose == null ? who + " dealt with it." : who + " chose to " + chose.Text.ToLowerInvariant() + ". " + chose.Result;
                    }
                case EventKind.OfficeEventLapsed:
                    {
                        string[] parts = e.Ref.Split('|');
                        OfficeEventConfig gone = c.Office.Event(parts[0]);
                        OfficeChoiceConfig taken = parts.Length == 2 && gone != null ? gone.Choice(parts[1]) : null;
                        if (taken != null) return who + " left '" + gone.Name + "' unanswered, so: " + taken.Text + ".";
                        return who + " did nothing about '" + (gone == null ? parts[0] : gone.Name) + "' and it blew over.";
                    }
                case EventKind.RedTeamRan: return who + " paid $" + e.Amount + " for a red team to attack " + e.Ref + ". It came back with a list, and the model is safer for a while.";
                case EventKind.GapFound: return who + "'s interpretability team has read all the way through " + e.Ref + ". The shown meter now matches the real one.";
                case EventKind.PayChanged: return who + " now pays " + Pay.Describe(c, e.SubjectId) + ". The weekly salary bill is $" + e.Amount + ".";
                case EventKind.ComplianceLapsed: return who + "'s compliance office has too few policy staff and fell from level " + e.Amount + " to level " + e.SubjectId + ".";
                case EventKind.PaperPublished: return who + " spent " + e.Amount + " research points on a paper. The field noticed, and the lab's prestige is now " + e.SubjectId + ".";
                case EventKind.SubletChanged: return who + " now lets " + (e.SubjectId / 10) + "% of its compute to outside customers" + (e.Amount > 0 ? ", worth $" + e.Amount + " a week." : ".");
                case EventKind.AuditDone: return who + " paid $" + e.Amount + " for outside auditors to test " + e.Ref + ". " + (e.Pick == 1 ? "Its numbers held up and it has a certificate." : e.Pick == 0 ? "The published score was not the one the lab had been showing." : "They published its real score.");
                case EventKind.FocusChanged:
                    {
                        FocusOptionConfig f = c.Focus.Option(e.Ref);
                        return who + (f == null ? " dropped its research focus" : " now focuses its research on " + f.Name) + " (it cost $" + e.Amount + ").";
                    }
                case EventKind.HackWeek:
                    return e.Pick == 1 ? who + " started a hack week for " + e.SubjectId + " weeks. It cost $" + e.Amount + " and the roadmap is on hold."
                        : who + "'s hack week is over and the team is back on the roadmap.";
                case EventKind.CampaignRun:
                    return e.Pick == 1 ? who + " started a level " + e.SubjectId + " marketing campaign in " + e.Ref + " for $" + e.Amount + "."
                        : e.Pick == 2 ? who + "'s campaign in " + e.Ref + " collapsed when its model had an incident, and customers noticed."
                        : who + "'s campaign in " + e.Ref + " is over. Customers are measuring it against what it promised.";
                case EventKind.RunScrapped:
                    return who + " scrapped the training run of " + e.Ref + (e.Pick == 1 ? " well into it" : " early") + " and kept " + e.Amount + " research points.";
                case EventKind.LayoffRound:
                    return who + " laid off " + e.Amount + " people (" + e.SubjectId / 10 + "% of the staff)" + (e.Pick == 1 ? ", a deep cut that shook the office." : ".");
                case EventKind.DebtPaid:
                    return who + " paid down " + e.Amount + " points of technical debt" + (e.Pick == 1 ? " and has none left." : ", " + e.SubjectId + " to go.");
                case EventKind.ReserveMoved:
                    return e.Pick == 1 ? who + " set $" + e.Amount + " aside in its reserve." : who + " took $" + e.Amount + " back out of its reserve.";
                case EventKind.ReserveCovered:
                    return who + "'s reserve paid out $" + e.Amount + " to cover a debt, at a steep fee.";
                case EventKind.PledgeSet:
                    return e.Pick == 1 ? who + " pledged publicly that its launches will show at least " + Pledge.Floor(c, e.SubjectId) + " alignment (level " + e.SubjectId + ")."
                        : e.SubjectId == 0 ? who + " dropped its public safety pledge." : who + " walked its safety pledge back to level " + e.SubjectId + ".";
                case EventKind.PledgeBroken:
                    return who + " broke its public safety pledge: " + e.Ref + " launched showing less than the " + e.Amount + " alignment it had promised.";
                case EventKind.RoadmapAnnounced:
                    return who + " announced a level " + e.SubjectId + " roadmap: a model with capability " + e.Amount + " inside " + e.Pick + " weeks.";
                case EventKind.RoadmapSettled:
                    return e.Pick == 1 ? who + " delivered its roadmap, a model with capability " + e.Amount + ", and customers noticed."
                        : who + " missed its roadmap for a model with capability " + e.Amount + ", and customers noticed that too.";
                case EventKind.MentoringStarted:
                    return who + " paired a senior with a junior " + c.Role(e.Ref).Name.ToLowerInvariant() + " for mentoring.";
                case EventKind.MentoringEnded:
                    return e.Pick == 0 ? who + " graduated a junior " + c.Role(e.Ref).Name.ToLowerInvariant() + " to senior after " + e.Amount + " weeks of mentoring."
                        : e.Pick == 1 ? "A mentoring pair at " + who + " broke up after " + e.Amount + " weeks."
                        : who + " called off a mentoring pair after " + e.Amount + " weeks.";
                case EventKind.SkunkworksStarted:
                    return who + " pulled " + e.Amount + " " + e.Ref + " people into a " + e.Pick + "-week skunkworks project.";
                case EventKind.SkunkworksDone:
                    return who + (e.Pick == 1 ? " finished" : " wrapped up early") + " its " + e.Ref + " skunkworks project: "
                        + (e.SubjectId == 1 ? e.Amount + " research points." : e.SubjectId == 2 ? e.Amount + " more alignment on its finished models." : e.Amount + " points of technical debt cleared.");
                case EventKind.LabourDrive:
                    return "Staff at " + who + " are organising; the lab has " + e.SubjectId + " weeks to answer.";
                case EventKind.LabourRecognised:
                    return e.Pick == 1 ? who + " recognised its staff union." : who + " was forced to recognise its staff union after a strike.";
                case EventKind.LabourResisted:
                    return e.Pick == 1 ? who + " hired lawyers to resist its staff drive for $" + e.Amount + "." : who + " let its staff drive go unanswered and is resisting it.";
                case EventKind.LabourHeld:
                    return "The staff drive at " + who + " fizzled out.";
                case EventKind.LabourStrike:
                    return "Staff at " + who + " went on strike for " + e.Amount + " weeks.";
                case EventKind.SunsetAnnounced:
                    return who + " announced that " + e.Ref + " will be retired in " + e.Amount + " weeks" + (e.Pick == 1 ? ", with the model it helped design as its successor." : ".");
                case EventKind.SunsetDone:
                    return who + " retired " + e.Ref + "; " + e.Amount + (e.Amount == 1 ? " sector moved" : " sectors moved") + " to its successor" + (e.Pick == 1 ? "." : " and the rest were dropped.");
                case EventKind.SunsetCalledOff:
                    return e.Pick == 1 ? who + " called off the retirement of " + e.Ref + "." : who + " withdrew the retirement of " + e.Ref + " because its successor was gone.";
                case EventKind.AccountTeamChanged:
                    return e.Pick == 1 ? who + " put " + e.Amount + " salespeople on the " + e.Ref + " account team."
                        : e.Pick == 0 ? who + " resized the " + e.Ref + " account team to " + e.Amount + " (" + e.SubjectId + " weeks of familiarity kept)."
                        : e.Pick == 2 ? who + " disbanded the " + e.Ref + " account team."
                        : e.Pick == 3 ? "The " + e.Ref + " account team at " + who + " is down to " + e.Amount + " people."
                        : "The " + e.Ref + " account team at " + who + " was disbanded when the lab left the sector.";
                case EventKind.PaperSubmitted:
                    return who + " sent a paper to a " + e.Ref + " (effort " + e.Pick + ", " + e.Amount + " research points).";
                case EventKind.PaperDecided:
                    return e.Pick == 1 ? "A " + e.Ref + " accepted a paper from " + who + ", adding " + e.Amount + " prestige."
                        : e.Pick == 0 ? "A " + e.Ref + " rejected a paper from " + who + "; " + e.Amount + " research points came back."
                        : who + " withdrew a paper from a " + e.Ref + " and got " + e.Amount + " research points back.";
                case EventKind.AlumniRehired:
                    return who + " rehired a former " + c.Role(e.Ref).Name.ToLowerInvariant() + (e.Pick == 1 ? " who had left on good terms" : e.Pick == 0 ? " who had been laid off" : " who had quit") + " for $" + e.Amount + ".";
                case EventKind.InternshipStarted:
                    return who + " took on " + e.Amount + " " + c.Role(e.Ref).Name.ToLowerInvariant() + (e.Amount == 1 ? " intern" : " interns") + " for " + e.Pick + " weeks.";
                case EventKind.InternshipsEnded:
                    return who + (e.Pick == 1 ? "'s internship ended: " : " sent its interns home early: ") + e.Amount + " of " + e.SubjectId + " taken on.";
                case EventKind.SupportDeskSet:
                    return who + (e.Amount == 0 ? " closed its support desk." : " put " + e.Amount + " engineers on its support desk.");
                case EventKind.SupportBacklogChanged:
                    return e.Pick == 1 ? who + "'s support backlog passed its limit at " + e.Amount + " tickets and customers are losing patience." : who + " cleared its support backlog down to " + e.Amount + " tickets.";
                case EventKind.SabbaticalStarted:
                    return who + " sent a " + c.Role(e.Ref).Name.ToLowerInvariant() + " on a " + e.Amount + "-week sabbatical.";
                case EventKind.SabbaticalEnded:
                    return "A " + c.Role(e.Ref).Name.ToLowerInvariant() + " at " + who + " came back from sabbatical " + (e.Pick == 2 ? "refreshed." : e.Pick == 1 ? "rested." : e.Pick == 0 ? "restless." : "early.");
                case EventKind.RequisitionOpened:
                    return who + " ordered " + e.Amount + " " + c.Role(e.Ref).Name.ToLowerInvariant() + (e.Amount == 1 ? "" : "s") + " for " + e.Pick + " weeks ahead.";
                case EventKind.RequisitionClosed:
                    return e.Pick == 1 ? who + "'s order of " + e.SubjectId + " " + c.Role(e.Ref).Name.ToLowerInvariant() + "s arrived." : e.Pick == 0 ? who + "'s order of " + e.SubjectId + " " + c.Role(e.Ref).Name.ToLowerInvariant() + "s arrived short: " + e.Amount + " came." : who + " cancelled an order of " + e.SubjectId + " " + c.Role(e.Ref).Name.ToLowerInvariant() + "s.";
                case EventKind.ScholarsArrived:
                    return who + " hosted " + e.Amount + " visiting " + (e.Amount == 1 ? "scholar" : "scholars") + " for " + e.Pick + " weeks.";
                case EventKind.ScholarsLeft:
                    return e.Pick == 1 ? who + "'s visiting scholars left, adding " + e.Amount + " prestige." : who + " sent its visiting scholars home early.";
                case EventKind.CaseFiled:
                    return "A claim for " + e.Amount + " was filed against " + who + ": " + (e.Pick == 0 ? "an incident" : e.Pick == 1 ? "a layoff round" : e.Pick == 2 ? "a breach of security" : e.Pick == 3 ? "its change of charter" : "a tax assessment") + ".";
                case EventKind.CaseJudged:
                    return who + " lost a hearing and owes " + e.Amount + (e.Pick == 1 ? " after its appeal." : ", unless it appeals.");
                case EventKind.CaseClosed:
                    return e.Pick == 0 ? who + " settled a claim for " + e.Amount + "." : e.Pick == 1 ? who + " won a claim in court." : e.Pick == 2 ? who + " paid " + e.Amount + " on a lost claim." : who + " had a judgment overturned on appeal.";
                case EventKind.SecurityBuilt:
                    return who + " finished security tier " + e.Amount + ".";
                case EventKind.BreachOpened:
                    return who + " found a breach of its systems.";
                case EventKind.BreachAnswered:
                    return e.Pick == 0 ? who + " paid " + e.Amount + " to the people behind a breach." : e.Pick == 1 ? who + " told everyone about a breach." : who + " locked its systems down after a breach for " + e.Amount + ".";
                case EventKind.BreachLeaked:
                    return "A breach at " + who + " leaked; the lab lost " + e.Amount + " reputation.";
                case EventKind.ContractOffered:
                    return "A government offered " + who + " a " + c.GovContracts.Kind(e.Ref).Name.ToLowerInvariant() + " contract worth " + e.Amount + " over " + e.Pick + " weeks.";
                case EventKind.ContractAccepted:
                    return who + " took on a " + c.GovContracts.Kind(e.Ref).Name.ToLowerInvariant() + " contract.";
                case EventKind.ContractDeclined:
                    return e.Pick == 1 ? who + " turned down a " + c.GovContracts.Kind(e.Ref).Name.ToLowerInvariant() + " contract." : "An offer of a " + c.GovContracts.Kind(e.Ref).Name.ToLowerInvariant() + " contract to " + who + " lapsed.";
                case EventKind.ContractInstalment:
                    return e.Pick == 1 ? who + " was paid " + e.Amount + " on its government contract." : who + " missed an instalment on its government contract.";
                case EventKind.ContractEnded:
                    return e.Pick == 1 ? who + " completed its government contract." : e.Pick == 0 ? who + " lost its government contract after missed instalments and paid " + e.Amount + "." : e.Pick == 2 ? who + " lost its government contract to a leak and paid " + e.Amount + "." : who + " walked away from its government contract and paid " + e.Amount + ".";
                case EventKind.CharterReviewStarted:
                    return who + " applied to become " + Charter.Describe(e.Pick) + ".";
                case EventKind.CharterReviewDropped:
                    return who + " dropped its plan to become " + Charter.Describe(e.Pick) + ".";
                case EventKind.CharterChanged:
                    return who + " is now " + Charter.Describe(e.Pick) + ".";
                case EventKind.TripwireHit:
                    return who + "'s " + e.Ref + " reached a level " + e.Amount + " tripwire and is held back.";
                case EventKind.TripwireCleared:
                    return e.Pick == 1 ? who + " put the safeguards for its level " + e.Amount + " tripwire in place." : who + " is no longer held back by its level " + e.Amount + " tripwire.";
                case EventKind.PolicyChanged:
                    return e.Ref == "break" ? who + " broke its scaling policy." : who + " " + (e.Ref == "raise" ? "raised" : "lowered") + " its scaling policy to level " + e.Amount + ".";
                case EventKind.TaxStructureBuilt:
                    return who + " opened " + (e.Amount == 2 ? "an offshore structure" : "a holding company") + " for its tax.";
                case EventKind.TaxPaid:
                    return who + " paid " + e.Amount + " in tax for the year.";
                case EventKind.TaxCreditClaimed:
                    return who + " claimed a research credit of " + e.Amount + ".";
                case EventKind.TaxAuditOpened:
                    return "The tax office opened an audit of " + who + " over " + e.Amount + ".";
                case EventKind.TaxAuditClosed:
                    return e.Pick == 0 ? who + " paid back " + e.Amount + " to the tax office." : e.Pick == 1 ? who + " settled a tax audit for " + e.Amount + "." : e.Pick == 2 ? who + " took a tax audit to court." : who + " let a tax audit lapse and paid " + e.Amount + ".";
                case EventKind.StandardDrafted:
                    return "The industry began drafting a level " + e.Amount + " standard.";
                case EventKind.StandardRatified:
                    return "A level " + e.Amount + " standard was ratified" + (e.Pick == 2 ? " with " + who + " as its sponsor." : e.Pick == 3 ? " over the objections of " + who + "." : ".");
                case EventKind.StandardAdopted:
                    return who + " adopted the standards up to level " + e.Amount + ".";
                case EventKind.StandardLapsed:
                    return who + " released a model that lacked " + e.Pick + " levels of the standards and was fined " + e.Amount + ".";
                case EventKind.GpusFailed:
                    return who + " lost " + e.Amount + " GPUs to failure" + (e.Pick == 1 ? ", swapped for spares." : e.Pick == 2 ? ", sent back under the warranty." : ".");
                case EventKind.ReplacementsArrived:
                    return who + " got " + e.Amount + " replacement GPUs back" + (e.Pick == 1 ? " in a rush." : ".");
                case EventKind.WarrantyChanged:
                    return who + " set its GPU warranty to tier " + e.Amount + ".";
                case EventKind.SparesStocked:
                    return who + " bought " + e.Amount + " spare GPUs.";
                case EventKind.CovenantBreached:
                    return who + " is in breach of its loan covenant: its chips cover " + e.Amount + " per mille of the debt.";
                case EventKind.CovenantCured:
                    return who + " is back within its loan covenant" + (e.Pick == 1 ? " after paying the loans down." : ".");
                case EventKind.CovenantWaived:
                    return who + " paid " + e.Amount + " to have its loan covenant waived.";
                case EventKind.LoansCalled:
                    return "The lender called " + who + "'s loans, " + e.Amount + " owed" + (e.Pick == 1 ? ", paid from cash." : ", taken from its chips.");
                case EventKind.CapitalPolicySet:
                    return who + " now " + (e.Amount == 0 ? "expenses its hardware and building spend." : e.Amount == 1 ? "capitalises its hardware and building spend." : "claims an enhanced capital allowance.");
                case EventKind.CompanyListed:
                    return who + " listed on the market and raised " + e.Amount + ".";
                case EventKind.EarningsReported:
                    return who + (e.Pick == 2 ? " beat its guidance" : e.Pick == 1 ? " met its guidance" : " missed its guidance") + "; the share price is " + e.Amount + ".";
                case EventKind.FollowOnSold:
                    return who + " sold more shares and raised " + e.Amount + ".";
                case EventKind.SharesBoughtBack:
                    return who + " bought back " + e.Amount + " of its shares.";
                case EventKind.ActivistArrived:
                    return "An activist investor took a step toward " + who + "'s board (" + e.Amount + ").";
                case EventKind.CompanyDelisted:
                    return who + " went private again for " + e.Amount + ".";
                case EventKind.RevenueAdvanceTaken:
                    return who + " took a revenue advance of " + e.Amount + ".";
                case EventKind.RevenueAdvanceCleared:
                    return who + (e.Pick == 1 ? " bought out" : " finished paying") + " its revenue advance.";
                case EventKind.RevenueAdvanceStretched:
                    return "A weak week stretched " + who + "'s revenue advance by " + e.Amount + ".";
                case EventKind.FoundationEndowed:
                    return who + " put " + e.Amount + " into its foundation.";
                case EventKind.FoundationReported:
                    return who + "'s foundation gave " + e.Amount + " in the last stretch; goodwill stands at " + e.SubjectId + ".";
                case EventKind.FoundationRefocused:
                    return who + "'s foundation turned to a new cause.";
                case EventKind.FoundationWoundUp:
                    return who + " wound up its foundation and took back " + e.Amount + ".";
                case EventKind.FoundationRanDry:
                    return who + "'s foundation ran dry after giving " + e.Amount + ".";
                case EventKind.GpusLeasedBack:
                    return who + " sold " + e.SubjectId + " GPUs and leased them back for an advance of " + e.Amount + ".";
                case EventKind.LeaseBoughtBack:
                    return who + " bought its leased GPUs back for " + e.Amount + ".";
                case EventKind.LeaseEnded:
                    return (e.Pick == 2 ? "The lessor repossessed " : e.Pick == 1 ? who + " handed back " : "The lease ran out and ") + e.Amount + " GPUs" + (e.Pick == 2 ? " from " + who + "." : e.Pick == 1 ? "." : " went back.");
                case EventKind.LeaseRentMissed:
                    return who + " missed the GPU rent (" + e.SubjectId + " in a row).";
                case EventKind.LeaseExtended:
                    return who + " extended its GPU lease at a rent of " + e.Amount + ".";
                case EventKind.NoteIssued:
                    return who + " issued a convertible note for " + e.Amount + ".";
                case EventKind.NoteConverted:
                    return who + "'s convertible note of " + e.Amount + " turned into " + (e.SubjectId / 10) + "." + (e.SubjectId % 10) + "% of the company" + (e.Pick == 2 ? " at maturity." : e.Pick == 1 ? " at the listing." : " in the new round.");
                case EventKind.NoteRepaid:
                    return who + " repaid its convertible note with " + e.Amount + ".";
                case EventKind.NoteExtended:
                    return who + " extended its convertible note.";
                case EventKind.DeviceDesigned:
                    return who + " started designing a device for " + e.Amount + ".";
                case EventKind.DeviceLaunched:
                    return who + (e.Pick == 1 ? "'s device launches to applause." : "'s device flops on stage.");
                case EventKind.DeviceRecalled:
                    return who + " recalled its device at a cost of " + e.Amount + ".";
                case EventKind.DeviceWithdrawn:
                    return who + " ended its device line after " + e.Amount + " units.";
                case EventKind.PlatformSigned:
                    return who + " signed a distribution deal, the platform taking " + e.Amount / 10 + "% of the lift.";
                case EventKind.PlatformHoldup:
                    return "The platform raised its cut on " + who + " to " + e.Amount / 10 + "% as the deal came up.";
                case EventKind.PlatformRenewed:
                    return who + " renewed its distribution deal at a " + e.Amount / 10 + "% cut.";
                case EventKind.PlatformEnded:
                    return who + (e.Pick == 1 ? "'s distribution deal ran out." : " walked from its distribution deal.");
                case EventKind.CreedSet:
                    return who + " put its values in writing.";
                case EventKind.TownHallHeld:
                    return who + " held a town hall on its values.";
                case EventKind.CreedOwnedUp:
                    return who + " told its staff it had not lived up to its values.";
                case EventKind.CreedHollow:
                    return who + "'s staff now laugh at its values.";
                case EventKind.CreedProud:
                    return who + " is held up as a lab that lives its values.";
                case EventKind.ShockStruck:
                    return e.SubjectId == 3 ? who + " is in the middle of a boom and has to decide how to meet it." : who + " is hit by the news and has to decide how to meet it.";
                case EventKind.ShockAnswered:
                    return who + (e.Pick == 1 ? " braced and took the news in its stride." : e.Pick == 3 ? " turned the news round." : e.Pick == 4 ? " tried to turn the news round and could not." : e.Pick == 5 ? " let the news happen to it." : " rode the news out.");
                case EventKind.CodeReleased:
                    return who + " put a paper's code and data out for anyone to run.";
                case EventKind.CodeWithheld:
                    return who + " kept a paper's code and data back.";
                case EventKind.PaperReplicated:
                    return e.Pick == 1 ? "Another lab reproduced " + who + "'s paper." : "Another lab could not reproduce " + who + "'s paper.";
                case EventKind.TechBetMade:
                    return who + " promised the world it would get " + e.Ref + " working.";
                case EventKind.TechBetEnded:
                    return who + (e.Pick == 1 ? " kept its promise on " : e.Pick == 2 ? " quietly took back its promise on " : " missed its promise on ") + e.Ref + ".";
                case EventKind.WaitlistOpened:
                    return who + " opened a public waitlist and promised the model in " + e.Amount + " weeks.";
                case EventKind.WaitlistEnded:
                    return who + (e.Pick == 1 ? " shipped to a waiting list in time." : e.Pick == 2 ? " closed its waitlist." : " let its waitlist promise lapse.");
                case EventKind.CareersOpened:
                    return who + " put up a careers page.";
                case EventKind.BadReview:
                    return "An anonymous review of " + who + " as a place to work went around.";
                case EventKind.ReviewAnswered:
                    return who + (e.Pick == 1 ? " owned up to a bad review and fixed it." : e.Pick == 2 ? " publicly rebutted a bad review." : " let a bad review stand.");
                case EventKind.FreeTierOpened:
                    return who + " opened a free tier (level " + e.Amount + ").";
                case EventKind.FreeTierClosed:
                    return who + (e.Pick == 1 ? " shut its free tier." : " lost the model behind its free tier.");
                case EventKind.WatchDeskOpened:
                    return who + " set up a desk to watch rival launches.";
                case EventKind.RivalLaunchSeen:
                    return who + (e.Pick == 1 ? " watched a rival launch overtake its best model." : " watched a rival launch it already beats.");
                case EventKind.LaunchAnswered:
                    return who + (e.Pick == 1 ? " counter-programmed a rival launch." : e.Pick == 3 ? " answered a rival launch with its safety record." : " let a rival launch land.");
                case EventKind.ProgrammeStarted:
                    return who + " started a research programme towards " + e.Ref + ".";
                case EventKind.ProgrammeStageDone:
                    return who + (e.Pick == 2 ? " got a bold stage of its programme working." : " finished a careful stage of its programme.");
                case EventKind.ProgrammeStageFailed:
                    return who + " lost a bold stage of its programme and has to decide what next.";
                case EventKind.ProgrammeEnded:
                    return who + (e.Pick == 1 ? " finished its programme and has " + e.Ref + "." : " gave up its programme towards " + e.Ref + ".");
                case EventKind.RunPriorityChanged:
                    return who + (e.Pick == 3 ? " rushed " : e.Pick == 1 ? " put " : " put ") + e.Ref + (e.Pick == 3 ? " to the front of the training queue." : e.Pick == 1 ? " at the back of the training queue." : " back in the training queue.");
                case EventKind.PodFormed:
                    return who + " formed a project pod of " + e.SubjectId + ".";
                case EventKind.PodSwapped:
                    return who + " swapped a member of its project pod.";
                case EventKind.PodEnded:
                    return who + (e.Pick == 1 ? " disbanded its project pod." : " lost its project pod when people left.");
                case EventKind.LicenceApplied:
                    return who + " applied for a licence.";
                case EventKind.LicenceDecided:
                    return who + (e.Pick == 1 ? " was granted its licence." : " was refused its licence.");
                case EventKind.LicenceStrike:
                    return who + " took a strike against its licence.";
                case EventKind.LicenceEnded:
                    return who + (e.Pick == 2 ? " had its licence suspended." : e.Pick == 1 ? " surrendered its licence." : " let its licence lapse.");
                case EventKind.SupportPledged:
                    return who + " promised to keep supporting " + e.Ref + ".";
                case EventKind.SupportPledgeEnded:
                    return who + (e.Pick == 1 ? " kept its support pledge for " : " broke its support pledge for ") + e.Ref + ".";
                case EventKind.ApiOpened:
                    return who + " opened the API of " + e.Ref + ".";
                case EventKind.ApiDeprecated:
                    return who + " announced the end of the API of " + e.Ref + ".";
                case EventKind.ApiEnded:
                    return who + (e.Pick == 1 ? " retired the API of " : " cut off customers on the API of ") + e.Ref + ".";
                case EventKind.ConformityStarted:
                    return who + " began a conformity assessment of " + e.Ref + ".";
                case EventKind.ConformityDecided:
                    return who + (e.Pick == 1 ? " was certified for " : " had findings against ") + e.Ref + ".";
                case EventKind.ConformityEnded:
                    return who + (e.Pick == 2 ? " had its certificate for " : e.Pick == 1 ? " saw the certificate for " : " dropped the assessment of ") + e.Ref + (e.Pick == 2 ? " suspended." : e.Pick == 1 ? " expire." : ".");
                case EventKind.OrgChanged:
                    return who + " reorganised how it works.";
                case EventKind.OrgStrained:
                    return who + " is feeling the strain of its structure.";
                case EventKind.FabricOrdered:
                    return who + " ordered a faster cluster fabric.";
                case EventKind.FabricDelivered:
                    return who + " finished building its cluster fabric.";
                case EventKind.FabricCancelled:
                    return who + " cancelled its cluster fabric order.";
                case EventKind.RegisterChanged:
                    return e.Pick == 1 ? who + " joined the incident register." : who + " left the incident register.";
                case EventKind.ReportFiled:
                    return who + " filed an incident report.";
                case EventKind.ReportLate:
                    return who + " was late with an incident report.";
                case EventKind.InspectionHeld:
                    return e.Pick == 1 ? "The register's inspection found nothing to fine at " + who + "." : "The register's inspection fined " + who + ".";
                case EventKind.DataAcquired:
                    return who + " brought in new training data.";
                case EventKind.DataCleaned:
                    return who + " finished cleaning a batch of training data.";
                case EventKind.DataLicensed:
                    return who + " licensed data it had already scraped.";
                case EventKind.DataSuit:
                    return who + " was hit by a claim over scraped training data.";
                case EventKind.UserDataChanged:
                    return who + " changed what it does with its users' data.";
                case EventKind.UserDataComplaint:
                    return who + " drew a privacy complaint over how it collects data.";
                case EventKind.UserDataDeleted:
                    return who + " deleted the users' data it had collected.";
                case EventKind.GovernanceChanged:
                    return who + " changed who controls the company.";
                case EventKind.DirectorSeated:
                    return who + " seated an independent director.";
                case EventKind.DirectorResigned:
                    return "An independent director resigned from " + who + ".";
                case EventKind.RoundBoughtBack:
                    return who + " bought its " + e.Ref + " investors out.";
                case EventKind.SpotPreempted:
                    return who + " lost part of " + e.Ref + " when spot capacity was taken back.";
                case EventKind.BoardVoted:
                    return "The board of " + who + (e.Pick == 1 ? " passed" : " refused") + " buying out the " + e.Ref + " investors.";
                case EventKind.AutonomySet:
                    return who + " changed how much freedom the agents of a model have.";
                case EventKind.AgentBlowup:
                    return "An agent run of " + e.Ref + " went wrong at " + who + " and customers were refunded $" + e.Amount + ".";
                case EventKind.AgentStreak:
                    return "The agents of " + e.Ref + " have run clean for a long while at " + who + ".";
                case EventKind.AnnotatorsContracted:
                    return who + " contracted an annotation vendor.";
                case EventKind.AnnotatorsEnded:
                    return who + " ended its annotation contract.";
                case EventKind.VendorAudited:
                    return who + " audited its annotation vendor.";
                case EventKind.VendorScandal:
                    return "A labour scandal about the annotators of " + who + " broke and cost $" + e.Amount + ".";
                case EventKind.PostTrainingStarted:
                    return who + " began tuning " + e.Ref + ".";
                case EventKind.PostTrainingDone:
                    return who + " finished tuning " + e.Ref + ".";
                case EventKind.PostTrainingAbandoned:
                    return who + " abandoned the tuning of " + e.Ref + ".";
                case EventKind.OwnerNamed:
                    return who + " named an owner for " + e.Ref + ".";
                case EventKind.OwnerLeft:
                    return "The owner of " + e.Ref + " left " + who + " and the gap is " + e.Amount + ".";
                case EventKind.DocsWritten:
                    return who + " wrote documentation for " + e.Ref + ".";
                case EventKind.DocsStale:
                    return "The documentation of " + e.Ref + " at " + who + " has gone stale.";
                case EventKind.MotionTabled:
                    return "The board of " + who + " tabled a motion.";
                case EventKind.MotionAnswered:
                    return who + " answered a board motion.";
                case EventKind.MotionIgnored:
                    return "A motion at the board of " + who + " went unanswered.";
                case EventKind.MotionJudged:
                    return e.Pick == 1 ? "The board of " + who + " saw a commitment kept." : "The board of " + who + " saw a commitment broken.";
                case EventKind.BoardForcedVote:
                    return "The board of " + who + " forced a vote on the founders.";
                case EventKind.DrillStarted:
                    return who + " started an incident drill.";
                case EventKind.DrillFinished:
                    return who + " finished its incident drill.";
                case EventKind.EvalRegistered:
                    return who + " put a model on a public eval schedule.";
                case EventKind.EvalPublished:
                    return who + " published a public eval score.";
                case EventKind.ToolPolicySet:
                    return who + " changed which tools the agents of a model may call.";
                case EventKind.HumanQueueDeep:
                    return who + " has a long human escalation queue.";
                case EventKind.HumanQueueCleared:
                    return who + " cleared its human escalation queue.";
                case EventKind.ChecklistSigned:
                    return who + " signed a release checklist item for a model.";
                case EventKind.SandboxStarted:
                    return who + " put a model in the agent sandbox for " + e.Amount + " weeks.";
                case EventKind.SandboxFinished:
                    return e.Pick == 1 ? who + " graduated a model from the agent sandbox with score " + e.Amount + "."
                        : who + " failed the agent sandbox with score " + e.Amount + ".";
                case EventKind.PlaybookSet:
                    return who + " set the runaway playbook to level " + e.Amount + ".";
                case EventKind.PlaybookArmed:
                    return who + " armed the runaway playbook for " + e.Amount + " weeks.";
                case EventKind.OrchBudgetSet:
                    return who + " set the orchestration budget to level " + e.Amount + ".";
                case EventKind.RouteShareSet:
                    return who + " set a route share for a model.";
                case EventKind.MetricsPublished:
                    return who + " published customer metrics for a model (score " + e.Amount + ").";
                case EventKind.MetricsStopped:
                    return who + " stopped publishing customer metrics for a model.";
                case EventKind.SpecialistBranched:
                    return who + " branched a model into a specialist line.";
                case EventKind.DataSourceAcquired:
                    return who + " acquired a data source.";
                case EventKind.DataSourceRenewed:
                    return who + " renewed a data source.";
                case EventKind.DataSourceLapsed:
                    return who + " let a data source lapse.";
                case EventKind.WeightsOpened:
                    return who + " released a model's weights.";
                case EventKind.WeightsClosed:
                    return who + " closed a model's weights.";
                case EventKind.WeightsCopied:
                    return who + " saw competitors copy open weights.";
                case EventKind.SectorPriceSet:
                    return who + " set a sector price tier.";
                case EventKind.CampaignDeadlineSet:
                    return who + " set a campaign deadline.";
                case EventKind.CampaignDeadlineHit:
                    return who + " reached its campaign deadline.";
                case EventKind.LawShaped:
                    return who + " shaped the law for next era.";
                case EventKind.ScenarioBaggagePicked:
                    return who + " started with scenario baggage.";
                case EventKind.MarginCall:
                    return who + " got a margin call on $" + e.Amount + " of chip-backed debt.";
                case EventKind.MarginCallSatisfied:
                    return who + " satisfied a margin call (" + e.Ref + ").";
                case EventKind.MarginCallDefaulted:
                    return who + " defaulted after a margin call.";
                case EventKind.HearingShareNudged:
                    return who + "'s hearing answer moved the market bubble permille.";
                case EventKind.ScenarioGoalMet:
                    return who + " met scenario goal \"" + e.Ref + "\".";
                case EventKind.ScenarioGoalMissed:
                    return who + " missed scenario goal \"" + e.Ref + "\".";
                case EventKind.LawTargetSet:
                    return who + " set law target \"" + e.Ref + "\" before lobbying.";
                case EventKind.ReservedComputeSigned:
                    return who + " signed a reserved compute block for " + e.Amount + " GPUs.";
                case EventKind.RivalIntentSurfaced:
                    return rivalName(c, state, e) + " tipped a rival move (" + e.Ref + ").";
                case EventKind.GpuTierLocked:
                    return who + " was refused " + e.Ref + " tier hardware (gate " + e.SubjectId + ").";
                case EventKind.ScenarioEndlessAck:
                    return who + " keeps playing after scenario \"" + e.Ref + "\" win.";
                case EventKind.SpecialistEraUnlocked:
                    return "Specialist and open-weights hooks unlock in the " + e.Ref + " era.";
                case EventKind.SectorPriceLadderPeak:
                    return who + " pegged premium pricing on \"" + e.Ref + "\" at tier " + e.SubjectId + ".";
                case EventKind.DataSourceSpecialistPaired:
                    return who + " paired data source \"" + e.Ref + "\" with a specialist line.";
                case EventKind.PublicMetricsEraBoost:
                    return who + " published metrics with an era-load trust boost of " + e.Amount + " on " + e.Ref + ".";
                case EventKind.StaffPromoted:
                    return who + " promoted a " + c.Role(e.Ref).Name.ToLowerInvariant() + " to senior for $" + e.Amount + ".";
                case EventKind.TargetSet:
                    return who + " told the board it would make $" + e.Amount + " in the next quarter (level " + e.SubjectId + ").";
                case EventKind.TargetSettled:
                    return e.Pick == 1 ? who + " hit its level " + e.SubjectId + " revenue target with $" + e.Amount + " and the board is pleased."
                        : who + " missed its level " + e.SubjectId + " revenue target, making $" + e.Amount + ", and the board is not pleased.";
                case EventKind.ChairEndowed:
                    return who + " endowed a university chair for $" + e.Amount + " and now holds " + e.SubjectId + (e.SubjectId == 1 ? " chair." : " chairs.");
                case EventKind.WhistleRaised:
                    return "A safety person at " + who + " says " + e.Ref + " shows " + e.Amount + " points more alignment than it has, and wants an answer.";
                case EventKind.WhistleAnswered:
                    return e.Pick == 0 ? who + " came clean about " + e.Ref + " and its shown alignment now matches the real one."
                        : who + " paid $" + e.Amount + " to keep the concern about " + e.Ref + " quiet.";
                case EventKind.WhistleLeaked:
                    return "The concern about " + e.Ref + " leaked and " + who + " lost " + e.Amount + " reputation" + (e.Pick == 1 ? "; a safety person resigned." : ".");
                case EventKind.BountySet:
                    return e.SubjectId == 0 ? who + " closed its bug bounty."
                        : who + (e.Pick == 1 ? " raised" : " lowered") + " its bug bounty to level " + e.SubjectId + (e.Amount > 0 ? " for $" + e.Amount + "." : ".");
                case EventKind.SectorPulledOut:
                    return who + " pulled out of " + e.Ref + (e.Amount > 0 ? ", paying $" + e.Amount + " to its customers" : "") + (e.Pick == 1 ? " and retired the model." : ".");
                case EventKind.LoanRestructured:
                    return who + " restructured loan " + e.SubjectId + ": a longer term and a lower payment for $" + e.Amount + " added to what it owes" + (e.Pick == 1 ? ", with its missed payments forgiven." : ".");
                case EventKind.GrantApplied:
                    return who + " applied for a tier " + e.SubjectId + " research grant for $" + e.Amount + ".";
                case EventKind.GrantDecided:
                    return e.Pick == 1 ? who + " was awarded a tier " + e.SubjectId + " research grant of $" + e.Amount + " and now owes the panel " + e.SubjectId + (e.SubjectId == 1 ? " paper." : " papers.")
                        : who + "'s tier " + e.SubjectId + " research grant application was declined.";
                case EventKind.GrantSettled:
                    return e.Pick == 1 ? who + " delivered the papers its tier " + e.SubjectId + " grant asked for." : who + " did not deliver the papers its tier " + e.SubjectId + " grant asked for and paid back $" + e.Amount + ".";
                case EventKind.SlaPledged:
                    return e.Pick == 1 ? who + " pledged a level " + e.SubjectId + " service level to its customers." : who + " backed its service-level pledge down to level " + e.SubjectId + ".";
                case EventKind.SlaBroken:
                    return who + " broke its level " + e.SubjectId + " service-level pledge " + (e.Pick == 1 ? "with an incident" : "by not keeping up with demand") + " and refunded $" + e.Amount + ".";
                case EventKind.PostMortemPublished:
                    return e.Pick == 1 ? who + " published a full post-mortem of " + e.Ref + " and told the public its real alignment. It won back " + e.Amount + " reputation."
                        : who + " published a summary of what went wrong with " + e.Ref + ". It won back " + e.Amount + " reputation.";
                case EventKind.ConsultantsHired:
                    return e.Pick == 1 ? who + " hired a level " + e.SubjectId + " firm of policy consultants for $" + e.Amount + "." : who + "'s policy consultants have gone home and left their invoices.";
                case EventKind.OnCallSet:
                    return e.Pick == 1 ? who + " put its team on an on-call rota (level " + e.SubjectId + ")." : who + " took its on-call rota down to level " + e.SubjectId + ".";
                case EventKind.OptimisationSet:
                    return e.Pick == 1 ? who + " raised inference optimisation to level " + e.SubjectId + " for $" + e.Amount + "." : who + " backed inference optimisation down to level " + e.SubjectId + ".";
                case EventKind.EthicsBoardChanged:
                    return e.Pick == 1 ? who + " seated an ethics board (level " + e.SubjectId + ") for $" + e.Amount + "." : who + " stepped its ethics board down to level " + e.SubjectId + ". The press noticed.";
                case EventKind.EthicsBoardResigned: return who + "'s ethics board resigned from level " + e.Amount + " to " + e.SubjectId + " because the safety team behind it was gone.";
                case EventKind.ChiefChanged:
                    return e.Pick == 1 ? who + " named a chief " + e.Ref + " officer for $" + e.Amount + "." : who + " stood down its chief " + e.Ref + " officer.";
                case EventKind.ChiefLeft: return who + " lost its chief " + e.Ref + " officer. The team lost " + e.Amount + " morale over it.";
                case EventKind.EquityGranted: return who + " gave the team equity (level " + e.SubjectId + "). The people now own " + (e.Amount / 10) + "." + (e.Amount % 10) + "% of the company.";
                case EventKind.StaffRetrained:
                    {
                        string[] roles = (e.Ref ?? "").Split('>');
                        RoleConfig from = roles.Length == 2 ? c.Role(roles[0]) : null;
                        RoleConfig to = roles.Length == 2 ? c.Role(roles[1]) : null;
                        return who + " retrained a " + (from == null ? "person" : from.Name) + " as a " + (to == null ? "new role" : to.Name) + " for $" + e.Amount + (e.Pick == 1 ? "." : ", and they are not thrilled.");
                    }
                case EventKind.AuditRevoked: return "The auditors pulled the certificate of " + e.Ref + " after its incident.";
                case EventKind.ModelFineTuned:
                    return who + " paid $" + e.Amount + " to fine-tune " + e.Ref + ". The glitching has stopped and it is a little more careful.";
                case EventKind.ModelRolledBack:
                    {
                        LabState backLab = state.Lab(e.LabId);
                        ModelState back = backLab == null ? null : backLab.Model(e.SubjectId);
                        return who + " pulled " + e.Ref + " and put " + (back == null ? "the older version" : back.Name) + " back on sale. Reputation -" + e.Amount + ".";
                    }
                case EventKind.LeadFound:
                    return who + "'s " + e.Ref + " came out " + e.Amount / 10 + "." + e.Amount % 10 + "% better than anyone aimed for. Nobody knows why. It is a research lead.";
                case EventKind.LeadResolved:
                    {
                        LeadOutcomeConfig found = c.Leads.Outcome(e.Ref);
                        if (found == null) return who + " looked into a lucky run and found nothing.";
                        string gain = found.Kind == LeadEffectKind.ResearchWeeks ? " (+" + e.Amount + " research points)"
                            : found.Kind == LeadEffectKind.TrainingSpeedPermille ? " (training +" + e.Amount / 10 + "." + e.Amount % 10 + "%)" : "";
                        return who + " looked into the lucky run: " + found.Name + gain + ". " + found.Blurb;
                    }
                case EventKind.LeadWentCold:
                    return "Nobody at " + who + " looked into why " + e.Ref + " came out so well. The lead has gone cold.";
                case EventKind.ControlStale:
                    {
                        TechConfig newest = c.Tech(e.Ref);
                        return who + "'s safety techniques are going stale: its newest is " + (newest == null ? e.Ref : newest.Name) + ", and they now count for " + (1000 - e.Amount) / 10 + "% of what they did. New safety research would help.";
                    }
                case EventKind.ControlRefreshed:
                    {
                        TechConfig fresh = c.Tech(e.Ref);
                        return who + " did new safety research (" + (fresh == null ? e.Ref : fresh.Name) + ") and won back " + e.Amount / 10 + "% of its techniques' strength.";
                    }
                case EventKind.RoomRefurbished:
                    {
                        RoomTypeConfig done = c.RoomType(e.Ref);
                        return who + " refurbished its " + (done == null ? e.Ref : done.Name) + " for $" + e.Amount + ".";
                    }
                case EventKind.GauntletSafest: return who + " is named the safest lab of the year.";
                case EventKind.GrantEnded: return who + " got the last cheque of its grant.";
                case EventKind.InDebt: return who + " is in debt ($" + e.Amount + ").";
                case EventKind.Bankrupt: return who + " went bankrupt.";
                case EventKind.EraBegan: return "A new era begins: " + c.Era(e.Ref).Name + ". " + c.Era(e.Ref).Tagline + EraShifts(c, e) + NewRisks(c, e);
                case EventKind.WorldNews:
                {
                    WorldEventConfig news = c.WorldEvent(e.Ref);
                    if (news != null) return "NEWS: " + news.Headline;
                    if (e.Ref == "bubble-crash")
                        return "NEWS: Funding dries up and AI valuations slide across the market.";
                    return "NEWS: " + e.Ref + ".";
                }
                case EventKind.LandBought: return who + " bought land at " + c.World.Site(e.Ref).Name + " for $" + e.Amount + ".";
                case EventKind.LandSold: return who + " sold its land at " + c.World.Site(e.Ref).Name + " for $" + e.Amount + ".";
                case EventKind.ColoLeased: return who + " now leases " + e.Amount + " kW at " + c.World.Site(e.Ref).Name + ".";
                case EventKind.ColoEnded: return who + " gave up its lease at " + c.World.Site(e.Ref).Name + ".";
                case EventKind.HallStarted: return who + " started a " + e.Amount + " kW data centre at " + c.World.Site(e.Ref).Name + ".";
                case EventKind.HallBuilt: return who + "'s " + e.Amount + " kW data centre at " + c.World.Site(e.Ref).Name + " is open.";
                case EventKind.TownVoteDelayed: return "The town voted on " + who + "'s data centre at " + c.World.Site(e.Ref).Name + ". The debate was about the noise. " + e.Amount + " weeks lost.";
                case EventKind.PowerDealStarted: return who + " bought " + c.World.PowerDeal(e.Ref).Name.ToLowerInvariant() + " for $" + e.Amount + ".";
                case EventKind.PowerDealReady: return who + "'s " + c.World.PowerDeal(e.Ref).Name.ToLowerInvariant() + " now delivers " + e.Amount + " kW.";
                case EventKind.HardwareMoved: return who + " moved " + e.Amount + " units of " + c.Compute.Generation(e.Ref).Name + ".";
                case EventKind.Heatwave: return "A heatwave throttles " + who + "'s data centre at " + c.World.Site(e.Ref).Name + " for " + e.Amount + " weeks. During launch week, naturally.";
                case EventKind.AttentionRose: return AttentionLine(c, e, who);
                case EventKind.AttentionFell: return who + " is under less scrutiny from " + c.World.Bloc(e.Ref).Agency + ".";
                case EventKind.Keynote: return c.Compute.VendorName + " unveils the " + c.Compute.Generation(e.Ref).Name + ". It was sold out before the presenter walked on stage. This month's supply: " + e.Amount + " units.";
                case EventKind.RivalJoined: return who + " has entered the race with $" + e.Amount + " and a plan.";
                case EventKind.BoardDrama: return who + "'s board fired its boss on Friday. The boss was back on Tuesday.";
                case EventKind.StaffPoached:
                    {
                        LabState victim = state.Lab(e.SubjectId);
                        RoleConfig role = c.Role(e.Ref);
                        return who + " poached a " + (victim == null ? "rival" : victim.Name) + " star " + (role == null ? e.Ref : role.Name) + ".";
                    }
                case EventKind.PoachOffered:
                    {
                        LabState victim = state.Lab(e.SubjectId);
                        RoleConfig role = c.Role(e.Ref);
                        return who + " offered one of " + (victim == null ? "your" : victim.Name + "'s") + " " + (role == null ? e.Ref : role.Name) + "s $" + e.Amount + " to leave. They have a few weeks to think about it.";
                    }
                case EventKind.PoachCountered:
                    {
                        CounterConfig counter = c.Talent.Counter(e.Ref);
                        string how = counter == null ? e.Ref : counter.Name;
                        return e.Pick == 1
                            ? who + " kept someone with a counter: " + how + " ($" + e.Amount + "). The offer was declined with a long email."
                            : who + " tried a counter (" + how + ", $" + e.Amount + ") and the person left anyway.";
                    }
                case EventKind.PoachBackTried:
                    {
                        LabState target = state.Lab(e.SubjectId);
                        RoleConfig role = c.Role(e.Ref);
                        string what = role == null ? e.Ref : role.Name;
                        return e.Pick == 1
                            ? who + " hired a " + what + " away from " + (target == null ? "a rival" : target.Name) + " for $" + e.Amount + ". The announcement was posted from the stage."
                            : who + " tried to hire a " + what + " away from " + (target == null ? "a rival" : target.Name) + " and only paid the headhunter ($" + e.Amount + ").";
                    }
                case EventKind.PressInquiry:
                    {
                        string[] bits = e.Ref.Split('|');
                        BeatConfig beat = c.Press.Beat(bits[0]);
                        JournalistConfig journalist = bits.Length > 1 ? c.Press.Journalist(bits[1]) : null;
                        string what = bits.Length > 2 && bits[2].Length > 0 ? " about " + bits[2] : "";
                        return (journalist == null ? "A journalist" : journalist.Name + " of " + journalist.Outlet) + " asked " + who + " for a line" + what + " (" + (beat == null ? bits[0] : beat.Name.ToLowerInvariant()) + ", " + e.Amount + " reputation at stake).";
                    }
                case EventKind.PressAnswered:
                    {
                        string[] bits = e.Ref.Split('|');
                        PressResponseConfig response = c.Press.Response(bits[0]);
                        string how = response == null ? bits[0] : response.Name.ToLowerInvariant();
                        return e.Pick == 1
                            ? who + " answered the press (" + how + ") and it landed: reputation " + (e.Amount >= 0 ? "+" : "") + e.Amount + "."
                            : who + " answered the press (" + how + ") and it backfired: reputation " + e.Amount + ".";
                    }
                case EventKind.StoryLeaked:
                    {
                        LabState target = state.Lab(e.SubjectId);
                        string rival = target == null ? "a rival" : target.Name;
                        string about = string.IsNullOrEmpty(e.Ref) ? "its problems" : "the trouble with " + e.Ref;
                        return e.Pick == 1
                            ? who + " leaked " + about + " at " + rival + ". It ran on the front page: " + rival + " lost " + e.Amount + " reputation."
                            : who + " tried to leak " + about + " at " + rival + " and was traced: " + who + " lost " + e.Amount + " reputation.";
                    }
                case EventKind.LaunchBuzz: return who + "'s launch of " + e.Ref + " drew a crowd to the press room: reputation +" + e.Amount + ".";
                case EventKind.GpuQueueStalled:
                    return who + "'s queued GPU purchase (" + e.Amount + " units) stalled: " + e.Ref + ".";
                case EventKind.FastBuild: return who + " built a data centre at " + c.World.Site(e.Ref).Name + " over a weekend. Mostly.";
                case EventKind.ChipTapedOut: return who + " sent " + e.Ref + " to the foundry for $" + e.Amount + ". Now everybody waits.";
                case EventKind.ChipRespin: return "A design bug was found at tape-out. Respin. " + who + " pays $" + e.Amount + " to try " + e.Ref + " again.";
                case EventKind.ChipReady:
                    return e.Pick == 1
                        ? who + ": Your first in-house chip works. It runs Minesweeper at 4,000 frames per second."
                        : who + "'s " + e.Ref + " works. Yield " + DecisionText.Percent((int)e.Amount) + "%.";
                case EventKind.ChipsDelivered:
                    if (e.Ref == "cloud") return who + " took delivery of " + e.Amount + " chips on a cloud deal.";
                    { SiteConfig site = c.World.Site(e.Ref); return who + " took delivery of " + e.Amount + " chips at " + (site == null ? e.Ref : site.Name) + "."; }
                case EventKind.ChipYieldCrash: return "Yield crash. " + who + "'s delivery of " + e.Ref + " came in at " + e.Amount + " working chips. The money is gone.";
                case EventKind.ChipsBought:
                    { LabState seller = state.Lab((int)e.SubjectId); return who + " bought " + e.Amount + " " + e.Ref + " chips from " + (seller == null ? "another lab" : seller.Name) + "."; }
                case EventKind.EndingReached:
                    {
                        EndingDefConfig def = c.Endings.Def(e.Ref);
                        return who + " reached an ending: " + (def == null ? e.Ref : def.Name) + ".";
                    }
                case EventKind.ShadyOpStarted: return who + " started " + c.Shady.Card(e.Ref).Name + " (" + e.Amount + " weeks).";
                case EventKind.ShadyOpResolved: return who + "'s shady job " + (e.Amount == 1 ? "succeeded" : "failed") + ".";
                case EventKind.ShadyTraced: return who + " was traced running " + c.Shady.Card(e.Ref).Name + ".";
                case EventKind.ShadyInvestigation: return who + " is under investigation.";
                case EventKind.ShadyConvicted: return who + " was convicted.";
                case EventKind.ShadyAcquitted: return who + " was acquitted.";
                case EventKind.ShadySideJob: return e.Ref + " did a little job on the side. It left you a tip.";
                case EventKind.ShadyTemptation: return "Runway is short. " + c.Shady.Card(e.Ref).Name + " is on the table.";
                case EventKind.FundingClosed:
                    return downRound(e)
                        ? who + " closed " + e.Ref + " on down-round terms ($" + e.Amount + ")."
                        : who + " closed " + e.Ref + " ($" + e.Amount + ").";
                case EventKind.InvestorBoardVisit:
                    return who + "'s investors held a board update. Reputation +" + e.Amount + ".";
                case EventKind.LabAcquired:
                    {
                        LabState bought = state.Lab(e.SubjectId);
                        return who + " acquired " + (bought == null ? "a rival lab" : bought.Name) + " for $" + e.Amount + ".";
                    }
                case EventKind.ModelSold:
                    return who + " sold " + e.Ref + " for $" + e.Amount + " and left those sectors.";
                default: return who + ": " + e.Kind + ".";
            }
        }

        static string AttentionLine(GameConfig c, GameEvent e, string who)
        {
            BlocConfig bloc = c.World.Bloc(e.Ref);
            switch (e.SubjectId)
            {
                case BlocStanding.Letter: return bloc.Agency + " has sent " + who + " a strongly worded letter.";
                case BlocStanding.Hearing:
                    return who + " was called to a hearing by " + bloc.Agency + (e.Amount >= 0 ? " and came prepared." : " and was not ready for it.");
                case BlocStanding.Audit: return bloc.Agency + " audited " + who + ". Fine: $" + e.Amount + ".";
                default: return bloc.Agency + " froze " + who + "'s permits in " + bloc.Name + ".";
            }
        }

        static bool downRound(GameEvent e) { return e.Pick == 1; }

        static string WithArticle(string name)
        {
            bool vowel = name.Length > 0 && "AEIOUaeiou".IndexOf(name[0]) >= 0;
            return (vowel ? "an " : "a ") + name;
        }

        static string rivalName(GameConfig c, GameState state, GameEvent e)
        {
            LabState lab = state.Lab(e.LabId);
            return lab == null ? "A rival" : lab.Name;
        }
    }
}
