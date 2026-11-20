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

    public static void ApplyRaise(GameConfig c, GameState state, LabState lab, FundingRoundConfig round)
    {
      lab.FundingRounds.Add(round.Id);
      Finance.Earn(lab, LedgerLine.CapitalIn, round.Cash);
      BubbleSystem.OnFundingRaise(c, state, round);
      lab.FounderSharePermille = (int)MathI.MulPermille(lab.FounderSharePermille, MathI.Permille - round.DilutionPermille);
      lab.BoardInvestorSeats += round.BoardSeats;
      TryOust(c, state, lab);
    }

    public static void TryOust(GameConfig c, GameState state, LabState lab)
    {
      if (lab.Id != state.PlayerLabId) return;
      if (lab.FounderSharePermille > c.Funding.ControlPermille) return;
      EndingSystem.RecordOusted(c, state, lab);
    }
  }
}