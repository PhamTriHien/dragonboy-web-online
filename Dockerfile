FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY DragonBoy_Shared/ ./DragonBoy_Shared/
COPY DragonBoy_Web/ ./DragonBoy_Web/

RUN dotnet publish "DragonBoy_Web/DragonBoy_Web_Server/DragonBoy_Web_Server.csproj" -c Release -p:UseAppHost=false -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish ./
COPY --from=build /src/DragonBoy_Shared/Assets ./DragonBoy_Shared/Assets
COPY --from=build /src/DragonBoy_Shared/Backgrounds ./DragonBoy_Shared/Backgrounds
COPY --from=build /src/DragonBoy_Shared/custom_logo.png ./DragonBoy_Shared/custom_logo.png

ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_gcServer=0
ENV PORT=10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "DragonBoy_Web_Server.dll"]
