// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using osu.Game.Collections;
using osu.Game.Database;
using osu.Game.Models;
using osu.Game.Rulesets;
using osu.Game.Scoring;
using Realms;

namespace osu.Game.Beatmaps
{
    /// <summary>
    /// A realm model containing metadata for a single beatmap difficulty.
    /// This should generally include anything which is required to be filtered on at song select, or anything pertaining to storage of beatmaps in the client.
    /// </summary>
    /// <remarks>
    /// There are some legacy fields in this model which are not persisted to realm. These are isolated in a code region within the class and should eventually be migrated to `Beatmap`.
    /// </remarks>
    [Serializable]
    [MapTo("Beatmap")]
    public class BeatmapInfo : RealmObject, IHasGuidPrimaryKey, IEquatable<BeatmapInfo>
    {
        [PrimaryKey]
        public Guid ID { get; set; }

        /// <summary>
        /// The user-specified name given to this beatmap.
        /// </summary>
        public string DifficultyName { get; set; } = string.Empty;

        /// <summary>
        /// The ruleset this beatmap was made for.
        /// </summary>
        public RulesetInfo Ruleset { get; set; } = null!;

        /// <summary>
        /// The difficulty settings for this beatmap.
        /// </summary>
        public BeatmapDifficulty Difficulty { get; set; } = null!;

        /// <summary>
        /// The metadata representing this beatmap. May be shared between multiple beatmaps.
        /// </summary>
        public BeatmapMetadata Metadata { get; set; } = null!;

        [JsonIgnore]
        [Backlink(nameof(ScoreInfo.BeatmapInfo))]
        public IQueryable<ScoreInfo> Scores { get; } = null!;

        public BeatmapUserSettings UserSettings { get; set; } = null!;

        public BeatmapInfo(RulesetInfo? ruleset = null, BeatmapDifficulty? difficulty = null, BeatmapMetadata? metadata = null)
        {
            ID = Guid.NewGuid();
            Ruleset = ruleset ?? new RulesetInfo
            {
                ShortName = @"osu",
                Name = @"null placeholder ruleset"
            };
            Difficulty = difficulty ?? new BeatmapDifficulty();
            Metadata = metadata ?? new BeatmapMetadata();
            UserSettings = new BeatmapUserSettings();
        }

        [UsedImplicitly]
        protected BeatmapInfo()
        {
        }

        /// <summary>
        /// The beatmap set this beatmap is part of.
        /// </summary>
        public BeatmapSetInfo? BeatmapSet { get; set; }

        [Ignored]
        public RealmNamedFileUsage? File => BeatmapSet?.Files.FirstOrDefault(f => f.File.Hash == Hash);

        /// <summary>
        /// The total length in milliseconds of this beatmap.
        /// </summary>
        public double Length { get; set; }

        /// <summary>
        /// The most common BPM of this beatmap.
        /// </summary>
        public double BPM { get; set; }

        /// <summary>
        /// The SHA-256 hash representing this beatmap's contents.
        /// </summary>
        public string Hash { get; set; } = string.Empty;

        /// <summary>
        /// Defaults to -1 (meaning not-yet-calculated).
        /// Will likely be superseded with a better storage considering ruleset/mods.
        /// </summary>
        public double StarRating { get; set; } = -1;

        /// <summary>
        /// MD5 is kept for legacy support (matching against replays etc.).
        /// </summary>
        [Indexed]
        public string MD5Hash { get; set; } = string.Empty;

        /// <summary>
        /// The last time of a modification.
        /// </summary>
        public DateTimeOffset? LastUpdate { get; set; }

        [JsonIgnore]
        public bool Hidden { get; set; }

        /// <summary>
        /// The number of hitobjects in the beatmap with a distinct end time.
        /// Defaults to -1 (meaning not-yet-calculated).
        /// </summary>
        /// <remarks>
        /// Canonically, these are hitobjects are either sliders or spinners.
        /// </remarks>
        public int EndTimeObjectCount { get; set; } = -1;

        /// <summary>
        /// The total number of hitobjects in the beatmap.
        /// Defaults to -1 (meaning not-yet-calculated).
        /// </summary>
        public int TotalObjectCount { get; set; } = -1;

        /// <summary>
        /// The time at which this beatmap was last played by the local user.
        /// </summary>
        public DateTimeOffset? LastPlayed { get; set; }

        public int BeatDivisor { get; set; } = 4;

        /// <summary>
        /// The time in milliseconds when last exiting the editor with this beatmap loaded.
        /// </summary>
        public double? EditorTimestamp { get; set; }

        public bool Equals(BeatmapInfo? other)
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

        public bool AudioEquals(BeatmapInfo? other) => other is not null
                                                       && BeatmapSet is not null
                                                       && other.BeatmapSet is not null
                                                       && compareFiles(this, other, m => m.AudioFile);

        public bool BackgroundEquals(BeatmapInfo? other) => other is not null
                                                            && BeatmapSet is not null
                                                            && other.BeatmapSet is not null
                                                            && compareFiles(this, other, m => m.BackgroundFile);

        private static bool compareFiles(BeatmapInfo x, BeatmapInfo y, Func<BeatmapMetadata, string> getFilename)
        {
            Debug.Assert(x.BeatmapSet is not null);
            Debug.Assert(y.BeatmapSet is not null);

            string? fileHashX = x.BeatmapSet.GetFile(getFilename(x.Metadata))?.File.Hash;
            string? fileHashY = y.BeatmapSet.GetFile(getFilename(y.Metadata))?.File.Hash;

            return fileHashX == fileHashY;
        }

        /// <summary>
        /// When updating a beatmap, its hashes will change. Collections currently track beatmaps by hash, so they need to be updated.
        /// This method will handle updating
        /// </summary>
        /// <param name="realm">A realm instance in an active write transaction.</param>
        /// <param name="previousMD5Hash">The previous MD5 hash of the beatmap before update.</param>
        public void TransferCollectionReferences(Realm realm, string previousMD5Hash)
        {
            var collections = realm.All<BeatmapCollection>().AsEnumerable().Where(c => c.BeatmapMD5Hashes.Contains(previousMD5Hash));

            foreach (var c in collections)
            {
                c.BeatmapMD5Hashes.Remove(previousMD5Hash);
                c.BeatmapMD5Hashes.Add(MD5Hash);
            }
        }

        /// <summary>
        /// Local scores are retained separate from a beatmap's lifetime, matched via <see cref="ScoreInfo.BeatmapHash"/>.
        /// Therefore we need to detach / reattach scores when a beatmap is edited or imported.
        /// </summary>
        /// <param name="realm">A realm instance in an active write transaction.</param>
        public void UpdateLocalScores(Realm realm)
        {
            // first disassociate any scores which are already attached and no longer valid.
            foreach (var score in Scores)
                score.BeatmapInfo = null;

            // then attach any scores which match the new hash.
            foreach (var score in realm.All<ScoreInfo>().Where(s => s.BeatmapHash == Hash))
                score.BeatmapInfo = this;
        }

        #region Compatibility properties

        [Ignored]
        public string? Path => File?.Filename;

        public BeatmapInfo Clone() => (BeatmapInfo)this.Detach().MemberwiseClone();

        public override string ToString() => this.GetDisplayTitle();

        #endregion
    }
}
