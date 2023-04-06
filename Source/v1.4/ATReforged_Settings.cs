using UnityEngine;
using Verse;
using RimWorld;
using System;
using System.Collections.Generic;
using MechHumanlikes;

namespace ATReforged
{
    public class ATReforged_Settings : ModSettings
    {
        // GENERAL SETTINGS
            // Settings for android gender
        public static bool androidsHaveGenders;
        public static bool androidsPickGenders;
        public static Gender androidsFixedGender;
        public static float androidsGenderRatio;

        // SECURITY SETTINGS
            // Settings for Enemy hacks
        public static bool enemyHacksOccur;
        public static float chanceAlliesInterceptHack;
        public static float pointsGainedOnInterceptPercentage;
        public static float enemyHackAttackStrengthModifier;
        public static float percentageOfValueUsedForRansoms;

            // Settings for player hacks
        public static bool playerCanHack = true;
        public static bool receiveHackingAlert = true;
        public static float retaliationChanceOnFailure = 0.4f;
        public static float minHackSuccessChance = 0.05f;
        public static float maxHackSuccessChance = 0.95f;

        // CONNECTIVITY SETTINGS
            // Settings for Surrogates
        public static bool surrogatesAllowed = true;
        public static bool otherFactionsAllowedSurrogates = true;
        public static int minGroupSizeForSurrogates = 5;
        public static float minSurrogatePercentagePerLegalGroup = 0.0f;
        public static float maxSurrogatePercentagePerLegalGroup = 0.7f;

        public static bool displaySurrogateControlIcon = true;
        public static int safeSurrogateConnectivityCountBeforePenalty = 1;

            // Settings for Skill Points
        public static bool receiveSkillAlert = true;
        public static int skillPointInsertionRate = 100;
        public static float skillPointConversionRate = 0.5f;
        public static int passionSoftCap = 8;
        public static float basePointsNeededForPassion = 5000f;

            // Settings for Cloud
        public static bool uploadingToSkyMindKills = true;
        public static bool uploadingToSkyMindPermaKills = true;
        public static int timeToCompleteSkyMindOperations = 24;

        // INTERNAL SETTINGS
        // Settings page
        public ATR_OptionsTab activeTab = ATR_OptionsTab.General;
        public MHC_SettingsPreset ActivePreset = MHC_SettingsPreset.None;
        public bool settingsEverOpened = false;

        public void StartupChecks()
        {
            if (ActivePreset == MHC_SettingsPreset.None)
            {
                settingsEverOpened = false;
                ApplyPreset(MHC_SettingsPreset.Default);
            }
        }

        Vector2 scrollPosition = Vector2.zero;
        float cachedScrollHeight = 0;

        public void DoSettingsWindowContents(Rect inRect)
        {
            settingsEverOpened = true;
            bool hasChanged = false;
            void onChange() { hasChanged = true; }

            Color colorSave = GUI.color;
            TextAnchor anchorSave = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleCenter;

            var headerRect = inRect.TopPartPixels(50);
            var restOfRect = new Rect(inRect);
            restOfRect.y += 50;
            restOfRect.height -= 50;

            Listing_Standard prelist = new Listing_Standard();
            prelist.Begin(headerRect);

            prelist.EnumSelector("MHC_SettingsTabTitle".Translate(), ref activeTab, "ATR_SettingsTabOption_", valueTooltipPostfix: null);
            prelist.GapLine();

            prelist.End();

            bool needToScroll = cachedScrollHeight > inRect.height;
            var viewRect = new Rect(restOfRect);
            if (needToScroll)
            {
                viewRect.width -= 20f;
                viewRect.height = cachedScrollHeight;
                Widgets.BeginScrollView(restOfRect, ref scrollPosition, viewRect);
            }

            Listing_Standard listingStandard = new Listing_Standard
            {
                maxOneColumn = true
            };
            listingStandard.Begin(viewRect);

            switch (activeTab)
            {
                case ATR_OptionsTab.General:
                {
                    // PRESET SETTINGS
                    if (listingStandard.ButtonText("MHC_ApplyPreset".Translate()))
                    {
                        List<FloatMenuOption> options = new List<FloatMenuOption>();
                        foreach (MHC_SettingsPreset s in Enum.GetValues(typeof(MHC_SettingsPreset)))
                        {
                            if (s == MHC_SettingsPreset.None) // Can not apply the None preset.
                            {
                                continue;
                            }
                            options.Add(new FloatMenuOption(("MHC_SettingsPreset" + s.ToString()).Translate(), () => ApplyPreset(s)));
                        }
                        Find.WindowStack.Add(new FloatMenu(options));
                    }
                    listingStandard.GapLine();

                    // GENDER SETTINGS
                    listingStandard.CheckboxLabeled("ATR_AndroidsHaveGenders".Translate(), ref androidsHaveGenders, tooltip:"ATR_AndroidGenderNotice".Translate(), onChange: onChange);

                    if (androidsHaveGenders)
                    {
                        listingStandard.CheckboxLabeled("ATR_AndroidsPickGenders".Translate(), ref androidsPickGenders, tooltip: "ATR_AndroidGenderNotice".Translate(), onChange: onChange);
                    }

                    if (androidsHaveGenders && !androidsPickGenders)
                    {
                        bool fixedGender = androidsFixedGender == Gender.Female;
                        listingStandard.CheckboxLabeled("ATR_AndroidsFixedGenderSelector".Translate(), ref fixedGender, tooltip: "ATR_AndroidGenderNotice".Translate(), onChange: onChange);
                        androidsFixedGender = fixedGender ? Gender.Female: Gender.Male;
                    }

                    if (androidsHaveGenders && androidsPickGenders)
                    {
                        listingStandard.SliderLabeled("ATR_AndroidsGenderRatio".Translate(), ref androidsGenderRatio, 0.0f, 1.0f, displayMult: 100, onChange: onChange);
                    }
                    listingStandard.GapLine();
                    break;
                }
                case ATR_OptionsTab.Security:
                {
                    listingStandard.CheckboxLabeled("ATR_EnemyHacksOccur".Translate(), ref enemyHacksOccur, onChange: onChange);
                    if (enemyHacksOccur)
                    {
                        listingStandard.SliderLabeled("ATR_EnemyHackAttackStrengthModifier".Translate(), ref enemyHackAttackStrengthModifier, 0.01f, 5f, displayMult: 100, valueSuffix: "%", tooltip: "ATR_EnemyHackAttackStrengthModifierDesc".Translate(), onChange: onChange);
                        listingStandard.SliderLabeled("ATR_ChanceAlliesInterceptHack".Translate(), ref chanceAlliesInterceptHack, 0.01f, 1f, displayMult: 100, valueSuffix: "%", tooltip: "ATR_ChanceAlliesInterceptHackDesc".Translate(), onChange: onChange);
                        listingStandard.SliderLabeled("ATR_PointsGainedOnInterceptPercentage".Translate(), ref pointsGainedOnInterceptPercentage, 0.00f, 3f, displayMult: 100, valueSuffix: "%", tooltip: "ATR_PointsGainedOnInterceptPercentageDesc".Translate(), onChange: onChange);
                        listingStandard.SliderLabeled("ATR_PercentageOfValueUsedForRansoms".Translate(), ref percentageOfValueUsedForRansoms, 0.01f, 2f, displayMult: 100, valueSuffix:"%", onChange: onChange);
                    }

                    listingStandard.CheckboxLabeled("ATR_PlayerCanHack".Translate(), ref playerCanHack, onChange: onChange);
                    if (playerCanHack)
                    {
                        listingStandard.CheckboxLabeled("ATR_receiveFullHackingAlert".Translate(), ref receiveHackingAlert, onChange: onChange);
                        listingStandard.SliderLabeled("ATR_RetaliationChanceOnFailure".Translate(), ref retaliationChanceOnFailure, 0.0f, 1f, displayMult: 100, valueSuffix: "%", onChange: onChange);
                        listingStandard.SliderLabeled("ATR_MinHackSuccessChance".Translate(), ref minHackSuccessChance, 0.0f, maxHackSuccessChance, displayMult: 100, valueSuffix: "%", onChange: onChange);
                        listingStandard.SliderLabeled("ATR_MaxHackSuccessChance".Translate(), ref maxHackSuccessChance, minHackSuccessChance, 1f, displayMult: 100, valueSuffix: "%", onChange: onChange);
                    }
                    break;
                }
                case ATR_OptionsTab.Connectivity:
                {
                    // SURROGATES
                    listingStandard.CheckboxLabeled("ATR_surrogatesAllowed".Translate(), ref surrogatesAllowed, onChange: onChange);
                    if (surrogatesAllowed)
                    {
                        listingStandard.CheckboxLabeled("ATR_otherFactionsAllowedSurrogates".Translate(), ref otherFactionsAllowedSurrogates, onChange: onChange);
                        if (otherFactionsAllowedSurrogates)
                        {
                            string minGroupSizeForSurrogatesBuffer = minGroupSizeForSurrogates.ToString();
                            listingStandard.TextFieldNumericLabeled("ATR_minGroupSizeForSurrogates".Translate(), ref minGroupSizeForSurrogates, ref minGroupSizeForSurrogatesBuffer, 1, 50);
                            listingStandard.SliderLabeled("ATR_minSurrogatePercentagePerLegalGroup".Translate(), ref minSurrogatePercentagePerLegalGroup, 0.01f, 1f, displayMult: 100, valueSuffix: "%", onChange: onChange);
                            listingStandard.SliderLabeled("ATR_maxSurrogatePercentagePerLegalGroup".Translate(), ref maxSurrogatePercentagePerLegalGroup, 0.01f, 1f, displayMult: 100, valueSuffix: "%", onChange: onChange);
                        }
                        listingStandard.CheckboxLabeled("ATR_displaySurrogateControlIcon".Translate(), ref displaySurrogateControlIcon, onChange: onChange);
                        string safeSurrogateConnectivityCountBeforePenaltyBuffer = safeSurrogateConnectivityCountBeforePenalty.ToString();
                        listingStandard.TextFieldNumericLabeled("ATR_safeSurrogateConnectivityCountBeforePenalty".Translate(), ref safeSurrogateConnectivityCountBeforePenalty, ref safeSurrogateConnectivityCountBeforePenaltyBuffer, 1, 40);
                    }
                    listingStandard.GapLine();

                    // SKILL POINTS
                    string skillPointInsertionRateBuffer = skillPointInsertionRate.ToString();
                    string skillPointConversionRateBuffer = skillPointConversionRate.ToString();
                    string passionSoftCapBuffer = passionSoftCap.ToString();
                    string basePointsNeededForPassionBuffer = basePointsNeededForPassion.ToString();
                    listingStandard.CheckboxLabeled("ATR_receiveFullSkillAlert".Translate(), ref receiveSkillAlert, onChange: onChange);
                    listingStandard.TextFieldNumericLabeled("ATR_skillPointInsertionRate".Translate(), ref skillPointInsertionRate, ref skillPointInsertionRateBuffer, 1f);
                    listingStandard.TextFieldNumericLabeled("ATR_skillPointConversionRate".Translate(), ref skillPointConversionRate, ref skillPointConversionRateBuffer, 0.01f, 10);
                    listingStandard.TextFieldNumericLabeled("ATR_passionSoftCap".Translate(), ref passionSoftCap, ref passionSoftCapBuffer, 0, 50);
                    listingStandard.TextFieldNumericLabeled("ATR_basePointsNeededForPassion".Translate(), ref basePointsNeededForPassion, ref basePointsNeededForPassionBuffer, 10, 10000);
                    listingStandard.GapLine();

                    // CLOUD
                    listingStandard.CheckboxLabeled("ATR_UploadingKills".Translate(), ref uploadingToSkyMindKills, onChange: onChange);
                    listingStandard.CheckboxLabeled("ATR_UploadingPermakills".Translate(), ref uploadingToSkyMindPermaKills, onChange: onChange);
                    string SkyMindOperationTimeBuffer = timeToCompleteSkyMindOperations.ToString();
                    listingStandard.TextFieldNumericLabeled("ATR_SkyMindOperationTimeRequired".Translate(), ref timeToCompleteSkyMindOperations, ref SkyMindOperationTimeBuffer, 1, 256);
                    listingStandard.GapLine();

                    break;
                }
                default:
                {
                    break;
                }
            }
            // Ending

            cachedScrollHeight = listingStandard.CurHeight;
            listingStandard.End();

            if (needToScroll)
            {
                Widgets.EndScrollView();
            }


            if (hasChanged)
                ApplyPreset(MHC_SettingsPreset.Custom);

            GUI.color = colorSave;
            Text.Anchor = anchorSave;
        }

        public void ApplyBaseSettings()
        {
            // Reset Gender Settings
            androidsHaveGenders = false;
            androidsPickGenders = false;
            androidsFixedGender = 0;
            androidsGenderRatio = 0.5f;

            // SECURITY SETTINGS
            enemyHacksOccur = true;
            chanceAlliesInterceptHack = 0.05f;
            pointsGainedOnInterceptPercentage = 0.25f;
            enemyHackAttackStrengthModifier = 1.0f;
            percentageOfValueUsedForRansoms = 0.25f;

            playerCanHack = true;
            receiveHackingAlert = true;
            retaliationChanceOnFailure = 0.4f;
            minHackSuccessChance = 0.05f;
            maxHackSuccessChance = 0.95f;

            // CONNECTIVITY SETTINGS
                // Surrogates
            surrogatesAllowed = true;
            otherFactionsAllowedSurrogates = true;
            minGroupSizeForSurrogates = 5;
            minSurrogatePercentagePerLegalGroup = 0.0f;
            maxSurrogatePercentagePerLegalGroup = 0.7f;
            displaySurrogateControlIcon = true;
            safeSurrogateConnectivityCountBeforePenalty = 1;

            // Skills
            skillPointInsertionRate = 100;
            skillPointConversionRate = 0.5f;
            passionSoftCap = 8;
            basePointsNeededForPassion = 5000f;

            // Cloud
            receiveSkillAlert = true;
            uploadingToSkyMindKills = true;
            uploadingToSkyMindPermaKills = true;
            timeToCompleteSkyMindOperations = 24;
        }

        public void ApplyPreset(MHC_SettingsPreset preset)
        {
            if (preset == MHC_SettingsPreset.None)
                throw new InvalidOperationException("[ATR] Applying the None preset is illegal - were the mod options properly initialized?");

            ActivePreset = preset;
            if (preset == MHC_SettingsPreset.Custom) // Custom settings are inherently not a preset, so apply no new settings.
                return;

            ApplyBaseSettings();

            switch (preset)
            {
                case MHC_SettingsPreset.Default:
                    break;
                default:
                    throw new InvalidOperationException("Attempted to apply a nonexistent preset.");
            }

        }

        public override void ExposeData()
        {
            base.ExposeData();

            /* == INTERNAL === */
            Scribe_Values.Look(ref ActivePreset, "MHC_ActivePreset", MHC_SettingsPreset.None, true);

            /* === GENERAL === */

            // Gender
            Scribe_Values.Look(ref androidsHaveGenders, "ATR_androidsHaveGenders", false);
            Scribe_Values.Look(ref androidsPickGenders, "ATR_androidsPickGenders", false);
            Scribe_Values.Look(ref androidsFixedGender, "ATR_androidsFixedGender", Gender.None);
            Scribe_Values.Look(ref androidsGenderRatio, "ATR_androidsGenderRatio", 0.5f);

            /* === SECURITY === */

            // Hostile Hacks
            Scribe_Values.Look(ref enemyHacksOccur, "ATR_enemyHacksOccur", true);
            Scribe_Values.Look(ref chanceAlliesInterceptHack, "ATR_chanceAlliesInterceptHack", 0.05f);
            Scribe_Values.Look(ref pointsGainedOnInterceptPercentage, "ATR_pointsGainedOnInterceptPercentage", 0.25f);
            Scribe_Values.Look(ref enemyHackAttackStrengthModifier, "ATR_enemyHackAttackStrengthModifier", 1.0f);
            Scribe_Values.Look(ref percentageOfValueUsedForRansoms, "ATR_percentageOfValueUsedForRansoms", 0.25f);

            // Player Hacks
            Scribe_Values.Look(ref playerCanHack, "ATR_playerCanHack", true);
            Scribe_Values.Look(ref receiveHackingAlert, "ATR_receiveHackingAlert", true);
            Scribe_Values.Look(ref retaliationChanceOnFailure, "ATR_retaliationChanceOnFailure", 0.4f);
            Scribe_Values.Look(ref minHackSuccessChance, "ATR_minHackSuccessChance", 0.05f);
            Scribe_Values.Look(ref maxHackSuccessChance, "ATR_maxHackSuccessChance", 0.95f);

            /* === CONNECTIVITY === */
            // Surrogates
            Scribe_Values.Look(ref surrogatesAllowed, "ATR_surrogatesAllowed", true);
            Scribe_Values.Look(ref otherFactionsAllowedSurrogates, "ATR_otherFactionsAllowedSurrogates", true);
            Scribe_Values.Look(ref minGroupSizeForSurrogates, "ATR_minGroupSizeForSurrogates", 5);
            Scribe_Values.Look(ref minSurrogatePercentagePerLegalGroup, "ATR_minSurrogatePercentagePerLegalGroup", 0.0f);
            Scribe_Values.Look(ref maxSurrogatePercentagePerLegalGroup, "ATR_maxSurrogatePercentagePerLegalGroup", 0.7f);
            Scribe_Values.Look(ref displaySurrogateControlIcon, "ATR_displaySurrogateControlIcon", true);
            Scribe_Values.Look(ref safeSurrogateConnectivityCountBeforePenalty, "ATR_safeSurrogateConnectivityCountBeforePenalty", 1);

            // Skills
            Scribe_Values.Look(ref receiveSkillAlert, "ATR_receiveSkillAlert", true);
            Scribe_Values.Look(ref skillPointInsertionRate, "ATR_skillPointInsertionRate", 100);
            Scribe_Values.Look(ref skillPointConversionRate, "ATR_skillPointConversionRate", 0.5f);
            Scribe_Values.Look(ref passionSoftCap, "ATR_passionSoftCap", 8);
            Scribe_Values.Look(ref basePointsNeededForPassion, "ATR_basePointsNeededForPassion", 5000f);

            // Cloud
            Scribe_Values.Look(ref uploadingToSkyMindKills, "ATR_uploadingToSkyMindKills", true);
            Scribe_Values.Look(ref uploadingToSkyMindPermaKills, "ATR_uploadingToSkyMindPermaKills", true);
            Scribe_Values.Look(ref timeToCompleteSkyMindOperations, "ATR_timeToCompleteSkyMindOperations", 24);
        }
    }

}