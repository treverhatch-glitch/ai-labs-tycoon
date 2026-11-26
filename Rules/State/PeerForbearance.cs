using AiLabs.Rules.Core;

namespace AiLabs.Rules.State
{
  /// <summary>Landlord lab forgiving peer-lease debt trims for a renter until a tick (Q181).</summary>
  public sealed class PeerForbearance : ISync
  {
    public int RenterLabId;
    public int UntilTick = -1;

    public void Sync(Syncer s)
    {
      s.Int("renterLabId", ref RenterLabId);
      s.Int("untilTick", ref UntilTick);
    }
  }
}