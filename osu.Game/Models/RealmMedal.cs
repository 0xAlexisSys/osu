// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Database;
using Realms;

namespace osu.Game.Models
{
    [MapTo("Medal")]
    public class RealmMedal : RealmObject, IHasGuidPrimaryKey
    {
        [PrimaryKey]
        public Guid ID { get; set; }

        [Indexed]
        public string Slug { get; set; } = string.Empty;

        public DateTimeOffset UnlockedAt { get; set; }
    }
}
