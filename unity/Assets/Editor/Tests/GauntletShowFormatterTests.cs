using System.Collections.Generic;
using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class GauntletShowFormatterTests
    {
        [Test]
        public void TopSectorWins_skips_crown_bracket()
        {
            var record = new AiLabs.Rules.State.GauntletRecord();
            record.Entries = new List<AiLabs.Rules.State.GauntletEntry>();
            record.Entries.Add(Mk("crown", 1, "Grokk", 900));
            record.Entries.Add(Mk("game-ai", 1, "Clause", 400));
            record.Entries.Add(Mk("game-ai", 2, "Other", 200));

            var lines = GauntletShowFormatter.TopSectorWins(record, 5);
            Assert.AreEqual(1, lines.Count);
            StringAssert.Contains("game-ai", lines[0]);
        }

        static AiLabs.Rules.State.GauntletEntry Mk(string bracket, int rank, string name, long score)
        {
            var e = new AiLabs.Rules.State.GauntletEntry();
            e.Bracket = bracket;
            e.Rank = rank;
            e.ModelName = name;
            e.Score = score;
            return e;
        }
    }
}