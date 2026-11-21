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