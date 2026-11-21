using System;
using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public sealed class NewGameOptions
    {
        public long Seed;
        /// <summary>Empty to use the config's default.</summary>
        public string LabName = "";
        public string Leader = "";
        public string FamilyName = "";
        /// <summary>The rivals to play against, by config id. Null for all of them.</summary>
        public List<string> RivalIds;
        /// <summary>The era to start in (the start-era picker), by config id. Empty for the first era, the 1995 default.</summary>
        public string StartEraId = "";
        /// <summary>Who the player is (a founder id); empty for none.</summary>
        public string FounderId = "";
        /// <summary>How the player's lab is good (a style id); empty for none.</summary>
        public string StyleId = "";
        /// <summary>The bloc the lab is incorporated in; empty for the config's default.</summary>
        public string HomeBlocId = "";
    }

    /// <summary>
    /// The game, as the outside world sees it: start or load, give commands, step a week at a time, save.
    /// Everything else in the rules is reached through here. Given the same config, the same start and
    /// the same commands on the same ticks, two simulations are the same to the last bit.
    /// </summary>
    public sealed class Simulation
    {
        public readonly GameConfig Config;
        public readonly GameState State;
        /// <summary>Every command accepted from outside since this simulation was created, with its tick.</summary>
        public readonly ReplayLog Journal;

        Simulation(GameConfig config, GameState state)
        {
            Config = config;
            State = state;
            Journal = new ReplayLog();
            Journal.StartSave = SaveGame.Write(state);
            Journal.EndTick = state.Tick;
        }

        public static Simulation NewGame(GameConfig config, NewGameOptions options)
        {
            return new Simulation(config, WorldSetup.NewGame(config, options ?? new NewGameOptions()));
        }

        public static Simulation NewGame(GameConfig config, long seed)
        {
            var options = new NewGameOptions();
            options.Seed = seed;
            return NewGame(config, options);
        }

        /// <summary>Loads a save. Throws <see cref="SaveException"/> when the save is broken or does not fit the config.</summary>
        public static Simulation Load(GameConfig config, string saveJson)
        {
            return new Simulation(config, SaveGame.Read(config, saveJson));
        }

        public LabState Player { get { return State.Player; } }
        public int Tick { get { return State.Tick; } }
        public int Year { get { return GameDate.Year(Config.Calendar, State.Tick); } }
        public EraConfig Era { get { return EraSystem.AtTick(Config, State.Tick); } }
        public string DateLabel { get { return GameDate.Label(Config.Calendar, State.Tick); } }

        public string Save() { return SaveGame.Write(State); }

        /// <summary>A fingerprint of the whole game state. Two games with the same digest are the same game.</summary>
        public string Digest() { return SaveGame.Digest(State); }

        /// <summary>Gives a command. A refused command changes nothing and is not journaled.</summary>
        public CommandResult Apply(Command command)
        {
            if (State.PlayerLabId < 0 && command.Kind != CommandKind.ContinueAfterEnding)
                return new CommandResult { Ok = false, Error = CommandError.Spectating, Message = "The player is spectating." };
            CommandResult result = CommandProcessor.Apply(Config, State, command);
            if (result.Ok) Journal.Add(State.Tick, command);
            return result;
        }

        /// <summary>
        /// Lets a plan run a lab for this week: the autopilot. Its commands are journaled like any others,
        /// so a game driven by a plan from outside replays too.
        /// </summary>
        public void RunPlan(int labId, PlanConfig plan)
        {
            LabState lab = State.Lab(labId);
            if (lab == null) throw new ArgumentException("There is no lab " + labId + ".");
            LabPlanner.Run(Config, State, lab, plan, cmd =>
            {
                CommandResult result = CommandProcessor.Apply(Config, State, cmd);
                if (result.Ok) Journal.Add(State.Tick, cmd);
                return result;
            });
        }

        public void StepMany(int weeks)
        {
            for (int i = 0; i < weeks; i++) Step();
        }

        /// <summary>Plays one week.</summary>
        public void Step()
        {
            GameConfig c = Config;
            GameState state = State;
            int tick = state.Tick;

            EraSystem.RunWeek(c, state);

            if (GameDate.IsMonthStart(c.Calendar, tick))
            {
                RivalSystem.RunMonth(c, state);
                FundingSystem.RunMonth(c, state);
                VendorSystem.RunMonth(c, state);
                ChipSystem.RunMonth(c, state);
                SiteSystem.RollHeatwaves(c, state);
                for (int i = 0; i < state.Labs.Count; i++)
                {
                    LabState lab = state.Labs[i];
                    if (lab.Controller != LabController.Plan || lab.Bankrupt || lab.AbsorbedByLabId >= 0) continue;
                    LabPlanner.Run(c, state, lab, c.Rival(lab.RivalId).Plan, ApplyInternal);
                }
            }

            SiteSystem.RunWeek(c, state);

            for (int i = 0; i < state.Labs.Count; i++)
            {
                LabState lab = state.Labs[i];
                lab.LastWeek = new WeekReport();
                if (lab.Bankrupt) continue;
                TrainingSystem.RunResearch(c, lab, tick);
                ChipSystem.RunWeek(c, state, lab);
                TrainingSystem.RunWeek(c, state, lab);
            }

            MarketSystem.RunWeek(c, state);

            for (int i = 0; i < state.Labs.Count; i++)
            {
                if (!state.Labs[i].Bankrupt)
                {
                    SafetySystem.RunWeek(c, state, state.Labs[i]);
                    Crunch.RunWeek(c, state, state.Labs[i]);
                    HackWeek.RunWeek(c, state, state.Labs[i]);
                    Chiefs.RunWeek(c, state, state.Labs[i]);
                    MarketingCampaign.RunWeek(c, state, state.Labs[i]);
                    OnCall.RunWeek(c, state, state.Labs[i]);
                    PolicyConsultants.RunWeek(c, state, state.Labs[i]);
                    Sla.RunWeek(c, state, state.Labs[i]);
                    EthicsBoard.RunWeek(c, state, state.Labs[i]);
                    Interpretability.RunWeek(c, state, state.Labs[i]);
                    Compliance.RunWeek(c, state, state.Labs[i]);
                    Prestige.RunWeek(c, state, state.Labs[i]);
                    ResearchGrants.RunWeek(c, state, state.Labs[i]);
                    RevenueTargets.RunWeek(c, state, state.Labs[i]);
                    Whistle.RunWeek(c, state, state.Labs[i]);
                    BugBounty.RunWeek(c, state, state.Labs[i]);
                    Roadmap.RunWeek(c, state, state.Labs[i]);
                    Mentoring.RunWeek(c, state, state.Labs[i]);
                    Skunkworks.RunWeek(c, state, state.Labs[i]);
                    Alumni.RunWeek(c, state, state.Labs[i]);
                    Leaseback.RunWeek(c, state, state.Labs[i]);
                    Foundation.RunWeek(c, state, state.Labs[i]);
                    RevenueFinancing.RunWeek(c, state, state.Labs[i]);
                    Listing.RunWeek(c, state, state.Labs[i]);
                    Covenants.RunWeek(c, state, state.Labs[i]);
                    HardwareReliability.RunWeek(c, state, state.Labs[i]);
                    Standards.RunWeek(c, state, state.Labs[i]);
                    Tax.RunWeek(c, state, state.Labs[i]);
                    ScalingPolicy.RunWeek(c, state, state.Labs[i]);
                    Charter.RunWeek(c, state, state.Labs[i]);
                    GovContracts.RunWeek(c, state, state.Labs[i]);
                    Security.RunWeek(c, state, state.Labs[i]);
                    Litigation.RunWeek(c, state, state.Labs[i]);
                    Scholars.RunWeek(c, state, state.Labs[i]);
                    Requisitions.RunWeek(c, state, state.Labs[i]);
                    Sabbatical.RunWeek(c, state, state.Labs[i]);
                    SupportDesk.RunWeek(c, state, state.Labs[i]);
                    Internships.RunWeek(c, state, state.Labs[i]);
                    Conference.RunWeek(c, state, state.Labs[i]);
                    AccountTeams.RunWeek(c, state, state.Labs[i]);
                    Sunset.RunWeek(c, state, state.Labs[i]);
                    Labour.RunWeek(c, state, state.Labs[i]);
                    StaffSystem.RunWeek(c, state.Labs[i]);
                    Traits.RunQuits(c, state, state.Labs[i]);
                    TalentSystem.RunWeek(c, state, state.Labs[i]);
                    PressSystem.RunWeek(c, state, state.Labs[i]);
                    TrustSystem.RunWeek(c, state, state.Labs[i]);
                    SafetyCulture.RunWeek(c, state, state.Labs[i]);
                    ResearchLeads.RunWeek(c, state, state.Labs[i]);
                    OfficeEvents.RunWeek(c, state, state.Labs[i]);
                    ShadySystem.RunWeek(c, state, state.Labs[i]);
                    MuseumSystem.RunWeek(c, state.Labs[i]);
                }
            }

            for (int i = 0; i < state.Labs.Count; i++)
            {
                if (!state.Labs[i].Bankrupt)
                {
                    Finance.RunWeek(c, state, state.Labs[i]);
                    ComputeContractSystem.RunWeek(c, state.Labs[i]);
                }
                Finance.CloseWeek(state.Labs[i]);
            }

            if (GameDate.IsMonthEnd(c.Calendar, tick))
            {
                for (int i = 0; i < state.Labs.Count; i++)
                    if (!state.Labs[i].Bankrupt) GovernmentSystem.CloseMonth(c, state, state.Labs[i]);
                LabState player = state.Player;
                if (player != null && !player.Bankrupt) ShadySystem.RunMonth(c, state, player);
                for (int i = 0; i < state.Labs.Count; i++) Finance.CloseMonth(c, state, state.Labs[i]);
                BubbleSystem.RunMonth(c, state);
                AcquisitionSystem.RunMonth(c, state);
                ShareSystem.RunMonth(c, state);
                RegulatorSystem.RunMonth(c, state);
                if (player != null && !player.Bankrupt) ChinaSystem.RunMonth(c, state, player);
            }

            if (GameDate.IsYearEnd(c.Calendar, tick))
            {
                GauntletSystem.RunYear(c, state);
                MarketSystem.GrowYear(c, state);
                EndingSystem.TryMonopoly(c, state);
                EndingSystem.TrySuperintelligence(c, state);
            }

            state.Tick = tick + 1;
            Journal.EndTick = state.Tick;
            Events.Trim(c, state);
        }

        CommandResult ApplyInternal(Command command) { return CommandProcessor.Apply(Config, State, command); }

        /// <summary>
        /// Plays a whole journal again from its starting save. The result is the same game the journal
        /// was recorded from: this is how a bug report or a test pins a run.
        /// </summary>
        public static Simulation Replay(GameConfig config, ReplayLog log)
        {
            return Replay(config, log, log.EndTick, true);
        }

        /// <summary>Replays a journal up to the start of a week, before any of that week's orders.</summary>
        public static Simulation Replay(GameConfig config, ReplayLog log, int untilTick)
        {
            return Replay(config, log, untilTick, false);
        }

        static Simulation Replay(GameConfig config, ReplayLog log, int untilTick, bool withOrdersOfLastTick)
        {
            Simulation sim = Load(config, log.StartSave);
            int next = 0;
            while (true)
            {
                if (sim.State.Tick >= untilTick && !withOrdersOfLastTick) break;
                while (next < log.Entries.Count && log.Entries[next].Tick == sim.State.Tick)
                {
                    CommandResult result = sim.Apply(log.Entries[next].Command);
                    if (!result.Ok)
                        throw new SaveException("The replay went off the rails at tick " + sim.State.Tick + ": " + result.Message);
                    next++;
                }
                if (sim.State.Tick >= untilTick) break;
                sim.Step();
            }
            return sim;
        }
    }

    public sealed class JournalEntry : ISync
    {
        public int Tick;
        public Command Command;

        public void Sync(Syncer s)
        {
            s.Int("tick", ref Tick);
            s.Obj("command", ref Command);
        }
    }

    /// <summary>A starting save plus the commands given after it. Enough to play the same game again.</summary>
    public sealed class ReplayLog : ISync
    {
        public string StartSave = "";
        /// <summary>The week the recording has reached.</summary>
        public int EndTick;
        public List<JournalEntry> Entries = new List<JournalEntry>();

        public void Add(int tick, Command command)
        {
            var e = new JournalEntry();
            e.Tick = tick;
            e.Command = command;
            Entries.Add(e);
        }

        public void Sync(Syncer s)
        {
            s.Str("startSave", ref StartSave);
            s.Int("endTick", ref EndTick);
            s.ObjList("entries", ref Entries);
        }

        public string ToJson() { return Json.Write(Syncer.Write(this), false); }

        public static ReplayLog FromJson(string json) { return Syncer.Read<ReplayLog>(Json.Parse(json), "replay"); }
    }
}
