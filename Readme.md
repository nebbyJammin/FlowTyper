Flow.Launcher.Plugin.FlowTyper
==================

A plugin for the [Flow launcher](https://github.com/Flow-Launcher/Flow.Launcher) that adds a simple endless typing test, that measures accuracy and typing speed. Languages/WordLists are from [MonkeyType](https://github.com/monkeytypegame/monkeytype), and is overall inspired by MonkeyType.

This project includes files derived from the MonkeyType project
(https://github.com/monkeytypegame/monkeytype), which are licensed under the
GNU General Public License v3.0.

The MonkeyType-derived files are:
- Flow.Launcher.Plugin.FlowTyper/Static/Wordlists/...

These files remain under the GPL-3.0 license and include the required copyright
and license notices from the original project.

All other files in this project are still licensed under GPL-3.0.

### Usage
The default action keyword is `ft`. Use `ft <arguments>`.

#### Typing
In main mode, to start a typing test `ft type`<br>

To return to main mode from typing mode, __press enter on the first result__<br>

In main mode, to change settings, use `ft settings`. Alternatively, you can edit the __.config__ file, located in __*%APPDATA%\\FlowLauncher\\Plugins\\FlowTyper\\.config\\.config.json*__ by default. This is not recommended. If you have modified the configuration file, and that has stopped Flow Typer / Flow Launcher from loading, delete the configuration file. Flow Typer will create a new default configuration upon the next restart.

#### Settings and Configuration
> These settings correspond one-to-one with the JSON configuration in camelCase, but depending on localization, may vary within Flow Launcher / Flow Typer.

`ShowIncorrectCharacters` - `[true|false]` When incorrect characters are typed, overwrite the visible word list with the incorrectly typed character(s).

`Language` - Specifies the language / word list to use.

`NumbersRate` - `[0-1]` Specifies the proportion of words that are numbers.

`PunctuationRate` - `[0-1]` Specifies the proportion of (non-number) "words" with punctuation attached to them.

`CapitalizeRate` - `[0-1]` Specifies the proportion of (non-number) words whose first letter is capitalized.