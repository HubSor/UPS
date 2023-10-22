cd ./UPS
dotnet ef migrations add $1 --project ../Data/Data.csproj -c UnitOfWork