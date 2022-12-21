using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TelegramTeamprojectBot.DTOs;

public class UserDto
{
    public UserRawDto[] User;

    public UserDto(UserRawDto[] user)
    {
        User = user;
    }

    public UserModel GetModel() =>
        new UserModel(
            User[0].id,
            User[0].firstName,
            User[0].lastName,
            User[0].patronymic,
            User[0].user_role,
            User.Select(userModel => userModel.project_id).ToArray()
        );
}