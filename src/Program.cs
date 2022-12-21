using System.Net;
using Deployf.Botf;
using TelegramTeamprojectBot.Controllers;
using TelegramTeamprojectBot.Factories;
using TelegramTeamprojectBot.Models;
using TelegramTeamprojectBot.Models.User;

BotfProgram.StartBot(args, onConfigure: (svc, cfg) =>
{
    svc.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = cfg.GetConnectionString("Redis");
        options.InstanceName = "RedisDemo_";
    });
    svc.AddSingleton<UserController>();
    svc.AddSingleton<TgBotController>();
    svc.AddSingleton<ITeamprojectApiController>(x =>
        new TeamprojectApiController(cfg.GetConnectionString("TeamprojectApi")));
});
