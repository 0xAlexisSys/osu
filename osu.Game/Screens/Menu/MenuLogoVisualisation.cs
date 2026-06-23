// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Game.Online.API;
using osu.Game.Skinning;
using osu.Game.Users;
using osuTK.Graphics;

namespace osu.Game.Screens.Menu
{
    public partial class MenuLogoVisualisation : LogoVisualisation
    {
        private IBindable<User> user = null!;
        private Bindable<Skin> skin = null!;

        [BackgroundDependencyLoader]
        private void load(DummyAPIAccess api, SkinManager skinManager)
        {
            user = api.User.GetBoundCopy();
            skin = skinManager.CurrentSkin.GetBoundCopy();

            user.ValueChanged += _ => UpdateColour();
            skin.BindValueChanged(_ => UpdateColour(), true);
        }

        protected virtual void UpdateColour()
        {
            Colour = skin.Value.GetConfig<GlobalSkinColours, Color4>(GlobalSkinColours.MenuGlow)?.Value ?? Color4.White;
        }
    }
}
