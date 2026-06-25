// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Configuration;

namespace osu.Game.Rulesets.Mods
{
    public interface IMod : IEquatable<IMod>
    {
        /// <summary>
        /// The shortened name of this mod.
        /// </summary>
        string Acronym { get; }

        /// <summary>
        /// The name of this mod.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Short important information to display on the mod icon. For example, a rate adjust mod's rate
        /// or similarly important setting.
        /// Use <see cref="string.Empty"/> if the icon should not display any additional info.
        /// </summary>
        string ExtendedIconInformation { get; }

        /// <summary>
        /// The user readable description of this mod.
        /// </summary>
        LocalisableString Description { get; }

        /// <summary>
        /// The type of this mod.
        /// </summary>
        ModType Type { get; }

        /// <summary>
        /// The icon of this mod.
        /// </summary>
        IconUsage? Icon { get; }

        /// <summary>
        /// Create a fresh <see cref="Mod"/> instance based on this mod.
        /// </summary>
        Mod CreateInstance() => (Mod)Activator.CreateInstance(GetType())!;

        /// <summary>
        /// Whether any user adjustable setting attached to this mod has a non-default value.
        /// </summary>
        /// <remarks>
        /// This returns the instantaneous state of this mod. It may change over time.
        /// For tracking changes on a dynamic display, make sure to setup a <see cref="ModSettingChangeTracker"/>.
        /// </remarks>
        bool HasNonDefaultSettings
        {
            get
            {
                bool hasAdjustments = false;

                foreach (var (_, property) in this.GetSettingsSourceProperties())
                {
                    var bindable = (IBindable)property.GetValue(this)!;

                    if (!bindable.IsDefault)
                    {
                        hasAdjustments = true;
                        break;
                    }
                }

                return hasAdjustments;
            }
        }
    }
}
