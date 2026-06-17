// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Localisation;

namespace osu.Game.Localisation
{
    public static class NamedOverlayComponentStrings
    {
        private const string prefix = @"osu.Game.Resources.Localisation.NamedOverlayComponent";

        /// <summary>
        /// "get up-to-date on community happenings"
        /// </summary>
        public static LocalisableString NewsDescription => new TranslatableString(getKey(@"news_description"), @"get up-to-date on community happenings");

        /// <summary>
        /// "knowledge base"
        /// </summary>
        public static LocalisableString WikiDescription => new TranslatableString(getKey(@"wiki_description"), @"knowledge base");

        private static string getKey(string key) => $@"{prefix}:{key}";
    }
}
