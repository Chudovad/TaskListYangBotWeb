#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
# ������, �� ������ ������ ������ ����� 
# ������ ��� ���������. ��� ������ ���������� ���������� dotnet-sdk 
# � ������ ���� ����� builder
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# ������ ���������� ��� ������ ���������� ������ ����������
WORKDIR /Application
EXPOSE 8080
EXPOSE 8082

# ��������� ��� ����� �� ������� � �������� ������� ����������
COPY . ./
# �������� restore ��� �������� ������������
RUN dotnet restore
# ���������� ��������� dll � ����� "output"
RUN dotnet publish -c Release -o output

# ������ ������ �����, � ������� ���� ���������� 
# ����� ��������. ��� ������� ���������� ����������
# ����� ���������� aspnet, ��� sdk
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /Application
ENV ASPNETCORE_ENVIRONMENT=Development
# ��������� ����� ���������� �� ����������� ������ 
COPY --from=build /Application/output .
# ������ �������, ������� ����� ��������� ����������
ENTRYPOINT ["dotnet", "TaskListYangBotWeb.dll"]