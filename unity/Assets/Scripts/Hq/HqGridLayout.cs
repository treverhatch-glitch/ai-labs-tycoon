using UnityEngine;

namespace AiLabs.Unity.Hq
{
    /// <summary>World-space layout for the HQ lot grid (presentation constants only).</summary>
    public static class HqGridLayout
    {
        public const float TileSize = 0.45f;
        public const float FloorY = 0f;
        public const float RoomHeight = 0.35f;

        public static bool TryWorldToTile(Vector3 world, int lotWidth, int lotHeight, out int tileX, out int tileY)
        {
            tileX = Mathf.FloorToInt(world.x / TileSize);
            tileY = Mathf.FloorToInt(world.z / TileSize);
            return tileX >= 0 && tileY >= 0 && tileX < lotWidth && tileY < lotHeight;
        }

        public static Vector3 TileCornerWorld(int tileX, int tileY)
        {
            return new Vector3(tileX * TileSize, FloorY, tileY * TileSize);
        }
    }
}