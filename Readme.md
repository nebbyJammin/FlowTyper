Flow.Launcher.Plugin.FlowTyper
==================

A plugin for the [Flow launcher](https://github.com/Flow-Launcher/Flow.Launcher) that adds a typing test, that measures accuracy and typing speed. Languages/WordLists are from [MonkeyType](https://github.com/monkeytypegame/monkeytype), and is overall inspired by MonkeyType.

### Usage
The default action keyword is `ft`. Use `ft <arguments>`.

#### Typing
In main mode, to start a typing test `ft type`<br>

To return to main mode from typing mode, __press enter on the first result__<br>

In main mode, to change settings, use `ft settings`. Alternatively, you can edit the __.config__ file, located in __*%APPDATA%\\FlowLauncher\\Plugins\\FlowTyper\\.config\\.config.json*__ by default. This is not recommended. If you have modifying the configuration file results in Flow Typer not loading anymore (and you're not sure why), delete the configuration file. Flow Typer will create a new default configuration upon the next restart.

#### Settings and Configuration
> In the `.config` configuration file, parameters are PascalCase. In `ft settings <arg>` they are camelCase.

`ShowIncorrectCharacters` - `[true|false]` When incorrect characters are typed, overwrite the visible word list with the incorrectly typed character(s).

`Language` - Specifies the language / word list to use.

`Punctuation` - `[true|false]` Randomly adds punctuation to a word.

`PunctuationRate` - `[0-1]` Specifies the proportion of words with punctuation.

`Numbers` - `[true|false]` Randomly adds numbers.

`NumbersMin` - `[int]` Smallest Number.

`NumbersMax` - `[int]` Largest Number.

`NumbersRate` - `[0-1]` Specifies the proportion of words that are numbers.

`Capitalise` - `[true|false]` Randomly capitalises words.

`CapitaliseRate` - `[0-1]` Specifies the proportion of words whose first letter is Capitalised.