// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using osu.Game.Database;
using osu.Game.Extensions;
using osu.Game.Models;
using Realms;

namespace osu.Game.Beatmaps
{
    /// <summary>
    /// A realm model containing metadata for a beatmap set (containing multiple <see cref="BeatmapInfo"/>s).
    /// </summary>
    [MapTo("BeatmapSet")]
    public class BeatmapSetInfo : RealmObject, IHasGuidPrimaryKey, IHasRealmFiles, ISoftDelete, IEquatable<BeatmapSetInfo>
    {
        [PrimaryKey]
        public Guid ID { get; set; }

        /// <summary>
        /// The date when this beatmap was imported.
        /// </summary>
        public DateTimeOffset DateAdded { get; set; }

        /// <summary>
        /// The best-effort metadata representing this set. In the case metadata differs between contained beatmaps, one is arbitrarily chosen.
        /// </summary>
        [JsonIgnore]
        [Ignored]
        public BeatmapMetadata Metadata => Beatmaps.FirstOrDefault()?.Metadata ?? new BeatmapMetadata();

        /// <summary>
        /// All beatmaps contained in this set.
        /// </summary>
        public IList<BeatmapInfo> Beatmaps { get; } = null!;

        public IList<RealmNamedFileUsage> Files { get; } = null!;

        public bool DeletePending { get; set; }

        /// <summary>
        /// Whether this beatmap set has been favourited.
        /// </summary>
        public bool HasFavourited { get; set; }

        public string Hash { get; set; } = string.Empty;

        /// <summary>
        /// Whether deleting this beatmap set should be prohibited (due to it being a system requirement to be present).
        /// </summary>
        public bool Protected { get; set; }

        /// <summary>
        /// The maximum star difficulty of all beatmaps in this set.
        /// </summary>
        public double MaxStarDifficulty => Beatmaps.Count == 0 ? 0 : Beatmaps.Max(b => b.StarRating);

        /// <summary>
        /// The maximum playable length in milliseconds of all beatmaps in this set.
        /// </summary>
        public double MaxLength => Beatmaps.Count == 0 ? 0 : Beatmaps.Max(b => b.Length);

        /// <summary>
        /// The maximum BPM of all beatmaps in this set.
        /// </summary>
        public double MaxBPM => Beatmaps.Count == 0 ? 0 : Beatmaps.Max(b => b.BPM);

        public BeatmapSetInfo(IEnumerable<BeatmapInfo>? beatmaps = null)
            : this()
        {
            ID = Guid.NewGuid();
            if (beatmaps is not null)
                Beatmaps.AddRange(beatmaps);
        }

        [UsedImplicitly] // Realm
        private BeatmapSetInfo()
        {
        }

        public bool Equals(BeatmapSetInfo? other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null) return false;

            return ID == other.ID;
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return ID.GetHashCode();
        }

        public override string ToString() => Metadata.GetDisplayString();

        IEnumerable<INamedFileUsage> IHasNamedFiles.Files => Files;
    }
}
