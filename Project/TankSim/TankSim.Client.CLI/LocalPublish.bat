dotnet publish -c Release -f net5.0 -r win-x64 /p:PublishSingleFile=True /p:PublishTrimmed=True /p:PublishReadyToRun=True /p:Self-Contained=True
dotnet publish -c Release -f net5.0 -r win10-x64 /p:PublishSingleFile=True /p:PublishTrimmed=True /p:PublishReadyToRun=True /p:Self-Contained=True

echo done