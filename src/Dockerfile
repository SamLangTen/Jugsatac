
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS builder
LABEL maintainer="samlangten@outlook.com"
COPY . /src/Jugsatac
WORKDIR /src/Jugsatac
RUN dotnet restore && \ 
    dotnet publish --output /app/ --configuration Release

FROM mcr.microsoft.com/dotnet/core/runtime:3.1
LABEL maintainer="samlangten@outlook.com"
WORKDIR /app
COPY --from=builder /app .
ENTRYPOINT ["dotnet", "Jugsatac.dll"]
CMD ["-h"]