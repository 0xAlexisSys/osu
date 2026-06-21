// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Platform;

namespace osu.Game.Beatmaps
{
    /// <summary>
    /// A component which handles population of online IDs for beatmaps using a two part lookup procedure.
    /// </summary>
    public class BeatmapUpdaterMetadataLookup : IDisposable
    {
        private readonly IOnlineBeatmapMetadataSource cachedMetadataSource;

        internal BeatmapUpdaterMetadataLookup(Storage storage)
        {
            cachedMetadataSource = new CachedBeatmapMetadataSource(storage);
        }

        /// <summary>
        /// Queue an update for a beatmap set.
        /// </summary>
        /// <remarks>
        /// This may happen during initial import, or at a later stage in response to a user action or server event.
        /// </remarks>
        /// <param name="beatmapSet">The beatmap set to update. Updates will be applied directly (so a transaction should be started if this instance is managed).</param>
        public void Update(BeatmapSetInfo beatmapSet)
        {
            var lookupResults = new List<OnlineBeatmapMetadata?>();

            foreach (var beatmapInfo in beatmapSet.Beatmaps)
            {
                // note that these lookups DO NOT ACTUALLY FULLY GUARANTEE that the beatmap is what it claims it is,
                // i.e. the correctness of this lookup should be treated as APPROXIMATE AT WORST.
                // this is because the beatmap filename is used as a fallback in some scenarios where the MD5 of the beatmap may mismatch.
                // this is considered to be an acceptable casualty so that things can continue to work as expected for users in some rare scenarios
                // (stale beatmap files in beatmap packs, beatmap mirror desyncs).
                // however, all this means that other places such as score submission ARE EXPECTED TO VERIFY THE MD5 OF THE BEATMAP AGAINST THE ONLINE ONE EXPLICITLY AGAIN.
                //
                // additionally note that the online ID stored to the map is EXPLICITLY NOT USED because some users in a silly attempt to "fix" things for themselves on stable
                // would reuse online IDs of already submitted beatmaps, which means that information is pretty much expected to be bogus in a nonzero number of beatmapsets.
                if (!tryLookup(beatmapInfo, out var res))
                    continue;

                if (res == null)
                {
                    lookupResults.Add(null); // mark lookup failure
                    continue;
                }

                lookupResults.Add(res);
            }
        }

        /// <summary>
        /// Attempts to retrieve the <see cref="OnlineBeatmapMetadata"/> for the given <paramref name="beatmapInfo"/>.
        /// </summary>
        /// <param name="beatmapInfo">The beatmap to perform the online lookup for.</param>
        /// <param name="result">The result of the lookup. Can be <see langword="null"/> if no matching beatmap was found (or the lookup failed).</param>
        /// <returns>
        /// <see langword="true"/> if any of the metadata sources were available and returned a valid <paramref name="result"/>.
        /// <see langword="false"/> if none of the metadata sources were available, or if there was insufficient data to return a valid <paramref name="result"/>.
        /// </returns>
        /// <remarks>
        /// There are two cases wherein this method will return <see langword="false"/>:
        /// <list type="bullet">
        /// <item>If neither the local cache or the API are available to query.</item>
        /// <item>If the API is not available to query, and a positive match was not made in the local cache.</item>
        /// </list>
        /// In either case, the online ID read from the .osu file will be preserved, which may not necessarily be what we want.
        /// TODO: reconsider this if/when a better flow for queueing online retrieval is implemented.
        /// </remarks>
        private bool tryLookup(BeatmapInfo beatmapInfo, out OnlineBeatmapMetadata? result)
        {
            if (cachedMetadataSource.TryLookup(beatmapInfo, out result))
                return true;

            result = null;
            return false;
        }

        public void Dispose()
        {
            cachedMetadataSource.Dispose();
        }
    }
}
