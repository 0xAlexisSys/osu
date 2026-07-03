// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using System.Collections.Generic;
using osu.Framework.Extensions.ListExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Lists;
using osu.Framework.Localisation;
using osu.Framework.Platform;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays;
using osuTK;

namespace osu.Game.Graphics.Containers
{
    public sealed partial class LinkFlowContainer : OsuTextFlowContainer
    {
        public LinkFlowContainer(Action<SpriteText>? defaultCreationParameters = null)
            : base(defaultCreationParameters)
        {
        }

        [Resolved]
        private GameHost host { get; set; } = null!;

        public void AddLink(LocalisableString text, string? url = null, Action? action = null, LocalisableString? tooltipText = null, Action<SpriteText>? creationParameters = null)
        {
            AddPart(new TextLink(CreateChunkFor(text, true, CreateSpriteText, creationParameters), tooltipText ?? string.Empty, () =>
            {
                action?.Invoke();
                if (Uri.IsWellFormedUriString(url, UriKind.Absolute)) host.OpenUrlExternally(url);
            }));
        }

        private partial class TextLink : TextPart
        {
            private readonly ITextPart innerPart;
            private readonly LocalisableString tooltipText;
            private readonly Action action;

            public TextLink(ITextPart innerPart, LocalisableString tooltipText, Action action)
            {
                this.innerPart = innerPart;
                this.tooltipText = tooltipText;
                this.action = action;
            }

            protected override IEnumerable<Drawable> CreateDrawablesFor(TextFlowContainer textFlowContainer)
            {
                var linkFlowContainer = (LinkFlowContainer)textFlowContainer;

                innerPart.RecreateDrawablesFor(linkFlowContainer);
                var drawables = innerPart.Drawables.ToList();

                drawables.Add(new DrawableLinkCompiler(innerPart).With(c =>
                {
                    c.TooltipText = tooltipText;
                    c.Action = action;
                }));

                return drawables;
            }

            /// <summary>
            /// An invisible drawable that brings multiple <see cref="Drawable"/> pieces together to form a consumable clickable link.
            /// </summary>
            private partial class DrawableLinkCompiler : OsuHoverContainer
            {
                /// <summary>
                /// Each word part of a chat link (split for word-wrap support).
                /// </summary>
                private readonly SlimReadOnlyListWrapper<Drawable> parts;

                [Resolved]
                private OverlayColourProvider? overlayColourProvider { get; set; }

                public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => parts.Any(part => part.ReceivePositionalInputAt(screenSpacePos));

                protected override HoverSounds CreateHoverSounds(HoverSampleSet sampleSet) => new LinkHoverSounds(sampleSet, parts);

                public DrawableLinkCompiler(ITextPart part)
                    : this(part.Drawables.OfType<SpriteText>())
                {
                }

                public DrawableLinkCompiler(IEnumerable<Drawable> parts)
                {
                    this.parts = parts.ToList().AsSlimReadOnly();
                }

                [BackgroundDependencyLoader]
                private void load(OsuColour colours)
                {
                    IdleColour ??= overlayColourProvider?.Light2 ?? colours.Blue;
                }

                protected override IEnumerable<Drawable> EffectTargets => parts;

                private partial class LinkHoverSounds : HoverClickSounds
                {
                    private readonly SlimReadOnlyListWrapper<Drawable> parts;

                    public LinkHoverSounds(HoverSampleSet sampleSet, SlimReadOnlyListWrapper<Drawable> parts)
                        : base(sampleSet)
                    {
                        this.parts = parts;
                    }

                    public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => parts.Any(part => part.ReceivePositionalInputAt(screenSpacePos));
                }
            }
        }
    }
}
