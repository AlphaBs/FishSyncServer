FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

COPY ./FishSyncServer.sln ./
COPY ./FishBucketServer/AlphabetUpdateServer.csproj ./FishBucketServer/
COPY ./FishBucket/FishBucket.csproj ./FishBucket/
COPY ./FishBucket.Alphabet/FishBucket.Alphabet.csproj ./FishBucket.Alphabet/
COPY ./FishBucket.ApiClient/FishBucket.ApiClient.csproj ./FishBucket.ApiClient/
COPY ./FishBucket.ChecksumStorages/FishBucket.ChecksumStorages.csproj ./FishBucket.ChecksumStorages/
COPY ./FishBucket.SyncClient/FishBucket.SyncClient.csproj ./FishBucket.SyncClient/

RUN dotnet restore "./FishBucketServer/AlphabetUpdateServer.csproj"

COPY . .
RUN dotnet publish "./FishBucketServer/AlphabetUpdateServer.csproj" -c release -o /app --no-restore

FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/aspnet:8.0 AS final
EXPOSE 8080
WORKDIR /app
COPY --from=build /app .
USER $APP_UID
ENTRYPOINT ["./AlphabetUpdateServer"]