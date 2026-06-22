// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Online.API.Requests.Responses;

namespace osu.Game.Online.API
{
    public partial class LocalUserState : Component, ILocalUserState
    {
        public IBindable<APIUser> User => localUser;

        private readonly IAPIProvider api;

        private readonly Bindable<APIUser> localUser = new Bindable<APIUser>(new APIUser
        {
            Username = @"Guest",
            Id = 0,
        });

        public LocalUserState(IAPIProvider api)
        {
            this.api = api;
        }
    }
}
