#Build Stage
FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /source
COPY . .
RUN dotnet restore "./TelegramTeamprojectBot.csproj" --disable-parallel
RUN dotnet publish "./TelegramTeamprojectBot.csproj" -c release -o /app --no-restore
RUN rm -f /app/appsettings*

#Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal
WORKDIR /app
COPY --from=build /app ./

EXPOSE 5001

ENTRYPOINT ["dotnet",  "TelegramTeamprojectBot.dll"]
#ENTRYPOINT ["tail", "-f", "/dev/null"]