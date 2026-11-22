using AiLabs.Rules.State;
using AiLabs.Unity.Hq;
using NUnit.Framework;
using UnityEngine;

namespace AiLabs.Unity.Editor.Tests
{
    public sealed class HqFinancialHudTests
    {
        [Test]
        public void Bankrupt_and_zero_cash_change_colour_and_status()
        {
            var ok = new LabState { Cash = 100_000, Bankrupt = false };
            Assert.AreEqual(Color.white, HqFinancialHud.CashColor(ok));
            Assert.AreEqual("", HqFinancialHud.StatusLine(ok));

            var broke = new LabState { Cash = 0, Bankrupt = false };
            Assert.AreNotEqual(Color.white, HqFinancialHud.CashColor(broke));
            Assert.AreEqual("Out of cash.", HqFinancialHud.StatusLine(broke));

            var bankrupt = new LabState { Cash = -50_000, Bankrupt = true, WeeksInDebt = 3 };
            Assert.AreEqual("Bankrupt — retrench or restart.", HqFinancialHud.StatusLine(bankrupt));
        }
    }
}