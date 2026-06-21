// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using MessagePack;

namespace osu.Game.Rulesets.Replays
{
    [MessagePackObject]
    public class ReplayFrame
    {
        /// <summary>
        /// The time at which this <see cref="ReplayFrame"/> takes place.
        /// </summary>
        [Key(0)]
        public double Time;

        public ReplayFrame()
        {
        }

        public ReplayFrame(double time)
        {
            Time = time;
        }

        /// <summary>
        /// Whether this frame is equivalent to <paramref name="other"/> with respect to replay recording.
        /// </summary>
        public virtual bool IsEquivalentTo(ReplayFrame other) => Time == other.Time;
    }
}
