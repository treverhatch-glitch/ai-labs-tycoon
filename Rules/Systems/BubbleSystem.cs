using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
  public static class BubbleSystem
  {
    public static void Boost(GameConfig c, GameState state, int permille)
    {
      if (permille <= 0) return;
      state.BubblePermille = (int)MathI.Min(MathI.Permille, state.BubblePermille + permille);
    }

    public static void OnFundingRaise(GameConfig c, GameState state, FundingRoundConfig round)
    {
      BubbleConfig b = c.Bubble;
      if (b == null) return;
      int boost = round.BubbleBoostPermille > 0 ? round.BubbleBoostPermille : b.DefaultFundingBoostPermille;
      state.BubblePermille = (int)MathI.Min(MathI.Permille, state.BubblePermille + boost);
    }

    public static void OnListedChipTrade(GameConfig c, GameState state)
    {
      BubbleConfig b = c.Bubble;
      if (b == null || b.ListedChipBoostPermille <= 0) return;
      state.BubblePermille = (int)MathI.Min(MathI.Permille, state.BubblePermille + b.ListedChipBoostPermille);
    }

    public static void OnWorldEvent(GameConfig c, GameState state, WorldEventConfig e)
    {
      BubbleConfig b = c.Bubble;
      if (b == null || string.IsNullOrEmpty(b.PopWorldEventId)) return;
      if (e.Id == b.PopWorldEventId) Pop(c, state);
    }

    public static void RunMonth(GameConfig c, GameState state)
    {
      BubbleConfig b = c.Bubble;
      if (b == null) return;
      if (state.BubblePermille > 0)
        state.BubblePermille = (int)MathI.Max(0, state.BubblePermille - b.DecayPermillePerMonth);
      if (state.BubblePermille >= b.PopAtPermille) Pop(c, state);
    }

    public static void Pop(GameConfig c, GameState state)
    {
      BubbleConfig b = c.Bubble;
      if (b == null || state.BubblePermille <= 0) return;
      for (int i = 0; i < state.Labs.Count; i++)
      {
        LabState lab = state.Labs[i];
        if (lab.Bankrupt) continue;
        int share = GovernmentSystem.RevenueSharePpm(state, lab);
        if (share >= b.SurvivorRevenuePpm) continue;
        long cut = MathI.MulPermille(lab.Cash, b.WeakCashHaircutPermille);
        if (cut > 0) Finance.Spend(lab, LedgerLine.Fines, cut);
      }
      state.BubblePermille = 0;
      Events.Add(state, EventKind.WorldNews, -1, -1, 0, "bubble-crash");
    }
  }
}