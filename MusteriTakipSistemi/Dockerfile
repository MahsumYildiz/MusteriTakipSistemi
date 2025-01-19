# Temel imaj olarak .NET 8 kullanıyoruz
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# .NET SDK kullanarak proje oluşturma
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MusteriTakipSistemi/MusteriTakipSistemi.csproj", "MusteriTakipSistemi/"]
RUN dotnet restore "MusteriTakipSistemi/MusteriTakipSistemi.csproj"
COPY . .
WORKDIR "/src/MusteriTakipSistemi"
RUN dotnet build "MusteriTakipSistemi.csproj" -c Release -o /app/build

# Yayınlama aşaması
FROM build AS publish
RUN dotnet publish "MusteriTakipSistemi.csproj" -c Release -o /app/publish

# Son aşama, uygulama çalıştırılacak
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MusteriTakipSistemi.dll"]
