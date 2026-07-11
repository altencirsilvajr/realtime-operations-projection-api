namespace RealtimeOperationsProjection.Domain.Tests;

public sealed class DocumentationGovernanceTests
{
    [Fact]
    public void Required_architecture_decisions_and_journal_traceability_exist()
    {
        var root = FindRepositoryRoot();
        Assert.True(File.Exists(Path.Combine(root, "PROJECT_VISION.md")));
        for (var number = 1; number <= 5; number++)
        {
            Assert.True(File.Exists(Path.Combine(root, "docs", "adr", $"ADR-{number:0000}-{AdrSuffix(number)}.md")));
        }

        foreach (var journal in Directory.EnumerateFiles(Path.Combine(root, "journal"), "*.md"))
        {
            Assert.Contains("Rastreabilidade ADR", File.ReadAllText(journal));
        }
    }

    [Fact]
    public void Every_project_targets_net9_only()
    {
        var root = FindRepositoryRoot();
        foreach (var project in Directory.EnumerateFiles(root, "*.csproj", SearchOption.AllDirectories))
        {
            Assert.Contains("<TargetFramework>net9.0</TargetFramework>", File.ReadAllText(project));
        }
    }

    private static string FindRepositoryRoot()
    {
        for (var directory = new DirectoryInfo(Directory.GetCurrentDirectory()); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "PROJECT_VISION.md"))) return directory.FullName;
        }

        throw new DirectoryNotFoundException("PROJECT_VISION.md was not found from the test directory.");
    }

    private static string AdrSuffix(int number) => number switch
    {
        1 => "project-vision-source-of-truth",
        2 => "keep-business-logic-outside-signalr-hubs",
        3 => "use-projections-for-operational-status-reads",
        4 => "reconnect-recovers-from-persisted-snapshot",
        5 => "local-blazor-learning-dashboard",
        _ => throw new ArgumentOutOfRangeException(nameof(number))
    };
}
