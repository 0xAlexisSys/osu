// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK.Graphics;

namespace osu.Game.Screens.Play.HUD
{
    public partial class DefaultKeyCounterDisplay : KeyCounterDisplay
    {
        private const double key_fade_time = 80;

        protected override FillFlowContainer<KeyCounter> KeyFlow { get; }

        public DefaultKeyCounterDisplay()
        {
            Child = KeyFlow = new FillFlowContainer<KeyCounter>
            {
                Direction = FillDirection.Horizontal,
                AutoSizeAxes = Axes.Both,
            };
        }

        protected override KeyCounter CreateCounter(InputTrigger trigger) => new DefaultKeyCounter(trigger)
        {
            FadeTime = key_fade_time,
            KeyDownTextColor = KeyDownTextColor,
            KeyUpTextColor = KeyUpTextColor,
        };

        public Color4 KeyDownTextColor
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    foreach (var child in KeyFlow.Cast<DefaultKeyCounter>())
                        child.KeyDownTextColor = value;
                }
            }
        } = Color4.DarkGray;

        public Color4 KeyUpTextColor
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    foreach (var child in KeyFlow.Cast<DefaultKeyCounter>())
                        child.KeyUpTextColor = value;
                }
            }
        } = Color4.White;
    }
}
