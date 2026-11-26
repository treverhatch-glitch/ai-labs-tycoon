using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.State;
using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class GlowSpreadMapTests
    {
        [Test]
        public void Empty_lab_has_no_glow()
        {
            var config = new GameConfig();
            config.RoomTypes = new List<RoomTypeConfig>();
            var lab = new LabState();
            lab.Rooms = new List<RoomState>();
            lab.Models = new List<ModelState>();
            Assert.AreEqual(0, GlowSpreadMap.Compute(config, lab, 0).Count);
        }

        [Test]
        public void Training_model_seeds_compute_glow_on_adjacent_rooms()
        {
            var config = new GameConfig();
            config.RoomTypes = new List<RoomTypeConfig>
            {
                new RoomTypeConfig { IdValue = "research-lab", Purpose = RoomPurpose.Research },
                new RoomTypeConfig { IdValue = "compute-hall", Purpose = RoomPurpose.Compute },
            };
            var lab = new LabState { Id = 1 };
            lab.Rooms = new List<RoomState>
            {
                new RoomState { Id = 10, TypeId = "research-lab", X = 0, Y = 0, Width = 2, Height = 2 },
                new RoomState { Id = 11, TypeId = "compute-hall", X = 2, Y = 0, Width = 2, Height = 2 },
            };
            lab.Models = new List<ModelState>
            {
                new ModelState { Id = 100, Status = ModelStatus.Training, Alignment = 800 },
            };

            var map = GlowSpreadMap.Compute(config, lab, 0);
            Assert.True(map.ContainsKey(10));
            Assert.Greater(map[10], 0);
        }
    }
}