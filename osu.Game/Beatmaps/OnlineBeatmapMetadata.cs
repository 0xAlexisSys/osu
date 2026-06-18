// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;

namespace osu.Game.Beatmaps
{
    /// <summary>
    /// This structure contains parts of beatmap metadata which are involved with the online parts
    /// of the game, and therefore must be treated with particular care.
    /// This data is retrieved from trusted sources (such as osu-web API, or a locally downloaded sqlite snapshot
    /// of osu-web metadata).
    /// </summary>
    public class OnlineBeatmapMetadata
    {
        /// <summary>
        /// The online ID of the beatmap.
        /// </summary>
        public int BeatmapID { get; init; }

        /// <summary>
        /// The online ID of the beatmap set.
        /// </summary>
        public int BeatmapSetID { get; init; }

        /// <summary>
        /// The online ID of the author.
        /// </summary>
        public int AuthorID { get; init; }

        /// <summary>
        /// The MD5 hash of the beatmap. Used to verify integrity.
        /// </summary>
        public string MD5Hash { get; init; } = string.Empty;

        /// <summary>
        /// The date when this metadata was last updated.
        /// </summary>
        public DateTimeOffset LastUpdated { get; init; }

        /// <summary>
        /// The list of tags that users have assigned to this beatmap.
        /// </summary>
        public List<string> UserTags { get; } = [];
    }
}
