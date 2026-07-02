// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.IO.Stores;

namespace osu.Game.IO
{
    public sealed class FileSystemResourceStore : IResourceStore<byte[]>
    {
        public byte[] Get(string name)
        {
            if (!File.Exists(name))
                return null;

            try
            {
                return File.ReadAllBytes(name);
            }
            catch
            {
                return null;
            }
        }

        public async Task<byte[]> GetAsync(string name, CancellationToken cancellationToken = default)
        {
            if (!File.Exists(name))
                return null;

            try
            {
                return await File.ReadAllBytesAsync(name, cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                return null;
            }
        }

        public Stream GetStream(string name)
        {
            if (!File.Exists(name))
                return null;

            try
            {
                return File.OpenRead(name);
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<string> GetAvailableResources() => throw new NotSupportedException();

        public void Dispose() { }
    }
}
