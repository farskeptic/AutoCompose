SET AppName=AutoCompose
SET kebabAppName=auto-compose

mkdir %AppName%.Sample
cd %AppName%.Sample

dotnet new classlib

cd..

mkdir %AppName%.Sample_20
cd %AppName%.Sample_20

dotnet new classlib -f netstandard2.0

cd..

mkdir %AppName%.Sample_21
cd %AppName%.Sample_21

dotnet new classlib -f  netstandard2.1

cd..

mkdir %AppName%.Sample_31
cd %AppName%.Sample_31

dotnet new classlib -f  netcoreapp3.1

cd..

mkdir %AppName%.Sample_50
cd %AppName%.Sample_50

dotnet new classlib -f  net5.0

cd..

mkdir %AppName%.Sample_60
cd %AppName%.Sample_60

dotnet new classlib -f  net6.0

cd..

mkdir %AppName%.Sample_70
cd %AppName%.Sample_70

dotnet new classlib -f  net7.0

cd..

mkdir %AppName%.Sample_80
cd %AppName%.Sample_80

dotnet new classlib -f  net8.0

cd..

::ADD PROJECT REFERENCES

dotnet add %AppName%.Sample/%AppName%.Sample.csproj reference %AppName%.Sample_20/%AppName%.Sample_20.csproj
dotnet add %AppName%.Sample/%AppName%.Sample.csproj reference %AppName%.Sample_21/%AppName%.Sample_21.csproj
dotnet add %AppName%.Sample/%AppName%.Sample.csproj reference %AppName%.Sample_31/%AppName%.Sample_31.csproj
dotnet add %AppName%.Sample/%AppName%.Sample.csproj reference %AppName%.Sample_50/%AppName%.Sample_50.csproj
dotnet add %AppName%.Sample/%AppName%.Sample.csproj reference %AppName%.Sample_60/%AppName%.Sample_60.csproj
dotnet add %AppName%.Sample/%AppName%.Sample.csproj reference %AppName%.Sample_70/%AppName%.Sample_70.csproj
dotnet add %AppName%.Sample/%AppName%.Sample.csproj reference %AppName%.Sample_80/%AppName%.Sample_80.csproj


:: Go back to src directory
cd..
cd src


:: ADD TO EXISTING SOLUTION

dotnet sln %AppName%.sln add ..\samples\%AppName%.Sample\%AppName%.Sample.csproj
dotnet sln %AppName%.sln add ..\samples\%AppName%.Sample_20\%AppName%.Sample_20.csproj
dotnet sln %AppName%.sln add ..\samples\%AppName%.Sample_21\%AppName%.Sample_21.csproj
dotnet sln %AppName%.sln add ..\samples\%AppName%.Sample_31\%AppName%.Sample_31.csproj
dotnet sln %AppName%.sln add ..\samples\%AppName%.Sample_50\%AppName%.Sample_50.csproj
dotnet sln %AppName%.sln add ..\samples\%AppName%.Sample_60\%AppName%.Sample_60.csproj
dotnet sln %AppName%.sln add ..\samples\%AppName%.Sample_70\%AppName%.Sample_70.csproj
dotnet sln %AppName%.sln add ..\samples\%AppName%.Sample_80\%AppName%.Sample_80.csproj





