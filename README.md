[![tau](https://alten.s-ul.eu/rdXkomwh.png)](https://github.com/Altenhh/tau "tau")
# tau for osu!

<div>
    <a href="https://discord.gg/7Y8GXAa"><img src="https://canary.discordapp.com/api/guilds/689728872282849313/widget.png?style=banner2" alt="Join Discord Server"/></a>
</div>

[![release](https://img.shields.io/badge/build-2020.403.6B3-brightgreen?style=flat-square)](https://github.com/Altenhh/tau/releases)
[![GitHub license](https://img.shields.io/github/license/Altenhh/tau.svg?style=flat-square)](https://github.com/Altenhh/tau/LICENSE)
[![GitHub stars](https://img.shields.io/github/stars/Altenhh/tau.svg?style=flat-square)](https://github.com/Altenhh/tau/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/Altenhh/tau.svg?style=flat-square)](https://github.com/Altenhh/tau/network)
[![GitHub issues](https://img.shields.io/github/issues/Altenhh/tau.svg?style=flat-square)](https://github.com/Altenhh/tau/issues)

A customized [osu!](https://github.com/ppy/osu) mode surrounding a paddle and some notes. [Original](https://deadlysprinklez.itch.io/tau) credit to the idea belonging to *pizzapip*.

## Running the Gamemode
We have [prebuilt libraries](https://github.com/Altenhh/tau/releases) for users looking to play the mode without creating a development environment. All releases will work on all operating systems that *osu!* supports.
### [Latest Releases](https://github.com/Altenhh/tau/releases)

### Instructions

Since version [2020.429.0](https://github.com/ppy/osu/releases/tag/2020.429.0) The installation method has now changed and is much more consistent than the old method. If you wish to see the old method of how to install this ruleset the please expand details below.

<details>
<summary><b>Old method</b></summary>
<p>

##### Windows
On Windows, the library must be put in `%localappdata%\osulazer\app-(Current osu!lazer Version)`, inside the directory of the current osu!lazer version. osu!lazer will automatically work with the `.dll` when you open it, so nothing else needs to be done.

##### Linux
On Linux, you will need to extract the app image with the following command line:
```sh
./osu.AppImage --appimage-extract
```
This will extract all files to `squashfs-root`, in this folder go to `usr/bin` and put Tau's dll there.

###### Special thanks to `Kotypey#9393` for figuring out how to install Tau on linux.

##### macOS
On macOS, the library must be put inside the osu!lazer app contents here `osu!.app/Contents/MacOS/`

![Context menu](https://cdn.discordapp.com/attachments/699046236979986483/699060248391974982/tau.png)

You can access the app contents by right-clicking on osu! and clicking "Show Package Contents"

###### Special thanks to `sexnine#6969` for figuring out how to install Tau on macOS.

</p>
</details>

##### New Method
From the osu settings menu scroll down till you see `Open osu! folder`, that button should take you under `%appdata%/osu`.
![open osu! folder](https://github.com/LumpBloom7/sentakki/wiki/images/Instuction1.png)

Copy the ruleset file into the `rulesets` directory, do make sure that duplicate copies of the ruleset is overwritten.

Once done, restart osu!lazer, if lazer is already open. Once lazer is started, you should see the ruleset alongside the standard rulesets on the toolbar at the top.

###### Do note that this instruction will only work with desktop devices.

## Development
When developing or debugging the tau codebase, a few prerequisites are required as following:
* An IDE that supports the C# language in automatic completion, and syntax highlighting; examples of such being [Visual Studio 2019](https://visualstudio.microsoft.com/vs/) and above, or [JetBrains Rider](https://www.jetbrains.com/rider/).
* The [osu!framework](https://github.com/ppy/osu-framework/tree/master/osu.Framework), and [osu!](https://github.com/ppy/osu) codebases are added as dependencies for building

### Source Code
You are able to clone the repository over command line, or by downloading it. Updating this code to the latest commit would be done with `git pull`, inside the tau directory.
```sh
git clone https://github.com/Altenhh/tau.git
cd tau
```

### Building the Gamemode From Source
To build Tau, you will need to have [.NET 5](https://dotnet.microsoft.com/download) installed on your computer.

First, open a terminal and navigate to wherever you have the Tau source code downloaded. Once you are in the root of the repository, enter the directory named `osu.Game.Rulesets.Tau`.

Next, run the command `dotnet build` and wait for the project to be built. This shouldn't take very long.

Once the project has finished building, dotnet should tell you where the binary was built to (usually somewhere along the lines of ./tau/osu.Game.Rulesets.Tau/bin/Debug/netstandardx.x/). Find the .dll binary in the given location and follow the installation instructions above.

## Contributions
All contributions are appreciated, as to improve the mode on its playability and functionality. As this gamemode isn't perfect, we would enjoy all additions to the code through bugfixing and ideas. Contributions should be done over an issue or a pull request, to give maintainers a chance to review changes to the codebase.

For new ideas and features, we would prefer for you to write an issue before trying to add it to show the maintainers.

## License
tau is licenced under the [MIT](https://opensource.org/licenses/MIT) License. For licensing information, refer to the [license file](https://github.com/Altenhh/tau/blob/master/LICENSE) regarding what is permitted regarding the codebase of tau.

The licensing here does not directly apply to [osu!](https://github.com/ppy/osu), as it is bound to its own licensing. What is reflected in our licensing *may* not be allowed in the [osu!](https://github.com/ppy/osu) github repository.
