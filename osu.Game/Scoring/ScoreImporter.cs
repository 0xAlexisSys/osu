// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using osu.Framework.Extensions;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Game.Beatmaps;
using osu.Game.Database;
using osu.Game.IO.Archives;
using osu.Game.Rulesets;
using osu.Game.Scoring.Legacy;
using osu.Game.Users;
using osu.Game.Utils;
using Realms;

namespace osu.Game.Scoring
{
    public class ScoreImporter : RealmArchiveModelImporter<ScoreInfo>
    {
        private static readonly TimeSpan legacy_score_user_id_get_timeout = TimeSpan.FromSeconds(10.0d);

        public override IEnumerable<string> HandledExtensions => new[] { @".osr" };

        protected override string[] HashableFileTypes => new[] { @".osr" };

        private readonly RulesetStore rulesets;
        private readonly Func<BeatmapManager> beatmaps;
        private readonly Session session;

        public ScoreImporter(RulesetStore rulesets, Func<BeatmapManager> beatmaps, Storage storage, RealmAccess realm, Session session)
            : base(storage, realm)
        {
            this.rulesets = rulesets;
            this.beatmaps = beatmaps;
            this.session = session;
        }

        protected override ScoreInfo? CreateModel(ArchiveReader archive, ImportParameters parameters)
        {
            string name = archive.Filenames.First(f => f.EndsWith(@".osr", StringComparison.OrdinalIgnoreCase));

            using (var stream = archive.GetStream(name))
            {
                try
                {
                    return new DatabasedLegacyScoreDecoder(rulesets, beatmaps()).Parse(stream).ScoreInfo;
                }
                catch (LegacyScoreDecoder.BeatmapNotFoundException notFound)
                {
                    Logger.Log($@"Score '{archive.Name}' failed to import: no corresponding beatmap with the hash '{notFound.Hash}' could be found.", LoggingTarget.Database);
                    return null;
                }
                catch (Exception e)
                {
                    Logger.Log($@"Failed to parse headers of score '{archive.Name}': {e}.", LoggingTarget.Database);
                    return null;
                }
            }
        }

        public Score GetScore(ScoreInfo score) => new LegacyDatabasedScore(score, rulesets, beatmaps(), Files.Store);

        protected override void Populate(ScoreInfo model, ArchiveReader? archive, Realm realm, CancellationToken cancellationToken = default)
        {
            Debug.Assert(model.BeatmapInfo != null);

            // Ensure the beatmap is not detached.
            if (!model.BeatmapInfo.IsManaged)
                model.BeatmapInfo = realm.Find<BeatmapInfo>(model.BeatmapInfo.ID)!;

            if (!model.Ruleset.IsManaged)
                model.Ruleset = realm.Find<RulesetInfo>(model.Ruleset.ShortName)!;

            // These properties are known to be non-null, but these final checks ensure a null hasn't come from somewhere (or the refetch has failed).
            // Under no circumstance do we want these to be written to realm as null.
            ArgumentNullException.ThrowIfNull(model.BeatmapInfo);
            ArgumentNullException.ThrowIfNull(model.Ruleset);

            if (!ModUtils.CheckCompatibleSet(model.Mods))
                throw new InvalidOperationException(@"The score specifies an incompatible set of mods!");

            if (string.IsNullOrEmpty(model.StatisticsJson))
                model.StatisticsJson = JsonConvert.SerializeObject(model.Statistics);

            if (string.IsNullOrEmpty(model.MaximumStatisticsJson))
                model.MaximumStatisticsJson = JsonConvert.SerializeObject(model.MaximumStatistics);
        }

        protected override void PostImport(ScoreInfo model, Realm realm, ImportParameters parameters)
        {
            base.PostImport(model, realm, parameters);

            Debug.Assert(model.BeatmapInfo != null);

            if (model.BeatmapInfo.LastPlayed == null || model.Date > model.BeatmapInfo.LastPlayed)
                model.BeatmapInfo.LastPlayed = model.Date;

            if (model.IsLegacyScore && model.User.ID == User.OTHER_USER_ID)
            {
                // HACK: [alexis] Since stable scores don't store the user ID, it must be retrieved with a GET request;
                //                thankfully, osu.ppy.sh redirects paths like /users/peppy to /users/2. This comes
                //                at the cost of slowing down imports.
                try
                {
                    var responseTask = session.HttpClient.GetAsync($@"https://osu.ppy.sh/users/{model.User.Username}");

                    if (Task.WaitAny([responseTask], legacy_score_user_id_get_timeout) == -1)
                        throw new TimeoutException();

                    var response = responseTask.GetResultSafely();
                    if (response.RequestMessage is { RequestUri: not null })
                        model.User.ID = Convert.ToInt32(response.RequestMessage.RequestUri.AbsolutePath[7..]);
                }
                catch (TimeoutException)
                {
                    Logger.Log($@"User ID GET request for legacy score (ID {model.ID}) timed out", LoggingTarget.Network);
                }
                catch (Exception e)
                {
                    Logger.Log($@"Failed to populate user ID in legacy score (ID {model.ID}): {e}", LoggingTarget.Database);
                }
            }
        }
    }
}
