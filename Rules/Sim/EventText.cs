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
    }
}
