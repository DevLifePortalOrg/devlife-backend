# --- Build Stage ---
# გამოიყენეთ .NET SDK image პროექტის ასაწყობად
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# დააკოპირეთ DevLifeBackend.csproj ფაილი და აღადგინეთ NuGet პაკეტები.
# ეს ეტაპი გამოიყენება დამოკიდებულებების ქეშირებისთვის.
COPY DevLifeBackend.csproj .
RUN dotnet restore

# დააკოპირეთ პროექტის დანარჩენი ნაწილი
COPY . .

# ააწყვეთ და გამოაქვეყნეთ (publish) აპლიკაცია Release კონფიგურაციით
# -c Release: Build configuration
# -o /app: Output directory inside the container
RUN dotnet publish "DevLifeBackend.csproj" -c Release -o /app

# --- Runtime Stage ---
# გამოიყენეთ ASP.NET Runtime image აპლიკაციის გასაშვებად (უფრო პატარაა ვიდრე SDK image)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# დააკოპირეთ გამოქვეყნებული აპლიკაცია Build Stage-დან
COPY --from=build /app .

# გამოაქვეყნეთ პორტები, რომლებზეც აპლიკაცია მოუსმენს
# (შეამოწმეთ თქვენი launchSettings.json -ში, თუ სხვა პორტებს იყენებთ)
EXPOSE 80
EXPOSE 443

# კონტეინერის გაშვებისას შესასრულებელი ბრძანება
ENTRYPOINT ["dotnet", "DevLifeBackend.dll"]