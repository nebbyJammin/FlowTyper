using System;

namespace Flow.Launcher.Plugin.FlowTyper.Typer {
    public record struct WordLists (
        string[] wordlists
    );
    public record struct WordList (
        string name,
        string[] words
    );
}