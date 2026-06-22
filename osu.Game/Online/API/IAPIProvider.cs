// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Threading.Tasks;
using osu.Framework.Bindables;
using osu.Game.Localisation;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Notifications.WebSocket;

namespace osu.Game.Online.API
{
    public interface IAPIProvider
    {
        /// <summary>
        /// The local user.
        /// </summary>
        IBindable<APIUser> LocalUser { get; }

        /// <summary>
        /// The local user's current state.
        /// Contains auxiliary information such as the user's friends, blocks, and favourites,
        /// as well as methods to manage those in a way that keeps this state consistent throughout the game.
        /// </summary>
        ILocalUserState LocalUserState { get; }

        /// <summary>
        /// The language supplied by this provider to API requests.
        /// </summary>
        Language Language { get; }

        /// <summary>
        /// Retrieve the OAuth access token.
        /// </summary>
        string AccessToken { get; }

        /// <summary>
        /// Holds configuration for online endpoints.
        /// </summary>
        EndpointConfiguration Endpoints { get; }

        /// <summary>
        /// The version of the API.
        /// </summary>
        int APIVersion { get; }

        /// <summary>
        /// Queue a new request.
        /// </summary>
        /// <param name="request">The request to perform.</param>
        void Queue(APIRequest request);

        /// <summary>
        /// Perform a request immediately, bypassing any API state checks.
        /// </summary>
        /// <remarks>
        /// Can be used to run requests as a guest user.
        /// </remarks>
        /// <param name="request">The request to perform.</param>
        void Perform(APIRequest request);

        /// <summary>
        /// Perform a request immediately, bypassing any API state checks.
        /// </summary>
        /// <remarks>
        /// Can be used to run requests as a guest user.
        /// </remarks>
        /// <param name="request">The request to perform.</param>
        Task PerformAsync(APIRequest request);

        /// <summary>
        /// Schedule a callback to run on the update thread.
        /// </summary>
        internal void Schedule(Action action);

        /// <summary>
        /// Accesses the <see cref="INotificationsClient"/> used to receive asynchronous notifications from web.
        /// </summary>
        INotificationsClient NotificationsClient { get; }
    }
}
