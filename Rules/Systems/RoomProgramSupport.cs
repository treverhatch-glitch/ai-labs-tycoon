using AiLabs.Rules.Config;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
  public static class RoomProgramSupport
  {
    public static bool HasRoom(LabState lab, string roomTypeId)
    {
      if (lab == null || string.IsNullOrEmpty(roomTypeId)) return false;
      for (int i = 0; i < lab.Rooms.Count; i++)
        if (lab.Rooms[i].TypeId == roomTypeId) return true;
      return false;
    }

    public static bool Staffed(LabState lab, string roleId, int min = 1)
    {
      if (lab == null || string.IsNullOrEmpty(roleId)) return false;
      return LabQueries.StaffCount(lab, roleId) >= min;
    }

    public static bool Gate(GameConfig c, GameState state, LabState lab, string eraId, string roomTypeId, string staffRoleId)
    {
      if (lab == null || lab.Bankrupt) return false;
      if (!EraSystem.Open(c, eraId, state.Tick)) return false;
      if (!HasRoom(lab, roomTypeId)) return false;
      if (!string.IsNullOrEmpty(staffRoleId) && !Staffed(lab, staffRoleId)) return false;
      return true;
    }

    public static bool ActiveUntil(int untilTick, int tick) => untilTick >= 0 && tick < untilTick;
  }
}