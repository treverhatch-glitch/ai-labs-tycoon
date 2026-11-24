using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>Read-only decompositions of numbers the UI shows. Each part sums to the total.</summary>
    public static class DisplayQueries
    {
        public struct QualityBreakdown
        {
            public long FromCapability;
            public long FromAlignment;
            public long SandbagBoost;
            public long GlitchAdjustment;

            public long Total { get { return FromCapability + FromAlignment + SandbagBoost + GlitchAdjustment; } }
        }

        public struct AppealBreakdown
        {
            public long Core;
            public long ReputationBoost;
            public long SalesBoost;
            public long RoleBoost;
            public long TrustBoost;
            public long ChampionBoost;
            /// <summary>Zero or negative: customers who cannot tell the lab's models apart.</summary>
            public long NamingConfusion;
            /// <summary>Positive for a model trained to be engaging, negative for one trained to be careful; zero for the default.</summary>
            public long ObjectiveEffect;
            /// <summary>Positive while the launch window of a rushed release is open; zero otherwise.</summary>
            public long LaunchEffect;
            /// <summary>Zero or negative: customers still confused by the lab's last change of style.</summary>
            public long PivotConfusion;
            /// <summary>Positive while a marketing campaign runs in the sector, negative in the hangover after it; zero otherwise.</summary>
            public long CampaignEffect;
            /// <summary>Positive while an account team works the sector; zero otherwise.</summary>
            public long AccountEffect;

            public long Total { get { return Core + ReputationBoost + SalesBoost + RoleBoost + TrustBoost + ChampionBoost + NamingConfusion + ObjectiveEffect + LaunchEffect + PivotConfusion + CampaignEffect + AccountEffect; } }
        }

        public struct IncidentBreakdown
        {
            public int FromConfig;
            public int FromAlignment;
            public int FromCorruption;
            /// <summary>Extra chance from skipping testing before launch; zero for a tested launch.</summary>
            public int FromRush;
            /// <summary>Extra chance while the lab cannot serve its customers; zero from the model-only query.</summary>
            public int FromOverload;
            public int FromCrunch;
            /// <summary>What the lab's technical debt adds to the weekly chance; zero for a lab with none.</summary>
            public int FromDebt;
            /// <summary>What inference optimisation adds to the weekly chance (the models get brittle); zero when the lab does none.</summary>
            public int FromOptimisation;
            /// <summary>What a red team took off while its weeks last; zero or negative.</summary>
            public int FromRedTeam;
            /// <summary>What a sitting chief safety officer takes off the weekly chance: zero or negative.</summary>
            public int FromChief;
            /// <summary>What the lab's bug bounty takes off the weekly chance: zero or negative.</summary>
            public int FromBounty;

            public int Total { get { return FromConfig + FromAlignment + FromCorruption + FromRush + FromOverload + FromCrunch + FromDebt + FromOptimisation + FromRedTeam + FromChief + FromBounty; } }
        }

        public struct ExitPriceBreakdown
        {
            public long RevenueYears;
            public long FleetResale;
            public long CashOnHand;
            /// <summary>What the people own of the price (taken off the total). Zero until the team has equity.</summary>
            public long StaffShare;
            public long Total;
        }

        public struct BubbleMeterBreakdown
        {
            public int Permille;
            public int GraceMonths;
            public int GraceMonthsNeeded;
            public int CrashMonthsRemaining;
            public long CapitalRaised;
            public long MarketRevenueLastMonth;
            /// <summary>0 calm, 1 warm, 2 hot, 3 crash.</summary>
            public int WarningLevel;
        }

        public struct RevenueBreakdown
        {
            public List<RevenuePart> Parts;
            public long Total;
        }

        public struct RevenuePart
        {
            public string SectorId;
            public long Amount;
        }

        public static QualityBreakdown QualityParts(GameConfig c, SectorConfig sector, ModelState model, int tick)
        {
            int effective = SafetySystem.EffectiveAlignment(c, model);
            int displayed = SafetySystem.DisplayedAlignment(model);
            int sandbagGap = (int)MathI.Max(0, displayed - model.Alignment);
            var b = new QualityBreakdown();
            b.FromCapability = MathI.MulPermille(model.Capability, sector.CapabilityWeightPermille);
            long effectiveAlign = MathI.MulPermille(model.Capability, MathI.MulPermille(sector.SafetyWeightPermille, effective));
            long displayBoost = MathI.MulPermille(model.Capability, MathI.MulPermille(sector.SafetyWeightPermille, sandbagGap));
            b.FromAlignment = effectiveAlign;
            b.SandbagBoost = displayBoost;
            if (SectorFlavours.IsGlitching(c, model, sector.Id, tick))
            {
                long before = b.FromCapability + effectiveAlign + displayBoost;
                long after = MathI.MulPermille(before, SectorFlavours.GlitchQualityPermille(c, sector.Id));
                b.GlitchAdjustment = after - before;
            }
            return b;
        }

        public static long Quality(GameConfig c, SectorConfig sector, ModelState model, int tick)
        {
            return QualityParts(c, sector, model, tick).Total;
        }

        public static AppealBreakdown AppealParts(GameConfig c, GameState state, SectorConfig sector, SectorState sectorState, LabState lab, ModelState model)
        {
            long quality = Quality(c, sector, model, state.Tick);
            var b = new AppealBreakdown();
            b.Core = quality * quality;
            long afterRep = MathI.MulPermille(b.Core, c.Safety.ReputationBaseline + lab.Reputation);
            b.ReputationBoost = afterRep - b.Core;
            long afterSales = MathI.MulPermille(afterRep, MarketSystem.SalesPermille(c, lab));
            b.SalesBoost = afterSales - afterRep;
            long afterRole = MathI.MulPermille(afterSales, (int)MathI.Max(1, MathI.Permille + RolePerks.SectorAppealPermille(c, lab, sector.Id) + Founding.AppealPermille(c, lab, sector.Id)));
            b.RoleBoost = afterRole - afterSales;
            long afterTrust = MathI.MulPermille(afterRole, TrustSystem.AppealPermille(c, lab, sector.Id));
            b.TrustBoost = afterTrust - afterRole;
            long afterNames = MathI.MulPermille(afterTrust, VersionNaming.AppealPermille(c, lab));
            b.NamingConfusion = afterNames - afterTrust;
            if (sectorState.ChampionLabId == lab.Id)
            {
                long afterCrown = MathI.MulPermille(afterNames, MathI.Permille + c.Gauntlet.BracketWinnerAppealPermille);
                b.ChampionBoost = afterCrown - afterNames;
            }
            b.ObjectiveEffect = MathI.MulPermille(b.Core + b.ReputationBoost + b.SalesBoost + b.RoleBoost + b.TrustBoost + b.NamingConfusion + b.ChampionBoost, TrainingObjectives.AppealPermille(c, model)) - (b.Core + b.ReputationBoost + b.SalesBoost + b.RoleBoost + b.TrustBoost + b.NamingConfusion + b.ChampionBoost);
            long beforeLaunch = b.Core + b.ReputationBoost + b.SalesBoost + b.RoleBoost + b.TrustBoost + b.NamingConfusion + b.ChampionBoost + b.ObjectiveEffect;
            b.LaunchEffect = MathI.MulPermille(beforeLaunch, RushedLaunch.LaunchAppealPermille(c, state.Tick, model)) - beforeLaunch;
            long beforePivot = beforeLaunch + b.LaunchEffect;
            b.PivotConfusion = MathI.MulPermille(beforePivot, Pivot.AppealPermille(c, state, lab)) - beforePivot;
            long beforeCampaign = beforePivot + b.PivotConfusion;
            b.CampaignEffect = MathI.MulPermille(beforeCampaign, MarketingCampaign.AppealPermille(c, state, lab, sector.Id)) - beforeCampaign;
            long beforeAccount = beforeCampaign + b.CampaignEffect;
            b.AccountEffect = MathI.MulPermille(beforeAccount, MathI.Permille + AccountTeams.AppealPermille(c, state, lab, sector.Id)) - beforeAccount;
            return b;
        }

        public static long Appeal(GameConfig c, GameState state, SectorConfig sector, SectorState sectorState, LabState lab, ModelState model)
        {
            return AppealParts(c, state, sector, sectorState, lab, model).Total;
        }

        public static IncidentBreakdown IncidentParts(GameConfig c, int tick, ModelState model)
        {
            int eraLoad = EraSystem.AtTick(c, tick).IncidentLoadPermille;
            if (eraLoad < 1) eraLoad = MathI.Permille;
            int scaledConfig = (int)MathI.MulPermille(c.Safety.IncidentChancePermille, eraLoad);
            var b = new IncidentBreakdown();
            b.FromConfig = scaledConfig;
            long baseChance = MathI.MulPermille(scaledConfig, MathI.Permille - model.Alignment);
            b.FromAlignment = (int)(baseChance - scaledConfig);
            int withoutRush = SafetySystem.IncidentChanceWithoutRush(c, tick, model);
            b.FromCorruption = withoutRush - (int)baseChance;
            int withRush = (int)MathI.Min(MathI.Permille, withoutRush + RushedLaunch.IncidentPermille(c, tick, model));
            b.FromRush = withRush - withoutRush;
            b.FromRedTeam = RedTeam.IncidentPermille(c, tick, model, withRush);
            return b;
        }

        /// <summary>The same breakdown for a model in a lab, with what overload adds.</summary>
        public static IncidentBreakdown IncidentParts(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            IncidentBreakdown b = IncidentParts(c, state.Tick, model);
            b.FromOverload = SafetySystem.OverloadPermille(c, state, lab);
            b.FromCrunch = Crunch.IncidentPermille(c, lab);
            b.FromDebt = TechDebt.IncidentPermille(c, lab);
            b.FromOptimisation = Optimisation.IncidentPermille(c, lab);
            long uncut = b.Total;
            b.FromChief = Chiefs.IncidentChange(c, lab, uncut);
            b.FromBounty = BugBounty.IncidentChange(c, lab, uncut);
            return b;
        }

        public static AcquisitionSystem.AcquisitionQuote AcquisitionQuoteParts(GameConfig c, GameState state, LabState buyer, LabState target)
        {
            return AcquisitionSystem.Quote(c, state, buyer, target);
        }

        public static ModelSalesSystem.ModelSaleQuote ModelSaleParts(GameConfig c, GameState state, LabState lab, ModelState model)
        {
            return ModelSalesSystem.Quote(c, state, lab, model);
        }

        public static BubbleMeterBreakdown BubbleMeterParts(GameConfig c, GameState state)
        {
            BubbleConfig b = c.Bubble;
            var p = new BubbleMeterBreakdown();
            p.Permille = state.BubblePermille;
            p.GraceMonths = state.BubbleGraceMonths;
            p.CrashMonthsRemaining = state.BubbleCrashMonthsRemaining;
            p.CapitalRaised = state.BubbleCapitalRaised;
            p.MarketRevenueLastMonth = BubbleSystem.MarketRevenueLastClosedMonth(state);
            p.GraceMonthsNeeded = b == null ? 0 : b.GraceMonthsBeforeCrash;
            int popWarn = b == null ? 50 : b.PopWarningMarginPermille;
            if (popWarn < 1) popWarn = 50;
            if (BubbleSystem.InCrash(state)) p.WarningLevel = 3;
            else if (b != null && state.BubblePermille >= b.PopAtPermille - popWarn) p.WarningLevel = 2;
            else if (b != null && state.BubblePermille >= b.HotPermille) p.WarningLevel = 1;
            return p;
        }

        public static ExitPriceBreakdown ExitPriceParts(GameConfig c, GameState state, LabState lab)
        {
            var b = new ExitPriceBreakdown();
            b.RevenueYears = EndingSystem.RevenueLast12Months(lab) * c.Endings.ExitRevenueYears;
            b.FleetResale = MathI.MulPermille(LabQueries.FleetResaleValue(c, state, lab), c.Endings.ExitFleetPermille);
            b.CashOnHand = lab.Cash;
            long gross = b.RevenueYears + b.FleetResale + b.CashOnHand;
            b.StaffShare = StaffEquity.StaffShare(lab, gross);
            b.Total = gross - b.StaffShare;
            return b;
        }

        public static RevenueBreakdown WeeklyRevenueParts(GameState state, LabState lab)
        {
            var b = new RevenueBreakdown();
            b.Parts = new List<RevenuePart>();
            for (int s = 0; s < state.Sectors.Count; s++)
            {
                SectorState sector = state.Sectors[s];
                long sectorTotal = 0;
                for (int i = 0; i < sector.Entries.Count; i++)
                {
                    SectorEntry entry = sector.Entries[i];
                    if (entry.LabId == lab.Id) sectorTotal += entry.LastRevenue;
                }
                if (sectorTotal != 0)
                {
                    b.Parts.Add(new RevenuePart { SectorId = sector.Id, Amount = sectorTotal });
                    b.Total += sectorTotal;
                }
            }
            return b;
        }
    }
}