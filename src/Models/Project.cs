using System.Text.RegularExpressions;

namespace TelegramTeamprojectBot.Models;

public class Project
{
    public bool IsSuccess { get; }
    private string Period { get; }
    private string ProjectName { get; }
    private string Title { get; }
    private string Description { get; }
    private string Curator { get; }

    public Project(string? response)
    {
        if (response == null)
        {
            IsSuccess = false;
            return;
        }
        var startNameIndex = response.IndexOf("project_name", StringComparison.Ordinal) + 15;
        var endNameIndex = response.IndexOf("\"", startNameIndex, StringComparison.Ordinal);
        ProjectName = RemoveTags(response.Substring(startNameIndex, endNameIndex - startNameIndex));

        var startTitleIndex = response.IndexOf("title", StringComparison.Ordinal) + 8;
        var endTitleIndex = response.IndexOf("\"", startTitleIndex, StringComparison.Ordinal);
        Title = RemoveTags(response.Substring(startTitleIndex, endTitleIndex - startTitleIndex));

        var startDescriptionIndex = response.IndexOf("description", StringComparison.Ordinal) + 14;
        var endDescriptionIndex = response.IndexOf("\"", startDescriptionIndex, StringComparison.Ordinal);
        Description = RemoveTags(response.Substring(startDescriptionIndex, endDescriptionIndex - startDescriptionIndex));

        var startPeriodIndex = response.IndexOf("period", StringComparison.Ordinal) + 9;
        var endPeriodIndex = response.IndexOf("\"", startPeriodIndex, StringComparison.Ordinal);
        Period = RemoveTags(response.Substring(startPeriodIndex, endPeriodIndex - startPeriodIndex));

        var startCuratorIndex = response.IndexOf("fullname", StringComparison.Ordinal) + 11;
        var endCuratorIndex = response.IndexOf("\"", startCuratorIndex, StringComparison.Ordinal);
        Curator = RemoveTags(response.Substring(startCuratorIndex, endCuratorIndex - startCuratorIndex));
    }

    public override string ToString()
    {
        return $"Period = {Period}," +
               $"\nProjectName = {ProjectName}," +
               $"\nTitle = {Title}," +
               $"\nDescription = {Description}," +
               $"\nCurator = {Curator}\n";
    }

    public string RemoveTags(string input)
    {
        return Regex.Replace(input, @"<[^>]*>", String.Empty);
    }
}