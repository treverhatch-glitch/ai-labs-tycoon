using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
  public static class FundingSystem
  {
    public static bool HasRound(LabState lab, string roundId)
    {
      for (int i = 0; i < lab.FundingRounds.Count; i++) if (lab.FundingRounds[i] == roundId) return true;
      return false;
    }

    static int RoundIndex(GameConfig c, string roundId)
    {
      for (int i = 0; i < c.Funding.Rounds.Count; i++) if (c.Funding.Rounds[i].Id == roundId) return i;
      return -1;
    }

    public static bool PriorRoundsDone(GameConfig c, LabState lab, FundingRoundConfig round)
    {
      int idx = RoundIndex(c, round.Id);
      if (idx < 0) return false;
      for (int i = 0; i < idx; i++) if (!HasRound(lab, c.Funding.Rounds[i].Id)) return false;
      return true;
    }

    public static bool IsDownRound(GameConfig c, GameState state)
    {
      FundingFollowOnConfig f = c.Funding.FollowOn;
      if (f == null || f.DownRoundBubblePermille < 1) return false;
      return state.BubblePermille >= f.DownRoundBubblePermille;
    }

    public static long EffectiveCash(GameConfig c, GameState state, FundingRoundConfig round)
    {
      FundingFollowOnConfig f = c.Funding.FollowOn;
      long cash = round.Cash;
      if (IsDownRound(c, state) && f != null && f.DownRoundCashPermille >= 1)
        cash = MathI.MulPermille(cash, f.DownRoundCashPermille);
      int crash = BubbleSystem.FundingCashPermille(c, state);
      if (crash < MathI.Permille) cash = MathI.MulPermille(cash, crash);
      return cash;
    }

    public static int EffectiveDilution(GameConfig c, GameState state, FundingRoundConfig round)
    {
      FundingFollowOnConfig f = c.Funding.FollowOn;
      if (!IsDownRound(c, state) || f == null || f.DownRoundExtraDilutionPermille <= 0) return round.DilutionPermille;
      return MathI.Clamp(round.DilutionPermille + f.DownRoundExtraDilutionPermille, 0, MathI.Permille);
    }

    public static bool PlayerCommandLocked(GameState state, LabState lab)
    {
      return lab != null && lab.Id == state.PlayerLabId && state.Tick < lab.BoardLockedUntilTick;
    }

    public static void ApplyRaise(GameConfig c, GameState state, LabState lab, FundingRoundConfig round)
    {
      bool down = IsDownRound(c, state);
      long cash = EffectiveCash(c, state, round);
      int dilution = EffectiveDilution(c, state, round);
      lab.FundingRounds.Add(round.Id);
      Finance.Earn(lab, LedgerLine.CapitalIn, cash);
      BubbleSystem.OnFundingRaise(c, state, round, cash);
      lab.FounderSharePermille = (int)MathI.MulPermille(lab.FounderSharePermille, MathI.Permille - dilution);
      StaffEquity.OnRaise(lab, dilution);
      lab.BoardInvestorSeats += round.BoardSeats;
      GameEvent closed = Events.Add(state, EventKind.FundingClosed, lab.Id, -1, cash, round.Id);
      closed.Pick = down ? 1 : 0;
      TryOust(c, state, lab);
    }

  public static void RunMonth(GameConfig c, GameState state)
    {
      if (state.PlayerLabId < 0) return;
      LabState player = state.Player;
      if (player == null || player.Bankrupt) return;
      FundingFollowOnConfig f = c.Funding.FollowOn;
      if (f == null) return;
      TryPlayerBoardDrama(c, state, player, f);
      TryInvestorEvent(c, state, player, f);
    }

    static void TryPlayerBoardDrama(GameConfig c, GameState state, LabState lab, FundingFollowOnConfig f)
    {
      if (f.BoardDramaMinInvestorSeats < 1 || lab.BoardInvestorSeats < f.BoardDramaMinInvestorSeats) return;
      if (f.BoardDramaPermillePerYear <= 0) return;
      if (state.Tick < lab.BoardLockedUntilTick) return;
      if (!state.Rng.Rivals.Chance(f.BoardDramaPermillePerYear / c.Calendar.MonthsPerYear)) return;
      int weeks = f.BoardDramaWeeks > 0 ? f.BoardDramaWeeks : c.Rivalry.BoardDramaWeeks;
      lab.BoardLockedUntilTick = state.Tick + weeks;
      Events.Add(state, EventKind.BoardDrama, lab.Id, -1, weeks, "");
    }

    static void TryInvestorEvent(GameConfig c, GameState state, LabState lab, FundingFollowOnConfig f)
    {
      if (f.InvestorEventMinRounds < 1 || lab.FundingRounds.Count < f.InvestorEventMinRounds) return;
      if (f.InvestorEventPermillePerMonth <= 0) return;
      if (!state.Rng.Rivals.Chance(f.InvestorEventPermillePerMonth)) return;
      if (f.InvestorEventReputation > 0)
        lab.Reputation = MathI.Clamp(lab.Reputation + f.InvestorEventReputation, 0, MathI.Permille);
      if (f.InvestorEventBubblePermille > 0)
        BubbleSystem.Boost(c, state, f.InvestorEventBubblePermille);
      Events.Add(state, EventKind.InvestorBoardVisit, lab.Id, -1, f.InvestorEventReputation, "");
    }

    public static void TryOust(GameConfig c, GameState state, LabState lab)
    {
      if (lab.Id != state.PlayerLabId) return;
      if (lab.FounderSharePermille > c.Funding.ControlPermille) return;
      EndingSystem.RecordOusted(c, state, lab);
    }
  }
}