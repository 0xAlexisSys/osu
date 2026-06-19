// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using osu.Framework.Graphics;
using osu.Framework.Allocation;
using osu.Game.Database;
using osu.Game.Scoring;

namespace osu.Game.Medals
{
    public partial class MedalEvaluator : Component
    {
        public event Action<Medal>? MedalUnlocked;

        [Resolved]
        private RealmAccess realm { get; set; } = null!;

        public void EvaluateScore(ScoreInfo score)
        {
            Task.Run(() =>
            {
                var unlockedMedals = realm.Run(r => r.All<MedalInfo>().AsEnumerable().Select(m => m.InternalName).ToHashSet());

                var context = buildContext(score);
                evaluateForContext(context, unlockedMedals);
            });
        }

        public void EvaluateAll()
        {
            Task.Run(() =>
            {
                var unlockedMedals = realm.Run(r => r.All<MedalInfo>().AsEnumerable().Select(m => m.InternalName).ToHashSet());
                var cumulativeContext = buildContext(null);

                evaluateForContext(cumulativeContext, unlockedMedals);

                var scores = realm.Run(r => r.All<ScoreInfo>().Where(s => !s.DeletePending).AsEnumerable().Detach());

                foreach (var score in scores)
                {
                    var scoreContext = new MedalEvaluationContext
                    {
                        LatestScore = score,
                        TotalPlayCount = cumulativeContext.TotalPlayCount,
                        TotalPassCount = cumulativeContext.TotalPassCount,
                        BestCombo = cumulativeContext.BestCombo,
                        BestAccuracy = cumulativeContext.BestAccuracy,
                        BestRank = cumulativeContext.BestRank,
                        ByRuleset = cumulativeContext.ByRuleset
                    };

                    evaluateForContext(scoreContext, unlockedMedals);
                }
            });
        }

        private void evaluateForContext(MedalEvaluationContext context, HashSet<string> unlockedMedalNames)
        {
            foreach (var medal in MedalRegistry.AllMedals)
            {
                if (unlockedMedalNames.Contains(medal.InternalName)) continue;
                if (!medal.CanUnlock(context)) continue;

                realm.Write(r => r.Add(new MedalInfo
                {
                    ID = Guid.NewGuid(),
                    InternalName = medal.InternalName,
                    UnlockedAt = DateTimeOffset.UtcNow,
                }));

                unlockedMedalNames.Add(medal.InternalName);

                TriggerMedalUnlocked(medal);
            }
        }

        private MedalEvaluationContext buildContext(ScoreInfo? latestScore)
        {
            return realm.Run(r =>
            {
                var scores = r.All<ScoreInfo>().Where(s => !s.DeletePending).ToList();

                int playCount = scores.Count;
                int passCount = scores.Count(s => s.Rank > ScoreRank.F);

                int bestCombo = scores.Count != 0 ? scores.Max(s => s.MaxCombo) : 0;
                double bestAccuracy = scores.Count != 0 ? scores.Max(s => s.Accuracy) : 0.0;
                ScoreRank bestRank = scores.Count != 0 ? scores.Max(s => s.Rank) : ScoreRank.F;

                var byRuleset = new Dictionary<string, MedalEvaluationContext.RulesetStats>();

                foreach (var group in scores.GroupBy(s => s.Ruleset.ShortName))
                {
                    var rScores = group.ToList();
                    int rPlayCount = rScores.Count;
                    int rPassCount = rScores.Count(s => s.Rank > ScoreRank.F);
                    int rBestCombo = rScores.Count != 0 ? rScores.Max(s => s.MaxCombo) : 0;
                    double rBestAccuracy = rScores.Count != 0 ? rScores.Max(s => s.Accuracy) : 0.0;
                    ScoreRank rBestRank = rScores.Count != 0 ? rScores.Max(s => s.Rank) : ScoreRank.F;

                    byRuleset[group.Key] = new MedalEvaluationContext.RulesetStats(
                        rPlayCount,
                        rPassCount,
                        rBestCombo,
                        rBestAccuracy,
                        rBestRank
                    );
                }

                return new MedalEvaluationContext
                {
                    LatestScore = latestScore,
                    TotalPlayCount = playCount,
                    TotalPassCount = passCount,
                    BestCombo = bestCombo,
                    BestAccuracy = bestAccuracy,
                    BestRank = bestRank,
                    ByRuleset = byRuleset
                };
            });
        }

        public void TriggerMedalUnlocked(Medal medal)
        {
            MedalUnlocked?.Invoke(medal);
            // Schedule(() => MedalUnlocked?.Invoke(medal));
        }
    }
}
