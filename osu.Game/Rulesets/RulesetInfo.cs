// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using JetBrains.Annotations;
using osu.Game.Rulesets.Difficulty;
using Realms;

namespace osu.Game.Rulesets
{
    /// <summary>
    /// A representation of a ruleset's metadata.
    /// </summary>
    [MapTo("Ruleset")]
    public class RulesetInfo : RealmObject, IEquatable<RulesetInfo>, IComparable<RulesetInfo>
    {
        /// <summary>
        /// An acronym defined by the ruleset that can be used as a permanent identifier.
        /// </summary>
        [PrimaryKey]
        public string ShortName { get; set; } = string.Empty;

        /// <summary>
        /// The internal ID of this ruleset.
        /// </summary>
        [Indexed]
        public int ID { get; set; } = -1;

        /// <summary>
        /// The user-exposed name of this ruleset.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// A string representation of this ruleset, to be used with reflection to instantiate the ruleset represented by this metadata.
        /// </summary>
        public string InstantiationInfo { get; set; } = string.Empty;

        /// <summary>
        /// Stores the last applied <see cref="DifficultyCalculator.Version"/>
        /// </summary>
        public int LastAppliedDifficultyVersion { get; set; }

        public RulesetInfo(string shortName, string name, string instantiationInfo, int id)
        {
            ShortName = shortName;
            Name = name;
            InstantiationInfo = instantiationInfo;
            ID = id;
        }

        [UsedImplicitly]
        public RulesetInfo()
        {
        }

        public bool Available { get; set; }

        public bool Equals(RulesetInfo? other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null) return false;

            return ShortName == other.ShortName;
        }

        public int CompareTo(RulesetInfo? other)
        {
            if (ID >= 0 && other?.ID >= 0)
                return ID.CompareTo(other.ID);

            return string.Compare(ShortName, other?.ShortName, StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            // Importantly, ignore the underlying realm hash code, as it will usually not match.
            var hashCode = new HashCode();
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            hashCode.Add(ShortName);
            return hashCode.ToHashCode();
        }

        public override string ToString() => Name;

        public RulesetInfo Clone() => new RulesetInfo
        {
            ID = ID,
            Name = Name,
            ShortName = ShortName,
            InstantiationInfo = InstantiationInfo,
            Available = Available,
            LastAppliedDifficultyVersion = LastAppliedDifficultyVersion,
        };

        public Ruleset CreateInstance()
        {
            if (!Available)
                throw new RulesetLoadException(@"Ruleset not available");

            var type = Type.GetType(InstantiationInfo);

            if (type is null)
                throw new RulesetLoadException(@"Type lookup failure");

            if (Activator.CreateInstance(type) is not Ruleset ruleset)
                throw new RulesetLoadException(@"Instantiation failure");

            // overwrite the pre-populated RulesetInfo with a potentially database attached copy.
            // TODO: figure if we still want/need this after switching to realm.
            // ruleset.RulesetInfo = this;

            return ruleset;
        }
    }
}
