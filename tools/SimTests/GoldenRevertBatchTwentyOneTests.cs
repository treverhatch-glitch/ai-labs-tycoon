using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

[Collection("GoldenRevertBatch12")]
public class GoldenRevertBatchTwentyOneTests
{
    static void Revert(int schema)
    {
        string root = TestData.RepoRoot();
        string restore = System.IO.Path.Combine(root, "tools", "restore-golden-tip.sh");
        try
        {
            string script = System.IO.Path.Combine(root, "tools", "golden-revert-batch21-run.sh");
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
    public void Schema_161_reverts_to_schema_160_digest() => Revert(161);

    [Fact]
    public void Schema_162_reverts_to_schema_161_digest() => Revert(162);

    [Fact]
    public void Schema_163_reverts_to_schema_162_digest() => Revert(163);
}