// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Medals.Definitions;
using osu.Game.Rulesets.Mods;

namespace osu.Game.Medals
{
    public static class MedalRegistry
    {
        private const string osu_ruleset_short_name = @"osu";
        private const string taiko_ruleset_short_name = @"taiko";
        private const string catch_ruleset_short_name = @"fruits";
        private const string mania_ruleset_short_name = @"mania";

        public static IReadOnlyList<Medal> AllMedals { get; } = new Medal[]
        {
            #region Mod Introduction

            new ModIntroAcronymMedal(
                @"PassWithSD",
                @"Finality",
                @"High stakes, no regrets.",
                @"complete a beatmap with the Sudden Death mod enabled",
                @"SD"),
            new ModIntroAcronymMedal(
                @"PassWithPF",
                @"Perfectionist",
                @"Accept nothing but the best.",
                @"complete a beatmap with the Perfect mod enabled",
                @"PF"),
            new ModIntroAcronymMedal(
                @"PassWithHR",
                @"Rock Around The Clock",
                @"You can't stop the rock.",
                @"complete a beatmap with the Hard Rock mod enabled",
                @"HR"),
            new ModIntroAcronymMedal(
                @"PassWithDT",
                @"Time And A Half",
                @"Having a right ol' time. One and a half of them, almost.",
                @"complete a beatmap with the Double Time mod enabled",
                @"DT"),
            new ModIntroAcronymMedal(
                @"PassWithNC",
                @"Sweet Rave Party",
                @"Founded in the fine tradition of changing things that were just fine as they were.",
                @"complete a beatmap with the Nightcore mod enabled",
                @"NC"),
            new ModIntroAcronymMedal(
                @"PassWithHD",
                @"Blindsight",
                @"I can see just perfectly.",
                @"complete a beatmap with the Hidden mod enabled",
                @"HD"),
            new ModIntroAcronymMedal(
                @"PassWithFL",
                @"Are You Afraid Of The Dark?",
                @"Harder than it looks, probably because it's hard to look.",
                @"complete a beatmap with the Flashlight mod enabled",
                @"FL"),
            new ModIntroAcronymMedal(
                @"PassWithEZ",
                @"Dial It Right Back",
                @"Sometimes you just want to take it easy.",
                @"complete a beatmap with the Easy mod enabled",
                @"EZ"),
            new ModIntroAcronymMedal(
                @"PassWithNF",
                @"Risk Averse",
                @"Safety nets are fun!",
                @"complete a beatmap with the No Fail mod enabled",
                @"NF"),
            new ModIntroAcronymMedal(
                @"PassWithHT",
                @"Slowboat",
                @"You got there. Eventually.",
                @"complete a beatmap with the Half Time mod enabled",
                @"HT"),
            new ModIntroAcronymMedal(
                @"PassWithHD",
                @"Burned Out",
                @"One cannot always spin to win.",
                @"complete a beatmap with the Spun Out mod enabled",
                @"SO"),
            new ModIntroTypeMedal(
                @"PassWithConversion",
                @"Gear Shift",
                @"Tailor your experience to your perfect fit.",
                @"complete a beatmap with any Conversion mod enabled",
                ModType.Conversion),
            new ModIntroTypeMedal(
                @"PassWithFun",
                @"Game Night",
                @"Mum said it's my turn with the beatmap!",
                @"complete a beatmap with any Fun mod enabled",
                ModType.Fun),

            #endregion Mod Introduction

            #region Skill & Dedication

            #region All rulesets

            // TODO: [alexis] Implement play count tracking.
            new PlayCountMedal( // [alexis] Change requirement from 5000 to 500.
                @"Play500Times",
                @"500 Plays",
                @"There's a lot more where that came from.",
                500),
            new PlayCountMedal(@"Play1500Times", // [alexis] Change requirement from 15000 to 1500, use three dots for ellipses.
                @"1,500 Plays",
                @"Must... click... circles...",
                1500),
            new PlayCountMedal(@"Play2500Times", // [alexis] Change requirement from 25000 to 2500.
                @"2,500 Plays",
                @"There's no going back.",
                2500),
            new PlayCountMedal(@"Play5000Times", // [alexis] Change requirement from 50000 to 5000.
                @"5,000 Plays",
                @"You're here forever.",
                5000),

            new ComboClearMedal(
                @"GetCombo500",
                @"500 Combo",
                @"500 big ones! You're moving up in the world!",
                @"complete a beatmap with your highest combo being 500 or higher",
                500),
            new ComboClearMedal(
                @"GetCombo750",
                @"750 Combo",
                @"750 notes back to back? Woah!",
                @"complete a beatmap with your highest combo being 750 or higher",
                750),
            new ComboClearMedal(
                @"GetCombo1000",
                @"1,000 Combo",
                @"A thousand reasons why you rock at this time.",
                @"complete a beatmap with your highest combo being 1,000 or higher",
                1000),
            new ComboClearMedal(
                @"GetCombo2000",
                @"2,000 Combo",
                @"Nothing can stop you now.",
                @"complete a beatmap with your highest combo being 2,000 or higher",
                2000),

            #endregion All rulesets

            #region osu!standard-only

            new StarRatingPassMedal(
                @"OsuPass1Star",
                @"Rising Star",
                @"Can't go forward without the first steps.",
                @"in osu!standard, complete a 1* beatmap",
                (double.MinValue, 2.0d),
                osu_ruleset_short_name),
            new StarRatingPassMedal(
                @"OsuPass2Star",
                @"Constellation Prize",
                @"Definitely not a consolation prize. Now things start getting hard!",
                @"in osu!standard, complete a 2* beatmap",
                (2.0d, 3.0d),
                osu_ruleset_short_name),
            new StarRatingPassMedal(
                @"OsuPass3Star",
                @"Building Confidence",
                @"Oh, you've SO got this.",
                @"in osu!standard, complete a 3* beatmap",
                (3.0d, 4.0d),
                osu_ruleset_short_name),
            new StarRatingPassMedal(
                @"OsuPass4Star",
                @"Insanity Approaches",
                @"You're not twitching, you're just ready.",
                @"in osu!standard, complete a 4* beatmap",
                (4.0d, 5.0d),
                osu_ruleset_short_name),
            new StarRatingPassMedal(
                @"OsuPass5Star",
                @"These Clarion Skies",
                @"Everything seems so clear now.",
                @"in osu!standard, complete a 5* beatmap",
                (5.0d, 6.0d),
                osu_ruleset_short_name),
            new StarRatingPassMedal(
                @"OsuPass6Star",
                @"Above and Beyond",
                @"A cut above the rest.",
                @"in osu!standard, complete a 6* beatmap",
                (6.0d, 7.0d),
                osu_ruleset_short_name),
            new StarRatingPassMedal(
                @"OsuPass7Star",
                @"Supremacy",
                @"All marvel before your prowess.",
                @"in osu!standard, complete a 7* beatmap",
                (7.0d, 8.0d),
                osu_ruleset_short_name),
            new StarRatingPassMedal(
                @"OsuPass8Star",
                @"Absolution",
                @"My god, you're full of stars!",
                @"in osu!standard, complete a 8* beatmap",
                (8.0d, 9.0d),
                osu_ruleset_short_name),
            new StarRatingPassMedal(
                @"OsuPass9Star",
                @"Event Horizon",
                @"No force dares to pull you under.",
                @"in osu!standard, complete a 9* beatmap",
                (9.0d, 10.0d),
                osu_ruleset_short_name),
            new StarRatingPassMedal(
                @"OsuPass10Star",
                @"Phantasm",
                @"Fevered is your passion, extraordinary is your skill.",
                @"in osu!standard, complete a 10* beatmap",
                (10.0d, double.MaxValue),
                osu_ruleset_short_name),

            new StarRatingFullComboMedal(
                @"OsuFC1Star",
                @"Totality",
                @"All the notes. Every single one.",
                @"in osu!standard, complete a 1* beatmap with no misses",
                (double.MinValue, 2.0d),
                osu_ruleset_short_name),
            new StarRatingFullComboMedal(
                @"OsuFC2Star",
                @"Business As Usual",
                @"Two to go, please.",
                @"in osu!standard, complete a 2* beatmap with no misses",
                (2.0d, 3.0d),
                osu_ruleset_short_name),
            new StarRatingFullComboMedal(
                @"OsuFC3Star",
                @"Building Steam",
                @"Hey, this isn't so bad.",
                @"in osu!standard, complete a 3* beatmap with no misses",
                (3.0d, 4.0d),
                osu_ruleset_short_name),
            new StarRatingFullComboMedal(
                @"OsuFC4Star",
                @"Moving Forward",
                @"Bet you feel good about that.",
                @"in osu!standard, complete a 4* beatmap with no misses",
                (4.0d, 5.0d),
                osu_ruleset_short_name),
            new StarRatingFullComboMedal(
                @"OsuFC5Star",
                @"Paradigm Shift",
                @"Surprisingly difficult.",
                @"in osu!standard, complete a 5* beatmap with no misses",
                (5.0d, 6.0d),
                osu_ruleset_short_name),
            new StarRatingFullComboMedal(
                @"OsuFC6Star",
                @"Anguish Quelled",
                @"Don't choke.",
                @"in osu!standard, complete a 6* beatmap with no misses",
                (6.0d, 7.0d),
                osu_ruleset_short_name),
            new StarRatingFullComboMedal(
                @"OsuFC7Star",
                @"Never Give Up",
                @"Excellence is its own reward.",
                @"in osu!standard, complete a 7* beatmap with no misses",
                (7.0d, 8.0d),
                osu_ruleset_short_name),
            new StarRatingFullComboMedal(
                @"OsuFC8Star",
                @"Aberration",
                @"They said it couldn't be done. They were wrong.",
                @"in osu!standard, complete a 8* beatmap with no misses",
                (8.0d, 9.0d),
                osu_ruleset_short_name),
            new StarRatingFullComboMedal(
                @"OsuFC9Star",
                @"Chosen",
                @"Reign among the Prometheans, where you belong.",
                @"in osu!standard, complete a 9* beatmap with no misses",
                (9.0d, 10.0d),
                osu_ruleset_short_name),
            new StarRatingFullComboMedal(
                @"OsuFC10Star",
                @"Unfathomable",
                @"You have no equal.",
                @"in osu!standard, complete a 10* beatmap with no misses",
                (10.0d, double.MaxValue),
                osu_ruleset_short_name),

            // TODO: [alexis] Implement "Cyclone" medal for reaching 477 RPM on a spinner.

            #endregion osu!standard-only

            #region osu!taiko-only

            // TODO: [alexis] Implement hit milestone medals (e.g., 300,000 Drum Hits).

            new StarRatingPassMedal(
                @"TaikoPass1Star",
                @"My First Don",
                @"Marching to the beat of your own drum. Literally.",
                @"in osu!taiko, complete a 1* beatmap",
                (double.MinValue, 2.0d),
                taiko_ruleset_short_name),
            new StarRatingPassMedal(
                @"TaikoPass2Star",
                @"Katsu Katsu Katsu",
                @"Hora! Ikuzo!",
                @"in osu!taiko, complete a 2* beatmap",
                (2.0d, 3.0d),
                taiko_ruleset_short_name),
            new StarRatingPassMedal(
                @"TaikoPass3Star",
                @"Not Even Trying",
                @"Muzukashii? Not even.",
                @"in osu!taiko, complete a 3* beatmap",
                (3.0d, 4.0d),
                taiko_ruleset_short_name),
            new StarRatingPassMedal(
                @"TaikoPass4Star",
                @"Face Your Demons",
                @"The first trials are now behind you, but are you a match for the Oni?",
                @"in osu!taiko, complete a 4* beatmap",
                (4.0d, 5.0d),
                taiko_ruleset_short_name),
            new StarRatingPassMedal(
                @"TaikoPass5Star",
                @"The Demon Within",
                @"No rest for the wicked.",
                @"in osu!taiko, complete a 5* beatmap",
                (5.0d, 6.0d),
                taiko_ruleset_short_name),
            new StarRatingPassMedal(
                @"TaikoPass6Star",
                @"Drumbreaker",
                @"Too strong.",
                @"in osu!taiko, complete a 6* beatmap",
                (6.0d, 7.0d),
                taiko_ruleset_short_name),
            new StarRatingPassMedal(
                @"TaikoPass7Star",
                @"The Godfather",
                @"You are the Don of Dons.",
                @"in osu!taiko, complete a 7* beatmap",
                (7.0d, 8.0d),
                taiko_ruleset_short_name),
            new StarRatingPassMedal(
                @"TaikoPass8Star",
                @"Rhythm Incarnate",
                @"Feel the beat. Become the beat.",
                @"in osu!taiko, complete a 8* beatmap",
                (8.0d, double.MaxValue),
                taiko_ruleset_short_name),

            new StarRatingFullComboMedal( // [alexis] Use three dots for ellipses.
                @"TaikoFC1Star",
                @"Keeping Time",
                @"Don, then katsu. Don, then katsu...",
                @"in osu!taiko, complete a 1* beatmap with no misses",
                (double.MinValue, 2.0d),
                taiko_ruleset_short_name),
            new StarRatingFullComboMedal(
                @"TaikoFC2Star",
                @"To Your Own Beat",
                @"Straight and steady.",
                @"in osu!taiko, complete a 2* beatmap with no misses",
                (2.0d, 3.0d),
                taiko_ruleset_short_name),
            new StarRatingFullComboMedal(
                @"TaikoFC3Star",
                @"Big Drums",
                @"Bigger scores to match.",
                @"in osu!taiko, complete a 3* beatmap with no misses",
                (3.0d, 4.0d),
                taiko_ruleset_short_name),
            new StarRatingFullComboMedal(
                @"TaikoFC4Star",
                @"Adversity Overcome",
                @"Difficult? Not for you.",
                @"in osu!taiko, complete a 4* beatmap with no misses",
                (4.0d, 5.0d),
                taiko_ruleset_short_name),
            new StarRatingFullComboMedal(
                @"TaikoFC5Star",
                @"Demonslayer",
                @"An Oni felled forevermore.",
                @"in osu!taiko, complete a 5* beatmap with no misses",
                (5.0d, 6.0d),
                taiko_ruleset_short_name),
            new StarRatingFullComboMedal(
                @"TaikoFC6Star",
                @"Rhythm's True Call",
                @"Heralding true skill.",
                @"in osu!taiko, complete a 6* beatmap with no misses",
                (6.0d, 7.0d),
                taiko_ruleset_short_name),
            new StarRatingFullComboMedal(
                @"TaikoFC7Star",
                @"Time Everlasting",
                @"Not a single beat escapes you.",
                @"in osu!taiko, complete a 7* beatmap with no misses",
                (7.0d, 8.0d),
                taiko_ruleset_short_name),
            new StarRatingFullComboMedal(
                @"TaikoFC8Star",
                @"The Drummer's Throne",
                @"Percussive brilliance befitting royalty alone.",
                @"in osu!taiko, complete a 8* beatmap with no misses",
                (8.0d, double.MaxValue),
                taiko_ruleset_short_name),

            #endregion osu!taiko-only

            #region osu!catch-only

            // TODO: [alexis] Implement hit milestone medals (e.g., Catch 200,000 Fruits).

            new StarRatingPassMedal(
                @"CatchPass1Star",
                @"A Slice Of Life",
                @"Hey, this fruit catching business isn't bad.",
                @"in osu!catch, complete a 1* beatmap",
                (double.MinValue, 2.0d),
                catch_ruleset_short_name),
            new StarRatingPassMedal(
                @"CatchPass2Star",
                @"Dashing Ever Forward",
                @"Fast is how you do it.",
                @"in osu!catch, complete a 2* beatmap",
                (2.0d, 3.0d),
                catch_ruleset_short_name),
            new StarRatingPassMedal(
                @"CatchPass3Star",
                @"Zesty Disposition",
                @"No scurvy for you, not with that much fruit.",
                @"in osu!catch, complete a 3* beatmap",
                (3.0d, 4.0d),
                catch_ruleset_short_name),
            new StarRatingPassMedal(
                @"CatchPass4Star",
                @"Hyperdash ON!",
                @"Time and distance is no obstacle to you.",
                @"in osu!catch, complete a 4* beatmap",
                (4.0d, 5.0d),
                catch_ruleset_short_name),
            new StarRatingPassMedal( // [alexis] Change name from "It's Raining Fruit" to "It's Raining Fruits".
                @"CatchPass5Star",
                @"It's Raining Fruits",
                @"And you can catch them all.",
                @"in osu!catch, complete a 5* beatmap",
                (5.0d, 6.0d),
                catch_ruleset_short_name),
            new StarRatingPassMedal(
                @"CatchPass6Star",
                @"Fruit Ninja",
                @"Legendary techniques.",
                @"in osu!catch, complete a 6* beatmap",
                (6.0d, 7.0d),
                catch_ruleset_short_name),
            new StarRatingPassMedal(
                @"CatchPass7Star",
                @"Dreamcatcher",
                @"No fruit, only dreams now.",
                @"in osu!catch, complete a 7* beatmap",
                (7.0d, 8.0d),
                catch_ruleset_short_name),
            new StarRatingPassMedal( // [alexis] Change name from "Lord of the Catch" to "Lord Of The Catch".
                @"CatchPass8Star",
                @"Lord Of The Catch",
                @"Your kingdom kneels before you.",
                @"in osu!catch, complete a 8* beatmap",
                (8.0d, double.MaxValue),
                catch_ruleset_short_name),

            new StarRatingFullComboMedal(
                @"CatchFC1Star",
                @"Sweet And Sour",
                @"Apples and oranges, literally.",
                @"in osu!catch, complete a 1* beatmap with no misses",
                (double.MinValue, 2.0d),
                catch_ruleset_short_name),
            new StarRatingFullComboMedal(
                @"CatchFC2Star",
                @"Reaching The Core",
                @"The seeds of future success.",
                @"in osu!catch, complete a 2* beatmap with no misses",
                (2.0d, 3.0d),
                catch_ruleset_short_name),
            new StarRatingFullComboMedal(
                @"CatchFC3Star",
                @"Clean Patter",
                @"Clean only of failure. It is completely full, otherwise.",
                @"in osu!catch, complete a 3* beatmap with no misses",
                (3.0d, 4.0d),
                catch_ruleset_short_name),
            new StarRatingFullComboMedal(
                @"CatchFC4Star",
                @"Between The Rain",
                @"No umbrella needed.",
                @"in osu!catch, complete a 4* beatmap with no misses",
                (4.0d, 5.0d),
                catch_ruleset_short_name),
            new StarRatingFullComboMedal(
                @"CatchFC5Star",
                @"Addicted",
                @"That was an overdose?",
                @"in osu!catch, complete a 5* beatmap with no misses",
                (5.0d, 6.0d),
                catch_ruleset_short_name),
            new StarRatingFullComboMedal(
                @"CatchFC6Star",
                @"Quickening",
                @"A dash above normal limits.",
                @"in osu!catch, complete a 6* beatmap with no misses",
                (6.0d, 7.0d),
                catch_ruleset_short_name),
            new StarRatingFullComboMedal(
                @"CatchFC7Star",
                @"Supersonic",
                @"Faster than is reasonably necessary.",
                @"in osu!catch, complete a 7* beatmap with no misses",
                (7.0d, 8.0d),
                catch_ruleset_short_name),
            new StarRatingFullComboMedal(
                @"CatchFC8Star",
                @"Dashing Scarlet",
                @"Speed beyond mortal reckoning.",
                @"in osu!catch, complete a 8* beatmap with no misses",
                (8.0d, double.MaxValue),
                catch_ruleset_short_name),

            #endregion osu!catch-only

            #region osu!mania-only

            // TODO: [alexis] Implement hit milestone medals (e.g., 400,000 Keys).

            new StarRatingPassMedal(
                @"ManiaPass1Star",
                @"First Steps",
                @"It isn't 9-to-5, but 1-to-9. Keys, that is.",
                @"in osu!mania, complete a 1* beatmap",
                (double.MinValue, 2.0d),
                mania_ruleset_short_name),
            new StarRatingPassMedal(
                @"ManiaPass2Star",
                @"No Normal Player",
                @"Not anymore, at least.",
                @"in osu!mania, complete a 2* beatmap",
                (2.0d, 3.0d),
                mania_ruleset_short_name),
            new StarRatingPassMedal(
                @"ManiaPass3Star",
                @"Impulse Drive",
                @"Not quite hyperspeed, but getting close.",
                @"in osu!mania, complete a 3* beatmap",
                (3.0d, 4.0d),
                mania_ruleset_short_name),
            new StarRatingPassMedal(
                @"ManiaPass4Star",
                @"Hyperspeed",
                @"Woah.",
                @"in osu!mania, complete a 4* beatmap",
                (4.0d, 5.0d),
                mania_ruleset_short_name),
            new StarRatingPassMedal(
                @"ManiaPass5Star",
                @"Ever Onwards",
                @"Another challenge is just around the corner.",
                @"in osu!mania, complete a 5* beatmap",
                (5.0d, 6.0d),
                mania_ruleset_short_name),
            new StarRatingPassMedal(
                @"ManiaPass6Star",
                @"Another Surpassed",
                @"Is there no limit to your skills?",
                @"in osu!mania, complete a 6* beatmap",
                (6.0d, 7.0d),
                mania_ruleset_short_name),
            new StarRatingPassMedal(
                @"ManiaPass7Star",
                @"Extra Credit",
                @"See me after class.",
                @"in osu!mania, complete a 7* beatmap",
                (7.0d, 8.0d),
                mania_ruleset_short_name),
            new StarRatingPassMedal(
                @"ManiaPass8Star",
                @"Maniac",
                @"There's just no stopping you.",
                @"in osu!mania, complete a 8* beatmap",
                (8.0d, double.MaxValue),
                mania_ruleset_short_name),

            new StarRatingFullComboMedal(
                @"ManiaFC1Star",
                @"Keystruck",
                @"The beginning of a new story.",
                @"in osu!mania, complete a 1* beatmap with no misses",
                (double.MinValue, 2.0d),
                mania_ruleset_short_name),
            new StarRatingFullComboMedal(
                @"ManiaFC2Star",
                @"Keying In",
                @"Finding your groove.",
                @"in osu!mania, complete a 2* beatmap with no misses",
                (2.0d, 3.0d),
                mania_ruleset_short_name),
            new StarRatingFullComboMedal( // [alexis] Remove asterisks surrounding "feel".
                @"ManiaFC3Star",
                @"Hyperflow",
                @"You can feel the rhythm.",
                @"in osu!mania, complete a 3* beatmap with no misses",
                (3.0d, 4.0d),
                mania_ruleset_short_name),
            new StarRatingFullComboMedal(
                @"ManiaFC4Star",
                @"Breakthrough",
                @"Many skills mastered, rolled into one.",
                @"in osu!mania, complete a 4* beatmap with no misses",
                (4.0d, 5.0d),
                mania_ruleset_short_name),
            new StarRatingFullComboMedal(
                @"ManiaFC5Star",
                @"Everything Extra",
                @"Giving your all is giving everything you have.",
                @"in osu!mania, complete a 5* beatmap with no misses",
                (5.0d, 6.0d),
                mania_ruleset_short_name),
            new StarRatingFullComboMedal(
                @"ManiaFC6Star",
                @"Level Breaker",
                @"Finesse beyond reason.",
                @"in osu!mania, complete a 6* beatmap with no misses",
                (6.0d, 7.0d),
                mania_ruleset_short_name),
            new StarRatingFullComboMedal(
                @"ManiaFC7Star",
                @"Step Up",
                @"A precipice rarely seen.",
                @"in osu!mania, complete a 7* beatmap with no misses",
                (7.0d, 8.0d),
                mania_ruleset_short_name),
            new StarRatingFullComboMedal(
                @"ManiaFC8Star",
                @"Behind The Veil",
                @"Supernatural!",
                @"in osu!mania, complete a 8* beatmap with no misses",
                (8.0d, double.MaxValue),
                mania_ruleset_short_name),

            #endregion osu!mania-only

            #endregion Skill & Dedication
        };
    }
}
