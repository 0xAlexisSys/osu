// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Extensions.ObjectExtensions;
using osu.Framework.Graphics.UserInterface;
using osu.Game.Extensions;
using osu.Game.Rulesets;

namespace osu.Game.Overlays.Profile.Header.Components
{
    public partial class ProfileRulesetSelector : OverlayRulesetSelector
    {
        public readonly Bindable<UserProfileData?> User = new Bindable<UserProfileData?>();

        protected override void LoadComplete()
        {
            base.LoadComplete();

            User.BindValueChanged(user => updateState(user.NewValue), true);
        }

        private void updateState(UserProfileData? user)
        {
            Current.Value = Items.SingleOrDefault(ruleset => user?.Ruleset.MatchesOnlineID(ruleset) == true);
            SetDefaultRuleset(Rulesets.GetRuleset(user?.User.PlayMode ?? @"osu").AsNonNull());
        }

        public void SetDefaultRuleset(RulesetInfo ruleset)
        {
            foreach (var tabItem in TabContainer)
                ((ProfileRulesetTabItem)tabItem).IsDefault = ((ProfileRulesetTabItem)tabItem).Value.Equals(ruleset);
        }

        protected override TabItem<RulesetInfo> CreateTabItem(RulesetInfo value) => new ProfileRulesetTabItem(value);
    }
}
