:: INSTALL STRYKER IF REQUIRED
::dotnet tool uninstall -g dotnet-stryker
::dotnet tool install -g dotnet-stryker

SET AppName=AutoCompose
SET kebabAppName=auto-compose

mkdir src\%AppName%.Generator
cd src\%AppName%.Generator

dotnet new classlib
dotnet add package Newtonsoft.Json
dotnet add package MediatR

cd..
cd..

mkdir tests\%AppName%.Tests
cd tests\%AppName%.Tests
dotnet new xunit
dotnet add package Autofixture
dotnet add package Autofixture.AutoMoq
dotnet add package CompareNETObjects
dotnet add package Faker.Net
dotnet add package MockQueryable.Moq
dotnet add package Moq
dotnet add package Newtonsoft.Json
dotnet add package xunit
dotnet add package xunit.Core
dotnet add package xunit.runner.visualstudio

cd..
cd..

cd src

:: CREATE NEW SOLUTION
dotnet new sln -n %AppName%

dotnet sln %AppName%.sln add .\%AppName%.Generator\%AppName%.Generator.csproj
dotnet sln %AppName%.sln add ..\tests\%AppName%.Tests\%AppName%.Tests.csproj

cd..

cd tests

::ADD PROJECT REFERENCES
dotnet add %AppName%.Tests/%AppName%.Tests.csproj reference ..\src\%AppName%.Generator/%AppName%.Generator.csproj

cd..



