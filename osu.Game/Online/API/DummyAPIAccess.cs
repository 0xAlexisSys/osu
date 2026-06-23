// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Users;

namespace osu.Game.Online.API
{
    public partial class DummyAPIAccess : Component
    {
        public Bindable<User> User { get; }

        public DummyAPIAccess(string username)
        {
            User = new Bindable<User>(new User
            {
                Username = username,
                ID = Users.User.PERSONAL_USER_ID,
            });
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            // Ensure (as much as we can) that any pending tasks are run.
            Scheduler.Update();
        }
    }
}
