// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Threading.Tasks;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Localisation;
using osu.Game.Online.API.Requests;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Notifications.WebSocket;

namespace osu.Game.Online.API
{
    public partial class DummyAPIAccess : Component, IAPIProvider
    {
        public const int DUMMY_USER_ID = 1001;

        public DummyLocalUserState LocalUserState { get; } = new DummyLocalUserState();
        public Bindable<APIUser> LocalUser => LocalUserState.User;

        ILocalUserState IAPIProvider.LocalUserState => LocalUserState;
        IBindable<APIUser> IAPIProvider.LocalUser => LocalUser;

        public DummyNotificationsClient NotificationsClient { get; } = new DummyNotificationsClient();
        INotificationsClient IAPIProvider.NotificationsClient => NotificationsClient;

        public Language Language => Language.en;

        public string AccessToken => "token";

        public Guid SessionIdentifier { get; } = Guid.NewGuid();

        public string ProvidedUsername => LocalUser.Value.Username;

        public EndpointConfiguration Endpoints { get; } = new EndpointConfiguration
        {
            APIUrl = "http://localhost",
            WebsiteUrl = "http://localhost",
        };

        public int APIVersion => int.Parse(DateTime.Now.ToString("yyyyMMdd"));

        public Exception? LastLoginError { get; private set; }

        /// <summary>
        /// Provide handling logic for an arbitrary API request.
        /// Should return true is a request was handled. If null or false return, the request will be failed with a <see cref="NotSupportedException"/>.
        /// </summary>
        public Func<APIRequest, bool>? HandleRequest;

        public SessionVerificationMethod? SessionVerificationMethod { get; set; } = Requests.Responses.SessionVerificationMethod.EmailMessage;

        public virtual void Queue(APIRequest request)
        {
            request.AttachAPI(this);

            Schedule(() =>
            {
                if (HandleRequest?.Invoke(request) != true)
                {
                    // Noisy so let's silently allow these to succeed.
                    if (request is ChatAckRequest ack)
                    {
                        ack.TriggerSuccess(new ChatAckResponse());
                        return;
                    }

                    request.Fail(new InvalidOperationException($@"{nameof(DummyAPIAccess)} cannot process this request."));
                }
            });
        }

        void IAPIProvider.Schedule(Action action) => base.Schedule(action);

        public void Perform(APIRequest request)
        {
            request.AttachAPI(this);
            HandleRequest?.Invoke(request);
        }

        public Task PerformAsync(APIRequest request)
        {
            request.AttachAPI(this);
            HandleRequest?.Invoke(request);
            return Task.CompletedTask;
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            // Ensure (as much as we can) that any pending tasks are run.
            Scheduler.Update();
        }

        public class DummyLocalUserState : ILocalUserState
        {
            public Bindable<APIUser> User { get; } = new Bindable<APIUser>(new APIUser
            {
                Username = @"Local user",
                Id = DUMMY_USER_ID,
            });

            IBindable<APIUser> ILocalUserState.User => User;
        }
    }
}
