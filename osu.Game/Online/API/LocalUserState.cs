// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Configuration;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Users;

namespace osu.Game.Online.API
{
    public partial class LocalUserState : Component, ILocalUserState
    {
        public IBindable<APIUser> User => localUser;

        private readonly IAPIProvider api;

        private readonly Bindable<APIUser> localUser = new Bindable<APIUser>(createGuestUser());

        private readonly Bindable<UserStatus> configStatus = new Bindable<UserStatus>();
        private readonly Bindable<bool> configSupporter = new Bindable<bool>();

        public LocalUserState(IAPIProvider api, OsuConfigManager config)
        {
            this.api = api;

            config.BindWith(OsuSetting.UserOnlineStatus, configStatus);
            config.BindWith(OsuSetting.WasSupporter, configSupporter);
        }

        #region Logging in / out

        private static APIUser createGuestUser() => new GuestUser();

        /// <summary>
        /// Show a placeholder user if saved credentials are available.
        /// This is useful for storing local scores and showing a placeholder username after starting the game,
        /// until a valid connection has been established.
        /// </summary>
        public void SetPlaceholderLocalUser(string username)
        {
            if (!localUser.IsDefault)
                return;

            localUser.Value = new APIUser
            {
                Username = username,
                IsSupporter = configSupporter.Value,
            };
        }

        public void SetLocalUser(APIMe me)
        {
            localUser.Value = me;
            configSupporter.Value = me.IsSupporter;

            // `last_visit` is assumed to be `null` if and only if the web-side "hide online presence toggle" is enabled
            if (me.LastVisit == null)
                configStatus.Value = UserStatus.Offline;
        }

        public void ClearLocalUser()
        {
            // Reset the status to be broadcast on the next login, in case multiple players share the same system.
            configStatus.Value = UserStatus.Online;

            // Scheduled prior to state change such that the state changed event is invoked with the correct user and their friends present
            Schedule(() =>
            {
                localUser.Value = createGuestUser();
                configSupporter.Value = false;
            });
        }

        #endregion
    }
}
