// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Localisation;

namespace osu.Game.Localisation
{
    public static class NotificationsStrings
    {
        private const string prefix = @"osu.Game.Resources.Localisation.Notifications";

        /// <summary>
        /// "notifications"
        /// </summary>
        public static LocalisableString HeaderTitle => new TranslatableString(getKey(@"header_title"), @"notifications");

        /// <summary>
        /// "waiting for &#39;ya"
        /// </summary>
        public static LocalisableString HeaderDescription => new TranslatableString(getKey(@"header_description"), @"waiting for 'ya");

        /// <summary>
        /// "Running Tasks"
        /// </summary>
        public static LocalisableString RunningTasks => new TranslatableString(getKey(@"running_tasks"), @"Running Tasks");

        /// <summary>
        /// "Clear All"
        /// </summary>
        public static LocalisableString ClearAll => new TranslatableString(getKey(@"clear_all"), @"Clear All");

        /// <summary>
        /// "Your battery level is low! Charge your device to prevent interruptions during gameplay."
        /// </summary>
        public static LocalisableString BatteryLow => new TranslatableString(getKey(@"battery_low"), @"Your battery level is low! Charge your device to prevent interruptions during gameplay.");

        /// <summary>
        /// "Your game volume is too low to hear anything! Click here to restore it."
        /// </summary>
        public static LocalisableString GameVolumeTooLow => new TranslatableString(getKey(@"game_volume_too_low"), @"Your game volume is too low to hear anything! Click here to restore it.");

        /// <summary>
        /// "The current ruleset doesn&#39;t have an autoplay mod available!"
        /// </summary>
        public static LocalisableString NoAutoplayMod => new TranslatableString(getKey(@"no_autoplay_mod"), @"The current ruleset doesn't have an autoplay mod available!");

        /// <summary>
        /// "osu! doesn&#39;t seem to be able to play audio correctly.
        ///
        /// Please try changing your audio device to a working setting."
        /// </summary>
        public static LocalisableString AudioPlaybackIssue => new TranslatableString(getKey(@"audio_playback_issue"), @"osu! doesn't seem to be able to play audio correctly.

Please try changing your audio device to a working setting.");

        /// <summary>
        /// "The score overlay is currently disabled. You can toggle this by pressing {0}."
        /// </summary>
        public static LocalisableString ScoreOverlayDisabled(LocalisableString arg0) => new TranslatableString(getKey(@"score_overlay_disabled"), @"The score overlay is currently disabled. You can toggle this by pressing {0}.", arg0);

        /// <summary>
        /// "Subsequent messages have been logged. Click to view log files."
        /// </summary>
        public static LocalisableString SubsequentMessagesLogged => new TranslatableString(getKey(@"subsequent_messages_logged"), @"Subsequent messages have been logged. Click to view log files.");

        /// <summary>
        /// "Disabling tablet support due to error: &quot;{0}&quot;"
        /// </summary>
        public static LocalisableString TabletSupportDisabledDueToError(string message) => new TranslatableString(getKey(@"tablet_support_disabled_due_to_error"), @"Disabling tablet support due to error: ""{0}""", message);

        /// <summary>
        /// "Encountered tablet warning, your tablet may not function correctly. Click here for a list of all tablets supported."
        /// </summary>
        public static LocalisableString EncounteredTabletWarning => new TranslatableString(getKey(@"encountered_tablet_warning"), @"Encountered tablet warning, your tablet may not function correctly. Click here for a list of all tablets supported.");

        /// <summary>
        /// "Collections import is initialising..."
        /// </summary>
        public static LocalisableString CollectionsImportInitialising => new TranslatableString(getKey(@"collections_import_initialising"), @"Collections import is initialising...");

        /// <summary>
        /// "Reading collections..."
        /// </summary>
        public static LocalisableString ReadingCollections => new TranslatableString(getKey(@"reading_collections"), @"Reading collections...");

        /// <summary>
        /// "Imported {0} collections"
        /// </summary>
        public static LocalisableString CollectionsImportProgress(int count) => new TranslatableString(getKey(@"collections_import_progress"), @"Imported {0} collections", count);

        /// <summary>
        /// "Imported {0} of {1} collections"
        /// </summary>
        public static LocalisableString CollectionsImportProgressTotal(int count, int totalCount) => new TranslatableString(getKey(@"collections_import_progress_total"), @"Imported {0} of {1} collections", count, totalCount);

        /// <summary>
        /// "This error has been automatically reported to the dev team."
        /// </summary>
        public static LocalisableString ErrorAutomaticallyReported => new TranslatableString(getKey(@"error_automatically_reported"), @"This error has been automatically reported to the dev team.");

        /// <summary>
        /// "An action was interrupted due to a dialog being displayed."
        /// </summary>
        public static LocalisableString ActionInterruptedByDialog => new TranslatableString(getKey(@"action_interrupted_by_dialog"), @"An action was interrupted due to a dialog being displayed.");

        /// <summary>
        /// "Exporting {0}..."
        /// </summary>
        public static LocalisableString FileExportOngoing(string filename) => new TranslatableString(getKey(@"file_export_ongoing"), @"Exporting {0}...", filename);

        /// <summary>
        /// "Exported {0}! Click to view."
        /// </summary>
        public static LocalisableString FileExportFinished(string filename) => new TranslatableString(getKey(@"file_export_finished"), @"Exported {0}! Click to view.", filename);

        /// <summary>
        /// "Exporting logs..."
        /// </summary>
        public static LocalisableString LogsExportOngoing => new TranslatableString(getKey(@"logs_export_ongoing"), @"Exporting logs...");

        /// <summary>
        /// "Exported logs! Click to view."
        /// </summary>
        public static LocalisableString LogsExportFinished => new TranslatableString(getKey(@"logs_export_finished"), @"Exported logs! Click to view.");

        /// <summary>
        /// "Running osu! as {0} does not improve performance, may break integrations and poses a security risk. Please run the game as a normal user."
        /// </summary>
        public static LocalisableString ElevatedPrivileges(LocalisableString user) => new TranslatableString(getKey(@"elevated_privileges"), @"Running osu! as {0} does not improve performance, may break integrations and poses a security risk. Please run the game as a normal user.", user);

        /// <summary>
        /// "On macOS, installing osu! to a directory other than /Applications or {0}/Applications can cause issues with updating the game. Please move your game installation to one of these locations."
        /// </summary>
        public static LocalisableString MacOSAppLocation(LocalisableString userProfile) => new TranslatableString(getKey(@"macos_app_location"), @"On macOS, installing osu! to a directory other than /Applications or {0}/Applications can cause issues with updating the game. Please move your game installation to one of these locations.", userProfile);

        /// <summary>
        /// "Screenshot saved! Click to view.
        /// {0}"
        /// </summary>
        public static LocalisableString ScreenshotSaved(string filename) => new TranslatableString(getKey(@"screenshot_saved"), @"Screenshot saved! Click to view.
{0}", filename);

        private static string getKey(string key) => $@"{prefix}:{key}";
    }
}
