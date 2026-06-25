// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Scoring;

namespace osu.Game.Users
{
    /// <summary>
    /// Base class for all structures describing the user's current activity.
    /// </summary>
    public abstract class UserActivity
    {
        public abstract string GetStatus(bool hideIdentifiableInformation = false);
        public virtual string? GetDetails(bool hideIdentifiableInformation = false) => null;

        public class ChoosingBeatmap : UserActivity
        {
            public override string GetStatus(bool hideIdentifiableInformation = false) => @"Choosing a beatmap";
        }

        public class PlayingBeatmap : UserActivity
        {
            private string rulesetPlayingVerb { get; } // TODO: i'm going with this for now, but this is wasteful
            private string beatmapDisplayTitle { get; }

            public PlayingBeatmap(IBeatmapInfo beatmapInfo, IRulesetInfo ruleset)
            {
                rulesetPlayingVerb = ruleset.CreateInstance().PlayingVerb;
                beatmapDisplayTitle = beatmapInfo.GetDisplayTitle();
            }

            public override string GetStatus(bool hideIdentifiableInformation = false) => rulesetPlayingVerb;
            public override string GetDetails(bool hideIdentifiableInformation = false) => beatmapDisplayTitle;
        }

        public class TestingBeatmap : EditingBeatmap
        {
            public TestingBeatmap(IBeatmapInfo beatmapInfo)
                : base(beatmapInfo)
            {
            }

            public override string GetStatus(bool hideIdentifiableInformation = false) => @"Testing a beatmap";
        }

        public class EditingBeatmap : UserActivity
        {
            private string beatmapDisplayTitle { get; }

            public EditingBeatmap(IBeatmapInfo info)
            {
                beatmapDisplayTitle = info.GetDisplayTitle();
            }

            public override string GetStatus(bool hideIdentifiableInformation = false) => @"Editing a beatmap";

            public override string GetDetails(bool hideIdentifiableInformation = false) => hideIdentifiableInformation
                // For now let's assume that showing the beatmap a user is editing could reveal unwanted information.
                ? string.Empty
                : beatmapDisplayTitle;
        }

        public class WatchingReplay : UserActivity
        {
            private string playerName { get; }
            private string? beatmapDisplayTitle { get; }

            public WatchingReplay(ScoreInfo score)
            {
                playerName = score.User.Username;
                beatmapDisplayTitle = score.BeatmapInfo?.GetDisplayTitle();
            }

            public override string GetStatus(bool hideIdentifiableInformation = false) => hideIdentifiableInformation ? @"Watching a replay" : $@"Watching {playerName}'s replay";
            public override string? GetDetails(bool hideIdentifiableInformation = false) => beatmapDisplayTitle;
        }
    }
}
