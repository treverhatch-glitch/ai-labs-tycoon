using System.Diagnostics;
using System.Text;
using Xunit;

namespace AiLabs.SimTests.Support;

static class GoldenRevertTestRunner
{
    public static void Run(string scriptName, int schema)
    {
        string root = TestData.RepoRoot();
        string restore = System.IO.Path.Combine(root, "tools", "restore-golden-tip.sh");
        try
        {
            string script = System.IO.Path.Combine(root, "tools", scriptName);
            var psi = new ProcessStartInfo("bash", script + " " + schema)
            {
                WorkingDirectory = root,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };
            using var p = Process.Start(psi);
            var stdout = new StringBuilder();
            var stderr = new StringBuilder();
            p.OutputDataReceived += (_, e) => { if (e.Data != null) stdout.AppendLine(e.Data); };
            p.ErrorDataReceived += (_, e) => { if (e.Data != null) stderr.AppendLine(e.Data); };
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            Assert.True(p.WaitForExit(1_800_000), $"golden revert {schema} timed out");
            string outAll = stdout + stderr.ToString();
            Assert.Equal(0, p.ExitCode);
            Assert.Contains("Passed!", outAll);
            Assert.Contains($"schema {schema} revert: Passed!", stdout.ToString());
        }
        finally
        {
            var pr = Process.Start(new ProcessStartInfo("bash", restore)
            {
                WorkingDirectory = root,
                RedirectStandardError = true,
                UseShellExecute = false,
            });
            pr.WaitForExit(120_000);
        }
    }
}