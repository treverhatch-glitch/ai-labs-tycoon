using AiLabs.Rules.Config;

namespace AiLabs.Unity.Hq
{
    /// <summary>Default prop ids per room purpose (see PROP-PLACEMENT-HINTS.md).</summary>
    public static class CampusPropPlacementHints
    {
        public static string[] DefaultPropsFor(RoomPurpose purpose)
        {
            switch (purpose)
            {
                case RoomPurpose.Research:
                    return new[] { "prop-research-bench", "prop-whiteboard" };
                case RoomPurpose.Engineering:
                    return new[] { "prop-desk", "prop-desk", "prop-chair", "prop-chair" };
                case RoomPurpose.Safety:
                    return new[] { "prop-safety-cone", "prop-whiteboard" };
                case RoomPurpose.Sales:
                    return new[] { "prop-desk", "prop-desk", "prop-potted-plant" };
                case RoomPurpose.Compute:
                    return new[] { "prop-server-rack", "prop-gpu-blade", "prop-gpu-blade", "prop-gpu-blade" };
                case RoomPurpose.Infrastructure:
                    return new[] { "prop-monitor-wall" };
                case RoomPurpose.Policy:
                    return new[] { "prop-whiteboard", "prop-water-cooler" };
                default:
                    return new[] { "prop-desk" };
            }
        }
    }
}