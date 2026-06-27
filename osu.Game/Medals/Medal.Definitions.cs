// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Immutable;

namespace osu.Game.Medals
{
    public partial record Medal
    {
        private const string category_hush_hush = @"Hush-Hush";
        private const string category_hush_hush_expert = @"Hush-Hush (Expert)";
        private const string category_mod_introduction = @"Mod Introduction";
        private const string category_skill_and_dedication = @"Skill & Dedication";

        public static readonly ImmutableArray<Medal> DEFINITIONS =
        [
            new Medal
            {
                Slug = @"secret-all-consolation_prize",
                Title = @"Consolation Prize",
                Description = @"Well, it could be worse.",
                Category = category_hush_hush,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"secret-all-stumbler",
                Title = @"Stumbler",
                Description = @"No regrets.",
                Category = category_hush_hush,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"secret-all-prepared",
                Title = @"Prepared",
                Description = @"Do it for real next time.",
                Category = category_hush_hush,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"secret-all-the_sum_of_all_fears",
                Title = @"The Sum Of All Fears",
                Description = @"Unfortunate.",
                Category = category_hush_hush,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"secret-osu-equilibrium",
                Title = @"Equilibrium",
                Description = @"Balance in all things.",
                Category = category_hush_hush,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"secret-all-value_your_identity",
                Title = @"Value Your Identity",
                Description = @"As perfect as you are.",
                Category = category_hush_hush,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"secret-all-by_the_skin_of_the_teeth",
                Title = @"By The Skin Of The Teeth",
                Description = @"You're that accurate.",
                Category = category_hush_hush,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"secret-all-meticulous_mayhem",
                Title = @"Meticulous Mayhem",
                Description = @"How did we get here?",
                Category = category_hush_hush,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"secret-ui-hamster_wheel",
                Title = @"Hamster Wheel",
                Description = @"Feeling dizzy yet?",
                Category = category_hush_hush,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"secret-ui-courier_catapult",
                Title = @"Courier Catapult",
                Description = @"YEET!",
                Category = category_hush_hush,
                IsCustom = false,
            },

            new Medal
            {
                Slug = @"secret-all-perseverance",
                Title = @"Perseverance",
                Description = @"Endure.",
                Category = category_hush_hush_expert,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"secret-all-feel_the_burn",
                Title = @"Feel The Burn",
                Description = @"It isn't all about how fast you manage.",
                Category = category_hush_hush_expert,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"secret-all-up_for_the_challenge",
                Title = @"Up For The Challenge",
                Description = @"Turn it up to eleven.",
                Category = category_hush_hush_expert,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"secret-osu-overconfident",
                Title = @"Overconfident",
                Description = @"Try again later, maybe?",
                Category = category_hush_hush_expert,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"secret-osu-spooked",
                Title = @"Spooked",
                Description = @"Something moved. It wasn't your cursor!",
                Category = category_hush_hush_expert,
                IsCustom = false,
            },

            new Medal
            {
                Slug = @"mods-all-sd",
                Title = @"Finality",
                Description = @"High stakes, no regrets.",
                TooltipText = @"complete a beatmap with the Sudden Death mod enabled",
                Category = category_mod_introduction,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"mods-all-pf",
                Title = @"Perfectionist",
                Description = @"Accept nothing but the best.",
                TooltipText = @"complete a beatmap with the Perfect mod enabled",
                Category = category_mod_introduction,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"mods-all-hr",
                Title = @"Rock Around The Clock",
                Description = @"You can't stop the rock.",
                TooltipText = @"complete a beatmap with the Hard Rock mod enabled",
                Category = category_mod_introduction,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"mods-all-dt",
                Title = @"Time And A Half",
                Description = @"Having a right ol' time. One and a half of them, almost.",
                TooltipText = @"complete a beatmap with the Double Time mod enabled",
                Category = category_mod_introduction,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"mods-all-nc",
                Title = @"Sweet Rave Party",
                Description = @"Founded in the fine tradition of changing things that were just fine as they were.",
                TooltipText = @"complete a beatmap with the Nightcore mod enabled",
                Category = category_mod_introduction,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"mods-all-hd",
                Title = @"Blindsight",
                Description = @"I can see just perfectly.",
                TooltipText = @"complete a beatmap with the Hidden mod enabled",
                Category = category_mod_introduction,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"mods-all-fl",
                Title = @"Are You Afraid Of The Dark?",
                Description = @"Harder than it looks, probably because it's hard to look.",
                TooltipText = @"complete a beatmap with the Flashlight mod enabled",
                Category = category_mod_introduction,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"mods-all-ez",
                Title = @"Dial It Right Back",
                Description = @"Sometimes you just want to take it easy.",
                TooltipText = @"complete a beatmap with the Easy mod enabled",
                Category = category_mod_introduction,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"mods-all-nf",
                Title = @"Risk Averse",
                Description = @"Safety nets are fun!",
                TooltipText = @"complete a beatmap with the No Fail mod enabled",
                Category = category_mod_introduction,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"mods-all-ht",
                Title = @"Slowboat",
                Description = @"You got there. Eventually",
                TooltipText = @"complete a beatmap with the Half Time mod enabled",
                Category = category_mod_introduction,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"mods-all-so",
                Title = @"Burned Out",
                Description = @"One cannot always spin to win.",
                TooltipText = @"complete a beatmap with the Spun Out mod enabled",
                Category = category_mod_introduction,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"mods-all-any_conversion",
                Title = @"Gear Shift",
                Description = @"Tailor your experience to your perfect fit.",
                TooltipText = @"complete a beatmap with any Conversion mod enabled",
                Category = category_mod_introduction,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"mods-all-any_fun",
                Title = @"Game Night",
                Description = @"Mum said it's my turn with the beatmap!",
                TooltipText = @"complete a beatmap with any Fun mod enabled",
                Category = category_mod_introduction,
                IsCustom = false,
            },

            new Medal
            {
                Slug = @"skill-all-reach_combo_500",
                Title = @"500 Combo",
                Description = @"500 big ones! You're moving up in the world!",
                TooltipText = @"complete a beatmap with your highest combo being 500 or higher",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-all-reach_combo_750",
                Title = @"750 Combo",
                Description = @"750 notes back to back? Woah!",
                TooltipText = @"complete a beatmap with your highest combo being 750 or higher",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-all-reach_combo_1000",
                Title = @"1,000 Combo",
                Description = @"A thousand reasons why you rock at this game.",
                TooltipText = @"complete a beatmap with your highest combo being 1000 or higher",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-all-reach_combo_2000",
                Title = @"2,000 Combo",
                Description = @"Nothing can stop you now.",
                TooltipText = @"complete a beatmap with your highest combo being 2000 or higher",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },

            #region osu!standard-only

            new Medal
            {
                Slug = @"skill-osu-pass_1_star",
                Title = @"Rising Star",
                Description = @"Can't go forward without the first steps.",
                TooltipText = @"in osu!standard, complete a 1* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-osu-pass_2_star",
                Title = @"Constellation Prize",
                Description = @"Definitely not a consolation prize. Now things start getting hard!",
                TooltipText = @"in osu!standard, complete a 2* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-osu-pass_3_star",
                Title = @"Building Confidence",
                Description = @"Oh, you've SO got this.",
                TooltipText = @"in osu!standard, complete a 3* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-osu-pass_4_star",
                Title = @"Insanity Approaches",
                Description = @"You're not twitching, you're just ready.",
                TooltipText = @"in osu!standard, complete a 4* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-osu-pass_5_star",
                Title = @"These Clarion Skies",
                Description = @"Everything seems so clear now.",
                TooltipText = @"in osu!standard, complete a 5* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-osu-pass_6_star",
                Title = @"Above and Beyond",
                Description = @"A cut above the rest.",
                TooltipText = @"in osu!standard, complete a 6* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-osu-pass_7_star",
                Title = @"Supremacy",
                Description = @"All marvel before your prowess.",
                TooltipText = @"in osu!standard, complete a 7* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-osu-pass_8_star",
                Title = @"Absolution",
                Description = @"My god, you're full of stars!",
                TooltipText = @"in osu!standard, complete a 8* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-osu-pass_9_star",
                Title = @"Event Horizon",
                Description = @"No force dares to pull you under.",
                TooltipText = @"in osu!standard, complete a 9* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-osu-pass_10_star",
                Title = @"Phantasm",
                Description = @"Fevered is your passion, extraordinary is your skill.",
                TooltipText = @"in osu!standard, complete a 10* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },

            new Medal
            {
                Slug = @"skill-osu-fc_1_star",
                Title = @"Totality",
                Description = @"All the notes. Every single one.",
                TooltipText = @"in osu!standard, complete a 1* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-osu-fc_2_star",
                Title = @"Business As Usual",
                Description = @"Two to go, please.",
                TooltipText = @"in osu!standard, complete a 2* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-osu-fc_3_star",
                Title = @"Building Steam",
                Description = @"Hey, this isn't so bad.",
                TooltipText = @"in osu!standard, complete a 3* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-osu-fc_4_star",
                Title = @"Moving Forward",
                Description = @"Bet you feel good about that.",
                TooltipText = @"in osu!standard, complete a 4* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-osu-fc_5_star",
                Title = @"Paradigm Shift",
                Description = @"Surprisingly difficult.",
                TooltipText = @"in osu!standard, complete a 5* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-osu-fc_6_star",
                Title = @"Anguish Quelled",
                Description = @"Don't choke.",
                TooltipText = @"in osu!standard, complete a 6* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-osu-fc_7_star",
                Title = @"Never Give Up",
                Description = @"Excellence is its own reward.",
                TooltipText = @"in osu!standard, complete a 7* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-osu-fc_8_star",
                Title = @"Aberration",
                Description = @"They said it couldn't be done. They were wrong.",
                TooltipText = @"in osu!standard, complete a 8* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-osu-fc_9_star",
                Title = @"Chosen",
                Description = @"Reign among the Prometheans, where you belong.",
                TooltipText = @"in osu!standard, complete a 9* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-osu-fc_10_star",
                Title = @"Unfathomable",
                Description = @"You have no equal.",
                TooltipText = @"in osu!standard, complete a 10* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },

            new Medal
            {
                Slug = @"skill-osu-reach_477_spm",
                Title = @"Cyclone",
                Description = @"Clockwise or anticlockwise, that is the question.",
                TooltipText = @"in osu!standard, reach 477 spins per minute on a spinner",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },

            #endregion osu!standard-only

            #region osu!taiko-only

            new Medal
            {
                Slug = @"skill-taiko-pass_1_star",
                Title = @"My First Don",
                Description = @"Marching to the beat of your own drum. Literally.",
                TooltipText = @"in osu!taiko, complete a 1* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-taiko-pass_2_star",
                Title = @"Katsu Katsu Katsu",
                Description = @"Hora! Ikuzo!",
                TooltipText = @"in osu!taiko, complete a 2* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-taiko-pass_3_star",
                Title = @"Not Even Trying",
                Description = @"Muzukashii? Not even.",
                TooltipText = @"in osu!taiko, complete a 3* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-taiko-pass_4_star",
                Title = @"Face Your Demons",
                Description = @"The first trials are now behind you, but are you a match for the Oni?",
                TooltipText = @"in osu!taiko, complete a 4* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-taiko-pass_5_star",
                Title = @"The Demon Within",
                Description = @"No rest for the wicked.",
                TooltipText = @"in osu!taiko, complete a 5* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-taiko-pass_6_star",
                Title = @"Drumbreaker",
                Description = @"Too strong.",
                TooltipText = @"in osu!taiko, complete a 6* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-taiko-pass_7_star",
                Title = @"The Godfather",
                Description = @"You are the Don of Dons.",
                TooltipText = @"in osu!taiko, complete a 7* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-taiko-pass_8_star",
                Title = @"Rhythm Incarnate",
                Description = @"Feel the beat. Become the beat.",
                TooltipText = @"in osu!taiko, complete a 8* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },

            new Medal
            {
                Slug = @"skill-taiko-fc_1_star",
                Title = @"Keeping Time",
                Description = @"Don, then katsu. Don, then katsu...",
                TooltipText = @"in osu!taiko, complete a 1* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-taiko-fc_2_star",
                Title = @"To Your Own Beat",
                Description = @"Straight and steady.",
                TooltipText = @"in osu!taiko, complete a 2* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-taiko-fc_3_star",
                Title = @"Big Drums",
                Description = @"Bigger scores to match.",
                TooltipText = @"in osu!taiko, complete a 3* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-taiko-fc_4_star",
                Title = @"Adversity Overcome",
                Description = @"Difficult? Not for you.",
                TooltipText = @"in osu!taiko, complete a 4* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-taiko-fc_5_star",
                Title = @"Demonslayer",
                Description = @"An Oni felled forevermore.",
                TooltipText = @"in osu!taiko, complete a 5* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-taiko-fc_6_star",
                Title = @"Rhythm's True Call",
                Description = @"Heralding true skill.",
                TooltipText = @"in osu!taiko, complete a 6* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-taiko-fc_7_star",
                Title = @"Time Everlasting",
                Description = @"Not a single beat escapes you.",
                TooltipText = @"in osu!taiko, complete a 7* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-taiko-fc_8_star",
                Title = @"The Drummer's Throne",
                Description = @"Percussive brilliance befitting royalty alone.",
                TooltipText = @"in osu!taiko, complete a 8* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },

            #endregion osu!taiko-only

            #region osu!catch-only

            new Medal
            {
                Slug = @"skill-fruits-pass_1_star",
                Title = @"A Slice Of Life",
                Description = @"Hey, this fruit catching business isn't bad.",
                TooltipText = @"in osu!catch, complete a 1* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-fruits-pass_2_star",
                Title = @"Dashing Ever Forward",
                Description = @"Fast is how you do it.",
                TooltipText = @"in osu!catch, complete a 2* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-fruits-pass_3_star",
                Title = @"Zesty Disposition",
                Description = @"No scurvy for you, not with that much fruit.",
                TooltipText = @"in osu!catch, complete a 3* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-fruits-pass_4_star",
                Title = @"Hyperdash ON!",
                Description = @"Time and distance is no obstacle to you.",
                TooltipText = @"in osu!catch, complete a 4* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-fruits-pass_5_star",
                Title = @"It's Raining Fruits",
                Description = @"And you can catch them all.",
                TooltipText = @"in osu!catch, complete a 5* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-fruits-pass_6_star",
                Title = @"Fruit Ninja",
                Description = @"Legendary techniques.",
                TooltipText = @"in osu!catch, complete a 6* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-fruits-pass_7_star",
                Title = @"Dreamcatcher",
                Description = @"No fruit, only dreams now.",
                TooltipText = @"in osu!catch, complete a 7* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-fruits-pass_8_star",
                Title = @"Lord Of The Catch",
                Description = @"Your kingdom kneels before you.",
                TooltipText = @"in osu!catch, complete a 8* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },

            new Medal
            {
                Slug = @"skill-fruits-fc_1_star",
                Title = @"Sweet And Sour",
                Description = @"Apples and oranges, literally.",
                TooltipText = @"in osu!catch, complete a 1* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-fruits-fc_2_star",
                Title = @"Reaching The Core",
                Description = @"The seeds of future success.",
                TooltipText = @"in osu!catch, complete a 2* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-fruits-fc_3_star",
                Title = @"Clean Patter",
                Description = @"Clean only of failure. It is completely full, otherwise.",
                TooltipText = @"in osu!catch, complete a 3* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-fruits-fc_4_star",
                Title = @"Between The Rain",
                Description = @"No umbrella needed.",
                TooltipText = @"in osu!catch, complete a 4* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-fruits-fc_5_star",
                Title = @"Addicted",
                Description = @"That was an overdose?",
                TooltipText = @"in osu!catch, complete a 5* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-fruits-fc_6_star",
                Title = @"Quickening",
                Description = @"A dash above normal limits.",
                TooltipText = @"in osu!catch, complete a 6* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-fruits-fc_7_star",
                Title = @"Supersonic",
                Description = @"Faster than is reasonably necessary.",
                TooltipText = @"in osu!catch, complete a 7* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-fruits-fc_8_star",
                Title = @"Dashing Scarlet",
                Description = @"Speed beyond mortal reckoning.",
                TooltipText = @"in osu!catch, complete a 8* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },

            #endregion osu!catch-only

            #region osu!mania-only

            new Medal
            {
                Slug = @"skill-mania-pass_1_star",
                Title = @"First Steps",
                Description = @"It isn't 9-to-5, but 1-to-9. Keys, that is.",
                TooltipText = @"in osu!mania, complete a 1* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-mania-pass_2_star",
                Title = @"No Normal Player",
                Description = @"Not anymore, at least.",
                TooltipText = @"in osu!mania, complete a 2* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-mania-pass_3_star",
                Title = @"Impulse Drive",
                Description = @"Not quite hyperspeed, but getting close.",
                TooltipText = @"in osu!mania, complete a 3* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-mania-pass_4_star",
                Title = @"Hyperspeed",
                Description = @"Woah.",
                TooltipText = @"in osu!mania, complete a 4* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-mania-pass_5_star",
                Title = @"Ever Onwards",
                Description = @"Another challenge is just around the corner.",
                TooltipText = @"in osu!mania, complete a 5* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-mania-pass_6_star",
                Title = @"Another Surpassed",
                Description = @"Is there no limit to your skills?",
                TooltipText = @"in osu!mania, complete a 6* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-mania-pass_7_star",
                Title = @"Extra Credit",
                Description = @"See me after class.",
                TooltipText = @"in osu!mania, complete a 7* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-mania-pass_8_star",
                Title = @"Maniac",
                Description = @"There's just no stopping you.",
                TooltipText = @"in osu!mania, complete a 8* beatmap",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },

            new Medal
            {
                Slug = @"skill-mania-fc_1_star",
                Title = @"Keystruck",
                Description = @"The beginning of a new story.",
                TooltipText = @"in osu!mania, complete a 1* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-mania-fc_2_star",
                Title = @"Keying In",
                Description = @"Finding your groove.",
                TooltipText = @"in osu!mania, complete a 2* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-mania-fc_3_star",
                Title = @"Hyperflow",
                Description = @"You can feel the rhythm.",
                TooltipText = @"in osu!mania, complete a 3* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-mania-fc_4_star",
                Title = @"Breakthrough",
                Description = @"Many skills mastered, rolled into one.",
                TooltipText = @"in osu!mania, complete a 4* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-mania-fc_5_star",
                Title = @"Everything Extra",
                Description = @"Giving your all is giving everything you have.",
                TooltipText = @"in osu!mania, complete a 5* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-mania-fc_6_star",
                Title = @"Level Breaker",
                Description = @"Finesse beyond reason.",
                TooltipText = @"in osu!mania, complete a 6* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-mania-fc_7_star",
                Title = @"Step Up",
                Description = @"A precipice rarely seen.",
                TooltipText = @"in osu!mania, complete a 7* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },
            new Medal
            {
                Slug = @"skill-mania-fc_8_star",
                Title = @"Behind The Veil",
                Description = @"Supernatural!",
                TooltipText = @"in osu!mania, complete a 8* beatmap with no misses",
                Category = category_skill_and_dedication,
                IsCustom = false,
            },

            #endregion osu!mania-only
        ];
    }
}
