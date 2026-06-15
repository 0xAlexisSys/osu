// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Testing;
using osu.Game.Configuration;
using osu.Game.Graphics.Containers;
using osu.Game.Online.API;
using osu.Game.Online.API.Requests;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Overlays;
using osu.Game.Overlays.Profile;
using osu.Game.Overlays.Profile.Header.Components;
using osu.Game.Rulesets.Osu;
using osu.Game.Tests.Resources;
using osu.Game.Users;

namespace osu.Game.Tests.Visual.Online
{
    public partial class TestSceneUserProfileHeader : OsuTestScene
    {
        [Cached]
        private readonly OverlayColourProvider colourProvider = new OverlayColourProvider(OverlayColourScheme.Green);

        private DummyAPIAccess dummyAPI => (DummyAPIAccess)API;

        private readonly ManualResetEventSlim requestLock = new ManualResetEventSlim();

        [Resolved]
        private OsuConfigManager configManager { get; set; } = null!;

        private ProfileHeader header = null!;

        [SetUpSteps]
        public void SetUpSteps()
        {
            AddStep("create header", () =>
            {
                Child = new OsuScrollContainer(Direction.Vertical)
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = header = new ProfileHeader()
                };
            });
        }

        [Test]
        public void TestBasic()
        {
            AddStep("Show example user", () => header.User.Value = new UserProfileData(TestSceneUserProfileOverlay.TEST_USER, new OsuRuleset().RulesetInfo));
        }

        [Test]
        public void TestProfileCoverExpanded()
        {
            AddStep("Set cover to expanded", () => configManager.SetValue(OsuSetting.ProfileCoverExpanded, true));
            AddStep("Show example user", () => header.User.Value = new UserProfileData(TestSceneUserProfileOverlay.TEST_USER, new OsuRuleset().RulesetInfo));
            AddUntilStep("Cover is expanded", () => header.ChildrenOfType<UserCoverBackground>().Single().Height, () => Is.GreaterThan(0));
        }

        [Test]
        public void TestProfileCoverCollapsed()
        {
            AddStep("Set cover to collapsed", () => configManager.SetValue(OsuSetting.ProfileCoverExpanded, false));
            AddStep("Show example user", () => header.User.Value = new UserProfileData(TestSceneUserProfileOverlay.TEST_USER, new OsuRuleset().RulesetInfo));
            AddUntilStep("Cover is collapsed", () => header.ChildrenOfType<UserCoverBackground>().Single().Height, () => Is.EqualTo(0));
        }

        [Test]
        public void TestOnlineState()
        {
            AddStep("Show online user", () => header.User.Value = new UserProfileData(new APIUser
            {
                Id = 1001,
                Username = "IAmOnline",
                LastVisit = DateTimeOffset.Now,
                WasRecentlyOnline = true,
            }, new OsuRuleset().RulesetInfo));

            AddStep("Show offline user", () => header.User.Value = new UserProfileData(new APIUser
            {
                Id = 1002,
                Username = "IAmOffline",
                LastVisit = DateTimeOffset.Now.AddDays(-10),
                WasRecentlyOnline = false,
            }, new OsuRuleset().RulesetInfo));
        }

        [Test]
        public void TestRankedState()
        {
            AddStep("Show ranked user", () => header.User.Value = new UserProfileData(new APIUser
            {
                Id = 2001,
                Username = "RankedUser",
                Groups = new[] { new APIUserGroup { Colour = "#EB47D0", ShortName = "DEV", Name = "Developers" } },
                Statistics = new UserStatistics
                {
                    IsRanked = true,
                    GlobalRank = 15000,
                    CountryRank = 1500,
                    RankHistory = new APIRankHistory
                    {
                        Mode = @"osu",
                        Data = Enumerable.Range(2345, 45).Concat(Enumerable.Range(2109, 40)).ToArray()
                    },
                }
            }, new OsuRuleset().RulesetInfo));

            AddStep("Show unranked user", () => header.User.Value = new UserProfileData(new APIUser
            {
                Id = 2002,
                Username = "UnrankedUser",
                Statistics = new UserStatistics
                {
                    IsRanked = false,
                    // web will sometimes return non-empty rank history even for unranked users.
                    RankHistory = new APIRankHistory
                    {
                        Mode = @"osu",
                        Data = Enumerable.Range(2345, 85).ToArray()
                    },
                }
            }, new OsuRuleset().RulesetInfo));
        }

        [Test]
        public void TestPreviousUsernames()
        {
            AddStep("Show user w/ previous usernames", () => header.User.Value = new UserProfileData(new APIUser
            {
                Id = 727,
                Username = "SomeoneIndecisive",
                CoverUrl = TestResources.COVER_IMAGE_1,
                Groups = new[]
                {
                    new APIUserGroup { Colour = "#EB47D0", ShortName = "DEV", Name = "Developers" },
                },
                Statistics = new UserStatistics
                {
                    IsRanked = false,
                    // web will sometimes return non-empty rank history even for unranked users.
                    RankHistory = new APIRankHistory
                    {
                        Mode = @"osu",
                        Data = Enumerable.Range(2345, 85).ToArray()
                    },
                },
                PreviousUsernames = new[] { "tsrk.", "quoicoubeh", "apagnan", "epita" }
            }, new OsuRuleset().RulesetInfo));
        }

        private APIUser nonFriend => new APIUser
        {
            Id = 727,
            Username = "Whatever",
        };

        [Test]
        public void TestAddFriend()
        {
            AddStep("Setup request", () =>
            {
                requestLock.Reset();

                dummyAPI.HandleRequest = request =>
                {
                    if (request is not AddFriendRequest req)
                        return false;

                    if (req.TargetId != nonFriend.OnlineID)
                        return false;

                    var apiRelation = new APIRelation
                    {
                        TargetID = nonFriend.OnlineID,
                        Mutual = true,
                        RelationType = RelationType.Friend,
                        TargetUser = nonFriend
                    };

                    Task.Run(() =>
                    {
                        requestLock.Wait(3000);
                        dummyAPI.LocalUserState.Friends.Add(apiRelation);
                        req.TriggerSuccess(new AddFriendResponse
                        {
                            UserRelation = apiRelation
                        });
                    });

                    return true;
                };
            });
            AddStep("clear friend list", () => dummyAPI.LocalUserState.Friends.Clear());
            AddStep("Show non-friend user", () => header.User.Value = new UserProfileData(nonFriend, new OsuRuleset().RulesetInfo));
            AddStep("Click followers button", () => this.ChildrenOfType<FollowersButton>().First().TriggerClick());
            AddStep("Complete request", () => requestLock.Set());
            AddUntilStep("Friend added", () => API.LocalUserState.Friends.Any(f => f.TargetID == nonFriend.OnlineID));
        }

        [Test]
        public void TestAddFriendNonMutual()
        {
            AddStep("Setup request", () =>
            {
                requestLock.Reset();

                dummyAPI.HandleRequest = request =>
                {
                    if (request is not AddFriendRequest req)
                        return false;

                    if (req.TargetId != nonFriend.OnlineID)
                        return false;

                    var apiRelation = new APIRelation
                    {
                        TargetID = nonFriend.OnlineID,
                        Mutual = false,
                        RelationType = RelationType.Friend,
                        TargetUser = nonFriend
                    };

                    Task.Run(() =>
                    {
                        requestLock.Wait(3000);
                        dummyAPI.LocalUserState.Friends.Add(apiRelation);
                        req.TriggerSuccess(new AddFriendResponse
                        {
                            UserRelation = apiRelation
                        });
                    });

                    return true;
                };
            });
            AddStep("clear friend list", () => dummyAPI.LocalUserState.Friends.Clear());
            AddStep("Show non-friend user", () => header.User.Value = new UserProfileData(nonFriend, new OsuRuleset().RulesetInfo));
            AddStep("Click followers button", () => this.ChildrenOfType<FollowersButton>().First().TriggerClick());
            AddStep("Complete request", () => requestLock.Set());
            AddUntilStep("Friend added", () => API.LocalUserState.Friends.Any(f => f.TargetID == nonFriend.OnlineID));
        }
    }
}
