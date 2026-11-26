using System.Collections.Generic;
using AiLabs.Rules.Core;

namespace AiLabs.Rules.Config
{
  public sealed class FundingConfig : ISync
  {
    /// <summary>Founder's equity at incorporation, in permille of the company.</summary>
    public int FounderStartPermille;
    /// <summary>At or below this, investors control the board and can oust the founder.</summary>
    public int ControlPermille;
    public string OustedSuccessor = "";
    public List<FundingRoundConfig> Rounds = new List<FundingRoundConfig>();

    public FundingRoundConfig Round(string id)
    {
      for (int i = 0; i < Rounds.Count; i++) if (Rounds[i].Id == id) return Rounds[i];
      return null;
    }

    public void Sync(Syncer s)
    {
      s.Int("founderStartPermille", ref FounderStartPermille);
      s.Int("controlPermille", ref ControlPermille);
      s.Str("oustedSuccessor", ref OustedSuccessor);
      s.ObjList("rounds", ref Rounds);
    }
  }

  public sealed class FundingRoundConfig : ISync, IHasId
  {
    public string IdValue = "";
    public string Name = "";
    public int OpensYear;
    public long Cash;
    /// <summary>Equity sold this round, in permille of the company before the round.</summary>
    public int DilutionPermille;
    public int MinReputation;
    /// <summary>Investor board seats added when the round closes.</summary>
    public int BoardSeats;

    public string Id { get { return IdValue; } }

    public void Sync(Syncer s)
    {
      s.Str("id", ref IdValue);
      s.Str("name", ref Name);
      s.Int("opensYear", ref OpensYear);
      s.Long("cash", ref Cash);
      s.Int("dilutionPermille", ref DilutionPermille);
      s.Int("minReputation", ref MinReputation);
      s.Int("boardSeats", ref BoardSeats);
    }
  }
}