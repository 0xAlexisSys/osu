// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics.Lines;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Osu.Skinning.Default
{
    public abstract partial class DrawableSliderPath : SmoothPath
    {
        public const float BORDER_PORTION = 0.128f;
        public const float GRADIENT_PORTION = 1 - BORDER_PORTION;

        private const float border_max_size = 8f;
        private const float border_min_size = 0f;

        public Color4 BorderColour
        {
            get;
            set
            {
                if (field == value)
                    return;

                field = value;

                InvalidateTexture();
            }
        } = Color4.White;

        public Color4 AccentColour
        {
            get;
            set
            {
                if (field == value)
                    return;

                field = value;

                InvalidateTexture();
            }
        } = Color4.White;

        public float BorderSize
        {
            get;
            set
            {
                if (field == value)
                    return;

                if (value is < border_min_size or > border_max_size)
                    return;

                field = value;

                InvalidateTexture();
            }
        } = 1;

        protected float CalculatedBorderPortion => BorderSize * BORDER_PORTION;
    }
}
