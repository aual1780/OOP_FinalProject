dotnet publish -c Release -f net5.0 -r win10-x64 /p:PublishReadyToRun=True /p:Self-Contained=True
dotnet publish -c Release -f net5.0

echo done