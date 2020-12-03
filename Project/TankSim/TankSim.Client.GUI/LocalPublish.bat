dotnet publish -c Release -f net5.0-windows -r win10-x64 /p:PublishReadyToRun=True /p:Self-Contained=True
dotnet publish -c Release -f net5.0-windows -r win10-x64 /p:PublishReadyToRun=True /p:PublishSingleFile=True /p:PublishTrimmed=true /p:TrimMode=Link
dotnet publish -c Release -f net5.0-windows

echo done