using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Unity.Hq
{
    /// <summary>
    /// Derives per-room glow strength for the HQ cutaway. Seeds from compute/research rooms that
    /// host active or released models; spreads to touching rooms with decay (visual only).
    /// </summary>
    public static class GlowSpreadMap
    {
        const int DecayPermille = 650;

        public static Dictionary<int, int> Compute(GameConfig c, LabState lab, int tick)
        {
            var map = new Dictionary<int, int>();
            if (lab.Rooms.Count == 0) return map;

            int maxAlignment = 0;
            bool hasReleased = false;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.Status == ModelStatus.Training || m.Status == ModelStatus.Trained)
                {
                    if (m.Alignment > maxAlignment) maxAlignment = m.Alignment;
                }
                if (m.Status == ModelStatus.Released)
                {
                    hasReleased = true;
                    if (m.Alignment > maxAlignment) maxAlignment = m.Alignment;
                }
            }

            int seedStrength = hasReleased ? maxAlignment : maxAlignment / 2;
            if (seedStrength <= 0) return map;

            var seeds = new List<int>();
            for (int i = 0; i < lab.Rooms.Count; i++)
            {
                RoomTypeConfig type = c.RoomType(lab.Rooms[i].TypeId);
                if (type == null) continue;
                if (type.Purpose == RoomPurpose.Compute || type.Purpose == RoomPurpose.Research)
                    seeds.Add(lab.Rooms[i].Id);
            }
            if (seeds.Count == 0 && lab.Rooms.Count > 0)
                seeds.Add(lab.Rooms[0].Id);

            var queue = new Queue<(int roomId, int strength)>();
            for (int i = 0; i < seeds.Count; i++)
            {
                map[seeds[i]] = seedStrength;
                queue.Enqueue((seeds[i], seedStrength));
            }

            while (queue.Count > 0)
            {
                var (roomId, strength) = queue.Dequeue();
                RoomState room = FindRoom(lab, roomId);
                if (room == null) continue;

                for (int i = 0; i < lab.Rooms.Count; i++)
                {
                    RoomState other = lab.Rooms[i];
                    if (other.Id == roomId) continue;
                    if (!Touches(room, other)) continue;

                    int spread = (strength * DecayPermille) / 1000;
                    if (spread <= 0) continue;
                    if (map.TryGetValue(other.Id, out int existing) && existing >= spread) continue;

                    map[other.Id] = spread;
                    queue.Enqueue((other.Id, spread));
                }
            }

            return map;
        }

        static bool Touches(RoomState a, RoomState b)
        {
            bool xTouch = a.X + a.Width == b.X || b.X + b.Width == a.X;
            bool yTouch = a.Y + a.Height == b.Y || b.Y + b.Height == a.Y;
            bool xOverlap = a.X < b.X + b.Width && b.X < a.X + a.Width;
            bool yOverlap = a.Y < b.Y + b.Height && b.Y < a.Y + a.Height;
            return (xTouch && yOverlap) || (yTouch && xOverlap);
        }

        static RoomState FindRoom(LabState lab, int id)
        {
            for (int i = 0; i < lab.Rooms.Count; i++)
                if (lab.Rooms[i].Id == id) return lab.Rooms[i];
            return null;
        }
    }
}