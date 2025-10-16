# ---- Build ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj và restore
COPY PRN232.Backend.csproj ./
RUN dotnet restore

# copy source và publish
COPY . .
RUN dotnet publish -c Release -o /app/publish

# ---- Runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# KHÔNG đặt ASPNETCORE_URLS với ${PORT} (Docker ENV không expand lúc runtime)
# Render sẽ set biến PORT; code đã đọc PORT và bind rồi.
# EXPOSE chỉ để tài liệu; Render không dựa vào đây:
EXPOSE 8080

ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "PRN232.Backend.dll"]
