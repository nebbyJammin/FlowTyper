using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Flow.Launcher.Plugin.FlowTyper.Typer;
using Flow.Launcher.Plugin.FlowTyper.Utils;
using JetBrains.Annotations;

namespace Flow.Launcher.Plugin.FlowTyper
{
    /// <summary>
    /// The entry point for the Flow Typer Plugin.
    /// </summary>
    public partial class FlowTyper : IPlugin, IPluginI18n
    {
        /// <summary>
        /// Represents the state of the program.
        /// </summary>
        public enum FlowTyperState {
            /// <summary>
            /// The main menu state
            /// </summary>
            MAIN,
            /// <summary>
            /// The state that represents when the program is in a typing test
            /// </summary>
            TYPING,
            SETTINGS,
            SETTINGS_INT_EDIT,
            ERROR
        }
        private FlowTyperState state = FlowTyperState.MAIN;
        private PluginInitContext _context;
        private TypingConfig _config;
        private TypingManager _typingManager;
        private const int TEST_WHITESPACE_PADDING = 5;
        private string previousTypingQuery = "";
        private Exception exception;

        private void saveConfig() {
            if (!Directory.Exists(Constants.CONFIG_DIR)) {
                Directory.CreateDirectory(Constants.CONFIG_DIR);
            }

            string path = Constants.CONFIG_DIR + ".config.json";
            string json = JsonSerializer.Serialize(_config);
            File.WriteAllText(path, json);
        }

        private bool loadConfig() {
            string path = Constants.CONFIG_DIR + ".config.json";
            if (File.Exists(path)) {
                try
                {
                    FileStream configFile = new FileStream(Constants.CONFIG_DIR + ".config.json", FileMode.Open);

                    using (StreamReader reader = new StreamReader(configFile))
                    {
                        string json = reader.ReadToEnd();

                        _config = JsonSerializer.Deserialize<TypingConfig>(json);
                    }

                    return true;
                }
                catch(Exception e)
                {
                    exception = e;
                    return false;
                }

            }
            else {
                // If FileNotFound, create the file
                _config = new();
                saveConfig();

                return true;
            }
        }

        void IPlugin.Init(PluginInitContext context)
        {
            _context = context;
            if(loadConfig()) {
                state = FlowTyperState.MAIN;
            }
            else {
                state = FlowTyperState.ERROR;
            }
          
            _typingManager = new TypingManager(_config.Language);
        }

        List<Result> IPlugin.Query(Query query)
        {
            List<Result> results = new List<Result>();

            switch (state) {
                case FlowTyperState.MAIN:
                    results = HandleMainQuery(query); 
                    break;
                case FlowTyperState.TYPING:
                    results = HandleTypingQuery(query);
                    break;
                case FlowTyperState.SETTINGS:
                    results = HandleSettingsQuery(query);
                    break;
                case FlowTyperState.ERROR:
                    results = HandleErrorQuery(query);
                    break;
                case FlowTyperState.SETTINGS_INT_EDIT:
                    //results = HandleIntEditQuery(query);
                    break;
            }   

            return results;
        }
        public string GetTranslatedPluginTitle()
        {
            return _context.API.GetTranslation("flowTyperTitle");
        }

        public string GetTranslatedPluginDescription()
        {
            return _context.API.GetTranslation("flowTyperDescription");
        }

        private List<Result> HandleErrorQuery(Query query) {
            List<Result> results = new List<Result>() {
                new Result() {
                    Title = "Oops, something went wrong...",
                    SubTitle = "If you think this is a bug please report this!",

                },
                new Result() {
                    Title = "Click me to see the crash report!",
                    Action = (ActionContext actionContext) => {
                        throw exception;
                    },
                }
            };

            return results;
        }

        private void ResetQuery(Query query, string suffix = "", bool requery = true, int whitespace = 1) {
            _context.API.ChangeQuery(query.ActionKeyword + new string(' ', whitespace) + suffix);
            _context.API.ReQuery(requery);
        }

        private Result GetMainModeResult(Query query) {
            Result returnMainResult = new TyperResult();
            returnMainResult.Title = _context.API.GetTranslation("flowTyperReturnToMainTitle");
            returnMainResult.SubTitle = _context.API.GetTranslation("flowTyperReturnToMainSubtitle");
            returnMainResult.Action = (ActionContext context) =>
            {
                state = FlowTyperState.MAIN;
                ResetQuery(query);

                return false;
            };

            return returnMainResult;
        }

    }
}