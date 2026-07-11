// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Bindables;
using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Scoring;
using osu.Game.Scoring;
using osu.Game.Storyboards;

namespace osu.Game.Screens.Play
{
    /// <summary>
    /// The state of an active gameplay session, generally constructed and exposed by <see cref="Player"/>.
    /// </summary>
    public class GameplayState
    {
        /// <summary>
        /// The final post-convert post-mod-application beatmap.
        /// </summary>
        public readonly IBeatmap Beatmap;

        /// <summary>
        /// The ruleset used in gameplay.
        /// </summary>
        public readonly Ruleset Ruleset;

        /// <summary>
        /// The mods applied to the gameplay.
        /// </summary>
        public readonly Mod[] Mods;

        /// <summary>
        /// The gameplay score.
        /// </summary>
        public readonly Score Score;

        public readonly ScoreProcessor ScoreProcessor;
        public readonly HealthProcessor HealthProcessor;

        /// <summary>
        /// The storyboard associated with the beatmap.
        /// </summary>
        public readonly Storyboard Storyboard;

        /// <summary>
        /// Whether gameplay completed without the user failing.
        /// </summary>
        public bool HasPassed { get; set; }

        /// <summary>
        /// Whether the user failed during gameplay. This is only set when the gameplay session has completed due to the fail.
        /// </summary>
        public bool HasFailed { get; set; }

        /// <summary>
        /// Whether the user quit gameplay without having either passed or failed.
        /// </summary>
        public bool HasQuit { get; set; }

        public bool HasCompleted => HasPassed || HasFailed || HasQuit;

        /// <summary>
        /// A bindable tracking the last judgement result applied to any hit object.
        /// </summary>
        public readonly Bindable<JudgementResult> LastJudgementResult = new Bindable<JudgementResult>();

        /// <summary>
        /// A dictionary tracking hit results per hit object combo.
        /// </summary>
        public readonly FrozenDictionary<int, HitResult[]> ComboHitResults;

        /// <summary>
        /// The local user's playing state (whether actively playing, paused, or not playing due to watching a replay or similar).
        /// </summary>
        public Bindable<LocalUserPlayingState> PlayingState { get; } = new Bindable<LocalUserPlayingState>();

        public GameplayState(
            IBeatmap beatmap,
            Ruleset ruleset,
            Mod[]? mods = null,
            Score? score = null,
            ScoreProcessor? scoreProcessor = null,
            HealthProcessor? healthProcessor = null,
            Storyboard? storyboard = null,
            Bindable<LocalUserPlayingState>? localUserPlayingState = null)
        {
            Beatmap = beatmap;
            Ruleset = ruleset;
            Score = score ?? new Score
            {
                ScoreInfo =
                {
                    BeatmapInfo = beatmap.BeatmapInfo,
                    Ruleset = ruleset.RulesetInfo,
                },
            };
            Mods = mods ?? [];
            ScoreProcessor = scoreProcessor ?? ruleset.CreateScoreProcessor();
            HealthProcessor = healthProcessor ?? ruleset.CreateHealthProcessor(beatmap.HitObjects[0].StartTime);
            Storyboard = storyboard ?? new Storyboard();

            Dictionary<int, HitResult[]> comboHitResults = [];
            int comboHitObjectCount = 0;

            foreach (HitObject hitObject in Beatmap.HitObjects)
            {
                if (hitObject is IHasComboInformation comboInfo)
                {
                    ++comboHitObjectCount;

                    // [alexis] Strangely, LastInCombo is not true for the last hit object, so it is necessary
                    //          to check if the current hit object is last.
                    if (comboInfo.LastInCombo || hitObject == Beatmap.HitObjects[^1])
                    {
                        comboHitResults.Add(comboInfo.ComboIndex, new HitResult[comboHitObjectCount]);
                        comboHitObjectCount = 0;
                    }
                }
            }

            ComboHitResults = comboHitResults.ToFrozenDictionary();

            if (localUserPlayingState is not null)
                PlayingState.BindTo(localUserPlayingState);
        }

        /// <summary>
        /// Applies the score change of a <see cref="JudgementResult"/> to this <see cref="GameplayState"/>.
        /// </summary>
        /// <param name="result">The <see cref="JudgementResult"/> to apply.</param>
        public void ApplyResult(JudgementResult result)
        {
            LastJudgementResult.Value = result;

            if (result.HitObject is IHasComboInformation comboInfo && result.Type.IsBasic())
                ComboHitResults[comboInfo.ComboIndex][comboInfo.IndexInCurrentCombo] = result.Type;
        }

        /// <summary>
        /// Reverts the score change of a <see cref="JudgementResult"/> that was applied to this <see cref="GameplayState"/>.
        /// </summary>
        /// <param name="result">The <see cref="JudgementResult"/> to revert.</param>
        public void RevertResult(JudgementResult result)
        {
            LastJudgementResult.Value = result;

            if (result.HitObject is IHasComboInformation comboInfo && result.Type.IsBasic())
                ComboHitResults[comboInfo.ComboIndex][comboInfo.IndexInCurrentCombo] = HitResult.None;
        }

        public SpecialJudgement GetSpecialJudgement()
        {
            if (ScoreProcessor.Ruleset.ShortName != @"osu")
                throw new InvalidOperationException($@"{nameof(GetSpecialJudgement)} should not be called from ruleset '{ScoreProcessor.Ruleset.ShortName}'");

            if (LastJudgementResult.Value?.HitObject is not IHasComboInformation comboInfo
                || !LastJudgementResult.Value.Type.IsBasic()
                || comboInfo.IndexInCurrentCombo != ComboHitResults[comboInfo.ComboIndex].Length - 1
                || ComboHitResults[comboInfo.ComboIndex].Any(hr => hr.IsMiss() || hr is HitResult.Meh))
                return SpecialJudgement.None;

            return ComboHitResults[comboInfo.ComboIndex].Any(hr => hr is HitResult.Ok) ? SpecialJudgement.Katu : SpecialJudgement.Geki;
        }
    }
}
