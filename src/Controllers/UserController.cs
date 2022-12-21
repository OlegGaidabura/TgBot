using System.Data;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.VisualBasic.CompilerServices;
using TelegramTeamprojectBot.DTOs;
using TelegramTeamprojectBot.Extensions;
using TelegramTeamprojectBot.Factories;
using TelegramTeamprojectBot.Models.User;

namespace TelegramTeamprojectBot.Controllers;

public class UserController
{
    private readonly IDistributedCache _cache;
    private readonly ITeamprojectApiController _teamprojectApi;
    
    public UserController(IDistributedCache cache, ITeamprojectApiController teamprojectApi)
    {
        _cache = cache;
        _teamprojectApi = teamprojectApi;
    }

    /// <summary>
    /// Search user in redis
    /// </summary>
    /// <param name="id">Telegram Chat Id or Teamproject Id</param>
    /// <returns><see cref="IUser"/> or null</returns>
    public async Task<IUser?> GetUserAsync(string id)
    {
        var userJson = await _cache.GetRecordAsync<string>(id);
        if (userJson is null) return default;
        var role = await _cache.GetRecordAsync<Role>(id + "_role");
        var userFactory = GetUserFactory(role);
        return userFactory.CreateUser(userJson, _teamprojectApi);
    }

    private IUserFactory GetUserFactory(Role role) => role switch
    { 
        Role.Student => new StudentUserFactory(),
        Role.Rop => new RopUserFactory(),
        Role.Curator => new CuratorUserFactory(),
        _ => throw new NotSupportedException()
    };

    /// <summary>
    /// Save user state. Use when end working with <see cref="IUser"/> model
    /// </summary>
    /// <param name="user"><see cref="IUser"/></param>
    public async Task SetUserAsync(IUser user)
    {
        await _cache.SetRecordAsync(user.ChatId.ToString() + "_role", user.GetRole());
        await _cache.SetRecordAsync(user.ChatId.ToString(), user);
        
        await _cache.SetRecordAsync(user.TeamprojectId + "_role", user.GetRole());
        await _cache.SetRecordAsync(user.TeamprojectId, user);
    }

    /// <summary>
    /// Register new <see cref="IUser"/> in redis. Use this method when <see cref="GetUserAsync"/> return null
    /// </summary>
    /// <param name="chatId">Telegram Chat Id</param>
    /// <param name="token">Teamproject Token</param>
    /// <returns><see cref="IUser"/></returns>
    public async Task<IUser?> RegisterUserAsync(long chatId, string token)
    {
        var dto = await GetNewUserDataAsync(token);
        if (dto is null) return default;
        var userFactory = GetUserFactory(ParseRoleByString(dto!.GetModel().user_role));
        var user = userFactory.CreateUser(chatId, dto, _teamprojectApi);
        await SetUserAsync(user);
        return user;
    }

    public void RemoveUser(IUser user)
    {
        _cache.RemoveRecord(user.TeamprojectId);
        _cache.RemoveRecord(user.ChatId.ToString());
    }

    public void RemoveUser(long chatId)
    {
        var result = GetUserAsync(chatId.ToString()).Result;
        if (result != null) RemoveUser(result);
        else
            throw new InvalidOperationException();
    }

    private async Task<UserDto?> GetNewUserDataAsync(string token)
    {
        var userId = await _teamprojectApi.RegisterUserAsync(token);
        if (!userId.IsSuccess) return null;
        var userData = await _teamprojectApi.GetUserDataAsync(userId.Data!);
        return !userData.IsSuccess ? null : userData.Data!;
    }

    private Role ParseRoleByString(string data)
    {
        data = data.ToLower();
        return data == Role.Student.ToString().ToLower() ? Role.Student :
            data == Role.Curator.ToString().ToLower() ? Role.Curator :
            data == Role.Rop.ToString().ToLower() ? Role.Rop : 
            throw new InvalidExpressionException();
    }
}