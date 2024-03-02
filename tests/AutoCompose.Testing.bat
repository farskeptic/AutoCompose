::set stryker = C:\Users\%UserId%\.dotnet\tools\dotnet-stryker.exe
:: TEST THE PROJECTS
cd CodeComposer.Tests


:: Run normal unit tests
dotnet test

:: Run mutation testing
cmd /c dotnet stryker --project-file=CodeComposer.Domain.csproj
cmd /c dotnet stryker --project-file=CodeComposer.Infrastructure.csproj
cmd /c dotnet stryker --project-file=CodeComposer.AppServices.csproj
cmd /c dotnet stryker --project-file=CodeComposer.Web.csproj

cd..

ECHO ADD TEST TO VERIFY TAGS ARE REPLACED...WHICH REQUIRES NAMES SUPPLIER DEPENDENCY