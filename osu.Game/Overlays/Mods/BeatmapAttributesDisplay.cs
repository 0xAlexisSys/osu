// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.LocalisationExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Drawables;
using osu.Game.Configuration;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Localisation;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Mods;
using osu.Game.Utils;
using osuTK.Graphics;

namespace osu.Game.Overlays.Mods
{
    /// <summary>
    /// On the mod select overlay, this provides a local updating view of BPM, star rating and other
    /// difficulty attributes so the user can have a better insight into what mods are changing.
    /// </summary>
    public partial class BeatmapAttributesDisplay : ModFooterInformationDisplay
    {
        private StarRatingDisplay starRatingDisplay = null!;
        private BPMDisplay bpmDisplay = null!;

        public Bindable<IBeatmapInfo?> BeatmapInfo { get; } = new Bindable<IBeatmapInfo?>();

        public Bindable<IReadOnlyList<Mod>> Mods { get; } = new Bindable<IReadOnlyList<Mod>>();

        public Bindable<double> ModMultiplier = new BindableDouble(1.0d);

        public BindableBool Collapsed { get; } = new BindableBool(true);

        private ModSettingChangeTracker? modSettingChangeTracker;

        [Resolved]
        private BeatmapDifficultyCache difficultyCache { get; set; } = null!;

        [Resolved]
        private OsuGameBase game { get; set; } = null!;

        [Resolved]
        private OsuColour colours { get; set; } = null!;

        private Bindable<RulesetInfo> gameRuleset = null!;
        private CancellationTokenSource? cancellationSource;
        private IBindable<StarDifficulty> starDifficulty = null!;
        private ScoreMultiplierDisplay scoreMultiplierDisplay = null!;

        private const float transition_duration = 250;

        [BackgroundDependencyLoader]
        private void load()
        {
            LeftContent.AddRange(new Drawable[]
            {
                starRatingDisplay = new StarRatingDisplay(default, animated: true)
                {
                    Origin = Anchor.CentreLeft,
                    Anchor = Anchor.CentreLeft,
                    Shear = -OsuGame.SHEAR,
                },
                bpmDisplay = new BPMDisplay
                {
                    Origin = Anchor.CentreLeft,
                    Anchor = Anchor.CentreLeft,
                    Shear = -OsuGame.SHEAR,
                    AutoSizeAxes = Axes.Y,
                    Width = 75,
                }
            });

            RightContent.Alpha = 0;
            RightContent.Add(scoreMultiplierDisplay = new ScoreMultiplierDisplay(ModMultiplier) { Shear = -OsuGame.SHEAR });
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Mods.BindValueChanged(_ =>
            {
                modSettingChangeTracker?.Dispose();
                modSettingChangeTracker = new ModSettingChangeTracker(Mods.Value);
                modSettingChangeTracker.SettingChanged += _ => updateValues();
                updateValues();
            }, true);

            ModMultiplier.BindValueChanged(e =>
            {
                if (e.NewValue > ModMultiplier.Default)
                {
                    scoreMultiplierDisplay.FadeColour(colours.ForModType(ModType.DifficultyIncrease));
                }
                else if (e.NewValue < ModMultiplier.Default)
                {
                    scoreMultiplierDisplay.FadeColour(colours.ForModType(ModType.DifficultyReduction));
                }
                else
                {
                    scoreMultiplierDisplay.FadeColour(Colour4.White);
                }
            }, true);

            Collapsed.BindValueChanged(_ =>
            {
                // Only start autosize animations on first collapse toggle. This avoids an ugly initial presentation.
                startAnimating();
                updateCollapsedState();
            });

            gameRuleset = game.Ruleset.GetBoundCopy();
            gameRuleset.BindValueChanged(_ => updateValues());

            BeatmapInfo.BindValueChanged(_ =>
            {
                updateStarDifficultyBindable();
                updateValues();
            }, true);

            updateCollapsedState();
        }

        private void updateStarDifficultyBindable()
        {
            cancellationSource?.Cancel();

            if (BeatmapInfo.Value == null)
                return;

            starDifficulty = difficultyCache.GetBindableDifficulty(BeatmapInfo.Value, (cancellationSource = new CancellationTokenSource()).Token);
            starDifficulty.BindValueChanged(s =>
            {
                starRatingDisplay.Current.Value = s.NewValue;

                if (!starRatingDisplay.IsPresent)
                    starRatingDisplay.FinishTransforms(true);
            });
        }

        protected override bool OnHover(HoverEvent e)
        {
            startAnimating();
            updateCollapsedState();
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            updateCollapsedState();
            base.OnHoverLost(e);
        }

        protected override bool OnMouseDown(MouseDownEvent e) => true;

        protected override bool OnClick(ClickEvent e) => true;

        private void startAnimating()
        {
            LeftContent.AutoSizeEasing = Content.AutoSizeEasing = Easing.OutQuint;
            LeftContent.AutoSizeDuration = Content.AutoSizeDuration = transition_duration;
        }

        private void updateValues() => Scheduler.AddOnce(() =>
        {
            if (BeatmapInfo.Value == null)
                return;

            double rate = ModUtils.CalculateRateWithMods(Mods.Value);

            bpmDisplay.Current.Value = FormatUtils.RoundBPM(BeatmapInfo.Value.BPM, rate);

            Ruleset ruleset = gameRuleset.Value.CreateInstance();
            var displayAttributes = ruleset.GetBeatmapAttributesForDisplay(BeatmapInfo.Value, Mods.Value).ToList();

            // if there are not enough attribute displays, make more
            for (int i = RightContent.Count; i < displayAttributes.Count + 1; i++)
                RightContent.Add(new VerticalAttributeDisplay { Shear = -OsuGame.SHEAR });

            // populate all attribute displays that need to be visible...
            for (int i = 0; i < displayAttributes.Count; i++)
            {
                var attribute = displayAttributes[i];
                var display = (VerticalAttributeDisplay)RightContent[i + 1];
                display.SetAttribute(attribute);
            }

            // and hide any extra ones
            for (int i = displayAttributes.Count + 1; i < RightContent.Count; i++)
                ((VerticalAttributeDisplay)RightContent[i - 1]).SetAttribute(null);
        });

        private void updateCollapsedState()
        {
            RightContent.FadeTo(Collapsed.Value && !IsHovered ? 0 : 1, transition_duration, Easing.OutQuint);
        }

        private partial class BPMDisplay : RollingCounter<int>
        {
            protected override double RollingDuration => 250;

            protected override LocalisableString FormatCount(int count) => count.ToLocalisableString("0 BPM");

            protected override OsuSpriteText CreateSpriteText() => new OsuSpriteText
            {
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Font = OsuFont.Default.With(size: 20, weight: FontWeight.SemiBold),
                UseFullGlyphHeight = false,
            };
        }

        private partial class ScoreMultiplierDisplay : Container, IHasTooltip
        {
            private const float sm_transition_duration = 200.0f;
            private const Easing sm_transition_easing = Easing.OutQuint;

            public LocalisableString TooltipText => ModSelectOverlayStrings.ScoreMultiplier;

            private readonly OsuSpriteText text;
            private readonly EffectCounter counter;

            public ScoreMultiplierDisplay(Bindable<double> modMultiplier)
            {
                AutoSizeAxes = Axes.X;
                RelativeSizeAxes = Axes.Y;

                Origin = Anchor.CentreLeft;
                Anchor = Anchor.CentreLeft;

                InternalChild = new FillFlowContainer
                {
                    Origin = Anchor.CentreLeft,
                    Anchor = Anchor.CentreLeft,
                    RelativeSizeAxes = Axes.Y,
                    Width = 42,
                    Direction = FillDirection.Vertical,
                    Children = new Drawable[]
                    {
                        text = new SpriteTextWithTooltip
                        {
                            Origin = Anchor.Centre,
                            Anchor = Anchor.Centre,
                            Text = @"SM",
                            Font = OsuFont.Default.With(size: 20, weight: FontWeight.Bold)
                        },
                        counter = new EffectCounter
                        {
                            Origin = Anchor.Centre,
                            Anchor = Anchor.Centre,
                            Current = { BindTarget = modMultiplier },
                        }
                    }
                };

                counter.SetCountWithoutRolling(modMultiplier.Value);
            }

            public void FadeColour(Color4 colour)
            {
                text.FadeColour(colour, sm_transition_duration, sm_transition_easing);
                counter.FadeColour(colour, sm_transition_duration, sm_transition_easing);
            }

            private partial class EffectCounter : RollingCounter<double>
            {
                protected override double RollingDuration => 250;

                protected override LocalisableString FormatCount(double count) => ModUtils.FormatScoreMultiplier(count);

                protected override OsuSpriteText CreateSpriteText() => new OsuSpriteText
                {
                    Font = OsuFont.Default.With(size: 18, weight: FontWeight.SemiBold)
                };
            }
        }
    }
}
