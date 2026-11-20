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
                case EventKind.TechResearched: return who + " researched " + c.Tech(e.Ref).Name + ".";
                case EventKind.TrainingStarted: return who + " started training " + e.Ref + ".";
                case EventKind.TrainingFinished: return who + " finished training " + e.Ref + " (capability " + e.Amount + ").";
                case EventKind.ModelReleased: return who + " released " + e.Ref + " into " + e.Amount + (e.Amount == 1 ? " sector." : " sectors.");
                case EventKind.ModelRetired: return who + " retired " + e.Ref + ".";
                case EventKind.Incident: return "INCIDENT at " + who + ": " + EraSystem.AtTick(c, e.Tick).IncidentHeadlines[e.Pick].Replace("{model}", e.Ref) + " Fine: $" + e.Amount + ".";
                case EventKind.GauntletCrown: return e.Ref + " by " + who + " takes the crown at " + c.Gauntlet.Name + ".";
                case EventKind.GauntletBracket: return who + " wins the " + c.Sector(e.Ref).Name + " bracket at " + c.Gauntlet.Name + ".";
                case EventKind.GauntletSafest: return who + " is named the safest lab of the year.";
                case EventKind.GrantEnded: return who + " got the last cheque of its grant.";
                case EventKind.InDebt: return who + " is in debt ($" + e.Amount + ").";
                case EventKind.Bankrupt: return who + " went bankrupt.";
                case EventKind.EraBegan: return "A new era begins: " + c.Era(e.Ref).Name + ". " + c.Era(e.Ref).Tagline;
                case EventKind.WorldNews: return "NEWS: " + c.WorldEvent(e.Ref).Headline;
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

        static string WithArticle(string name)
        {
            bool vowel = name.Length > 0 && "AEIOUaeiou".IndexOf(name[0]) >= 0;
            return (vowel ? "an " : "a ") + name;
        }
    }
}
