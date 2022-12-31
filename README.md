## Building self-contained package (with neccesary runtimes)
```
cd src\TeardownMultiplayerLauncher
dotnet publish -r win10-x64 --self-contained -c Release
```
Result will end up in `src\TeardownMultiplayerLauncher\bin\Release\net7.0-windows\win10-x64`  
A vast majority of the result files may not be neccesary, since the entire runtime and every dll it offers is bundled in with it.