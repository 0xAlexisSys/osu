// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using NUnit.Framework;
using osu.Framework.Testing;
using osu.Game.Online.API;
using osu.Game.Online.Metadata;
using osu.Game.Online.Rooms;
using osu.Game.Screens.Menu;
using osuTK.Input;

namespace osu.Game.Tests.Visual.Menus
{
    public partial class TestSceneMainMenu : OsuGameTestScene
    {
        public override void SetUpSteps()
        {
            base.SetUpSteps();
            AddStep("disable return to top on idle", () => Game.ChildrenOfType<ButtonSystem>().Single().ReturnToTopOnIdle = false);
        }

        [Test]
        public void TestDailyChallenge()
        {
            AddStep("set up API", () => ((DummyAPIAccess)API).HandleRequest = req =>
            {
                switch (req)
                {
                    case GetRoomRequest getRoomRequest:
                        if (getRoomRequest.RoomId != 1234)
                            return false;

                        var beatmap = CreateAPIBeatmap();
                        beatmap.OnlineID = 1001;
                        getRoomRequest.TriggerSuccess(new Room
                        {
                            RoomID = 1234,
                            Name = "Aug 8, 2024",
                            Playlist =
                            [
                                new PlaylistItem(beatmap)
                            ],
                            StartDate = DateTimeOffset.Now.AddMinutes(-30),
                            EndDate = DateTimeOffset.Now.AddSeconds(60)
                        });
                        return true;

                    default:
                        return false;
                }
            });

            AddStep("beatmap of the day active", () => Game.ChildrenOfType<IMetadataClient>().Single().DailyChallengeUpdated(new DailyChallengeInfo
            {
                RoomID = 1234,
            }));

            AddStep("enter menu", () => InputManager.Key(Key.P));
            AddStep("enter submenu", () => InputManager.Key(Key.P));
            AddStep("enter daily challenge", () => InputManager.Key(Key.D));

            AddUntilStep("wait for daily challenge screen", () => Game.ScreenStack.CurrentScreen, Is.TypeOf<Screens.OnlinePlay.DailyChallenge.DailyChallenge>);
        }
    }
}
