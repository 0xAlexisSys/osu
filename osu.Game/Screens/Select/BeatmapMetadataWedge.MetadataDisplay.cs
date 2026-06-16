// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Localisation;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Overlays;

namespace osu.Game.Screens.Select
{
    public partial class BeatmapMetadataWedge
    {
        public partial class MetadataDisplay : FillFlowContainer
        {
            private readonly OsuSpriteText labelText;
            private readonly OsuSpriteText contentText;
            private readonly OsuSpriteText contentClickableText;
            private readonly OsuHoverContainer contentClickableTextAction;
            private readonly TagsLine contentTags;

            private (LocalisableString value, Action? action)? data;

            public (LocalisableString value, Action? action)? Data
            {
                set
                {
                    data = value;

                    if (value?.action != null)
                        setClickableText(value.Value.value, value.Value.action);
                    else if (value.HasValue)
                        setText(value.Value.value);
                }
            }

            public (string[] tags, Action<string>? action)? Tags
            {
                set
                {
                    if (value?.action != null)
                        setTags(value.Value.tags, value.Value.action);
                    else if (value.HasValue)
                        setText(@"N/A");
                }
            }

            public MetadataDisplay(LocalisableString label)
            {
                RelativeSizeAxes = Axes.X;
                AutoSizeAxes = Axes.Y;

                Padding = new MarginPadding { Right = 10 };

                InternalChildren = new Drawable[]
                {
                    labelText = new OsuSpriteText
                    {
                        Text = label,
                        Font = OsuFont.Style.Caption1.With(weight: FontWeight.SemiBold),
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = OsuFont.Style.Caption1.Size,
                        Children = new Drawable[]
                        {
                            contentText = new TruncatingSpriteText
                            {
                                RelativeSizeAxes = Axes.X,
                                Font = OsuFont.Style.Caption1,
                            },
                            contentClickableTextAction = new OsuHoverContainer
                            {
                                AutoSizeAxes = Axes.Both,
                                Child = contentClickableText = new TruncatingSpriteText
                                {
                                    Font = OsuFont.Style.Caption1,
                                },
                            },
                            contentTags = new TagsLine(),
                        },
                    },
                };
            }

            [BackgroundDependencyLoader]
            private void load(OverlayColourProvider colourProvider)
            {
                labelText.Colour = colourProvider.Content1;
                contentText.Colour = colourProvider.Content2;
                contentClickableTextAction.IdleColour = colourProvider.Light2;
            }

            protected override void Update()
            {
                base.Update();
                contentClickableText.MaxWidth = ChildSize.X;
            }

            private void clear()
            {
                contentText.Text = string.Empty;
                contentClickableText.Text = string.Empty;
                contentTags.Tags = Array.Empty<string>();
            }

            private void setText(LocalisableString text)
            {
                clear();

                contentText.Text = text;
            }

            private void setClickableText(LocalisableString text, Action action)
            {
                clear();

                contentClickableText.Text = text;
                contentClickableTextAction.Action = action;
            }

            private void setTags(string[] tags, Action<string> searchAction)
            {
                clear();

                contentTags.PerformSearch = searchAction;
                contentTags.Tags = tags;
            }
        }
    }
}
