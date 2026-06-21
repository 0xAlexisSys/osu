// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Localisation;
using osu.Game.Localisation;

namespace osu.Game.Configuration
{
    public enum DiscordRichPresenceMode
    {
        [LocalisableDescription(typeof(GeneralSettingsStrings), nameof(GeneralSettingsStrings.DiscordPresenceOff))]
        Off,

        [LocalisableDescription(typeof(GeneralSettingsStrings), nameof(GeneralSettingsStrings.HideIdentifiableInformation))]
        Limited,

        [LocalisableDescription(typeof(GeneralSettingsStrings), nameof(GeneralSettingsStrings.DiscordPresenceFull))]
        Full
    }
}
