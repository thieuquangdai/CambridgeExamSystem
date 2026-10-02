using System.Reflection;
using CambridgeExamSystem.Application.DTOs;
using CambridgeExamSystem.Domain.Entities;

namespace CambridgeExamSystem.Tests;

public class ArchitectureTests
{
    private static IEnumerable<string> ProjectReferences(Assembly assembly) =>
        assembly.GetReferencedAssemblies()
            .Select(a => a.Name ?? string.Empty)
            .Where(n => n.StartsWith("CambridgeExamSystem.", StringComparison.Ordinal));

    [Fact]
    public void Domain_does_not_reference_other_projects() =>
        Assert.Empty(ProjectReferences(typeof(User).Assembly));

    [Fact]
    public void Application_references_only_domain() =>
        Assert.Equal(["CambridgeExamSystem.Domain"], ProjectReferences(typeof(TakeTestDto).Assembly));

    [Fact]
    public void Student_test_dtos_do_not_expose_answer_keys()
    {
        string[] forbidden = ["IsCorrect", "CorrectTextAnswer", "MatchingKey", "Explanation", "TranscriptText"];
        Type[] studentTypes = [typeof(TakeTestDto), typeof(TestSectionDto), typeof(TestGroupDto), typeof(TestQuestionDto), typeof(TestOptionDto), typeof(MediaDto), typeof(SavedAnswerDto)];

        var leaked = studentTypes
            .SelectMany(t => t.GetProperties().Select(p => $"{t.Name}.{p.Name}"))
            .Where(name => forbidden.Any(f => name.EndsWith("." + f, StringComparison.Ordinal)))
            .ToList();

        Assert.Empty(leaked);
    }
}
