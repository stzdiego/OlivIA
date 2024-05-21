#!/bin/bash
rm -rf Olivia.Tests/TestResults
rm -rf Reports
dotnet test Olivia.Tests --collect:"XPlat Code Coverage" --settings coverlet.runsettings
reportgenerator "-reports:Olivia.Tests/TestResults/**/*.cobertura.xml" "-targetdir:Reports" -reporttypes:"Html"
coverlet Olivia.Tests/bin/Debug/net8.0/Olivia.Tests.dll --target "dotnet" --targetargs "test Olivia.Tests --no-build" --output Olivia.Tests/TestResults/ --exclude-by-file "**/Migrations/*,**/Program.cs"
#open Reports/index.html