using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Batch-18 save steps 154-155 recover the previous golden at tip while newer config blocks remain loaded.</summary>
[Collection("GoldenRevertBatch12")]
public class GoldenRevertBatchEighteenTests
{
    static void Revert(int schema)
    {
        string root = TestData.RepoRoot();
        string restore = System.IO.Path.Combine(root, "tools", "restore-golden-tip.sh");
        try
        {
            string script = System.IO.Path.Combine(root, "tools", "golden-revert-batch18-run.sh");
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
            p.WaitForExit(1_200_000);
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
        }
    }

    [Fact]
    public void Schema_154_reverts_to_schema_153_digest() => Revert(154);

    [Fact]
    public void Schema_155_reverts_to_schema_154_digest() => Revert(155);
}