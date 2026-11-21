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