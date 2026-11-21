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
            if (schema < 66) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "bountyLevel", "bountyLockedUntil" }) JsonPatch.Remove(lab, key); });
            if (schema < 65) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "whistleWeeks", "whistleModel", "whistleDue", "silenced" }) JsonPatch.Remove(lab, key); });
            if (schema < 64) JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "chairs"));
            if (schema < 63) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "revenueTargetLevel", "revenueTargetEnd", "revenueTargetGoal", "revenueTargetSoFar" }) JsonPatch.Remove(lab, key); });
            if (schema < 62) JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "staff", person => JsonPatch.Remove(person, "senior")));
            if (schema < 61) JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "techDebt"));
            if (schema < 60) JsonPatch.ForEach(root, "labs", lab => { foreach (string key in new[] { "researchGrantStage", "researchGrantTier", "researchGrantTick", "researchGrantOwed", "researchGrantLockedUntil" }) JsonPatch.Remove(lab, key); });
            if (schema < 59) JsonPatch.ForEach(root, "labs", lab => { JsonPatch.Remove(lab, "slaLevel"); JsonPatch.Remove(lab, "slaLockedUntil"); JsonPatch.Remove(lab, "slaStrikes"); });
            if (schema < 58) JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "models", m => { JsonPatch.Remove(m, "incidentRepLoss"); JsonPatch.Remove(m, "postMortemDone"); }));
            if (schema < 57) JsonPatch.ForEach(root, "labs", lab => { JsonPatch.Remove(lab, "consultantLevel"); JsonPatch.Remove(lab, "consultantWeeks"); JsonPatch.Remove(lab, "consultantLockedUntil"); });
            if (schema < 56) JsonPatch.ForEach(root, "labs", lab => { JsonPatch.Remove(lab, "onCallLevel"); JsonPatch.Remove(lab, "onCallLockedUntil"); });
            if (schema < 55) JsonPatch.ForEach(root, "labs", lab => { JsonPatch.Remove(lab, "optimiseLevel"); JsonPatch.Remove(lab, "campaignSector"); JsonPatch.Remove(lab, "campaignLevel"); JsonPatch.Remove(lab, "campaignWeeks"); JsonPatch.Remove(lab, "hangoverWeeks"); JsonPatch.Remove(lab, "campaignLockedUntil"); });
            if (schema < 54) JsonPatch.ForEach(root, "labs", lab => { JsonPatch.Remove(lab, "ethicsLevel"); JsonPatch.Remove(lab, "ethicsLockedUntil"); JsonPatch.Remove(lab, "chiefScientistId"); JsonPatch.Remove(lab, "chiefSafetyId"); });
            if (schema < 53) JsonPatch.ForEach(root, "labs", lab => { JsonPatch.Remove(lab, "equityLevel"); JsonPatch.Remove(lab, "equityPermille"); });
            if (schema < 52) JsonPatch.ForEach(root, "labs", lab => { JsonPatch.Remove(lab, "researchFocus"); JsonPatch.Remove(lab, "focusLockedUntil"); JsonPatch.Remove(lab, "hackWeeks"); JsonPatch.Remove(lab, "hackLockedUntil"); });
            if (schema < 51) JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "models", m => JsonPatch.Remove(m, "auditedUntil")));
            if (schema < 50)
            {
                JsonPatch.ForEach(root, "labs", lab => { JsonPatch.Remove(lab, "subletPermille"); JsonPatch.Remove(lab, "subletLockedUntil"); });
                foreach (string place in SaveMigrations.LedgerPlaces) JsonPatch.ForEach(root, place, totals => JsonPatch.Remove(totals, "sublet"));
            }
            if (schema < 49) JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "prestige"));
            if (schema < 48) JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "complianceLevel"));
            if (schema < 47) JsonPatch.ForEach(root, "labs", lab => { JsonPatch.Remove(lab, "payLevel"); JsonPatch.Remove(lab, "payLockedUntil"); });
            if (schema < 46) JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "interpLevel"));
            if (schema < 45) JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "models", m => JsonPatch.Remove(m, "redTeamUntil")));
            if (schema < 44)
            {
                JsonPatch.ForEach(root, "labs", lab => { JsonPatch.Remove(lab, "insuranceLevel"); JsonPatch.Remove(lab, "insuranceClaims"); JsonPatch.Remove(lab, "insuranceLockedUntil"); });
                foreach (string place in SaveMigrations.LedgerPlaces) JsonPatch.ForEach(root, place, totals => JsonPatch.Remove(totals, "insurance"));
            }
            if (schema < 43) JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "crunchWeeks"));
            if (schema < 42) JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "researchSharePermille"));
            if (schema < 41) JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "loans"));
            if (schema < 40) JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "models", m => { JsonPatch.Remove(m, "approach"); JsonPatch.Remove(m, "checkpoints"); }));
            if (schema < 39) JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "eraScores"));
            if (schema < 38) JsonPatch.ForEach(root, "labs", lab => { JsonPatch.Remove(lab, "pivotEraId"); JsonPatch.Remove(lab, "pivotUntilTick"); });
            if (schema < 37) JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "blocs", b => JsonPatch.Remove(b, "lobbyUntilTick")));
            if (schema < 36) JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "models", m => JsonPatch.Remove(m, "rushWeeks")));
            if (schema < 35) JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "models", m => JsonPatch.Remove(m, "designerId")));
            if (schema < 34) JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "models", m => JsonPatch.Remove(m, "objective")));
            if (schema < 33) JsonPatch.ForEach(root, "labs", lab => { JsonPatch.Remove(lab, "officeEventId"); JsonPatch.Remove(lab, "officeEventExpires"); JsonPatch.Remove(lab, "officeSeen"); });
            if (schema < 32) JsonPatch.ForEach(root, "labs", lab => { JsonPatch.Remove(lab, "researchLeads"); JsonPatch.Remove(lab, "leadSpeedPermille"); });
            if (schema < 31) JsonPatch.ForEach(root, "labs", lab => JsonPatch.ForEach(lab, "rooms", room => JsonPatch.Remove(room, "eraAge")));
            if (schema < 30) JsonPatch.ForEach(root, "labs", lab => JsonPatch.Remove(lab, "namingMess"));
            if (schema < 29) StripCulture(root);
            if (schema < 27) StripSectorTrust(root);
            if (schema < 26) StripGauntletDepth(root);
            if (schema < 25) StripPressInquiries(root);
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
}