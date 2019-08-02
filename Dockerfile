FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build

# Set app as the working directory
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.csproj .

#RUN ["dotnet", "restore", "--configfile", "nuget.config"]
RUN ["dotnet", "restore"]

# copy and publish app and libraries
COPY *.* ./
RUN dotnet publish -c Release -o out

# Change back and remove user-secrets once this is figured out and tied up to keyvalue properly
FROM mcr.microsoft.com/dotnet/core/runtime:2.2 AS runtime
# FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS runtime

# Copy user secrets across
# COPY secrets.json /root/.microsoft/usersecrets/6724c2f3-2018-4e57-9aee-303d3ab984a4/

# copy the library
RUN ["apt-get", "update"]

WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "status-monitor.dll"]