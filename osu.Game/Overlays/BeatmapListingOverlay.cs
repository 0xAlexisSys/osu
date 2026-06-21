// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Localisation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.Containers;
using osu.Game.Overlays.BeatmapListing;
using osu.Game.Resources.Localisation.Web;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Overlays
{
    public partial class BeatmapListingOverlay : OnlineOverlay<BeatmapListingHeader>
    {
        private BeatmapListingFilterControl filterControl => Header.FilterControl;

        public BeatmapListingOverlay()
            : base(OverlayColourScheme.Blue)
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Child = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Children = new Drawable[]
                {
                    new Container
                    {
                        AutoSizeAxes = Axes.Y,
                        RelativeSizeAxes = Axes.X,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = ColourProvider.Background5,
                            },
                        },
                    },
                }
            };
        }

        public void ShowWithSearch(string query)
        {
            Show();
            ScrollFlow.ScrollToStart();
        }

        public void ShowWithGenreFilter(SearchGenre genre)
        {
            ShowWithSearch(string.Empty);
        }

        public void ShowWithLanguageFilter(SearchLanguage language)
        {
            ShowWithSearch(string.Empty);
        }

        protected override BeatmapListingHeader CreateHeader() => new BeatmapListingHeader();

        protected override Color4 BackgroundColour => ColourProvider.Background6;

        public partial class NotFoundDrawable : CompositeDrawable
        {
            public NotFoundDrawable()
            {
                RelativeSizeAxes = Axes.X;
                Height = 250;
                Alpha = 0;
                Margin = new MarginPadding { Top = 15 };
            }

            [BackgroundDependencyLoader]
            private void load(LargeTextureStore textures)
            {
                AddInternal(new FillFlowContainer
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Y,
                    AutoSizeAxes = Axes.X,
                    Direction = FillDirection.Horizontal,
                    Spacing = new Vector2(10, 0),
                    Children = new Drawable[]
                    {
                        new Sprite
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            RelativeSizeAxes = Axes.Both,
                            FillMode = FillMode.Fit,
                            Texture = textures.Get(@"Online/not-found")
                        },
                        new OsuSpriteText
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Text = BeatmapsStrings.ListingSearchNotFoundQuote,
                        }
                    }
                });
            }
        }

        // TODO: localisation requires Text/LinkFlowContainer support for localising strings with links inside
        // (https://github.com/ppy/osu-framework/issues/4530)
        public partial class SupporterRequiredDrawable : CompositeDrawable
        {
            private LinkFlowContainer supporterRequiredText;

            private readonly List<LocalisableString> filtersUsed;

            public SupporterRequiredDrawable(List<LocalisableString> filtersUsed)
            {
                RelativeSizeAxes = Axes.X;
                Height = 225;
                Alpha = 0;

                this.filtersUsed = filtersUsed;
            }

            [BackgroundDependencyLoader]
            private void load(LargeTextureStore textures)
            {
                AddInternal(new FillFlowContainer
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Y,
                    AutoSizeAxes = Axes.X,
                    Direction = FillDirection.Horizontal,
                    Children = new Drawable[]
                    {
                        new Sprite
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            RelativeSizeAxes = Axes.Both,
                            FillMode = FillMode.Fit,
                            Texture = textures.Get(@"Online/supporter-required"),
                        },
                        supporterRequiredText = new LinkFlowContainer
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            AutoSizeAxes = Axes.Both,
                            Margin = new MarginPadding { Bottom = 10 },
                        },
                    }
                });

                supporterRequiredText.AddText(
                    BeatmapsStrings.ListingSearchSupporterFilterQuoteDefault(string.Join(" and ", filtersUsed), "").ToString(),
                    t =>
                    {
                        t.Font = OsuFont.GetFont(size: 16);
                        t.Colour = Colour4.White;
                    }
                );

                supporterRequiredText.AddLink(BeatmapsStrings.ListingSearchSupporterFilterQuoteLinkText.ToString(), @"/store/products/supporter-tag");
            }
        }
    }
}
