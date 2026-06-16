// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Extensions.LocalisationExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Database;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Overlays;
using osu.Game.Resources.Localisation.Web;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Screens.Select
{
    public partial class BeatmapTitleWedge
    {
        public partial class FavouriteButton : OsuClickableContainer
        {
            private readonly BindableBool isFavourite = new BindableBool();

            private Box background = null!;
            private Box hoverLayer = null!;
            private HeartIcon icon = null!;

            [Resolved]
            private RealmAccess realm { get; set; } = null!;

            [Resolved]
            private OverlayColourProvider colourProvider { get; set; } = null!;

            [Resolved]
            private OsuColour colours { get; set; } = null!;

            private IDisposable? realmSubscription;
            private BeatmapSetInfo? beatmapSet;

            public FavouriteButton()
            {
                AutoSizeAxes = Axes.Both;
            }

            [BackgroundDependencyLoader]
            private void load()
            {
                Masking = true;
                CornerRadius = 5;
                Shear = OsuGame.SHEAR;

                AddRange(new Drawable[]
                {
                    background = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.Black.Opacity(0.2f),
                    },
                    new FillFlowContainer
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Horizontal,
                        Margin = new MarginPadding { Left = 10, Right = 10, Vertical = 5f },
                        Spacing = new Vector2(4f, 0f),
                        Shear = -OsuGame.SHEAR,
                        Children = new Drawable[]
                        {
                            icon = new HeartIcon
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Size = new Vector2(OsuFont.Style.Heading2.Size),
                            },
                        },
                    },
                    hoverLayer = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0,
                        Colour = Colour4.White.Opacity(0.1f),
                        Blending = BlendingParameters.Additive,
                    },
                });
                Action = toggleFavourite;
            }

            protected override bool OnHover(HoverEvent e)
            {
                hoverLayer.FadeIn(500, Easing.OutQuint);
                return true;
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                base.OnHoverLost(e);
                hoverLayer.FadeOut(500, Easing.OutQuint);
            }

            public override LocalisableString TooltipText => isFavourite.Value ? BeatmapsetsStrings.ShowDetailsUnfavourite.ToSentence() : BeatmapsetsStrings.ShowDetailsFavourite.ToSentence();

            public void UpdateFavouriteState(BeatmapSetInfo? set, bool withAnimation = false)
            {
                beatmapSet = set;
                realmSubscription?.Dispose();
                realmSubscription = null;

                Enabled.Value = set != null;

                if (Enabled.Value)
                {
                    realmSubscription = realm.RegisterForNotifications(r => r.All<BeatmapSetInfo>().Where(s => s.ID == set!.ID), (sender, _) =>
                    {
                        if (sender.Count > 0)
                        {
                            isFavourite.Value = sender[0].HasFavourited;
                            updateVisualState(withAnimation);
                        }
                    });

                    isFavourite.Value = realm.Run(r => r.Find<BeatmapSetInfo>(set!.ID)?.HasFavourited ?? false);
                }
                else
                {
                    isFavourite.Value = false;
                    updateVisualState(withAnimation);
                }
            }

            private void updateVisualState(bool withAnimation)
            {
                background.FadeColour(isFavourite.Value ? colours.Pink4.Darken(1f).Opacity(0.5f) : Color4.Black.Opacity(0.2f), 500, Easing.OutQuint);
                icon.SetActive(isFavourite.Value, withAnimation);
            }

            private void toggleFavourite()
            {
                realm.Write(r =>
                {
                    var set = r.Find<BeatmapSetInfo>(beatmapSet?.ID);
                    if (set != null)
                        set.HasFavourited = !set.HasFavourited;
                });
            }

            protected override void Dispose(bool isDisposing)
            {
                base.Dispose(isDisposing);

                realmSubscription?.Dispose();
                realmSubscription = null;
            }
        }

        private partial class HeartIcon : CompositeDrawable
        {
            private readonly SpriteIcon icon;

            [Resolved]
            private OverlayColourProvider colourProvider { get; set; } = null!;

            [Resolved]
            private OsuColour colours { get; set; } = null!;

            public HeartIcon()
            {
                InternalChildren = new Drawable[]
                {
                    icon = new SpriteIcon
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Icon = FontAwesome.Regular.Heart,
                        RelativeSizeAxes = Axes.Both,
                    },
                };
            }

            private const double pop_out_duration = 100;
            private const double pop_in_duration = 500;

            private bool active;

            public void SetActive(bool active, bool withAnimation = false)
            {
                if (this.active == active)
                    return;

                this.active = active;

                FinishTransforms(true);

                if (active)
                {
                    transitionIcon(FontAwesome.Solid.Heart, colours.Pink1, emphasised: withAnimation);

                    if (withAnimation)
                        playFavouriteAnimation();
                }
                else
                {
                    transitionIcon(FontAwesome.Regular.Heart, colourProvider.Content2);
                }
            }

            private void transitionIcon(IconUsage newIcon, Color4 colour, bool emphasised = false)
            {
                icon.ScaleTo(emphasised ? 0.5f : 0.8f, pop_out_duration, Easing.OutQuad)
                    .Then()
                    .FadeColour(colour)
                    .Schedule(() => icon.Icon = newIcon)
                    .ScaleTo(1, pop_in_duration, Easing.OutElasticHalf);
            }

            private void playFavouriteAnimation()
            {
                var circle = new FastCircle
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Scale = new Vector2(0.5f),
                    Blending = BlendingParameters.Additive,
                    Alpha = 0,
                    Depth = float.MinValue,
                };

                AddInternal(circle);

                circle.Delay(pop_out_duration)
                      .FadeTo(0.35f)
                      .FadeOut(1400, Easing.OutCubic)
                      .ScaleTo(10f, 750, Easing.OutQuint)
                      .Expire();

                const int num_particles = 8;

                static float randomFloat(float min, float max) => min + Random.Shared.NextSingle() * (max - min);

                for (int i = 0; i < num_particles; i++)
                {
                    double duration = randomFloat(600, 1000);
                    float angle = (i + randomFloat(0, 0.75f)) / num_particles * MathF.PI * 2;
                    var direction = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
                    float distance = randomFloat(DrawWidth / 2, DrawWidth);

                    var particle = new FastCircle
                    {
                        Position = direction * DrawWidth / 4,
                        Size = new Vector2(3),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Blending = BlendingParameters.Additive,
                        Alpha = 0,
                        Depth = 2,
                        Colour = colours.Pink,
                    };

                    AddInternal(particle);

                    particle
                        .Delay(pop_out_duration)
                        .FadeTo(0.5f)
                        .MoveTo(direction * distance, 1300, Easing.OutQuint)
                        .FadeOut(duration, Easing.Out)
                        .ScaleTo(0.5f, duration)
                        .Expire();
                }
            }
        }
    }
}
