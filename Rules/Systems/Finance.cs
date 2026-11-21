using AiLabs.Rules.Config;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>
    /// The only code that changes a lab's cash. Every dollar goes through <see cref="Earn"/> or
    /// <see cref="Spend"/> and lands on a ledger line, so cash always equals the founding cash plus the ledger.
    /// </summary>
    public static class Finance
    {
        public static void Earn(LabState lab, LedgerLine line, long amount)
        {
            if (amount <= 0) return;
            Move(lab, line, amount);
        }

        public static void Spend(LabState lab, LedgerLine line, long amount)
        {
            if (amount <= 0) return;
            Move(lab, line, -amount);
        }

        static void Move(LabState lab, LedgerLine line, long signed)
        {
            lab.Cash += signed;
            lab.Ledger.ThisWeek.Add(line, signed);
            lab.Ledger.ThisMonth.Add(line, signed);
            lab.Ledger.Lifetime.Add(line, signed);
        }

        /// <summary>The weekly bill: grants in, salaries, room upkeep and GPU costs out, then the debt clock.</summary>
        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (lab.GrantPerWeek > 0 && (lab.GrantEndsTick < 0 || state.Tick < lab.GrantEndsTick))
            {
                Earn(lab, LedgerLine.Grants, lab.GrantPerWeek);
                if (lab.GrantEndsTick >= 0 && state.Tick == lab.GrantEndsTick - 1)
                    Events.Add(state, EventKind.GrantEnded, lab.Id, -1, lab.GrantPerWeek, "");
            }

            Insurance.RunWeek(c, state, lab);
            Sublet.RunWeek(c, state, lab);
            Spend(lab, LedgerLine.Salaries, LabQueries.WeeklySalaries(c, lab));
            Spend(lab, LedgerLine.RoomUpkeep, LabQueries.WeeklyRoomUpkeep(c, lab));
            Spend(lab, LedgerLine.GpuRent, LabQueries.WeeklyGpuRent(c, state, lab));
            Spend(lab, LedgerLine.GpuUpkeep, LabQueries.WeeklyGpuUpkeep(c, lab));
            Spend(lab, LedgerLine.Power, WorldQueries.WeeklyPowerBill(c, state, lab));

            Loans.RunWeek(c, state, lab);
            Treasury.RunWeek(c, state, lab);

            if (lab.Cash < 0)
            {
                lab.WeeksInDebt++;
                if (lab.WeeksInDebt == 1) Events.Add(state, EventKind.InDebt, lab.Id, -1, lab.Cash, "");
                if (lab.WeeksInDebt >= c.Finance.BankruptcyGraceWeeks)
                {
                    GoBankrupt(state, lab);
                    if (lab.Id == state.PlayerLabId) EndingSystem.RecordBankrupt(c, state, lab);
                }
            }
            else
            {
                lab.WeeksInDebt = 0;
            }
        }

        /// <summary>The lab closes: its models leave the market, everyone goes home and the rented GPUs go back.</summary>
        public static void GoBankrupt(GameState state, LabState lab)
        {
            lab.Bankrupt = true;
            ChipSystem.OnBankrupt(state, lab);
            for (int i = 0; i < lab.Models.Count; i++) lab.Models[i].Status = ModelStatus.Retired;
            for (int s = 0; s < state.Sectors.Count; s++)
            {
                SectorState sector = state.Sectors[s];
                for (int i = sector.Entries.Count - 1; i >= 0; i--)
                    if (sector.Entries[i].LabId == lab.Id) sector.Entries.RemoveAt(i);
                if (sector.ChampionLabId == lab.Id) sector.ChampionLabId = -1;
            }
            lab.Staff.Clear();
            Loans.OnBankrupt(lab);
            lab.RentedGpus = 0;
            lab.ReservedContracts.Clear();
            lab.Fleet.RemoveAll(b => b.HoldingId != HardwareBatch.Hq);
            lab.Holdings.Clear();
            Events.Add(state, EventKind.Bankrupt, lab.Id, -1, lab.Cash, "");
        }

        public static void CloseWeek(LabState lab)
        {
            lab.Ledger.LastWeek = lab.Ledger.ThisWeek;
            lab.Ledger.ThisWeek = new LedgerTotals();
        }

        public static void CloseMonth(GameConfig c, GameState state, LabState lab)
        {
            var record = new MonthRecord();
            record.Year = GameDate.Year(c.Calendar, state.Tick);
            record.Month = GameDate.Month(c.Calendar, state.Tick);
            record.EndCash = lab.Cash;
            record.Totals = lab.Ledger.ThisMonth;
            lab.Ledger.History.Add(record);
            while (lab.Ledger.History.Count > c.Finance.HistoryMonths) lab.Ledger.History.RemoveAt(0);
            lab.Ledger.ThisMonth = new LedgerTotals();
        }
    }

    /// <summary>The event log: what happened, as data, newest last.</summary>
    public static class Events
    {
        public static GameEvent Add(GameState state, EventKind kind, int labId, int subjectId, long amount, string reference)
        {
            var e = new GameEvent();
            e.Tick = state.Tick;
            e.Kind = kind;
            e.LabId = labId;
            e.SubjectId = subjectId;
            e.Amount = amount;
            e.Ref = reference ?? "";
            state.Events.Add(e);
            return e;
        }

        public static void Trim(GameConfig c, GameState state)
        {
            int extra = state.Events.Count - c.EventLogMax;
            if (extra > 0) state.Events.RemoveRange(0, extra);
        }
    }
}
