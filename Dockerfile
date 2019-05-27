FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime
LABEL maintainer="luojun96@live.cn"
WORKDIR /app
COPY ./ /app
ENTRYPOINT ["dotnet", "NetGo.Web.dll"]