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
            if (fromSchema < 30) MigrateTo30(root);
            if (fromSchema < 31) MigrateTo31(root);
            if (fromSchema < 32) MigrateTo32(root);
            if (fromSchema < 33) MigrateTo33(root);
            if (fromSchema < 34) MigrateTo34(root);
            if (fromSchema < 35) MigrateTo35(root);
            if (fromSchema < 36) MigrateTo36(root);
            if (fromSchema < 37) MigrateTo37(root);
            if (fromSchema < 38) MigrateTo38(root);
            if (fromSchema < 39) MigrateTo39(root);
            if (fromSchema < 40) MigrateTo40(root);
            if (fromSchema < 41) MigrateTo41(root);
            if (fromSchema < 42) MigrateTo42(root);
            if (fromSchema < 43) MigrateTo43(root);
            if (fromSchema < 44) MigrateTo44(root);
            if (fromSchema < 45) MigrateTo45(root);
            if (fromSchema < 46) MigrateTo46(root);
            if (fromSchema < 47) MigrateTo47(root);
            if (fromSchema < 48) MigrateTo48(root);
            if (fromSchema < 49) MigrateTo49(root);
            if (fromSchema < 50) MigrateTo50(root);
            if (fromSchema < 51) MigrateTo51(root);
            if (fromSchema < 52) MigrateTo52(root);
            if (fromSchema < 53) MigrateTo53(root);
            if (fromSchema < 54) MigrateTo54(root);
            if (fromSchema < 55) MigrateTo55(root);
            if (fromSchema < 56) MigrateTo56(root);
            if (fromSchema < 57) MigrateTo57(root);
            if (fromSchema < 58) MigrateTo58(root);
            if (fromSchema < 59) MigrateTo59(root);
            if (fromSchema < 60) MigrateTo60(root);
            if (fromSchema < 61) MigrateTo61(root);
            if (fromSchema < 62) MigrateTo62(root);
            if (fromSchema < 63) MigrateTo63(root);
            if (fromSchema < 64) MigrateTo64(root);
            if (fromSchema < 65) MigrateTo65(root);
            if (fromSchema < 66) MigrateTo66(root);
            if (fromSchema < 67) MigrateTo67(root);
            if (fromSchema < 68) MigrateTo68(root);
            if (fromSchema < 69) MigrateTo69(root);
            if (fromSchema < 70) MigrateTo70(root);
            if (fromSchema < 71) MigrateTo71(root);
            if (fromSchema < 72) MigrateTo72(root);
            if (fromSchema < 73) MigrateTo73(root);
            if (fromSchema < 74) MigrateTo74(root);
            if (fromSchema < 75) MigrateTo75(root);
            if (fromSchema < 76) MigrateTo76(root);
            if (fromSchema < 77) MigrateTo77(root);
            if (fromSchema < 78) MigrateTo78(root);
            if (fromSchema < 79) MigrateTo79(root);
            if (fromSchema < 80) MigrateTo80(root);
            if (fromSchema < 81) MigrateTo81(root);
            if (fromSchema < 82) MigrateTo82(root);
            if (fromSchema < 83) MigrateTo83(root);
            if (fromSchema < 84) MigrateTo84(root);
            if (fromSchema < 85) MigrateTo85(root);
            if (fromSchema < 86) MigrateTo86(root);
            if (fromSchema < 87) MigrateTo87(root);
            if (fromSchema < 88) MigrateTo88(root);
            if (fromSchema < 89) MigrateTo89(root);
            if (fromSchema < 90) MigrateTo90(root);
            if (fromSchema < 91) MigrateTo91(root);
            if (fromSchema < 92) MigrateTo92(root);
            if (fromSchema < 93) MigrateTo93(root);
            if (fromSchema < 94) MigrateTo94(root);
            if (fromSchema < 95) MigrateTo95(root);
            if (fromSchema < 96) MigrateTo96(root);
            if (fromSchema < 97) MigrateTo97(root);
            if (fromSchema < 98) MigrateTo98(root);
            if (fromSchema < 99) MigrateTo99(root);
            if (fromSchema < 100) MigrateTo100(root);
            if (fromSchema < 101) MigrateTo101(root);
            if (fromSchema < 102) MigrateTo102(root);
            if (fromSchema < 103) MigrateTo103(root);
            if (fromSchema < 104) MigrateTo104(root);
            if (fromSchema < 105) MigrateTo105(root);
            if (fromSchema < 106) MigrateTo106(root);
            if (fromSchema < 107) MigrateTo107(root);
            if (fromSchema < 108) MigrateTo108(root);
            if (fromSchema < 109) MigrateTo109(root);
            if (fromSchema < 110) MigrateTo110(root);
            if (fromSchema < 111) MigrateTo111(root);
            if (fromSchema < 112) MigrateTo112(root);
            if (fromSchema < 113) MigrateTo113(root);
            if (fromSchema < 114) MigrateTo114(root);
            if (fromSchema < 115) MigrateTo115(root);
            if (fromSchema < 116) MigrateTo116(root);
            if (fromSchema < 117) MigrateTo117(root);
            if (fromSchema < 118) MigrateTo118(root);
            if (fromSchema < 119) MigrateTo119(root);
            if (fromSchema < 120) MigrateTo120(root);
            if (fromSchema < 121) MigrateTo121(root);
            if (fromSchema < 122) MigrateTo122(root);
            if (fromSchema < 123) MigrateTo123(root);
            if (fromSchema < 124) MigrateTo124(root);
            if (fromSchema < 125) MigrateTo125(root);
            if (fromSchema < 126) MigrateTo126(root);
            if (fromSchema < 127) MigrateTo127(root);
            if (fromSchema < 128) MigrateTo128(root);
            if (fromSchema < 129) MigrateTo129(root);
            if (fromSchema < 130) MigrateTo130(root);
            if (fromSchema < 131) MigrateTo131(root);
            if (fromSchema < 132) MigrateTo132(root);
            if (fromSchema < 133) MigrateTo133(root);
            if (fromSchema < 134) MigrateTo134(root);
            if (fromSchema < 135) MigrateTo135(root);
            if (fromSchema < 136) MigrateTo136(root);
            if (fromSchema < 137) MigrateTo137(root);
            if (fromSchema < 138) MigrateTo138(root);
            if (fromSchema < 139) MigrateTo139(root);
            if (fromSchema < 140) MigrateTo140(root);
            if (fromSchema < 141) MigrateTo141(root);
            if (fromSchema < 142) MigrateTo142(root);
            if (fromSchema < 143) MigrateTo143(root);
            if (fromSchema < 144) MigrateTo144(root);
            if (fromSchema < 145) MigrateTo145(root);
            if (fromSchema < 146) MigrateTo146(root);
            if (fromSchema < 147) MigrateTo147(root);
            if (fromSchema < 148) MigrateTo148(root);
            if (fromSchema < 149) MigrateTo149(root);
            if (fromSchema < 150) MigrateTo150(root);
            if (fromSchema < 151) MigrateTo151(root);
            if (fromSchema < 152) MigrateTo152(root);
            if (fromSchema < 153) MigrateTo153(root);
            if (fromSchema < 154) MigrateTo154(root);
            if (fromSchema < 155) MigrateTo155(root);
            if (fromSchema < 156) MigrateTo156(root);
            if (fromSchema < 157) MigrateTo157(root);
            if (fromSchema < 158) MigrateTo158(root);
            if (fromSchema < 159) MigrateTo159(root);
            if (fromSchema < 160) MigrateTo160(root);
            if (fromSchema < 161) MigrateTo161(root);
            if (fromSchema < 162) MigrateTo162(root);
            if (fromSchema < 163) MigrateTo163(root);
            if (fromSchema < 164) MigrateTo164(root);
            if (fromSchema < 165) MigrateTo165(root);
            if (fromSchema < 166) MigrateTo166(root);
            if (fromSchema < 167) MigrateTo167(root);
            if (fromSchema < 168) MigrateTo168(root);
            if (fromSchema < 169) MigrateTo169(root);
            if (fromSchema < 170) MigrateTo170(root);
            if (fromSchema < 171) MigrateTo171(root);
            if (fromSchema < 172) MigrateTo172(root);
            if (fromSchema < 173) MigrateTo173(root);
            if (fromSchema < 174) MigrateTo174(root);
            if (fromSchema < 175) MigrateTo175(root);
            if (fromSchema < 176) MigrateTo176(root);
            if (fromSchema < 177) MigrateTo177(root);
            if (fromSchema < 178) MigrateTo178(root);
            if (fromSchema < 179) MigrateTo179(root);
            if (fromSchema < 180) MigrateTo180(root);
            if (fromSchema < 181) MigrateTo181(root);
            if (fromSchema < 182) MigrateTo182(root);
            if (fromSchema < 183) MigrateTo183(root);
            if (fromSchema < 184) MigrateTo184(root);
            if (fromSchema < 185) MigrateTo185(root);
            if (fromSchema < 186) MigrateTo186(root);
            if (fromSchema < 187) MigrateTo187(root);
            if (fromSchema < 188) MigrateTo188(root);
            if (fromSchema < 189) MigrateTo189(root);
            if (fromSchema < 190) MigrateTo190(root);
            if (fromSchema < 191) MigrateTo191(root);
            if (fromSchema < 192) MigrateTo192(root);
            if (fromSchema < 193) MigrateTo193(root);
            if (fromSchema < 194) MigrateTo194(root);
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

        /// <summary>Schema 30: the version naming mess, tidy to start with.</summary>
        static void MigrateTo30(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.AddIfMissing(lab, "namingMess", JsonValue.Of(0L)));
        }

        /// <summary>Schema 51: no model was ever audited from outside.</summary>
        /// <summary>Schema 55: no lab has optimised its inference or run a marketing campaign.</summary>
        static void MigrateTo55(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "optimiseLevel", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "campaignSector", JsonValue.Of(""));
                JsonPatch.AddIfMissing(lab, "campaignLevel", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "campaignWeeks", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "hangoverWeeks", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "campaignLockedUntil", JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 138: no per-sector price tiers.</summary>
        static void MigrateTo138(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "priceSectors", JsonValue.Array());
                JsonPatch.AddIfMissing(lab, "priceTiers", JsonValue.Array());
            });
        }

        /// <summary>Schema 139: sandbox deadline off.</summary>
        static void MigrateTo139(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.AddIfMissing(lab, "campaignDeadlineTick", JsonValue.Of(0L)));
        }

        /// <summary>Schema 140: no law shaping.</summary>
        static void MigrateTo140(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "lawShapePending", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "lawShapeDelta", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "lawShapeEra", JsonValue.Of(""));
            });
        }

        /// <summary>Schema 141: exit debt display only.</summary>
        static void MigrateTo141(JsonValue root) { }

        /// <summary>Schema 142: no takeoff cooldown.</summary>
        static void MigrateTo142(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.AddIfMissing(lab, "takeoffCooldownUntil", JsonValue.Of(0L)));
        }

        /// <summary>Schema 143: no baggage.</summary>
        static void MigrateTo143(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "baggageId", JsonValue.Of(""));
                JsonPatch.AddIfMissing(lab, "baggagePicked", JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 144: no margin call clock.</summary>
        static void MigrateTo144(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.AddIfMissing(lab, "marginCallLeft", JsonValue.Of(0L)));
        }

        /// <summary>Schema 145: hearing share nudge is stateless.</summary>
        static void MigrateTo145(JsonValue root) { }

        /// <summary>Schema 146: no rung-six arc.</summary>
        static void MigrateTo146(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "rungSixArcUntil", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "rungSixCooldownUntil", JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 147: no scenario goal.</summary>
        static void MigrateTo147(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "scenarioId", JsonValue.Of(""));
                JsonPatch.AddIfMissing(lab, "scenarioGoalResult", JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 148: no law target.</summary>
        static void MigrateTo148(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.AddIfMissing(lab, "lawTarget", JsonValue.Of("")));
        }

        static void MigrateTo149(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.AddIfMissing(lab, "reservedSignCooldownUntil", JsonValue.Of(0L)));
        }

        static void MigrateTo150(JsonValue root)
        {
            JsonPatch.AddIfMissing(root, "rivalIntentLastTick", JsonValue.Of(-1L));
        }

        static void MigrateTo151(JsonValue root) { }

        static void MigrateTo152(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.AddIfMissing(lab, "scenarioEndlessAck", JsonValue.Of(0L)));
        }

        static void MigrateTo153(JsonValue root) { }

        static void MigrateTo154(JsonValue root) { }

        static void MigrateTo155(JsonValue root) { }

        static void MigrateTo156(JsonValue root) { }

        static void MigrateTo157(JsonValue root) { }

        static void MigrateTo158(JsonValue root) { }

        static void MigrateTo159(JsonValue root) { }

        static void MigrateTo160(JsonValue root) { }

        static void MigrateTo161(JsonValue root) { }

        static void MigrateTo162(JsonValue root) { }

        static void MigrateTo163(JsonValue root) { }

        static void MigrateTo164(JsonValue root) { }

        static void MigrateTo165(JsonValue root) { }

        static void MigrateTo166(JsonValue root) { }

        static void MigrateTo167(JsonValue root) { }

        static void MigrateTo168(JsonValue root) { }

        static void MigrateTo169(JsonValue root) { }

        static void MigrateTo170(JsonValue root) { }

        static void MigrateTo171(JsonValue root) { }

        static void MigrateTo172(JsonValue root) { }

        /// <summary>Schema 173: starter equipment tier.</summary>
        static void MigrateTo173(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.AddIfMissing(lab, "computeEquipTier", JsonValue.Of(0L)));
        }

        static void MigrateTo174(JsonValue root) { }

        static void MigrateTo175(JsonValue root) { }

        static void MigrateTo176(JsonValue root) { }

        static void MigrateTo177(JsonValue root) { }

        static void MigrateTo178(JsonValue root) { }

        static void MigrateTo179(JsonValue root) { }

        static void MigrateTo180(JsonValue root) { }

        static void MigrateTo181(JsonValue root) { }

        static void MigrateTo182(JsonValue root) { }

        static void MigrateTo183(JsonValue root) { }

        static void MigrateTo184(JsonValue root) { }

        static void MigrateTo185(JsonValue root) { }

        static void MigrateTo186(JsonValue root) { }

        static void MigrateTo187(JsonValue root) { }

        static void MigrateTo188(JsonValue root) { }

        static void MigrateTo189(JsonValue root) { }

        static void MigrateTo190(JsonValue root) { }

        static void MigrateTo191(JsonValue root) { }

        static void MigrateTo192(JsonValue root) { }

        static void MigrateTo193(JsonValue root) { }

        static void MigrateTo194(JsonValue root) { }

        /// <summary>Schema 137: weights closed.</summary>
        static void MigrateTo137(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "models", m => JsonPatch.AddIfMissing(m, "wgtOpen", JsonValue.Of(0L))));
        }

        /// <summary>Schema 136: no active data sources.</summary>
        static void MigrateTo136(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "srcWeeks0", "srcWeeks1", "srcWeeks2", "srcWeeks3" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 135: no specialist branch.</summary>
        static void MigrateTo135(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "models", m => JsonPatch.AddIfMissing(m, "specKind", JsonValue.Of(0L))));
        }

        /// <summary>Schema 134: no model publishes public metrics.</summary>
        static void MigrateTo134(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.ForEach(lab, "models", m =>
                {
                    foreach (string key in new[] { "pubOn", "pubScore", "pubMiss" })
                        JsonPatch.AddIfMissing(m, key, JsonValue.Of(0L));
                });
            });
        }

        /// <summary>Schema 133: no model has a traffic route.</summary>
        static void MigrateTo133(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.ForEach(lab, "models", m => JsonPatch.AddIfMissing(m, "routePpm", JsonValue.Of(0L)));
            });
        }

        /// <summary>Schema 132: no lab has an orchestration budget.</summary>
        static void MigrateTo132(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "orchLevel", "orchOver" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 131: no lab has a runaway playbook.</summary>
        static void MigrateTo131(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "playLevel", "playArmed", "playLeft", "playArmUntil" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 130: no model is in an agent sandbox.</summary>
        static void MigrateTo130(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.ForEach(lab, "models", m =>
                {
                    foreach (string key in new[] { "sandLeft", "sandGrad" })
                        JsonPatch.AddIfMissing(m, key, JsonValue.Of(0L));
                });
            });
        }

        /// <summary>Schema 129: no model has a release checklist.</summary>
        static void MigrateTo129(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.ForEach(lab, "models", m =>
                {
                    foreach (string key in new[] { "chkDone", "chkMissed" })
                        JsonPatch.AddIfMissing(m, key, JsonValue.Of(0L));
                });
            });
        }

        /// <summary>Schema 128: no lab has a human queue.</summary>
        static void MigrateTo128(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "humanQueue", "humanBusy", "humanCleared" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 127: no model has a tool policy.</summary>
        static void MigrateTo127(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.ForEach(lab, "models", m =>
                {
                    foreach (string key in new[] { "toolMask" })
                        JsonPatch.AddIfMissing(m, key, JsonValue.Of(0L));
                });
            });
        }

        /// <summary>Schema 126: no model is on a public eval cadence.</summary>
        static void MigrateTo126(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.ForEach(lab, "models", m =>
                {
                    foreach (string key in new[] { "evalSuite", "evalLeft", "evalScore", "evalRuns", "evalStreak" })
                        JsonPatch.AddIfMissing(m, key, JsonValue.Of(0L));
                });
            });
        }

        /// <summary>Schema 125: no lab has run an incident drill.</summary>
        static void MigrateTo125(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "drillPrep", "drillLeft", "drillDue", "drillPassed", "drillFailed" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 124: no lab holds board meetings.</summary>
        static void MigrateTo124(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "meetingsOn", "boardTension", "motionPend", "motionPendGoal", "motionAnswerBy", "motionKind", "motionGoal", "motionDue", "motionNext", "motionsTabled", "motionsKept", "motionsBroken", "motionsDeclined" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 123: no model has an owner or documentation.</summary>
        static void MigrateTo123(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.ForEach(lab, "models", m =>
                {
                    foreach (string key in new[] { "ownerId", "docLevel", "docAge", "knowGap", "handovers" })
                        JsonPatch.AddIfMissing(m, key, JsonValue.Of(0L));
                });
            });
        }

        /// <summary>Schema 122: no model has been tuned after training.</summary>
        static void MigrateTo122(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.ForEach(lab, "models", m =>
                {
                    foreach (string key in new[] { "postKind", "postLeft", "postPref", "postSafety" })
                        JsonPatch.AddIfMissing(m, key, JsonValue.Of(0L));
                });
            });
        }

        /// <summary>Schema 121: no lab has an annotation vendor.</summary>
        static void MigrateTo121(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "annVendor", "annLeft", "annExposure", "annStrikes", "annAuditWeek", "annCleaned" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 120: no model has agent autonomy.</summary>
        static void MigrateTo120(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "agentEarned", JsonValue.Of(0L));
                JsonPatch.ForEach(lab, "models", m =>
                {
                    foreach (string key in new[] { "agentLevel", "agentSince", "agentStreak", "agentBlowups" })
                        JsonPatch.AddIfMissing(m, key, JsonValue.Of(0L));
                });
            });
        }

        /// <summary>Schema 119: no lab runs spot capacity.</summary>
        static void MigrateTo119(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "spotPermille", "spotSince", "spotSaved", "spotPreempts", "spotLost", "spotOut", "spotWaves" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 118: no lab has bought a funding round back.</summary>
        static void MigrateTo118(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "boughtRounds", JsonValue.Array());
                foreach (string key in new[] { "boardVoteWeek", "boardWon", "boardLost" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 117: every lab is an ordinary company with no independent directors.</summary>
        static void MigrateTo117(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "govForm", "govSince", "govDirectors", "govSearch", "govResigned" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 116: no lab collects its users data.</summary>
        static void MigrateTo116(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "userPolicy", "userSince", "userBanked", "userTotal", "userComplaints" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 115: no lab has any data.</summary>
        static void MigrateTo115(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "dataRaw1", "dataRaw2", "dataRaw3", "dataRaw4", "dataClean1", "dataClean2", "dataClean3", "dataClean4", "dataJobKind", "dataJobUnits", "dataJobLeft", "dataSuits", "dataBackfilled" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 114: no lab has joined the register.</summary>
        static void MigrateTo114(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "discOn", "discOpen", "discLeft", "discPrep", "discFindings", "discFiled", "discInspect", "discModel" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 113: no lab has a fabric or is building one.</summary>
        static void MigrateTo113(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "fabTier", "fabBuild", "fabLeft", "fabGpus", "fabBuilt", "fabCancelled" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 112: no lab has chosen a structure.</summary>
        static void MigrateTo112(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "orgForm", "orgSince", "orgDisrupt", "orgReorgs" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 111: no model is in a conformity assessment.</summary>
        static void MigrateTo111(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "cfGranted", "cfFindings", "cfSuspended" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
                JsonPatch.ForEach(lab, "models", m =>
                {
                    foreach (string key in new[] { "certStage", "certLeft", "certScope", "certTries" })
                        JsonPatch.AddIfMissing(m, key, JsonValue.Of(0L));
                });
            });
        }

        /// <summary>Schema 110: no model has an open API or any integrations.</summary>
        static void MigrateTo110(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "apiMoved", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "apiLost", JsonValue.Of(0L));
                JsonPatch.ForEach(lab, "models", m =>
                {
                    foreach (string key in new[] { "integrations", "apiState", "apiLeft", "apiSuccessor" })
                        JsonPatch.AddIfMissing(m, key, JsonValue.Of(0L));
                });
            });
        }

        /// <summary>Schema 109: no model is under a support pledge.</summary>
        static void MigrateTo109(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "ltsKept", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "ltsBroken", JsonValue.Of(0L));
                JsonPatch.ForEach(lab, "models", m => JsonPatch.AddIfMissing(m, "ltsLeft", JsonValue.Of(0L)));
            });
        }

        /// <summary>Schema 108: no lab holds or has applied for a licence.</summary>
        static void MigrateTo108(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "licRegime", "licPhase", "licLeft", "licStrikes", "licCooldown", "licGranted", "licRefused", "licSuspended" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 107: no lab has a project pod.</summary>
        static void MigrateTo107(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "podIds", JsonValue.Array());
                foreach (string key in new[] { "podWeeks", "podChem", "podSwapAfter", "podPoints", "podsFormed" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 106: no training run has a queue level.</summary>
        static void MigrateTo106(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "models", m => JsonPatch.AddIfMissing(m, "queueLevel", JsonValue.Of(0L))));
        }

        /// <summary>Schema 105: no lab runs a research programme.</summary>
        static void MigrateTo105(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "pgTech", "pgDone", "pgPhase", "pgLeft", "pgBold", "pgInsight", "pgSpent", "pgWins", "pgLosses" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 104: no lab keeps a launch watch desk.</summary>
        static void MigrateTo104(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "lwOn", "lwRival", "lwLead", "lwWait", "lwHits", "lwAnswers" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 103: no lab runs a free tier.</summary>
        static void MigrateTo103(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "ftLevel", "ftReach", "ftWeeks", "ftSpent", "ftEarned" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 102: no lab keeps a careers page.</summary>
        static void MigrateTo102(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "brandOn", "brandScore", "brandReview", "brandLow", "brandReviews" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 101: no lab has a waitlist open.</summary>
        static void MigrateTo101(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "wlModel", "wlHype", "wlLeft", "wlWins", "wlFlops" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 100: no lab has a public bet on a tech.</summary>
        static void MigrateTo100(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "tbRef", "tbLeft", "tbWins", "tbLosses" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 99: no lab has a paper waiting for a decision about its code.</summary>
        static void MigrateTo99(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "replPending", "replWait", "replLeft", "replRigor", "replWins", "replFails" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 98: no lab has a shock waiting.</summary>
        static void MigrateTo98(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "shockKind", "shockRef", "shockLeft", "shockBraced", "shockRode", "shockCountered" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 97: no lab has stated its values.</summary>
        static void MigrateTo97(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "creedA", "creedB", "creedIntegrity", "creedBand", "creedLock", "creedTown", "creedKept", "creedBroken" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 96: no lab has a deal with a platform.</summary>
        static void MigrateTo96(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "platPlat", "platLeft", "platCut", "platRenewals", "platReliance", "platHoldup", "platPaid", "platKept" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 95: no lab has a device line.</summary>
        static void MigrateTo95(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "devLine", "devStage", "devWeeks", "devQuality", "devTier", "devSold", "devBase" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 94: no lab has a convertible note.</summary>
        static void MigrateTo94(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "noteOwed", "notePrincipal", "noteRate", "noteDiscount", "noteLeft", "noteExtended", "noteTier" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 93: no lab has sold its GPUs and leased them back.</summary>
        static void MigrateTo93(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "leaseUnits", "leaseValue", "leaseRent", "leaseBuyback", "leaseLeft", "leaseMissed", "leaseExtended" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 92: no lab has a foundation.</summary>
        static void MigrateTo92(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "foundFund", "foundGiven", "foundFocus", "foundPayout", "foundGoodwill", "foundSwitch", "foundDrip" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 91: no lab has a revenue advance.</summary>
        static void MigrateTo91(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "rbfAdvance", "rbfOwed", "rbfBase", "rbfShare", "rbfTier", "rbfWeeks", "rbfSlump" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 90: no lab is listed.</summary>
        static void MigrateTo90(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "listed", "listPrice", "listGuidance", "listTarget", "listQuarter", "listBase", "listNext", "listKeep", "listStreak", "listFollowOn", "listActivist" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 89: no lab has a capital allowance policy or pool.</summary>
        static void MigrateTo89(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "capPolicy", "capPool", "capSwitchTick", "capWritten" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 88: no lab is in breach of a covenant or has used a waiver.</summary>
        static void MigrateTo88(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "covBreach", "covWaiverUntil", "covWaivers", "covCalls" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 87: no lab has a warranty or spares and no GPU has failed.</summary>
        static void MigrateTo87(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "warrantyTier", "spares", "failCarry", "rmaUnits", "rmaDue", "gpuFailures" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 86: no standard has been drafted and no lab has a part in one.</summary>
        static void MigrateTo86(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "stdLevel", "stdDraft", "stdSupport", "stdRole", "stdNext", "stdAdopted", "stdLapses" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 85: no lab pays corporate tax or keeps a tax structure.</summary>
        static void MigrateTo85(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "taxStructure", "taxBuilding", "taxDue", "taxAccrued", "taxSaved", "taxResearch", "taxCredit", "taxClaimed", "taxExposure", "taxAuditDue", "taxAuditOwed" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 84: no lab has a scaling policy.</summary>
        static void MigrateTo84(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "policyLevel", "policyLockedUntil", "policyHold", "policyBreaks" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 83: every lab is a for-profit company with no change of charter under review.</summary>
        static void MigrateTo83(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "charter", "charterTo", "charterDue", "capPaid" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 82: no lab has a government contract or an offer.</summary>
        static void MigrateTo82(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "contractKind", JsonValue.Of(""));
                JsonPatch.AddIfMissing(lab, "offerKind", JsonValue.Of(""));
                foreach (string key in new[] { "contractStart", "contractDue", "contractPaid", "contractMissed", "offerDue", "offerSeq", "contractNext" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 81: nobody has been sued and every lab starts with no security built.</summary>
        static void MigrateTo81(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "cases", JsonValue.Array());
                foreach (string key in new[] { "caseSeq", "securityTier", "securityBuilding", "securityDue", "exposure", "breachDue", "ransoms" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 80: nobody has open orders or visiting scholars.</summary>
        static void MigrateTo80(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "requisitions", JsonValue.Array());
                JsonPatch.AddIfMissing(lab, "scholarCount", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "scholarStart", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "scholarDue", JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 79: nobody is on a sabbatical.</summary>
        static void MigrateTo79(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "staff", person =>
            {
                JsonPatch.AddIfMissing(person, "leaveDue", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(person, "leaveMorale", JsonValue.Of(0L));
            }));
        }

        /// <summary>Schema 78: nobody has a support desk.</summary>
        static void MigrateTo78(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "supportPeople", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "supportBacklog", JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 77: nobody has interns.</summary>
        static void MigrateTo77(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "internRole", JsonValue.Of(""));
                JsonPatch.AddIfMissing(lab, "internCount", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "internStart", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "internDue", JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 76: nobody has left a lab into the alumni pool.</summary>
        static void MigrateTo76(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.AddIfMissing(lab, "alumni", JsonValue.Array()));
        }

        /// <summary>Schema 75: no lab has a paper out for review.</summary>
        static void MigrateTo75(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.AddIfMissing(lab, "submissions", JsonValue.Array()));
        }

        /// <summary>Schema 74: no lab has account teams.</summary>
        static void MigrateTo74(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.AddIfMissing(lab, "accountTeams", JsonValue.Array()));
        }

        /// <summary>Schema 73: no model has an end-of-life notice.</summary>
        static void MigrateTo73(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "models", m =>
            {
                JsonPatch.AddIfMissing(m, "sunsetStart", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(m, "sunsetTick", JsonValue.Of(0L));
            }));
        }

        /// <summary>Schema 72: no lab has organised.</summary>
        static void MigrateTo72(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "labourStage", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "labourPressure", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "labourDue", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "labourHard", JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 71: no lab runs a skunkworks project.</summary>
        static void MigrateTo71(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "skunkKind", JsonValue.Of(""));
                JsonPatch.AddIfMissing(lab, "skunkPeople", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "skunkStart", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "skunkDue", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "skunkWork", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "skunkLockedUntil", JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 70: nobody is being mentored.</summary>
        static void MigrateTo70(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "staff", person =>
            {
                JsonPatch.AddIfMissing(person, "mentorId", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(person, "mentorWeeks", JsonValue.Of(0L));
            }));
        }

        /// <summary>Schema 69: no lab has announced a roadmap.</summary>
        static void MigrateTo69(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "roadmapLevel", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "roadmapStart", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "roadmapDue", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "roadmapGoal", JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 68: no lab has made a safety pledge.</summary>
        static void MigrateTo68(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "pledgeLevel", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "pledgeLockedUntil", JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 67: no lab has set cash aside.</summary>
        static void MigrateTo67(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.AddIfMissing(lab, "reserve", JsonValue.Of(0L)));
        }

        /// <summary>Schema 66: no lab runs a bug bounty.</summary>
        static void MigrateTo66(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "bountyLevel", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "bountyLockedUntil", JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 65: no lab has a whistleblower's concern open or a silence bought.</summary>
        static void MigrateTo65(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "whistleWeeks", "whistleModel", "whistleDue", "silenced" }) JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 64: no lab has endowed a chair.</summary>
        static void MigrateTo64(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.AddIfMissing(lab, "chairs", JsonValue.Of(0L)));
        }

        /// <summary>Schema 63: no lab has committed to a revenue target.</summary>
        static void MigrateTo63(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "revenueTargetLevel", "revenueTargetEnd", "revenueTargetGoal", "revenueTargetSoFar" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 62: nobody has been promoted to senior.</summary>
        static void MigrateTo62(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "staff", person => JsonPatch.AddIfMissing(person, "senior", JsonValue.Of(0L))));
        }

        /// <summary>Schema 61: no lab has any technical debt.</summary>
        static void MigrateTo61(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.AddIfMissing(lab, "techDebt", JsonValue.Of(0L)));
        }

        /// <summary>Schema 60: no lab has applied for a research grant.</summary>
        static void MigrateTo60(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                foreach (string key in new[] { "researchGrantStage", "researchGrantTier", "researchGrantTick", "researchGrantOwed", "researchGrantLockedUntil" })
                    JsonPatch.AddIfMissing(lab, key, JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 59: no lab has made a service-level pledge.</summary>
        static void MigrateTo59(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "slaLevel", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "slaLockedUntil", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "slaStrikes", JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 58: no incident was ever written up.</summary>
        static void MigrateTo58(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "models", m =>
            {
                JsonPatch.AddIfMissing(m, "incidentRepLoss", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(m, "postMortemDone", JsonValue.Of(0L));
            }));
        }

        /// <summary>Schema 57: no lab has hired policy consultants.</summary>
        static void MigrateTo57(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "consultantLevel", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "consultantWeeks", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "consultantLockedUntil", JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 56: no lab has an on-call rota.</summary>
        static void MigrateTo56(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "onCallLevel", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "onCallLockedUntil", JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 54: no lab has named a chief or seated an ethics board.</summary>
        static void MigrateTo54(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "ethicsLevel", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "ethicsLockedUntil", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "chiefScientistId", JsonValue.Of(-1L));
                JsonPatch.AddIfMissing(lab, "chiefSafetyId", JsonValue.Of(-1L));
            });
        }

        /// <summary>Schema 53: no lab gave its team equity.</summary>
        static void MigrateTo53(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "equityLevel", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "equityPermille", JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 52: no lab has picked a research focus or held a hack week.</summary>
        static void MigrateTo52(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "researchFocus", JsonValue.Of(""));
                JsonPatch.AddIfMissing(lab, "focusLockedUntil", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "hackWeeks", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "hackLockedUntil", JsonValue.Of(0L));
            });
        }

        static void MigrateTo51(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "models", m => JsonPatch.AddIfMissing(m, "auditedUntil", JsonValue.Of(0L))));
        }

        /// <summary>Schema 50: no lab sublet compute, so every ledger has an empty sublet line.</summary>
        static void MigrateTo50(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "subletPermille", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "subletLockedUntil", JsonValue.Of(0L));
            });
            foreach (string place in LedgerPlaces)
                JsonPatch.ForEach(root, place, totals => JsonPatch.AddIfMissing(totals, "sublet", JsonValue.Of(0L)));
        }

        /// <summary>Schema 49: no lab had published a paper.</summary>
        static void MigrateTo49(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.AddIfMissing(lab, "prestige", JsonValue.Of(0L)));
        }

        /// <summary>Schema 48: no lab ran a compliance office.</summary>
        static void MigrateTo48(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.AddIfMissing(lab, "complianceLevel", JsonValue.Of(0L)));
        }

        /// <summary>Schema 47: every lab paid market rates and had never changed its pay.</summary>
        static void MigrateTo47(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "payLevel", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "payLockedUntil", JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 46: no lab ran an interpretability programme.</summary>
        static void MigrateTo46(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.AddIfMissing(lab, "interpLevel", JsonValue.Of(0L)));
        }

        /// <summary>Schema 45: no model was ever red-teamed.</summary>
        static void MigrateTo45(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "models", m => JsonPatch.AddIfMissing(m, "redTeamUntil", JsonValue.Of(0L))));
        }

        /// <summary>Schema 44: no lab had insurance, and the ledger gained a premium line (zero, so cash is still explained).</summary>
        static void MigrateTo44(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "insuranceLevel", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "insuranceClaims", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "insuranceLockedUntil", JsonValue.Of(0L));
            });
            foreach (string place in LedgerPlaces)
                JsonPatch.ForEach(root, place, totals => JsonPatch.AddIfMissing(totals, "insurance", JsonValue.Of(0L)));
        }

        public static readonly string[] LedgerPlaces = { "labs.ledger.thisWeek", "labs.ledger.lastWeek", "labs.ledger.thisMonth", "labs.ledger.lifetime", "labs.ledger.history.totals" };

        /// <summary>Schema 43: no team was in crunch time.</summary>
        static void MigrateTo43(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.AddIfMissing(lab, "crunchWeeks", JsonValue.Of(0L)));
        }

        /// <summary>Schema 42: no lab had set compute aside for research experiments.</summary>
        static void MigrateTo42(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.AddIfMissing(lab, "researchSharePermille", JsonValue.Of(0L)));
        }

        /// <summary>Schema 41: no lab had borrowed.</summary>
        static void MigrateTo41(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.AddIfMissing(lab, "loans", JsonValue.Array()));
        }

        /// <summary>Schema 40: every run was built the known way with checkpoints now and then.</summary>
        static void MigrateTo40(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "models", m =>
            {
                JsonPatch.AddIfMissing(m, "approach", JsonValue.Of(""));
                JsonPatch.AddIfMissing(m, "checkpoints", JsonValue.Of(""));
            }));
        }

        /// <summary>Schema 39: no era had been scored.</summary>
        static void MigrateTo39(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.AddIfMissing(lab, "eraScores", JsonValue.Array()));
        }

        /// <summary>Schema 38: no lab had pivoted.</summary>
        static void MigrateTo38(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "pivotEraId", JsonValue.Of(""));
                JsonPatch.AddIfMissing(lab, "pivotUntilTick", JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 37: no government had been lobbied.</summary>
        static void MigrateTo37(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "blocs", b => JsonPatch.AddIfMissing(b, "lobbyUntilTick", JsonValue.Of(0L))));
        }

        /// <summary>Schema 36: every launch so far was a tested one.</summary>
        static void MigrateTo36(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "models", m => JsonPatch.AddIfMissing(m, "rushWeeks", JsonValue.Of(0L))));
        }

        /// <summary>Schema 35: every model was designed by people alone.</summary>
        static void MigrateTo35(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "models", m => JsonPatch.AddIfMissing(m, "designerId", JsonValue.Of(-1L))));
        }

        /// <summary>Schema 34: every model was trained to be correct.</summary>
        static void MigrateTo34(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "models", m => JsonPatch.AddIfMissing(m, "objective", JsonValue.Of(""))));
        }

        /// <summary>Schema 33: no office event waiting, none seen.</summary>
        static void MigrateTo33(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "officeEventId", JsonValue.Of(""));
                JsonPatch.AddIfMissing(lab, "officeEventExpires", JsonValue.Of(0L));
                JsonPatch.AddIfMissing(lab, "officeSeen", JsonValue.Array());
            });
        }

        /// <summary>Schema 32: no leads waiting and no tricks learned.</summary>
        static void MigrateTo32(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonPatch.AddIfMissing(lab, "researchLeads", JsonValue.Array());
                JsonPatch.AddIfMissing(lab, "leadSpeedPermille", JsonValue.Of(0L));
            });
        }

        /// <summary>Schema 31: rooms start fresh, as if built this era.</summary>
        static void MigrateTo31(JsonValue root)
        {
            JsonPatch.ForEach(root, "labs", lab =>
            {
                JsonValue rooms = lab.Get("rooms");
                if (rooms == null) return;
                JsonPatch.ForEach(lab, "rooms", room => JsonPatch.AddIfMissing(room, "eraAge", JsonValue.Of(0L)));
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
