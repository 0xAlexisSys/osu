// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Game.Medals
{
    public partial record Medal
    {
        /// <summary>
        /// The internal name of the medal, which also determines the icon.
        /// </summary>
        public string Slug { get; init; } = string.Empty;

        /// <summary>
        /// The title of the medal.
        /// </summary>
        public string Title { get; init; } = string.Empty;

        /// <summary>
        /// The description of the medal.
        /// </summary>
        public string Description { get; init; } = string.Empty;

        /// <summary>
        /// The text to show when the medal's title is hovered over.
        /// </summary>
        public string? TooltipText { get; init; }

        /// <summary>
        /// The category of the medal.
        /// </summary>
        public required string Category { get; init; }

        /// <summary>
        /// Whether this is an official osu! medal or not.
        /// </summary>
        public required bool IsCustom { get; init; }

        public string Icon => $@"Medals/{Slug}";
    }
}
