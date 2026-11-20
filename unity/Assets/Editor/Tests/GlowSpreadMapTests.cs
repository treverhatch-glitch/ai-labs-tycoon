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
    }
}