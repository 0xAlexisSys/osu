// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Localisation;
using osu.Game.Database;
using osu.Game.Localisation;
using osu.Game.Medals;

namespace osu.Game.Overlays.Settings.Sections.Maintenance
{
    public partial class MedalSettings : SettingsSubsection
    {
        protected override LocalisableString Header => CommonStrings.Medals;

        private SettingsButtonV2 resetMedalsButton = null!;
        private SettingsButtonV2 checkAllScoresForEligibleMedalsButton = null!;

        [Resolved]
        private MedalEvaluator medalEvaluator { get; set; } = null!;

        [BackgroundDependencyLoader]
        private void load(RealmAccess realm, IDialogOverlay? dialogOverlay)
        {
            Add(resetMedalsButton = new DangerousSettingsButtonV2
            {
                Text = MaintenanceSettingsStrings.ResetAllMedals,
                Action = () =>
                {
                    dialogOverlay?.Push(new MassDeleteConfirmationDialog(() =>
                    {
                        resetMedalsButton.Enabled.Value = false;
                        Task.Run(() => realm.Write(r => r.RemoveAll<MedalInfo>())).ContinueWith(_ => resetMedalsButton.Enabled.Value = true);
                    }, DeleteConfirmationContentStrings.Medals));
                }
            });

            Add(checkAllScoresForEligibleMedalsButton = new SettingsButtonV2
            {
                Text = MaintenanceSettingsStrings.CheckAllScoresForEligibleMedals,
                Action = () =>
                {
                    checkAllScoresForEligibleMedalsButton.Enabled.Value = false;
                    Task.Run(medalEvaluator.EvaluateAll).ContinueWith(_ => checkAllScoresForEligibleMedalsButton.Enabled.Value = true);
                },
            });
        }
    }
}
