namespace TelegramTeamprojectBot.DTOs;

public record DescriptionDto(
    string user_role,
    string about_roles,
    string about_iters,
    string about_iters_tasks,
    string about_iters_grade,
    string about_project_docs
)
{
}