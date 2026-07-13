// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.IO;
using System.Linq;
using NUnit.Framework;
using osu.Framework;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Platform;
using osu.Framework.Testing;
using osu.Game.IPC;
using osu.Game.Overlays.Notifications;
using osu.Game.Tests.Resources;
using osu.Game.Users;

namespace osu.Game.Tests.Visual.Navigation
{
    [TestFixture]
    [Ignore("This test cannot be run headless, as it requires the game host running the nested game to have IPC bound.")]
    public partial class TestSceneInterProcessCommunication : OsuGameTestScene
    {
        private HeadlessGameHost ipcSenderHost = null!;

        private ArchiveImportIPCChannel archiveImportIPCSender = null!;

        private const int requested_beatmap_set_id = 1;

        protected override TestOsuGame CreateTestGame() => new IpcGame(LocalStorage, Session);

        [Resolved]
        private GameHost gameHost { get; set; } = null!;

        public override void SetUpSteps()
        {
            base.SetUpSteps();
            AddStep("create IPC sender channels", () =>
            {
                ipcSenderHost = new HeadlessGameHost(gameHost.Name, new HostOptions { IPCPipeName = OsuGame.IPC_PIPE_NAME });
                archiveImportIPCSender = new ArchiveImportIPCChannel(ipcSenderHost);
            });
        }

        [Test]
        public void TestArchiveImportLinkIPCChannel()
        {
            string? beatmapFilepath = null;

            AddStep("import beatmap via IPC", () => archiveImportIPCSender.ImportAsync(beatmapFilepath = TestResources.GetQuickTestBeatmapForImport()).WaitSafely());
            AddUntilStep("import complete notification was presented", () => Game.Notifications.ChildrenOfType<ProgressCompletionNotification>().Count(), () => Is.EqualTo(1));
            AddAssert("original file deleted", () => File.Exists(beatmapFilepath), () => Is.False);
        }

        public override void TearDownSteps()
        {
            AddStep("dispose IPC senders", () =>
            {
                archiveImportIPCSender.Dispose();
                ipcSenderHost.Dispose();
            });
            base.TearDownSteps();
        }

        private partial class IpcGame : TestOsuGame
        {
            private ArchiveImportIPCChannel? archiveImportIPCChannel;

            public IpcGame(Storage storage, Session session, string[]? args = null)
                : base(storage, session, args)
            {
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();
                archiveImportIPCChannel = new ArchiveImportIPCChannel(Host, this);
            }

            protected override void Dispose(bool isDisposing)
            {
                base.Dispose(isDisposing);
                archiveImportIPCChannel?.Dispose();
            }
        }
    }
}
