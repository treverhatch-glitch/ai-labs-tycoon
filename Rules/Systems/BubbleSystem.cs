using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
  public static class BubbleSystem
  {
    public static bool InCrash(GameState state) => state.BubbleCrashMonthsRemaining > 0;

    public static int CustomerSpendPermille(GameConfig c, GameState state)
    {
      BubbleConfig b = c.Bubble;
      if (b == null || !InCrash(state) || b.CrashCustomerSpendPermille < 1) return MathI.Permille;
      return b.CrashCustomerSpendPermille;
    }

    public static int GpuPricePermille(GameConfig c, GameState state)
    {
      BubbleConfig b = c.Bubble;
      if (b == null || !InCrash(state) || b.CrashGpuPricePermille < 1) return MathI.Permille;
      return b.CrashGpuPricePermille;
    }

    public static int FundingCashPermille(GameConfig c, GameState state)
    {
      BubbleConfig b = c.Bubble;
      if (b == null || !InCrash(state) || b.CrashFundingCashPermille < 1) return MathI.Permille;
      return b.CrashFundingCashPermille;
    }

    public static void Boost(GameConfig c, GameState state, int permille)
    {
      if (permille <= 0) return;
      state.BubblePermille = (int)MathI.Min(MathI.Permille, state.BubblePermille + permille);
    }

    public static void OnFundingRaise(GameConfig c, GameState state, FundingRoundConfig round, long cash)
    {
      BubbleConfig b = c.Bubble;
      if (b == null) return;
      if (cash > 0) state.BubbleCapitalRaised += cash;
      int boost = round.BubbleBoostPermille > 0 ? round.BubbleBoostPermille : b.DefaultFundingBoostPermille;
      state.BubblePermille = (int)MathI.Min(MathI.Permille, state.BubblePermille + boost);
    }

    public static void OnListedChipTrade(GameConfig c, GameState state, long tradeCash)
    {
      BubbleConfig b = c.Bubble;
      if (b == null) return;
      if (tradeCash > 0) state.BubbleCapitalRaised += tradeCash;
      if (b.ListedChipBoostPermille <= 0) return;
      state.BubblePermille = (int)MathI.Min(MathI.Permille, state.BubblePermille + b.ListedChipBoostPermille);
    }

    public static void OnWorldEvent(GameConfig c, GameState state, WorldEventConfig e)
    {
      BubbleConfig b = c.Bubble;
      if (b == null || string.IsNullOrEmpty(b.PopWorldEventId)) return;
      if (e.Id == b.PopWorldEventId) StartCrash(c, state, force: true, postNews: false);
    }

    public static long MarketRevenueThisMonth(GameState state)
    {
      long total = 0;
      for (int i = 0; i < state.Labs.Count; i++)
      {
        LabState lab = state.Labs[i];
        if (!LabQueries.ParticipatesInEconomy(lab)) continue;
        total += lab.Ledger.ThisMonth.Revenue;
      }
      return total;
    }

    public static void RunMonth(GameConfig c, GameState state)
    {
      BubbleConfig b = c.Bubble;
      if (b == null) return;

      if (InCrash(state))
      {
        state.BubbleCrashMonthsRemaining--;
        ApplyCrashValuationCut(c, state, b);
        if (!InCrash(state))
        {
          state.BubblePermille = b.PostCrashResidualPermille;
          state.BubbleGraceMonths = 0;
          if (b.ResetCapitalRaisedAfterCrash) state.BubbleCapitalRaised = 0;
        }
        return;
      }

      if (b.PopAtPermille > 0 && state.BubblePermille >= b.PopAtPermille)
      {
        StartCrash(c, state, state.BubblePermille >= b.PopAtPermille, postNews: true);
        return;
      }

      if (state.BubblePermille > 0)
      {
        int next = state.BubblePermille - b.DecayPermillePerMonth;
        state.BubblePermille = state.BubbleHadCrash
          ? (int)MathI.Max(b.PostCrashResidualPermille, next)
          : (int)MathI.Max(0, next);
      }

      long revenue = MarketRevenueThisMonth(state);
      if (revenue <= 0) revenue = MarketRevenueLastClosedMonth(state);
      if (state.BubblePermille >= b.HotPermille && b.GraceMonthsBeforeCrash > 0)
      {
        bool returnsOk = state.BubbleCapitalRaised <= 0
          || revenue * MathI.Permille >= state.BubbleCapitalRaised * (long)b.ReturnsNeededPermille;
        if (returnsOk)
        {
          state.BubbleGraceMonths = 0;
          if (b.ReturnCoolingPermille > 0)
            state.BubblePermille = (int)MathI.Max(b.PostCrashResidualPermille, state.BubblePermille - b.ReturnCoolingPermille);
        }
        else state.BubbleGraceMonths++;
      }
      else state.BubbleGraceMonths = 0;

      if (state.BubbleGraceMonths >= b.GraceMonthsBeforeCrash)
        StartCrash(c, state, false, postNews: true);
    }

    public static long MarketRevenueLastClosedMonth(GameState state)
    {
      long total = 0;
      for (int i = 0; i < state.Labs.Count; i++)
      {
        LabState lab = state.Labs[i];
        if (!LabQueries.ParticipatesInEconomy(lab)) continue;
        if (lab.Ledger.History.Count > 0) total += lab.Ledger.History[lab.Ledger.History.Count - 1].Totals.Revenue;
        else total += lab.Ledger.ThisMonth.Revenue;
      }
      return total;
    }

    public static void StartCrash(GameConfig c, GameState state, bool force = false, bool postNews = true)
    {
      BubbleConfig b = c.Bubble;
      if (b == null) return;
      if (InCrash(state)) return;
      if (!force && state.BubbleGraceMonths < b.GraceMonthsBeforeCrash && state.BubblePermille < b.PopAtPermille) return;
      bool playerHaircut = ApplyWeakHaircuts(c, state, b);
      state.BubbleHadCrash = true;
      state.BubbleCrashMonthsRemaining = b.CrashDurationMonths > 0 ? b.CrashDurationMonths : 12;
      state.BubblePermille = b.PostCrashResidualPermille;
      state.BubbleGraceMonths = 0;
      if (postNews)
        Events.Add(state, EventKind.WorldNews, -1, -1, state.BubbleCrashMonthsRemaining, "bubble-crash");
      if (b.ThePopEndingForPlayer && playerHaircut && state.PlayerLabId >= 0)
        EndingSystem.RecordThePop(c, state, state.Player);
    }

    /// <summary>Legacy name; starts a sustained crash, not an instant meter wipe.</summary>
    public static void Pop(GameConfig c, GameState state) => StartCrash(c, state, force: true, postNews: true);

    static bool ApplyWeakHaircuts(GameConfig c, GameState state, BubbleConfig b)
    {
      bool playerHaircut = false;
      for (int i = 0; i < state.Labs.Count; i++)
      {
        LabState lab = state.Labs[i];
        if (!LabQueries.ParticipatesInEconomy(lab)) continue;
        int share = GovernmentSystem.RevenueSharePpm(state, lab);
        if (share >= b.SurvivorRevenuePpm) continue;
        long cut = MathI.MulPermille(lab.Cash, b.WeakCashHaircutPermille);
        if (cut > 0)
        {
          Finance.Spend(lab, LedgerLine.Fines, cut);
          if (lab.Id == state.PlayerLabId) playerHaircut = true;
        }
      }
      return playerHaircut;
    }

    static void ApplyCrashValuationCut(GameConfig c, GameState state, BubbleConfig b)
    {
      if (b.CrashFloatCutPpm <= 0) return;
      for (int i = 0; i < state.Labs.Count; i++)
      {
        LabState lab = state.Labs[i];
        if (lab.Bankrupt || lab.Controller != LabController.Plan) continue;
        lab.PublicFloatPpm = (int)MathI.Max(0, lab.PublicFloatPpm - b.CrashFloatCutPpm);
      }
    }
  }
}