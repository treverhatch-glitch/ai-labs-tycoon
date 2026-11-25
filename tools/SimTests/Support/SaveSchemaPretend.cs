using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using Xunit;

namespace AiLabs.SimTests.Support;

/// <summary>Rewrites a current save JSON to look like an older schema so migration steps can be tested in isolation.</summary>
public static class SaveSchemaPretend
{
    public static string At(GameConfig c, int schema, string saveJson)
    {
        JsonValue root = Json.Parse(saveJson);
        if (schema < GameState.CurrentSchema)
        {
            if (schema < 115) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "discOn", "discOpen", "discLeft", "discPrep", "discFindings", "discFiled", "discInspect", "discModel" }) JsonPatch.Remove(lab, key); });
            if (schema < 114) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "fabTier", "fabBuild", "fabLeft", "fabGpus", "fabBuilt", "fabCancelled" }) JsonPatch.Remove(lab, key); });
            if (schema < 113) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "orgForm", "orgSince", "orgDisrupt", "orgReorgs" }) JsonPatch.Remove(lab, key); });
            if (schema < 112) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "cfGranted", "cfFindings", "cfSuspended" }) JsonPatch.Remove(lab, key); });
            if (schema < 112) JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "models", m => { foreach (string key in new[] { "certStage", "certLeft", "certScope", "certTries" }) JsonPatch.Remove(m, key); }));
            if (schema < 111) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "apiMoved", "apiLost" }) JsonPatch.Remove(lab, key); });
            if (schema < 111) JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "models", m => { foreach (string key in new[] { "integrations", "apiState", "apiLeft", "apiSuccessor" }) JsonPatch.Remove(m, key); }));
            if (schema < 110) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "ltsKept", "ltsBroken" }) JsonPatch.Remove(lab, key); });
            if (schema < 110) JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "models", m => JsonPatch.Remove(m, "ltsLeft")));
            if (schema < 109) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "licRegime", "licPhase", "licLeft", "licStrikes", "licCooldown", "licGranted", "licRefused", "licSuspended" }) JsonPatch.Remove(lab, key); });
            if (schema < 108) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "podIds", "podWeeks", "podChem", "podSwapAfter", "podPoints", "podsFormed" }) JsonPatch.Remove(lab, key); });
            if (schema < 107) JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "models", m => JsonPatch.Remove(m, "queueLevel")));
            if (schema < 106) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "pgTech", "pgDone", "pgPhase", "pgLeft", "pgBold", "pgInsight", "pgSpent", "pgWins", "pgLosses" }) JsonPatch.Remove(lab, key); });
            if (schema < 105) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "lwOn", "lwRival", "lwLead", "lwWait", "lwHits", "lwAnswers" }) JsonPatch.Remove(lab, key); });
            if (schema < 104) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "ftLevel", "ftReach", "ftWeeks", "ftSpent", "ftEarned" }) JsonPatch.Remove(lab, key); });
            if (schema < 103) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "brandOn", "brandScore", "brandReview", "brandLow", "brandReviews" }) JsonPatch.Remove(lab, key); });
            if (schema < 102) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "wlModel", "wlHype", "wlLeft", "wlWins", "wlFlops" }) JsonPatch.Remove(lab, key); });
            if (schema < 101) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "tbRef", "tbLeft", "tbWins", "tbLosses" }) JsonPatch.Remove(lab, key); });
            if (schema < 100) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "replPending", "replWait", "replLeft", "replRigor", "replWins", "replFails" }) JsonPatch.Remove(lab, key); });
            if (schema < 99) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "shockKind", "shockRef", "shockLeft", "shockBraced", "shockRode", "shockCountered" }) JsonPatch.Remove(lab, key); });
            if (schema < 98) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "creedA", "creedB", "creedIntegrity", "creedBand", "creedLock", "creedTown", "creedKept", "creedBroken" }) JsonPatch.Remove(lab, key); });
            if (schema < 97) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "platPlat", "platLeft", "platCut", "platRenewals", "platReliance", "platHoldup", "platPaid", "platKept" }) JsonPatch.Remove(lab, key); });
            if (schema < 96) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "devLine", "devStage", "devWeeks", "devQuality", "devTier", "devSold", "devBase" }) JsonPatch.Remove(lab, key); });
            if (schema < 95) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "noteOwed", "notePrincipal", "noteRate", "noteDiscount", "noteLeft", "noteExtended", "noteTier" }) JsonPatch.Remove(lab, key); });
            if (schema < 94) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "leaseUnits", "leaseValue", "leaseRent", "leaseBuyback", "leaseLeft", "leaseMissed", "leaseExtended" }) JsonPatch.Remove(lab, key); });
            if (schema < 93) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "foundFund", "foundGiven", "foundFocus", "foundPayout", "foundGoodwill", "foundSwitch", "foundDrip" }) JsonPatch.Remove(lab, key); });
            if (schema < 92) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "rbfAdvance", "rbfOwed", "rbfBase", "rbfShare", "rbfTier", "rbfWeeks", "rbfSlump" }) JsonPatch.Remove(lab, key); });
            if (schema < 91) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "listed", "listPrice", "listGuidance", "listTarget", "listQuarter", "listBase", "listNext", "listKeep", "listStreak", "listFollowOn", "listActivist" }) JsonPatch.Remove(lab, key); });
            if (schema < 90) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "capPolicy", "capPool", "capSwitchTick", "capWritten" }) JsonPatch.Remove(lab, key); });
            if (schema < 89) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "covBreach", "covWaiverUntil", "covWaivers", "covCalls" }) JsonPatch.Remove(lab, key); });
            if (schema < 88) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "warrantyTier", "spares", "failCarry", "rmaUnits", "rmaDue", "gpuFailures" }) JsonPatch.Remove(lab, key); });
            if (schema < 87) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "stdLevel", "stdDraft", "stdSupport", "stdRole", "stdNext", "stdAdopted", "stdLapses" }) JsonPatch.Remove(lab, key); });
            if (schema < 86) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "taxStructure", "taxBuilding", "taxDue", "taxAccrued", "taxSaved", "taxResearch", "taxCredit", "taxClaimed", "taxExposure", "taxAuditDue", "taxAuditOwed" }) JsonPatch.Remove(lab, key); });
            if (schema < 85) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "policyLevel", "policyLockedUntil", "policyHold", "policyBreaks" }) JsonPatch.Remove(lab, key); });
            if (schema < 84) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "charter", "charterTo", "charterDue", "capPaid" }) JsonPatch.Remove(lab, key); });
            if (schema < 83) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "contractKind", "contractStart", "contractDue", "contractPaid", "contractMissed", "offerKind", "offerDue", "offerSeq", "contractNext" }) JsonPatch.Remove(lab, key); });
            if (schema < 82) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "cases", "caseSeq", "securityTier", "securityBuilding", "securityDue", "exposure", "breachDue", "ransoms" }) JsonPatch.Remove(lab, key); });
            if (schema < 81) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "requisitions", "scholarCount", "scholarStart", "scholarDue" }) JsonPatch.Remove(lab, key); });
            if (schema < 80) JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "staff", person => { JsonPatch.Remove(person, "leaveDue"); JsonPatch.Remove(person, "leaveMorale"); }));
            if (schema < 79) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "supportPeople", "supportBacklog" }) JsonPatch.Remove(lab, key); });
            if (schema < 78) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "internRole", "internCount", "internStart", "internDue" }) JsonPatch.Remove(lab, key); });
            if (schema < 77) JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "alumni"));
            if (schema < 76) JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "submissions"));
            if (schema < 75) JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "accountTeams"));
            if (schema < 74) JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "models", m => { JsonPatch.Remove(m, "sunsetStart"); JsonPatch.Remove(m, "sunsetTick"); }));
            if (schema < 73) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "labourStage", "labourPressure", "labourDue", "labourHard" }) JsonPatch.Remove(lab, key); });
            if (schema < 72) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "skunkKind", "skunkPeople", "skunkStart", "skunkDue", "skunkWork", "skunkLockedUntil" }) JsonPatch.Remove(lab, key); });
            if (schema < 71) JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "staff", person => { JsonPatch.Remove(person, "mentorId"); JsonPatch.Remove(person, "mentorWeeks"); }));
            if (schema < 70) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "roadmapLevel", "roadmapStart", "roadmapDue", "roadmapGoal" }) JsonPatch.Remove(lab, key); });
            if (schema < 69) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "pledgeLevel", "pledgeLockedUntil" }) JsonPatch.Remove(lab, key); });
            if (schema < 68) JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "reserve"));
            if (schema < 67) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "bountyLevel", "bountyLockedUntil" }) JsonPatch.Remove(lab, key); });
            if (schema < 66) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "whistleWeeks", "whistleModel", "whistleDue", "silenced" }) JsonPatch.Remove(lab, key); });
            if (schema < 65) JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "chairs"));
            if (schema < 64) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "revenueTargetLevel", "revenueTargetEnd", "revenueTargetGoal", "revenueTargetSoFar" }) JsonPatch.Remove(lab, key); });
            if (schema < 63) JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "staff", person => JsonPatch.Remove(person, "senior")));
            if (schema < 62) JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "techDebt"));
            if (schema < 61) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "researchGrantStage", "researchGrantTier", "researchGrantTick", "researchGrantOwed", "researchGrantLockedUntil" }) JsonPatch.Remove(lab, key); });
            if (schema < 60) JsonPatch.ForEach(root, "labs", lab => { JsonPatch.Remove(lab, "slaLevel"); JsonPatch.Remove(lab, "slaLockedUntil"); JsonPatch.Remove(lab, "slaStrikes"); });
            if (schema < 59) JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "models", m => { JsonPatch.Remove(m, "incidentRepLoss"); JsonPatch.Remove(m, "postMortemDone"); }));
            if (schema < 58) JsonPatch.ForEach(root, "labs", lab => { JsonPatch.Remove(lab, "consultantLevel"); JsonPatch.Remove(lab, "consultantWeeks"); JsonPatch.Remove(lab, "consultantLockedUntil"); });
            if (schema < 57) JsonPatch.ForEach(root, "labs", lab => { JsonPatch.Remove(lab, "onCallLevel"); JsonPatch.Remove(lab, "onCallLockedUntil"); });
            if (schema < 56) JsonPatch.ForEach(root, "labs", lab => { JsonPatch.Remove(lab, "optimiseLevel"); JsonPatch.Remove(lab, "campaignSector"); JsonPatch.Remove(lab, "campaignLevel"); JsonPatch.Remove(lab, "campaignWeeks"); JsonPatch.Remove(lab, "hangoverWeeks"); JsonPatch.Remove(lab, "campaignLockedUntil"); });
            if (schema < 55) JsonPatch.ForEach(root, "labs", lab => { JsonPatch.Remove(lab, "ethicsLevel"); JsonPatch.Remove(lab, "ethicsLockedUntil"); JsonPatch.Remove(lab, "chiefScientistId"); JsonPatch.Remove(lab, "chiefSafetyId"); });
            if (schema < 54) JsonPatch.ForEach(root, "labs", lab => { JsonPatch.Remove(lab, "equityLevel"); JsonPatch.Remove(lab, "equityPermille"); });
            if (schema < 53) JsonPatch.ForEach(root, "labs", lab => { JsonPatch.Remove(lab, "researchFocus"); JsonPatch.Remove(lab, "focusLockedUntil"); JsonPatch.Remove(lab, "hackWeeks"); JsonPatch.Remove(lab, "hackLockedUntil"); });
            if (schema < 52) JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "models", m => JsonPatch.Remove(m, "auditedUntil")));
            if (schema < 51)
            {
                JsonPatch.ForEach(root, "labs", lab => { JsonPatch.Remove(lab, "subletPermille"); JsonPatch.Remove(lab, "subletLockedUntil"); });
                foreach (string place in SaveMigrations.LedgerPlaces) JsonPatch.ForEach(root, place, totals => JsonPatch.Remove(totals, "sublet"));
            }
            if (schema < 50) JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "prestige"));
            if (schema < 49) JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "complianceLevel"));
            if (schema < 48) JsonPatch.ForEach(root, "labs", lab => { JsonPatch.Remove(lab, "payLevel"); JsonPatch.Remove(lab, "payLockedUntil"); });
            if (schema < 47) JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "interpLevel"));
            if (schema < 46) JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "models", m => JsonPatch.Remove(m, "redTeamUntil")));
            if (schema < 45)
            {
                foreach (string place in SaveMigrations.LedgerPlaces)
                    JsonPatch.ForEach(root, place, totals => JsonPatch.Remove(totals, "insurance"));
                JsonPatch.ForEach(root, "labs", lab =>
                {
                    JsonPatch.Remove(lab, "insuranceLevel");
                    JsonPatch.Remove(lab, "insuranceClaims");
                    JsonPatch.Remove(lab, "insuranceLockedUntil");
                });
            }
            if (schema < 44) JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "crunchWeeks"));
            if (schema < 43) JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "researchSharePermille"));
            if (schema < 42) JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "loans"));
            if (schema < 41) JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "models", m => { JsonPatch.Remove(m, "approach"); JsonPatch.Remove(m, "checkpoints"); }));
            if (schema < 40) JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "eraScores"));
            if (schema < 39) JsonPatch.ForEach(root, "labs", lab => { JsonPatch.Remove(lab, "pivotEraId"); JsonPatch.Remove(lab, "pivotUntilTick"); });
            if (schema < 38) JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "blocs", b => JsonPatch.Remove(b, "lobbyUntilTick")));
            if (schema < 37) JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "models", m => JsonPatch.Remove(m, "rushWeeks")));
            if (schema < 36) JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "models", m => JsonPatch.Remove(m, "designerId")));
            if (schema < 35) JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "models", m => JsonPatch.Remove(m, "objective")));
            if (schema < 34) JsonPatch.ForEach(root, "labs", lab => { JsonPatch.Remove(lab, "officeEventId"); JsonPatch.Remove(lab, "officeEventExpires"); JsonPatch.Remove(lab, "officeSeen"); });
            if (schema < 33) JsonPatch.ForEach(root, "labs", lab => { JsonPatch.Remove(lab, "researchLeads"); JsonPatch.Remove(lab, "leadSpeedPermille"); });
            if (schema < 32) JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "rooms", room => JsonPatch.Remove(room, "eraAge")));
            if (schema < 31) JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "namingMess"));
            if (schema < 30) StripCulture(root);
            if (schema < 29) StripFleetResaleLock(root);
            if (schema < 28) StripHatchLedgerLines(root);
            if (schema < 27) StripSectorTrust(root);
            if (schema < 26) StripGauntletDepth(root);
            if (schema < 25) StripPressInquiries(root);
            if (schema < 24) StripBubbleHadCrash(root);
            if (schema < 23) StripPoachOffers(root);
            if (schema < 22) StripFoundingChoices(root);
            if (schema < 21) StripEventRiskClass(root);
            if (schema < 20) StripAbsorbedByLab(root);
            if (schema < 19) StripBubbleCrashPhase(root);
            if (schema < 18) StripChinaDeclineCooldown(root);
            if (schema < 17) StripCrownRunnerUpStreak(root);
            if (schema < 16) StripBoardLockedUntilTick(root);
            if (schema < 14) StripChinaDealFlags(root);
            if (schema < 13) StripPublicFloatPpm(root);
            if (schema < 12) StripBubblePermille(root);
            if (schema < 11) StripReservedAndQueue(root);
            if (schema < 10) RevertStaffToPools(root);
            if (schema < 9) StripAlignmentGapFields(root);
        }
        root.Set("schema", JsonValue.Of((long)schema));
        return Json.Write(root, false);
    }

    public static void AssertRoundTrip(Simulation sim)
    {
        string json = sim.Save();
        Simulation again = Simulation.Load(sim.Config, json);
        Assert.Empty(SaveGame.Problems(sim.Config, again.State));
        Assert.Equal(sim.Digest(), again.Digest());
    }

    static void StripCulture(JsonValue root)
    {
        JsonPatch.ForEach(root, "labs", lab =>
        {
            JsonPatch.Remove(lab, "cultureShift");
            JsonPatch.Remove(lab, "releaseGate");
            JsonPatch.Remove(lab, "gateSetTick");
        });
    }

    static void StripSectorTrust(JsonValue root)
    {
        JsonPatch.ForEach(root, "labs", lab =>
        {
            JsonPatch.Remove(lab, "sectorTrust");
            JsonPatch.Remove(lab, "complaints");
        });
    }

    static void StripGauntletDepth(JsonValue root)
    {
        JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "gauntletTuning"));
        JsonPatch.ForEach(root, "gauntlets", g =>
        {
            JsonPatch.Remove(g, "confidentLabId");
            JsonPatch.Remove(g, "newcomerLabId");
        });
        JsonPatch.ForEach(root, "gauntlets.entries", e => JsonPatch.Remove(e, "tuned"));
    }

    static void StripPressInquiries(JsonValue root)
    {
        JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "pressInquiries"));
    }

    static void StripPoachOffers(JsonValue root)
    {
        JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "poachOffers"));
    }

    static void StripFoundingChoices(JsonValue root)
    {
        JsonPatch.ForEach(root, "labs", lab =>
        {
            JsonPatch.Remove(lab, "founderId");
            JsonPatch.Remove(lab, "styleId");
        });
    }

    static void StripEventRiskClass(JsonValue root)
    {
        JsonPatch.ForEach(root, "events", e => JsonPatch.Remove(e, "riskClass"));
    }

    static void StripAlignmentGapFields(JsonValue root)
    {
        JsonPatch.ForEach(root, "labs.models", model =>
        {
            JsonPatch.Remove(model, "displayedAlignment");
            JsonPatch.Remove(model, "lastIncidentRung");
        });
    }

    static void RevertStaffToPools(JsonValue root)
    {
        JsonPatch.ForEach(root, "labs", lab =>
        {
            JsonValue staff = lab.Get("staff");
            if (staff == null || staff.Kind != JsonKind.Array || staff.Items.Count == 0) return;
            JsonValue first = staff.Items[0];
            if (first.Get("count") != null) return;

            var pools = new Dictionary<string, int>();
            for (int i = 0; i < staff.Items.Count; i++)
            {
                JsonValue p = staff.Items[i];
                JsonValue roleNode = p.Get("roleId");
                if (roleNode == null) continue;
                string role = roleNode.Str;
                pools.TryGetValue(role, out int n);
                pools[role] = n + 1;
            }

            JsonValue arr = JsonValue.Array();
            foreach (KeyValuePair<string, int> kv in pools)
            {
                JsonValue o = JsonValue.Object();
                o.Set("roleId", JsonValue.Of(kv.Key));
                o.Set("count", JsonValue.Of((long)kv.Value));
                arr.Add(o);
            }
            lab.Set("staff", arr);
        });
    }

    static void StripBubblePermille(JsonValue root) => JsonPatch.Remove(root, "bubblePermille");

    static void StripBubbleHadCrash(JsonValue root) => JsonPatch.Remove(root, "bubbleHadCrash");

    static void StripAbsorbedByLab(JsonValue root)
    {
        JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "absorbedByLabId"));
    }

    static void StripBubbleCrashPhase(JsonValue root)
    {
        JsonPatch.Remove(root, "bubbleCapitalRaised");
        JsonPatch.Remove(root, "bubbleGraceMonths");
        JsonPatch.Remove(root, "bubbleCrashMonthsRemaining");
    }

    static void StripPublicFloatPpm(JsonValue root)
    {
        JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "publicFloatPpm"));
    }

    static void StripChinaDealFlags(JsonValue root)
    {
        JsonPatch.ForEach(root, "labs", lab =>
        {
            JsonPatch.Remove(lab, "chinaDealActive");
            JsonPatch.Remove(lab, "chinaDealPending");
        });
    }

    static void StripBoardLockedUntilTick(JsonValue root)
    {
        JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "boardLockedUntilTick"));
    }

    static void StripCrownRunnerUpStreak(JsonValue root)
    {
        JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "crownRunnerUpStreak"));
    }

    static void StripChinaDeclineCooldown(JsonValue root)
    {
        JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "chinaDealDeclinedUntilTick"));
    }

    static void StripReservedAndQueue(JsonValue root)
    {
        JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "reservedContracts"));
        JsonPatch.ForEach(root, "vendor.standings", s =>
        {
            JsonPatch.Remove(s, "queuedGpus");
            JsonPatch.Remove(s, "queueHoldingId");
        });
    }

    static void StripFleetResaleLock(JsonValue root)
    {
        JsonPatch.ForEach(root, "labs.fleet", batch => JsonPatch.Remove(batch, "resaleLockedUntilTick"));
    }

    static void StripHatchLedgerLines(JsonValue root)
    {
        foreach (string place in new[] { "labs.ledger.thisWeek", "labs.ledger.lastWeek", "labs.ledger.thisMonth", "labs.ledger.lifetime", "labs.ledger.history.totals" })
            JsonPatch.ForEach(root, place, totals =>
            {
                JsonPatch.Remove(totals, "acquisition");
                JsonPatch.Remove(totals, "assetSale");
            });
    }
}