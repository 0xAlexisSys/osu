// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Utils;
using Realms;

namespace osu.Game.Beatmaps
{
    /// <summary>
    /// A realm model containing metadata for a beatmap.
    /// </summary>
    /// <remarks>
    /// An instance of this object is stored against each beatmap difficulty.
    /// It is also provided via <see cref="BeatmapSetInfo"/> for convenience and historical purposes.
    /// Note that accessing the metadata via <see cref="BeatmapSetInfo"/> may result in indeterminate results
    /// as metadata can meaningfully differ per beatmap in a set.
    ///
    /// Note that difficulty name is not stored in this metadata but in <see cref="BeatmapInfo"/>.
    /// </remarks>
    public class BeatmapMetadata : RealmObject, IEquatable<BeatmapMetadata>, IDeepCloneable<BeatmapMetadata>
    {
        /// <summary>
        /// The romanised title of this beatmap.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// The unicode title of this beatmap.
        /// </summary>
        public string TitleUnicode { get; set; } = string.Empty;

        /// <summary>
        /// The romanised artist of this beatmap.
        /// </summary>
        public string Artist { get; set; } = string.Empty;

        /// <summary>
        /// The unicode artist of this beatmap.
        /// </summary>
        public string ArtistUnicode { get; set; } = string.Empty;

        /// <summary>
        /// The author of this beatmap.
        /// </summary>
        public string Author { get; set; } = string.Empty;

        /// <summary>
        /// The source of this beatmap.
        /// </summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// The tags of this beatmap.
        /// </summary>
        public string Tags { get; set; } = string.Empty;

        /// <summary>
        /// The time in milliseconds to begin playing the track for preview purposes.
        /// If -1, the track should begin playing at 40% of its length.
        /// </summary>
        public int PreviewTime { get; set; } = -1;

        /// <summary>
        /// The filename of the audio file consumed by this beatmap.
        /// </summary>
        public string AudioFile { get; set; } = string.Empty;

        /// <summary>
        /// The filename of the background image file consumed by this beatmap.
        /// </summary>
        public string BackgroundFile { get; set; } = string.Empty;

        public override string ToString() => this.GetDisplayTitle();

        public BeatmapMetadata DeepClone() => new BeatmapMetadata
        {
            Title = Title,
            TitleUnicode = TitleUnicode,
            Artist = Artist,
            ArtistUnicode = ArtistUnicode,
            Author = Author,
            Source = Source,
            Tags = Tags,
            PreviewTime = PreviewTime,
            AudioFile = AudioFile,
            BackgroundFile = BackgroundFile
        };

        bool IEquatable<BeatmapMetadata>.Equals(BeatmapMetadata? other)
        {
            if (other is null)
                return false;

            return Title == other.Title
                   && TitleUnicode == other.TitleUnicode
                   && Artist == other.Artist
                   && ArtistUnicode == other.ArtistUnicode
                   && Author == other.Author
                   && Source == other.Source
                   && Tags == other.Tags
                   && PreviewTime == other.PreviewTime
                   && AudioFile == other.AudioFile
                   && BackgroundFile == other.BackgroundFile;
        }
    }
}
