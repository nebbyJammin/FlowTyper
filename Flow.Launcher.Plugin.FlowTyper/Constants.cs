using System;

namespace Flow.Launcher.Plugin.FlowTyper {
    public static class Constants {
        public const string PLUGIN_SUBDIR_NAME = @"FlowTyper\";
        public static readonly string PLUGIN_DIR = Environment.ExpandEnvironmentVariables(@"%AppData%\FlowLauncher\Plugins\" + PLUGIN_SUBDIR_NAME);
        public static readonly string IMAGES_DIR = PLUGIN_DIR + @"Images\";
    }
}