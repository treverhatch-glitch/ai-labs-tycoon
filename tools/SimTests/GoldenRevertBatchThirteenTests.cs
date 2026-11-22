using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Batch-13 save steps 130-134 recover the previous golden at tip while new config blocks remain loaded.</summary>
[Collection("GoldenRevertBatch12")]
public class GoldenRevertBatchThirteenTests
{
    static void Revert(int schema)
    {
        string root = TestData.RepoRoot();
        string restore = System.IO.Path.Combine(root, "tools", "restore-golden-tip.sh");
        try
        {
            string script = System.IO.Path.Combine(root, "tools", "golden-revert-batch13-run.sh");
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
            p.WaitForExit(600_000);
            Xunit.Assert.Equal(0, p.ExitCode);
            Xunit.Assert.Contains("Passed!", stdout + stderr);
            Xunit.Assert.Contains($"schema {schema} revert: Passed!", stdout);
        }
        finally
        {
            var pr = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("bash", restore)
            {
                WorkingDirectory = root,
                RedirectStandardError = true,
                UseShellExecute = false,
            });
            pr.WaitForExit(120_000);
            Xunit.Assert.Equal(0, pr.ExitCode);
        }
    }

    [Theory]
    [InlineData(130)]
    [InlineData(131)]
    [InlineData(132)]
    [InlineData(133)]
    [InlineData(134)]
    public void Golden_revert_at_tip_for_each_batch_thirteen_schema(int schema) => Revert(schema);
}