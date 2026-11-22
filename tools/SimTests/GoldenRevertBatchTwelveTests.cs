using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Batch-12 save steps 125-129 recover the previous golden at tip while new config blocks remain loaded.</summary>
public class GoldenRevertBatchTwelveTests
{
    static void Revert(int schema)
    {
        string root = TestData.RepoRoot();
        string script = System.IO.Path.Combine(root, "tools", "golden-revert-batch12-run.sh");
        var psi = new System.Diagnostics.ProcessStartInfo("bash", script + " " + schema)
        {
            WorkingDirectory = root,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        using var p = System.Diagnostics.Process.Start(psi);
        string stdout = p.StandardOutput.ReadToEnd();
        string stderr = p.StandardError.ReadToEnd();
        p.WaitForExit(120_000);
        Xunit.Assert.Equal(0, p.ExitCode);
        Xunit.Assert.Contains("Passed!", stdout + stderr);
        Xunit.Assert.Contains($"schema {schema} revert: Passed!", stdout);
    }

    [Theory]
    [InlineData(125)]
    [InlineData(126)]
    [InlineData(127)]
    [InlineData(128)]
    [InlineData(129)]
    public void Golden_revert_at_tip_for_each_batch_twelve_schema(int schema) => Revert(schema);
}