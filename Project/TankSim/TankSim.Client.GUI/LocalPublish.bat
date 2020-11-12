dotnet publish -c Release -f net5.0-windows -r win-x64 /p:PublishSingleFile=True /p:PublishTrimmed=True /p:PublishReadyToRun=True /p:Self-Contained=True
dotnet publish -c Release -f net5.0-windows -r win10-x64 /p:PublishSingleFile=True /p:PublishTrimmed=True /p:PublishReadyToRun=True /p:Self-Contained=True

echo done