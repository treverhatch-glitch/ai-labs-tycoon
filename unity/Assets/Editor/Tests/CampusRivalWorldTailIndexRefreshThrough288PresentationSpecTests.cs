using AiLabs.Unity.Hq;
using NUnit.Framework;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class CampusRivalWorldTailIndexRefreshThrough288PresentationSpecTests
    {
        [Test]
        public void Tail_entry_count_positive()
        {
            Assert.Greater(
                CampusRivalWorldTailIndexRefreshThrough288PresentationSpec.TailEntryCount, 0);
        }
    }
}