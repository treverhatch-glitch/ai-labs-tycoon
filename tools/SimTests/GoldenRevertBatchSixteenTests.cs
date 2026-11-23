using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>Batch-16 save steps 144-148 recover the previous golden at tip while new config blocks remain loaded.</summary>
[Collection("GoldenRevertBatch12")]
public class GoldenRevertBatchSixteenTests
{
    static void Revert(int schema)
    {
        string root = TestData.RepoRoot();
        string restore = System.IO.Path.Combine(root, "tools", "restore-golden-tip.sh");
        try
        {
            string script = System.IO.Path.Combine(root, "tools", "golden-revert-batch16-run.sh");
            var psi = new System.Diagnostics.ProcessStartInfo("bash", script + " " + schema)
            {
                WorkingDirectory = root,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };
            psi.Environment["DOTNET"] = "/home/ubuntu/.dotnet/dotnet";
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
            Xunit.Assert.Equal(0, pr.ExitCode);
        }
    }

    [Theory]
    [InlineData(144)]
    [InlineData(145)]
    [InlineData(146)]
    [InlineData(147)]
    [InlineData(148)]
    public void Golden_revert_at_tip_for_each_batch_sixteen_schema(int schema) => Revert(schema);
}