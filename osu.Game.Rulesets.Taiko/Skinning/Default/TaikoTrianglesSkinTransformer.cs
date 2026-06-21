// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Graphics;
using osu.Game.Screens.Play.HUD;
using osu.Game.Skinning;
using osuTK;

namespace osu.Game.Rulesets.Taiko.Skinning.Default
{
    public class TaikoTrianglesSkinTransformer : SkinTransformer
    {
        public TaikoTrianglesSkinTransformer(ISkin skin)
            : base(skin)
        {
        }

        public override Drawable? GetDrawableComponent(ISkinComponentLookup lookup)
        {
            switch (lookup)
            {
                case GlobalSkinnableContainerLookup containerLookup:
                {
                    // Only handle per ruleset defaults here.
                    if (containerLookup.Ruleset == null)
                        return base.GetDrawableComponent(lookup);

                    switch (containerLookup.Lookup)
                    {
                        case GlobalSkinnableContainers.MainHUDComponents:
                            return new DefaultSkinComponentsContainer(container =>
                            {
                                var leaderboard = container.OfType<DrawableGameplayLeaderboard>().FirstOrDefault();

                                if (leaderboard != null)
                                {
                                    leaderboard.Position = new Vector2(40, -100);
                                    leaderboard.Height = 180;
                                    leaderboard.Anchor = Anchor.BottomLeft;
                                    leaderboard.Origin = Anchor.BottomLeft;
                                }

                                foreach (var d in container.OfType<ISerialisableDrawable>())
                                    d.UsesFixedAnchor = true;
                            })
                            {
                                RelativeSizeAxes = Axes.Both,
                                Child = new DrawableGameplayLeaderboard(),
                            };
                    }

                    return null;
                }
            }

            return base.GetDrawableComponent(lookup);
        }
    }
}
