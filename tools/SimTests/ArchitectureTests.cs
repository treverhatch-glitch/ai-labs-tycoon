using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

/// <summary>
/// The promises the rules library makes to everything built on it: no Unity, no clocks, no disk,
/// no fractions, and nothing in the state that a save would forget.
/// </summary>
public class ArchitectureTests
{
    static readonly Assembly Rules = typeof(Simulation).Assembly;

    static IEnumerable<string> RulesSourceFiles()
    {
        string dir = Path.Combine(TestData.RepoRoot(), "Rules");
        return Directory.GetFiles(dir, "*.cs", SearchOption.AllDirectories);
    }

    /// <summary>The code of a file with comments and text in quotes taken out, so only what runs is checked.</summary>
    static string CodeOnly(string path)
    {
        string text = File.ReadAllText(path);
        text = Regex.Replace(text, @"/\*.*?\*/", "", RegexOptions.Singleline);
        text = Regex.Replace(text, @"""(\\.|[^""\\])*""", "\"\"");
        text = Regex.Replace(text, @"//.*", "");
        return text;
    }

    [Fact]
    public void The_rules_library_targets_netstandard_and_references_nothing_else()
    {
        string[] references = Rules.GetReferencedAssemblies().Select(a => a.Name).ToArray();
        Assert.Equal(new[] { "netstandard" }, references);
    }

    [Fact]
    public void The_rules_use_no_engine_clock_thread_disk_or_unseeded_dice()
    {
        // The same list the studio harness bans from simulation code, plus Console and Environment.
        string[] banned =
        {
            "UnityEngine", "UnityEditor", "Time.", "new Random", "Random.Shared", "System.Random",
            "DateTime.Now", "DateTime.UtcNow", "DateTimeOffset", "Environment.", "Stopwatch", "Guid.NewGuid",
            "Task.Run", "Task.Delay", "new Thread", "ThreadPool", "Parallel.", "File.", "Directory.",
            "HttpClient", "WebRequest", "Console.", "System.IO", "System.Net", "System.Threading"
        };

        var found = new List<string>();
        foreach (string file in RulesSourceFiles())
        {
            string code = CodeOnly(file);
            foreach (string word in banned)
                if (Regex.IsMatch(code, @"(?<![A-Za-z0-9_])" + Regex.Escape(word)))
                    found.Add(Path.GetFileName(file) + " uses " + word);
        }
        Assert.True(found.Count == 0, string.Join("; ", found));
    }

    [Fact]
    public void The_rules_do_all_their_sums_in_whole_numbers()
    {
        // Fractions round differently on different machines. Money is whole dollars, rates are permille.
        var found = new List<string>();
        foreach (string file in RulesSourceFiles())
        {
            string code = CodeOnly(file);
            foreach (Match m in Regex.Matches(code, @"\b(float|double|decimal|Math\.(Sqrt|Pow|Round|Floor|Ceiling|Exp|Log))\b|\b\d+\.\d+[fdmFDM]?\b"))
                found.Add(Path.GetFileName(file) + " has " + m.Value);
        }
        Assert.True(found.Count == 0, string.Join("; ", found));

        foreach (Type type in Rules.GetTypes())
            foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                Assert.False(field.FieldType == typeof(float) || field.FieldType == typeof(double) || field.FieldType == typeof(decimal),
                    type.Name + "." + field.Name + " is a fraction");
    }

    [Fact]
    public void The_rules_keep_nothing_in_statics_so_two_games_never_share_state()
    {
        var found = new List<string>();
        foreach (Type type in Rules.GetTypes())
            foreach (FieldInfo field in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                if (!field.IsLiteral && !field.IsInitOnly && !type.IsEnum && !field.Name.Contains("<"))
                    found.Add(type.Name + "." + field.Name);
        Assert.True(found.Count == 0, "mutable static fields: " + string.Join(", ", found));
    }

    /// <summary>A syncer that only notes which keys a class syncs.</summary>
    sealed class KeyCounter : Syncer
    {
        public readonly List<string> Keys = new List<string>();
        public override bool IsReading { get { return false; } }
        public override void Int(string key, ref int value) { Keys.Add(key); }
        public override void Long(string key, ref long value) { Keys.Add(key); }
        public override void Bool(string key, ref bool value) { Keys.Add(key); }
        public override void Str(string key, ref string value) { Keys.Add(key); }
        public override void Enum<T>(string key, ref T value) { Keys.Add(key); }
        public override void Obj<T>(string key, ref T value) { Keys.Add(key); }
        public override void ObjList<T>(string key, ref List<T> list) { Keys.Add(key); }
        public override void IntList(string key, ref List<int> list) { Keys.Add(key); }
        public override void StrList(string key, ref List<string> list) { Keys.Add(key); }
    }

    [Fact]
    public void Every_field_of_every_saved_class_is_in_its_sync_list()
    {
        // The classic save bug: a field is added and nobody remembers to save it. Here that fails a test.
        Type[] saved = Rules.GetTypes().Where(t => typeof(ISync).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract).ToArray();
        Assert.True(saved.Length > 30, "expected to find the config and state classes, found " + saved.Length);

        foreach (Type type in saved)
        {
            var counter = new KeyCounter();
            ((ISync)Activator.CreateInstance(type)).Sync(counter);
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            Assert.True(fields.Length == counter.Keys.Count,
                type.Name + " has " + fields.Length + " fields but syncs " + counter.Keys.Count + " keys (" + string.Join(", ", counter.Keys) + ")");
            Assert.Equal(counter.Keys.Count, counter.Keys.Distinct().Count());
            Assert.All(counter.Keys, key => Assert.Matches("^[a-z][A-Za-z0-9]*$", key));
        }
    }

    [Fact]
    public void The_unity_assembly_definition_forbids_engine_references()
    {
        string root = TestData.RepoRoot();
        JsonValue asmdef = Json.Parse(File.ReadAllText(Path.Combine(root, "Rules", "AiLabs.Rules.asmdef")));
        Assert.Equal("AiLabs.Rules", asmdef.Get("name").Str);
        Assert.True(asmdef.Get("noEngineReferences").Bool);
        Assert.Equal(0, asmdef.Get("references").Count);

        JsonValue package = Json.Parse(File.ReadAllText(Path.Combine(root, "Rules", "package.json")));
        Assert.Equal("com.hatchtag.ailabs.rules", package.Get("name").Str);
    }

    [Fact]
    public void The_rules_folder_holds_no_build_output_because_unity_reads_it_as_a_package()
    {
        string rules = Path.Combine(TestData.RepoRoot(), "Rules");
        Assert.False(Directory.Exists(Path.Combine(rules, "bin")), "Rules/bin exists");
        Assert.False(Directory.Exists(Path.Combine(rules, "obj")), "Rules/obj exists");
    }

    [Fact]
    public void The_rules_compile_as_the_csharp_unity_speaks()
    {
        string csproj = File.ReadAllText(Path.Combine(TestData.RepoRoot(), "Rules", "AiLabs.Rules.csproj"));
        Assert.Contains("<TargetFramework>netstandard2.1</TargetFramework>", csproj);
        Assert.Contains("<LangVersion>9.0</LangVersion>", csproj);
    }

    [Fact]
    public void The_studio_harness_is_told_this_is_a_unity_only_game_checked_by_these_tests()
    {
        JsonValue game = Json.Parse(File.ReadAllText(Path.Combine(TestData.RepoRoot(), "studio", "game.json")));
        Assert.Equal("unity", game.Get("native").Str);

        JsonValue engines = game.Get("engines");
        Assert.Equal(new[] { "unity" }, engines.Keys.ToArray());
        JsonValue unity = engines.Get("unity");
        Assert.Equal("dotnet build tools/SimTests --nologo", unity.Get("quickCheck").Str);
        Assert.Equal("dotnet test tools/SimTests", unity.Get("rules").Get("test").Str);
        Assert.Equal("main", unity.Get("target").Str);
    }

    [Fact]
    public void Workflows_run_on_pull_requests_and_by_hand_only_and_cancel_the_run_before()
    {
        string dir = Path.Combine(TestData.RepoRoot(), ".github", "workflows");
        string[] files = Directory.GetFiles(dir, "*.yml");
        Assert.NotEmpty(files);
        foreach (string file in files)
        {
            string[] lines = File.ReadAllLines(file).Where(l => !l.TrimStart().StartsWith("#")).ToArray();
            string name = Path.GetFileName(file);
            Assert.DoesNotContain(lines, l => l.Trim() == "push:" || l.Trim().StartsWith("push:"));
            Assert.DoesNotContain(lines, l => l.Trim().StartsWith("schedule:"));
            Assert.DoesNotContain(lines, l => l.Trim().StartsWith("pull_request_target:"));
            Assert.DoesNotContain(lines, l => l.Contains("self-hosted"));
            Assert.True(lines.Any(l => l.Trim() == "workflow_dispatch:"), name + " can be run by hand");
            Assert.True(lines.Any(l => l.Trim() == "cancel-in-progress: true"), name + " cancels the run before it");
        }

        string unity = File.ReadAllText(Path.Combine(dir, "unity.yml"));
        Assert.DoesNotContain("pull_request", unity.Split('\n').Where(l => !l.TrimStart().StartsWith("#")).Select(l => l.Trim().TrimEnd(':')));
        Assert.Contains("github.ref == 'refs/heads/main'", unity);
        Assert.Contains("ProjectSettings/ProjectVersion.txt", unity);
    }

    [Fact]
    public void No_licence_key_or_env_file_is_tracked_in_the_repo()
    {
        string root = TestData.RepoRoot();
        string[] banned = { ".ulf", ".alf", ".pem", ".key", ".p12", ".pfx", ".keystore" };
        var found = TrackedFiles(root).Where(path =>
        {
            string n = Path.GetFileName(path);
            return banned.Contains(Path.GetExtension(n).ToLowerInvariant()) || n == ".env" || (n.StartsWith(".env.") && n != ".env.example");
        }).ToList();
        Assert.Empty(found);

        // Beta mode is the studio's switch and the owner's to flip (docs/SETUP-PC.md). It is never set from the game repo.
        Assert.False(File.Exists(Path.Combine(root, "studio", "beta.json")));
        Assert.False(File.Exists(Path.Combine(root, "studio", "delegation.json")));
    }

    /// <summary>What git tracks or would add (so an ignored local .env is not a finding). Without git: every file outside build folders.</summary>
    static List<string> TrackedFiles(string root)
    {
        try
        {
            var info = new System.Diagnostics.ProcessStartInfo("git", "ls-files --cached --others --exclude-standard")
            {
                WorkingDirectory = root,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };
            using var git = System.Diagnostics.Process.Start(info);
            string output = git.StandardOutput.ReadToEnd();
            git.WaitForExit();
            if (git.ExitCode == 0 && output.Length > 0)
                return output.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();
        }
        catch (Exception e) when (e is System.ComponentModel.Win32Exception || e is InvalidOperationException)
        {
        }

        var files = new List<string>();
        var todo = new Stack<string>();
        todo.Push(root);
        while (todo.Count > 0)
        {
            string dir = todo.Pop();
            foreach (string sub in Directory.GetDirectories(dir))
            {
                string n = Path.GetFileName(sub);
                if (n == ".git" || n == ".build" || n == "Library" || n == "node_modules") continue;
                todo.Push(sub);
            }
            foreach (string file in Directory.GetFiles(dir)) files.Add(Path.GetRelativePath(root, file));
        }
        return files;
    }
}
