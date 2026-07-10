// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Localisation;

namespace osu.Game.Localisation
{
    public static class ToolbarStrings
    {
        private const string prefix = @"osu.Game.Resources.Localisation.Toolbar";

        /// <summary>
        /// "home"
        /// </summary>
        public static LocalisableString HomeHeaderTitle => new TranslatableString(getKey(@"home_header_title"), @"home");

        /// <summary>
        /// "return to the main menu"
        /// </summary>
        public static LocalisableString HomeHeaderDescription => new TranslatableString(getKey(@"home_header_description"), @"return to the main menu");

        /// <summary>
        /// "play some {0}"
        /// </summary>
        public static LocalisableString PlaySomeRuleset(string arg0) => new TranslatableString(getKey(@"play_some_ruleset"), @"play some {0}", arg0);

        /// <summary>
        /// "running"
        /// </summary>
        public static LocalisableString TimeRunning => new TranslatableString(getKey(@"time_running"), @"running");

        private static string getKey(string key) => $@"{prefix}:{key}";
    }
}
