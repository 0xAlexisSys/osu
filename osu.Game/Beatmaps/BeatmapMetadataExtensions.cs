// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Localisation;
using osu.Game.Screens.Select;

namespace osu.Game.Beatmaps
{
    public static class BeatmapMetadataExtensions
    {
        public static bool Match(BeatmapMetadata metadataInfo, FilterCriteria.OptionalTextFilter filter) => filter.Matches(metadataInfo.Author)
                                                                                                            || filter.Matches(metadataInfo.Artist)
                                                                                                            || filter.Matches(metadataInfo.ArtistUnicode)
                                                                                                            || filter.Matches(metadataInfo.Title)
                                                                                                            || filter.Matches(metadataInfo.TitleUnicode)
                                                                                                            || filter.Matches(metadataInfo.Source)
                                                                                                            || filter.Matches(metadataInfo.Tags);

        /// <summary>
        /// A user-presentable display title representing this metadata.
        /// </summary>
        public static string GetDisplayTitle(this BeatmapMetadata metadataInfo)
        {
            string author = string.IsNullOrEmpty(metadataInfo.Author) ? string.Empty : $" ({metadataInfo.Author})";

            string artist = string.IsNullOrEmpty(metadataInfo.Artist) ? "unknown artist" : metadataInfo.Artist;
            string title = string.IsNullOrEmpty(metadataInfo.Title) ? "unknown title" : metadataInfo.Title;

            return $"{artist} - {title}{author}".Trim();
        }

        /// <summary>
        /// A user-presentable display title representing this beatmap, with localisation handling for potentially romanisable fields.
        /// </summary>
        public static RomanisableString GetDisplayTitleRomanisable(this BeatmapMetadata metadataInfo, bool includeCreator = true)
        {
            string author = !includeCreator || string.IsNullOrEmpty(metadataInfo.Author) ? string.Empty : $"({metadataInfo.Author})";
            string artistUnicode = string.IsNullOrEmpty(metadataInfo.ArtistUnicode) ? metadataInfo.Artist : metadataInfo.ArtistUnicode;
            string titleUnicode = string.IsNullOrEmpty(metadataInfo.TitleUnicode) ? metadataInfo.Title : metadataInfo.TitleUnicode;

            return new RomanisableString($"{artistUnicode} - {titleUnicode} {author}".Trim(), $"{metadataInfo.Artist} - {metadataInfo.Title} {author}".Trim());
        }
    }
}
