using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
    /// <summary>
    /// Customer support desk. The player can put engineers on a support desk (never the last one). Every released model brings tickets in proportion to what it earns; the desk clears a set number a person a week, and what it does not clear piles up as a backlog. A backlog past a limit costs trust in the lab's sectors every week until the desk catches up. Only the player's lab. Zero in <c>Enabled</c> turns it off.
    /// </summary>
    public sealed class SupportConfig : ISync
    {
        public int Enabled;
        /// <summary>Most engineers on the desk.</summary>
        public int MaxPeople;
        /// <summary>Engineers that must stay off it.</summary>
        public int KeepFree;
        /// <summary>Dollars of weekly revenue that bring one ticket a week.</summary>
        public int RevenuePerTicket;
        /// <summary>Tickets the sales team clears in a week with no desk at all.</summary>
        public int BaseClear;
        /// <summary>Tickets a person on the desk clears in a week.</summary>
        public int PerPerson;
        /// <summary>Backlog above which trust suffers.</summary>
        public int BacklogLimit;
        /// <summary>Backlog never grows past this.</summary>
        public int MaxBacklog;
        /// <summary>Trust lost each week in each sector for each ticket over the limit, in permille.</summary>
        public int TrustPerTicket;
        /// <summary>Most trust lost in a sector in a week, in permille.</summary>
        public int MaxWeeklyTrust;

        public void Sync(Syncer s)
        {
            s.Int("enabled", ref Enabled);
            s.Int("maxPeople", ref MaxPeople);
            s.Int("keepFree", ref KeepFree);
            s.Int("revenuePerTicket", ref RevenuePerTicket);
            s.Int("baseClear", ref BaseClear);
            s.Int("perPerson", ref PerPerson);
            s.Int("backlogLimit", ref BacklogLimit);
            s.Int("maxBacklog", ref MaxBacklog);
            s.Int("trustPerTicket", ref TrustPerTicket);
            s.Int("maxWeeklyTrust", ref MaxWeeklyTrust);
        }
    }
}
