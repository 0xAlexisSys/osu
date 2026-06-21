// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Game.Beatmaps
{
    /// <summary>
    /// Beatmap set info retrieved for previewing locally without having the set downloaded.
    /// </summary>
    public interface IBeatmapSetOnlineInfo
    {
        /// <summary>
        /// Whether or not this beatmap set has explicit content.
        /// </summary>
        bool HasExplicitContent { get; }

        /// <summary>
        /// Whether or not this beatmap set has a background video.
        /// </summary>
        bool HasVideo { get; }

        /// <summary>
        /// Whether or not this beatmap set has a storyboard.
        /// </summary>
        bool HasStoryboard { get; }

        /// <summary>
        /// A small sample clip of this beatmap set's song.
        /// </summary>
        string Preview { get; }

        /// <summary>
        /// The beats per minute of this beatmap set's song.
        /// </summary>
        double BPM { get; }

        /// <summary>
        /// Whether this beatmap set has been favourited by the current user.
        /// </summary>
        bool HasFavourited { get; }

        /// <summary>
        /// The track ID of this beatmap set.
        /// Non-null only if the track is linked to a featured artist track entry.
        /// </summary>
        int? TrackId { get; }

        /// <summary>
        /// Total vote counts of user ratings on a scale of 0..10 where 0 is unused (probably will be fixed at API?).
        /// </summary>
        int[]? Ratings { get; }
    }
}
