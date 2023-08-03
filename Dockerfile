FROM mcr.microsoft.com/dotnet/sdk AS build
WORKDIR /src

COPY . .
RUN dotnet restore

COPY src .
RUN dotnet publish -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet AS runtime
WORKDIR /publish

COPY --from=build /publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "RockPaperScissors.Api.dll"]