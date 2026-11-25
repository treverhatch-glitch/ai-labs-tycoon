using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>The lab's stated values and whether it lives up to them. See <see cref="CreedConfig"/>. Only the player's lab.</summary>
    public static class Creed
    {
        public const int Safety = 1;
        public const int Speed = 2;
        public const int Openness = 3;
        public const int People = 4;
        public const int Profit = 5;

        public static bool Enabled(GameConfig c) { return c.Creed.Enabled > 0; }

        public static bool Has(LabState lab) { return lab.CreedA > 0; }

        /// <summary>1 if the act is in keeping with the value, -1 if it is against it, 0 if it has nothing to do with it.</summary>
        public static int Score(int value, EventKind kind)
        {
            switch (value)
            {
                case Safety:
                    if (kind == EventKind.RedTeamRan || kind == EventKind.PostMortemPublished || kind == EventKind.BountySet || kind == EventKind.AuditDone) return 1;
                    if (kind == EventKind.ModelRushed || kind == EventKind.WhistleLeaked || kind == EventKind.DeviceRecalled) return -1;
                    return 0;
                case Speed:
                    if (kind == EventKind.ModelRushed || kind == EventKind.HackWeek || kind == EventKind.SkunkworksStarted || kind == EventKind.FastBuild || kind == EventKind.DeviceDesigned) return 1;
                    if (kind == EventKind.RedTeamRan || kind == EventKind.PostMortemPublished) return -1;
                    return 0;
                case Openness:
                    if (kind == EventKind.PaperPublished || kind == EventKind.PostMortemPublished || kind == EventKind.ScholarsArrived || kind == EventKind.CodeReleased) return 1;
                    if (kind == EventKind.ContractAccepted || kind == EventKind.ModelSold || kind == EventKind.CodeWithheld) return -1;
                    return 0;
                case People:
                    if (kind == EventKind.StaffPromoted || kind == EventKind.SabbaticalStarted || kind == EventKind.MentoringStarted || kind == EventKind.InternshipStarted || kind == EventKind.EquityGranted || kind == EventKind.LabourRecognised) return 1;
                    if (kind == EventKind.LayoffRound || kind == EventKind.LabourResisted) return -1;
                    return 0;
                case Profit:
                    if (kind == EventKind.FollowOnSold || kind == EventKind.RevenueAdvanceTaken || kind == EventKind.PlatformSigned || kind == EventKind.PlatformRenewed || kind == EventKind.ContractAccepted || kind == EventKind.FundingClosed) return 1;
                    if (kind == EventKind.FoundationEndowed || kind == EventKind.ChairEndowed) return -1;
                    return 0;
                default:
                    return 0;
            }
        }

        /// <summary>What a backlash costs this lab, in permille of the hit: less for a lab held up as an example, more for one whose creed is a joke.</summary>
        public static int Exposure(GameConfig c, LabState lab)
        {
            if (!Enabled(c) || !Has(lab)) return MathI.Permille;
            return lab.CreedBand == 1 ? c.Creed.ShieldPermille : lab.CreedBand == 2 ? c.Creed.ExposePermille : MathI.Permille;
        }

        static void Shift(GameConfig c, LabState lab, int delta)
        {
            for (int i = 0; i < lab.Staff.Count; i++) lab.Staff[i].Morale = (int)MathI.Clamp((long)lab.Staff[i].Morale + delta, 0, MathI.Permille);
        }

        public static void Set(GameConfig c, GameState state, LabState lab, int first, int second)
        {
            bool fresh = !Has(lab);
            lab.CreedA = first;
            lab.CreedB = second;
            lab.CreedIntegrity = fresh ? c.Creed.StartIntegrity : c.Creed.FlipIntegrity;
            lab.CreedBand = 0;
            lab.CreedLock = c.Creed.FlipLockWeeks;
            if (fresh)
            {
                lab.CreedTown = 0;
                lab.CreedKept = 0;
                lab.CreedBroken = 0;
            }
            Retune(c, lab);
            Events.Add(state, EventKind.CreedSet, lab.Id, first * 10 + second, lab.CreedIntegrity, "");
        }

        static void Retune(GameConfig c, LabState lab)
        {
            if (lab.CreedBand == 1 && lab.CreedIntegrity < c.Creed.HighBar - c.Creed.Gap) lab.CreedBand = 0;
            if (lab.CreedBand == 2 && lab.CreedIntegrity > c.Creed.LowBar + c.Creed.Gap) lab.CreedBand = 0;
        }

        public static void TownHall(GameConfig c, GameState state, LabState lab)
        {
            Finance.Spend(lab, LedgerLine.Hiring, c.Creed.TownCost);
            lab.CreedIntegrity = (int)MathI.Min(MathI.Permille, lab.CreedIntegrity + c.Creed.TownGain);
            lab.CreedTown = c.Creed.TownCooldown;
            Events.Add(state, EventKind.TownHallHeld, lab.Id, 0, lab.CreedIntegrity, "");
        }

        public static void Own(GameConfig c, GameState state, LabState lab)
        {
            PressSystem.AddReputation(lab, -c.Creed.OwnUpReputation);
            lab.CreedIntegrity = (int)MathI.Max(lab.CreedIntegrity, c.Creed.OwnUpRestore);
            lab.CreedTown = c.Creed.TownCooldown;
            lab.CreedBand = 0;
            Retune(c, lab);
            Events.Add(state, EventKind.CreedOwnedUp, lab.Id, 0, c.Creed.OwnUpReputation, "");
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId || !Has(lab)) return;
            if (lab.CreedLock > 0) lab.CreedLock--;
            if (lab.CreedTown > 0) lab.CreedTown--;
            long integrity = lab.CreedIntegrity;
            int lastTick = state.Tick - 1;
            for (int i = state.Events.Count - 1; i >= 0; i--)
            {
                GameEvent e = state.Events[i];
                if (e.Tick < lastTick) break;
                if (e.Tick != lastTick || e.LabId != lab.Id) continue;
                int kept = (Score(lab.CreedA, e.Kind) > 0 ? 1 : 0) + (Score(lab.CreedB, e.Kind) > 0 ? 1 : 0);
                int broke = (Score(lab.CreedA, e.Kind) < 0 ? 1 : 0) + (Score(lab.CreedB, e.Kind) < 0 ? 1 : 0);
                integrity += kept * (long)c.Creed.KeptPoints - broke * (long)c.Creed.BrokenPoints;
                lab.CreedKept += kept;
                lab.CreedBroken += broke;
            }
            lab.CreedIntegrity = (int)MathI.Clamp(integrity, 0, MathI.Permille);
            Retune(c, lab);
            if (lab.CreedBand != 2 && lab.CreedIntegrity <= c.Creed.LowBar)
            {
                lab.CreedBand = 2;
                int loss = PressSystem.Damp(c, lab, c.Creed.LowReputation);
                PressSystem.AddReputation(lab, -loss);
                Shift(c, lab, -c.Creed.LowMorale);
                Events.Add(state, EventKind.CreedHollow, lab.Id, 0, loss, "");
            }
            else if (lab.CreedBand == 0 && lab.CreedIntegrity >= c.Creed.HighBar)
            {
                lab.CreedBand = 1;
                PressSystem.AddReputation(lab, c.Creed.HighReputation);
                Shift(c, lab, c.Creed.HighMorale);
                Events.Add(state, EventKind.CreedProud, lab.Id, 0, c.Creed.HighReputation, "");
            }
        }
    }
}
