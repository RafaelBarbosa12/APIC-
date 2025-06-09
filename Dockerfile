# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Copy only the project file first and restore dependencies
COPY ["RecommendationApi.csproj", "./"]
RUN dotnet restore "RecommendationApi.csproj" \
    --runtime linux-musl-x64

# Copy the rest of the source code
COPY . .
RUN dotnet publish "RecommendationApi.csproj" \
    --configuration Release \
    --runtime linux-musl-x64 \
    --self-contained true \
    --no-restore \
    -p:PublishTrimmed=true \
    -p:PublishSingleFile=true \
    -o /app/publish

# Final Stage
FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-alpine
WORKDIR /app

# Copy the published application
COPY --from=build /app/publish .

# Configure environment
ENV ASPNETCORE_URLS=http://+:80
ENV PythonApi__BaseUrl="http://python-recommendation-service:5000"

# Expose the port
EXPOSE 80

# Set the entry point
ENTRYPOINT ["./RecommendationApi"]