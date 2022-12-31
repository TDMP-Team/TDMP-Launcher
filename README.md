# TDMP Launcher
The TDMP Launcher is a WPF app which handles auto-updating the TDMP core, launching Teardown, and injection of TDMP into the game.

## User Workflow
When opening the launcher for the first time, the user must browse for their `teardown.exe` where it is installed by Steam.

Upon selecting their `teardown.exe`, the version compatibility status in the UI will reflect if the user's `teardown.exe` is compatible
with TDMP by comparing the exe's MD5 hash with a predefined MD5 hash of the last known working version of the game (this hash is currently hardcoded in the launcher).

If their `teardown.exe` is compatible with TDMP, then they can press PLAY and this will proceed to check for an update for TDMP (only occurs once per 30 mins), download and install if available, then
launch the game and inject TDMP into it after the defined injection delay.

The user can optionally configure the injection delay which will determine how long to wait before injecting TDMP into the Teardown process once detected, which is
helpful if the user is experiencing crashes or freezes with the game due to the launcher injecting TDMP too early in the event that the user's PC is unable to
load the game to the Tuxedo Labs splash video or main menu within the time allotted.

## Architecture
The launcher is built in .NET7/C#/WPF.

The user's state/config is persisted in a `state.json` file which is generated during runtime and lives within the app's directory.
This state contains all user-specific and configuration info such as their selected language/locale, teardown.exe path, injection delay, update state, etc.

All top-level functionality should be exposed via the `CoreApi` object.
The client layer should not be aware of any low-level/implementation details such as underlying services or persistence,
and should only perform core launcher actions via the `CoreApi`.

When updating TDMP, the launcher will get the info for the latest release from the [TDMP-Public](https://github.com/TDMP-Team/TDMP-Public/releases) GitHub repo.
If the latest release version/tag does not match the currently installed version, then the update process will proceed before launching Teardown and injecting TDMP.

## Deployment
Ensure that you've bumped the [semantic version](https://semver.org/) of the launcher by editing the `TeardownMultiplayerLauncher.csproj` > `<PropertyGroup>` > `<AssemblyVersion>x.x.x</AssemblyVersion>`.

Build the launcher with the `Release|x64` configuration, rename the output folder at `bin\x64\Release\net7.0-windows` to `TDMP-Launcher-x.x.x` (where `x.x.x` denotes the launcher version).
It is important to retain this naming scheme, as the Launcher Updater searches for a zip file in the TDMP-Launcher-Public GitHub repo named with this format.

Create a zip archive for the `TDMP-Launcher-x.x.x` folder, to where the top-level within the archive contains the `TDMP-Launcher-x.x.x` folder (essentially archive the launcher folder and not its contents directly).

Publish a new release in the [TDMP-Launcher-Public](https://github.com/TDMP-Team/TDMP-Launcher-Public/releases) repo, which is the final step of the deployment process, making the new release
available to the end-users, and automatically downloaded and installed by the [TDMP-Launcher-Updater](https://github.com/TDMP-Team/TDMP-Launcher-Public).