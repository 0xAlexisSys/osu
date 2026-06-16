// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Rooms;
using osuTK;

namespace osu.Game.Screens.OnlinePlay.Matchmaking.Match.BeatmapSelect
{
    public partial class MatchmakingSelectPanelRandom : MatchmakingSelectPanel
    {
        public MatchmakingSelectPanelRandom(MultiplayerPlaylistItem item)
            : base(item)
        {
        }

        private readonly List<APIUser> users = new List<APIUser>();

        private Sample? swooshSample;

        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {
            swooshSample = audio.Samples.Get(@"SongSelect/options-pop-out");
        }

        public override void PresentAsChosenBeatmap(MatchmakingPlaylistItem playlistItem)
        {
            this.MoveTo(Vector2.Zero, 1000, Easing.OutExpo)
                .ScaleTo(1.5f, 1000, Easing.OutExpo);

            swooshSample?.Play();
        }

        public override void AddUser(APIUser user)
        {
            users.Add(user);
        }

        public override void RemoveUser(APIUser user)
        {
            users.Remove(user);
        }
    }
}
