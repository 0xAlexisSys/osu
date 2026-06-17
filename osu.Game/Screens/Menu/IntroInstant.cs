// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using JetBrains.Annotations;

namespace osu.Game.Screens.Menu
{
    public partial class IntroInstant : IntroScreen
    {
        private const float transition_fade_time = 500.0f;
        private const double transition_delay = 1000.0d;

        protected override string BeatmapHash => "64e00d7022195959bfa3109d09c2e2276c8f12f486b91fcf6175583e973b48f2";
        protected override string BeatmapFile => "welcome.osz";

        public IntroInstant([CanBeNull] Func<MainMenu> createNextScreen = null)
            : base(createNextScreen)
        {
        }

        protected override void LogoArriving(OsuLogo logo, bool resuming)
        {
            base.LogoArriving(logo, resuming);

            if (!resuming)
            {
                PrepareMenuLoad();

                // [alexis] Main menu loading is delayed to avoid the background suddenly popping into existence.
                Scheduler.AddDelayed(() =>
                {
                    FadeInBackground(transition_fade_time);
                    LoadMenu();
                }, transition_delay);
            }
        }
    }
}
