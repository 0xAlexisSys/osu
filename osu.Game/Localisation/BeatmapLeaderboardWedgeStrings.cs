// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Localisation;

namespace osu.Game.Localisation
{
    public class BeatmapLeaderboardWedgeStrings
    {
        private const string prefix = @"osu.Game.Resources.Localisation.BeatmapLeaderboardWedge";

        /// <summary>
        /// "Sort"
        /// </summary>
        public static LocalisableString Sort => new TranslatableString(getKey(@"sort"), @"Sort");

        /// <summary>
        /// "Score"
        /// </summary>
        public static LocalisableString Score => new TranslatableString(getKey(@"score"), @"Score");

        /// <summary>
        /// "Accuracy"
        /// </summary>
        public static LocalisableString Accuracy => new TranslatableString(getKey(@"accuracy"), @"Accuracy");

        /// <summary>
        /// "Max Combo"
        /// </summary>
        public static LocalisableString MaxCombo => new TranslatableString(getKey(@"max_combo"), @"Max Combo");

        /// <summary>
        /// "Misses"
        /// </summary>
        public static LocalisableString Misses => new TranslatableString(getKey(@"misses"), @"Misses");

        /// <summary>
        /// "Date"
        /// </summary>
        public static LocalisableString Date => new TranslatableString(getKey(@"date"), @"Date");

        /// <summary>
        /// "Personal Best"
        /// </summary>
        public static LocalisableString PersonalBest => new TranslatableString(getKey(@"personal_best"), @"Personal Best");

        /// <summary>
        /// "Personal Best (#{0:N0} of {1:N0})"
        /// </summary>
        public static LocalisableString PersonalBestWithPosition(int position, int totalCount) => new TranslatableString(getKey(@"personal_best_with_position"), @"Personal Best (#{0:N0} of {1:N0})", position, totalCount);

        private static string getKey(string key) => $@"{prefix}:{key}";
    }
}
