// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Allocation;
using osu.Game.Database;
using osu.Game.Models;

namespace osu.Game.Medals
{
    public partial class MedalEvaluator : Component
    {
        public event Action<Medal>? MedalUnlocked;

        private readonly HashSet<string> queuedMedalUnlocks = new HashSet<string>();

        [Resolved]
        private RealmAccess realm { get; set; } = null!;

        public void Unlock(string medalSlug)
        {
            realm.Write(r =>
            {
                HashSet<string> unlockedMedalSlugs = r.All<RealmMedal>().AsEnumerable().Select(m => m.Slug).ToHashSet();

                if (unlockedMedalSlugs.Contains(medalSlug)) return;

                Medal? medal = Medal.DEFINITIONS.FirstOrDefault(m => m.Slug == medalSlug);

                if (medal is null) return;

                r.Add(new RealmMedal
                {
                    ID = Guid.NewGuid(),
                    Slug = medal.Slug,
                    UnlockedAt = DateTimeOffset.UtcNow,
                });
                MedalUnlocked?.Invoke(medal);
            });
        }

        public void AddToQueue(string medalSlug)
        {
            queuedMedalUnlocks.Add(medalSlug);
        }

        public void ClearQueue()
        {
            queuedMedalUnlocks.Clear();
        }

        public void ProcessQueue()
        {
            realm.Write(r =>
            {
                HashSet<string> unlockedMedalSlugs = r.All<RealmMedal>().AsEnumerable().Select(m => m.Slug).ToHashSet();

                foreach (string medalSlug in queuedMedalUnlocks)
                {
                    if (unlockedMedalSlugs.Contains(medalSlug)) continue;

                    Medal? medal = Medal.DEFINITIONS.FirstOrDefault(m => m.Slug == medalSlug);

                    if (medal is null) continue;

                    r.Add(new RealmMedal
                    {
                        ID = Guid.NewGuid(),
                        Slug = medal.Slug,
                        UnlockedAt = DateTimeOffset.UtcNow,
                    });
                    unlockedMedalSlugs.Add(medal.Slug);
                    MedalUnlocked?.Invoke(medal);
                }

                ClearQueue();
            });
        }
    }
}
