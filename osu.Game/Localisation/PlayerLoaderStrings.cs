// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Localisation;

namespace osu.Game.Localisation
{
    public static class PlayerLoaderStrings
    {
        private const string prefix = @"osu.Game.Resources.Localisation.PlayerLoader";

        /// <summary>
        /// "This beatmap contains scenes with rapidly flashing colours"
        /// </summary>
        public static LocalisableString EpilepsyWarningTitle => new TranslatableString(getKey(@"epilepsy_warning_title"), @"This beatmap contains scenes with rapidly flashing colours");

        /// <summary>
        /// "Please take caution if you are affected by epilepsy."
        /// </summary>
        public static LocalisableString EpilepsyWarningContent => new TranslatableString(getKey(@"epilepsy_warning_content"), @"Please take caution if you are affected by epilepsy.");

        /// <summary>
        /// "Loading paused..."
        /// </summary>
        public static LocalisableString LoadingPaused => new TranslatableString(getKey(@"loading_paused"), @"Loading paused...");

        private static string getKey(string key) => $@"{prefix}:{key}";
    }
}
