// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Configuration;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Localisation;
using osu.Game.Utils;

namespace osu.Game.Overlays.Settings.Sections.General
{
    public partial class UserSettings : SettingsSubsection
    {
        private static readonly IEnumerable<string> keywords = new[] { @"user" };

        protected override LocalisableString Header => GeneralSettingsStrings.UserHeader;

        private FormFileSelector userAvatarFileSelector = null!;

        [BackgroundDependencyLoader]
        private void load(OsuConfigManager config)
        {
            Children = new Drawable[]
            {
                new SettingsItemV2(new FormTextBox
                {
                    Caption = GeneralSettingsStrings.Name,
                    Current = config.GetBindable<string>(OsuSetting.Username),
                    HintText = SettingsStrings.RestartRequiredSettingTooltip,
                })
                {
                    Keywords = keywords,
                },
                new SettingsItemV2(userAvatarFileSelector = new FormFileSelector(true, setting: OsuSetting.UserAvatar, handledExtensions: SupportedExtensions.IMAGE_EXTENSIONS)
                {
                    Caption = GeneralSettingsStrings.Avatar,
                    PlaceholderText = CommonStrings.Default,
                    HintText = SettingsStrings.RestartRequiredSettingTooltip,
                })
                {
                    Keywords = keywords,
                },
                new SettingsItemV2(new FormEnumDropdown<DiscordRichPresenceMode>
                {
                    Caption = GeneralSettingsStrings.DiscordRichPresence,
                    Current = config.GetBindable<DiscordRichPresenceMode>(OsuSetting.DiscordRichPresence),
                })
                {
                    Keywords = keywords,
                },
                new SettingsButtonV2
                {
                    Text = GeneralSettingsStrings.UseDefaultAvatar,
                    Keywords = keywords,
                    Action = () => userAvatarFileSelector.Current.Value = null,
                },
            };
        }
    }
}
