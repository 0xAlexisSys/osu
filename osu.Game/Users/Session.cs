// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Net.Http;
using osu.Framework.Graphics;

namespace osu.Game.Users
{
    public partial class Session : Component
    {
        public HttpClient HttpClient { get; } = new HttpClient();
        public User User { get; }

        public Session(string username)
        {
            User = new User
            {
                Username = username,
                ID = User.PERSONAL_USER_ID,
            };
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            // Ensure (as much as we can) that any pending tasks are run.
            Scheduler.Update();
        }
    }
}
