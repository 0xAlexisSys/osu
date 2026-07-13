// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using NUnit.Framework;
using NUnit.Framework.Legacy;
using osu.Game.Configuration;

namespace osu.Game.Tests.NonVisual
{
    [TestFixture]
    public class SessionStaticsTest
    {
        private SessionStatics sessionStatics;

        [Test]
        public void TestSessionStaticsReset()
        {
            sessionStatics = new SessionStatics();

            sessionStatics.SetValue(Static.MutedAudioNotificationShownOnce, true);
            sessionStatics.SetValue(Static.LowBatteryNotificationShownOnce, true);
            sessionStatics.SetValue(Static.LastHoverSoundPlaybackTime, (double?)1d);

            ClassicAssert.False(sessionStatics.GetBindable<bool>(Static.MutedAudioNotificationShownOnce).IsDefault);
            ClassicAssert.False(sessionStatics.GetBindable<bool>(Static.LowBatteryNotificationShownOnce).IsDefault);
            ClassicAssert.False(sessionStatics.GetBindable<double?>(Static.LastHoverSoundPlaybackTime).IsDefault);

            sessionStatics.ResetAfterInactivity();

            ClassicAssert.True(sessionStatics.GetBindable<bool>(Static.MutedAudioNotificationShownOnce).IsDefault);
            ClassicAssert.True(sessionStatics.GetBindable<bool>(Static.LowBatteryNotificationShownOnce).IsDefault);
            // some statics should not reset despite inactivity.
            ClassicAssert.False(sessionStatics.GetBindable<double?>(Static.LastHoverSoundPlaybackTime).IsDefault);
        }
    }
}
