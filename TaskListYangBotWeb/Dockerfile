#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
# укажем, на основе какого образа будем 
# делать наш контейнер. Для сборки приложения используем dotnet-sdk 
# и назовём этот образ builder
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# укажем директорию для нашего приложения внутри контейнера
WORKDIR /Application
EXPOSE 8080
EXPOSE 8082

# Скопируем все файлы из проекта в файловую систему контейнера
COPY . ./
# Запустим restore для загрузки зависимостей
RUN dotnet restore
# Опубликуем собранный dll в папку "output"
RUN dotnet publish -c Release -o output

# Теперь соберём образ, в котором наше приложение 
# будет запущено. Для запуска приложения достаточно
# среды выполнения aspnet, без sdk
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /Application
ENV ASPNETCORE_ENVIRONMENT=Development
# Скопируем файлы приложения из предыдущего образа 
COPY --from=build /Application/output .
# укажем команду, которая будет запускать приложение
ENTRYPOINT ["dotnet", "TaskListYangBotWeb.dll"]