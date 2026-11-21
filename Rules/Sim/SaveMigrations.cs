using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    /// <summary>
    /// Brings an older save up to the current schema, one step at a time. Each step is a pure function on the
    /// parsed JSON: it adds the keys the newer game expects, with their defaults, and rewrites what moved. A step
    /// names its keys in plain text, never through a C# class, so it keeps working however the classes change later.
    /// To add a field: bump <see cref="GameState.CurrentSchema"/>, add a <c>MigrateToN</c> and call it from <see cref="Run"/>.
    /// </summary>
    public static class SaveMigrations
    {
        public static JsonValue Run(GameConfig c, JsonValue root, int fromSchema)
        {
            if (fromSchema < 1) throw new SaveException("Save schema " + fromSchema + " is not known.");
            if (fromSchema < 2) MigrateTo2(c, root);
            if (fromSchema < 3) MigrateTo3(c, root);
            if (fromSchema < 4) MigrateTo4(c, root);
            if (fromSchema < 5) MigrateTo5(root);
            if (fromSchema < 6) MigrateTo6(c, root);
            if (fromSchema < 7) MigrateTo7(root);
            if (fromSchema < 8) MigrateTo8(c, root);
            if (fromSchema < 9) MigrateTo9(root);
            if (fromSchema < 10) MigrateTo10(c, root);
            if (fromSchema < 11) MigrateTo11(root);
            if (fromSchema < 12) MigrateTo12(root);
            if (fromSchema < 13) MigrateTo13(root);
            if (fromSchema < 14) MigrateTo14(root);
            if (fromSchema < 15) MigrateTo15(root);
            if (fromSchema < 16) MigrateTo16(root);
            if (fromSchema < 17) MigrateTo17(root);
            if (fromSchema < 18) MigrateTo18(root);
            if (fromSchema < 19) MigrateTo19(root);
            if (fromSchema < 20) MigrateTo20(root);
            if (fromSchema < 21) MigrateTo21(root);
            if (fromSchema < 22) MigrateTo22(root);
            if (fromSchema < 23) MigrateTo23(root);
            if (fromSchema < 25) MigrateTo25(root);
            if (fromSchema < 26) MigrateTo26(root);
            if (fromSchema < 27) MigrateTo27(root);
            if (fromSchema < 29) MigrateTo29(root);
            root.Set("schema", JsonValue.Of(GameState.CurrentSchema));
            return root;
        }

        static readonly string[] NewLedgerKeys = { "land", "siteBuild", "power", "chipDevelopment", "chipSales", "greyRevenue", "laundered", "capitalIn" };

        /// <summary>Schema 2: the three newer dice streams, and the eight newer ledger lines (all zero, so cash is still explained).</summary>
        static void MigrateTo2(GameConfig c, JsonValue root)
        {
            long seed = JsonPatch.IntOr(root, "seed", 0);
            JsonValue rng = root.Get("rng");
            if (rng != null)
            {
                JsonPatch.AddIfMissing(rng, "world", StreamAt(RngSet.MigratedState(seed, RngSet.WorldMigrationKey)));
                JsonPatch.AddIfMissing(rng, "chips", StreamAt(RngSet.MigratedState(seed, RngSet.ChipsMigrationKey)));
                JsonPatch.AddIfMissing(rng, "shady", StreamAt(RngSet.MigratedState(seed, RngSet.ShadyMigrationKey)));
            }

            foreach (string place in new[] { "labs.ledger.thisWeek", "labs.ledger.lastWeek", "labs.ledger.thisMonth", "labs.ledger.lifetime", "labs.ledger.history.totals" })
                JsonPatch.ForEach(root, place, totals =>
                {
                    foreach (string key in NewLedgerKeys) JsonPatch.AddIfMissing(totals, key, JsonValue.Of(0L));
                });
            MigrateFleet(c, root);
        }

        /// <summary>
        /// Owned GPUs were one number at the HQ. They become one batch of the generation that was current when the save
        /// was made (the numbers of that generation are the old single GPU's). The ids come from the save's own counter,
        /// which is bumped here so the next thing the game makes cannot reuse one.
        /// </summary>
        static void MigrateFleet(GameConfig c, JsonValue root)
        {
            long tick = JsonPatch.IntOr(root, "tick", 0);
            int year = GameDate.Year(c.Calendar, (int)tick);
            string genId = c.Compute.CurrentAtYear(year).Id;
            long nextId = JsonPatch.IntOr(root, "nextId", 1);

            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonValue owned = JsonPatch.Remove(lab, "ownedGpus");
                JsonValue fleet = JsonValue.Array();
                if (owned != null && owned.Kind == JsonKind.Int && owned.Int > 0)
                {
                    JsonValue batch = JsonValue.Object();
                    batch.Set("id", JsonValue.Of(nextId++));
                    batch.Set("holdingId", JsonValue.Of(-1L));
                    batch.Set("gpuGenId", JsonValue.Of(genId));
                    batch.Set("chipDesignId", JsonValue.Of(-1L));
                    batch.Set("designLabId", JsonValue.Of(-1L));
                    batch.Set("count", JsonValue.Of(owned.Int));
                    batch.Set("arrivedTick", JsonValue.Of(0L));
                    fleet.Add(batch);
                }
                JsonPatch.AddIfMissing(lab, "fleet", fleet);
            });
            root.Set("nextId", JsonValue.Of(nextId));
        }

        /// <summary>Schema 3: the world map. One state per site, and for every lab a home bloc, no holdings and a clean standing with every government.</summary>
        static void MigrateTo3(GameConfig c, JsonValue root)
        {
            var sites = JsonValue.Array();
            foreach (SiteConfig site in c.World.Sites)
            {
                JsonValue o = JsonValue.Object();
                o.Set("siteId", JsonValue.Of(site.Id));
                o.Set("heatwaveUntilTick", JsonValue.Of(-1L));
                sites.Add(o);
            }
            JsonPatch.AddIfMissing(root, "sites", sites);

            JsonPatch.ForEach(root, "labs", lab =>
            {
                RivalConfig rival = c.Rival(JsonPatch.StrOr(lab, "rivalId", ""));
                string home = rival != null ? rival.HomeBlocId : c.PlayerStart.HomeBlocId;
                JsonPatch.AddIfMissing(lab, "homeBlocId", JsonValue.Of(home));
                JsonPatch.AddIfMissing(lab, "holdings", JsonValue.Array());
                JsonPatch.AddIfMissing(lab, "incidentsThisMonth", JsonValue.Of(0L));
                var blocs = JsonValue.Array();
                foreach (BlocConfig bloc in c.World.Blocs)
                {
                    JsonValue o = JsonValue.Object();
                    o.Set("blocId", JsonValue.Of(bloc.Id));
                    o.Set("attention", JsonValue.Of(0L));
                    o.Set("level", JsonValue.Of(0L));
                    blocs.Add(o);
                }
                JsonPatch.AddIfMissing(lab, "blocs", blocs);
            });
        }

        /// <summary>
        /// Schema 4: rival depth. Every lab gets an empty decision log, no feeling about the player and no board fit or ambush. Every
        /// hold gets a build start time of -1. The vendor appears with every lab at the starting standing and the month's supply split
        /// evenly. A save that had rivals gets the later-joining rivals of the config as pending; a game with none (a solo game) stays empty.
        /// </summary>
        static void MigrateTo4(GameConfig c, JsonValue root)
        {
            long tick = JsonPatch.IntOr(root, "tick", 0);
            int year = GameDate.Year(c.Calendar, (int)tick);
            int supply = c.Vendor.SupplyInYear(year);
            int labCount = 0;
            bool hasRivals = false;
            JsonPatch.ForEach(root, "labs", lab =>
            {
                labCount++;
                if (JsonPatch.StrOr(lab, "controller", "") == "Plan") hasRivals = true;
            });

            var standings = JsonValue.Array();
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "log", JsonValue.Array());
                JsonPatch.AddIfMissing(lab, "feelingToPlayer", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "planPausedUntilTick", JsonValue.Of(-1L));
                JsonPatch.AddIfMissing(lab, "chipAmbushUntilTick", JsonValue.Of(-1L));
                JsonPatch.ForEach(lab, "holdings", holding => JsonPatch.AddIfMissing(holding, "hallStartTick", JsonValue.Of(-1L)));

                JsonValue s = JsonValue.Object();
                s.Set("labId", JsonValue.Of(JsonPatch.IntOr(lab, "id", 0)));
                s.Set("standing", JsonValue.Of((long)c.Vendor.StandingStart));
                s.Set("boughtThisMonth", JsonValue.Of(0L));
                s.Set("allocationThisMonth", JsonValue.Of((long)(supply / labCount)));
                s.Set("sharePermille", JsonValue.Of(0L));
                s.Set("fromBuying", JsonValue.Of(0L));
                s.Set("fromChips", JsonValue.Of(0L));
                s.Set("trainingChipSeen", JsonValue.Of(false));
                standings.Add(s);
            });

            JsonValue vendor = JsonValue.Object();
            vendor.Set("supplyThisMonth", JsonValue.Of((long)supply));
            vendor.Set("standings", standings);
            vendor.Set("log", JsonValue.Array());
            JsonPatch.AddIfMissing(root, "vendor", vendor);

            var pending = JsonValue.Array();
            if (hasRivals)
                foreach (RivalConfig rival in c.Rivals)
                    if (rival.JoinsYear > year && rival.JoinsYear < RivalConfig.NeverJoins) pending.Add(JsonValue.Of(rival.Id));
            JsonPatch.AddIfMissing(root, "pendingRivals", pending);
        }

        /// <summary>Schema 5: each lab gets an empty chip programme, the game an empty foundry, and every vendor standing the listing flag.</summary>
        static void MigrateTo5(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "chipDesigns", JsonValue.Array());
                JsonPatch.AddIfMissing(lab, "softwarePermille", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "chipOrders", JsonValue.Array());
                JsonPatch.AddIfMissing(lab, "chipListings", JsonValue.Array());
            });
            JsonPatch.AddIfMissing(root, "foundry", JsonValue.Array());
            JsonPatch.ForEach(root, "vendor.standings", s => JsonPatch.AddIfMissing(s, "listingSeen", JsonValue.Of(false)));
        }

        static void MigrateTo6(GameConfig c, JsonValue root)
        {
            JsonPatch.AddIfMissing(root, "endings", JsonValue.Array());
            long playerId = 0;
            JsonValue labs = root.Get("labs");
            if (labs != null && labs.Kind == JsonKind.Array && labs.Items.Count > 0)
                playerId = JsonPatch.IntOr(labs.Items[0], "id", 0);
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.AddIfMissing(lab, "frontierLevel", JsonValue.Of(0L)));
            JsonPatch.AddIfMissing(root, "playerLabId", JsonValue.Of(playerId));
            JsonPatch.ForEach(root, "labs", lab =>
            {
                if (JsonPatch.BoolOr(lab, "bankrupt", false) && JsonPatch.StrOr(lab, "controller", "") == "Player")
                {
                    JsonValue record = JsonValue.Object();
                    record.Set("tick", JsonValue.Of(JsonPatch.IntOr(root, "tick", 0)));
                    record.Set("endingId", JsonValue.Of("bankrupt"));
                    record.Set("labId", JsonValue.Of(JsonPatch.IntOr(lab, "id", 0)));
                    record.Set("choice", JsonValue.Of(""));
                    record.Set("choiceTick", JsonValue.Of(-1L));
                    root.Get("endings").Add(record);
                }
            });
        }

        static void MigrateTo7(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "heat", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "stash", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "shadyLockedUntilTick", JsonValue.Of(-1L));
                JsonPatch.AddIfMissing(lab, "caseEndsTick", JsonValue.Of(-1L));
                JsonPatch.AddIfMissing(lab, "shadyOps", JsonValue.Array());
                JsonPatch.ForEach(lab, "models", m => JsonPatch.AddIfMissing(m, "corruption", JsonValue.Of(0L)));
            });
        }

        static void MigrateTo8(GameConfig c, JsonValue root)
        {
            int start = c.Funding.FounderStartPermille;
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "founderSharePermille", JsonValue.Of((long)start));
                JsonPatch.AddIfMissing(lab, "boardInvestorSeats", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "fundingRounds", JsonValue.Array());
            });
        }

        static void MigrateTo9(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs.models", model =>
            {
                JsonValue align = model.Get("alignment");
                int a = align == null ? 0 : (int)align.Int;
                JsonPatch.AddIfMissing(model, "displayedAlignment", JsonValue.Of((long)a));
                JsonPatch.AddIfMissing(model, "lastIncidentRung", JsonValue.Of(0L));
            });
        }

        static void MigrateTo11(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.AddIfMissing(lab, "reservedContracts", JsonValue.Array()));
            JsonPatch.ForEach(root, "vendor.standings", s =>
            {
                JsonPatch.AddIfMissing(s, "queuedGpus", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(s, "queueHoldingId", JsonValue.Of((long)HardwareBatch.Hq));
            });
        }

        /// <summary>Q90 bubble meter (stack2). NOTE: rules-side-items also uses schema 12 for sector pricing — renumber on merge.</summary>
        static void MigrateTo12(JsonValue root)
        {
            JsonPatch.AddIfMissing(root, "bubblePermille", JsonValue.Of(0L));
        }

        /// <summary>Q92 rival public float (stack2).</summary>
        static void MigrateTo13(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.AddIfMissing(lab, "publicFloatPpm", JsonValue.Of(0L)));
        }

        /// <summary>Q93 China state deals (stack2).</summary>
        static void MigrateTo14(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "chinaDealActive", JsonValue.Of(false));
                JsonPatch.AddIfMissing(lab, "chinaDealPending", JsonValue.Of(false));
            });
        }

        /// <summary>Q94 containment + superintelligence endings (stack2, config-only; no new save keys).</summary>
        static void MigrateTo15(JsonValue root) { }

        /// <summary>Q95 player board lock after investor drama (stack2).</summary>
        static void MigrateTo16(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.AddIfMissing(lab, "boardLockedUntilTick", JsonValue.Of(-1L)));
        }

        /// <summary>Q98 gauntlet runner-up streak (stack2).</summary>
        static void MigrateTo17(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.AddIfMissing(lab, "crownRunnerUpStreak", JsonValue.Of(0L)));
        }

        /// <summary>China deal decline cooldown (Opus review).</summary>
        static void MigrateTo18(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.AddIfMissing(lab, "chinaDealDeclinedUntilTick", JsonValue.Of(-1L)));
        }

        /// <summary>Hatch bubble crash phase (Q103).</summary>
        static void MigrateTo19(JsonValue root)
        {
            JsonPatch.AddIfMissing(root, "bubbleCapitalRaised", JsonValue.Of(0L));
            JsonPatch.AddIfMissing(root, "bubbleGraceMonths", JsonValue.Of(0L));
            JsonPatch.AddIfMissing(root, "bubbleCrashMonthsRemaining", JsonValue.Of(0L));
        }

        /// <summary>Lab acquisition flag (Hatch).</summary>
        static void MigrateTo20(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.AddIfMissing(lab, "absorbedByLabId", JsonValue.Of(-1L)));
        }

        /// <summary>Risk classes (Q125): an incident event names its class; old events were the plain mishap.</summary>
        static void MigrateTo21(JsonValue root)
        {
            JsonPatch.ForEach(root, "events", e => JsonPatch.AddIfMissing(e, "riskClass", JsonValue.Of("")));
        }

        /// <summary>Founding choices (Q137): every lab has a founder and a style; saves from before chose neither.</summary>
        static void MigrateTo22(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "founderId", JsonValue.Of(""));
                JsonPatch.AddIfMissing(lab, "styleId", JsonValue.Of(""));
            });
        }

        /// <summary>The talent market (Q141): no offers were ever waiting in an older save.</summary>
        static void MigrateTo23(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.AddIfMissing(lab, "poachOffers", JsonValue.Array()));
        }

        /// <summary>The press (Q145): no journalist was ever waiting in an older save. Schema 24 is left to another thread.</summary>
        static void MigrateTo25(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.AddIfMissing(lab, "pressInquiries", JsonValue.Array()));
        }

        /// <summary>The Gauntlet, deeper (Q160): nobody tuned, no award was given and no entry was lifted in an older save.</summary>
        static void MigrateTo26(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.AddIfMissing(lab, "gauntletTuning", JsonValue.Of(0L)));
            JsonPatch.ForEach(root, "gauntlets", g =>
            {
                JsonPatch.AddIfMissing(g, "confidentLabId", JsonValue.Of(-1L));
                JsonPatch.AddIfMissing(g, "newcomerLabId", JsonValue.Of(-1L));
            });
            JsonPatch.ForEach(root, "gauntlets.entries", e => JsonPatch.AddIfMissing(e, "tuned", JsonValue.Of(false)));
        }

        /// <summary>Sector trust and the customer desk (Q165): every sector sits at the baseline and no complaint ever waited in an older save.</summary>
        static void MigrateTo27(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "sectorTrust", JsonValue.Array());
                JsonPatch.AddIfMissing(lab, "complaints", JsonValue.Array());
            });
        }

        /// <summary>Safety culture and the release gate: an older lab starts at the usual culture with no gate.</summary>
        static void MigrateTo29(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "cultureShift", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "releaseGate", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "gateSetTick", JsonValue.Of(0L));
            });
        }

        static void MigrateTo10(GameConfig c, JsonValue root)
        {
            string trait = StaffSystem.DefaultTrait(c.StaffPeople);
            if (string.IsNullOrEmpty(trait) && c.StaffPeople.Traits.Count > 0) trait = c.StaffPeople.Traits[0].Id;
            int morale = c.StaffPeople.MoraleBaseline;
            long nextId = JsonPatch.IntOr(root, "nextId", 1);
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonValue pools = lab.Get("staff");
                var members = JsonValue.Array();
                if (pools != null && pools.Kind == JsonKind.Array)
                {
                    for (int i = 0; i < pools.Items.Count; i++)
                    {
                        JsonValue pool = pools.Items[i];
                        JsonValue roleNode = pool.Get("roleId");
                        JsonValue countNode = pool.Get("count");
                        if (roleNode == null || countNode == null) continue;
                        string roleId = roleNode.Str;
                        int count = (int)countNode.Int;
                        for (int k = 0; k < count; k++)
                        {
                            JsonValue m = JsonValue.Object();
                            m.Set("id", JsonValue.Of(nextId++));
                            m.Set("roleId", JsonValue.Of(roleId));
                            m.Set("traitId", JsonValue.Of(trait));
                            m.Set("morale", JsonValue.Of((long)morale));
                            members.Add(m);
                        }
                    }
                }
                lab.Set("staff", members);
            });
            root.Set("nextId", JsonValue.Of(nextId));
        }

        static JsonValue StreamAt(long state)
        {
            JsonValue o = JsonValue.Object();
            o.Set("state", JsonValue.Of(state));
            return o;
        }
    }
}
