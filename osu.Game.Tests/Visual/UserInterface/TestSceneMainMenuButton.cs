// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Graphics;
using osu.Game.Localisation;
using osu.Game.Online.API;
using osu.Game.Online.Metadata;
using osu.Game.Screens.Menu;
using osuTK.Graphics;
using osuTK.Input;

namespace osu.Game.Tests.Visual.UserInterface
{
    [TestFixture]
    public partial class TestSceneMainMenuButton : OsuTestScene
    {
        [Resolved]
        private MetadataClient metadataClient { get; set; } = null!;

        private DummyAPIAccess dummyAPI => (DummyAPIAccess)API;

        [Test]
        public void TestStandardButton()
        {
            AddStep("add button", () => Child = new MainMenuButton(
                ButtonSystemStrings.Play, @"button-default-select", OsuIcon.Player, new Color4(102, 68, 204, 255), (_, _) => { }, 0, Key.P)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                ButtonSystemState = ButtonSystemState.TopLevel,
            });
        }
    }
}
