dotnet publish -c Release -r win-x64 /p:PublishSingleFile=True /p:PublishTrimmed=True /p:PublishReadyToRun=True /p:Self-Contained=True
dotnet publish -c Release -r win10-x64 /p:PublishSingleFile=True /p:PublishTrimmed=True /p:PublishReadyToRun=True /p:Self-Contained=True

echo done