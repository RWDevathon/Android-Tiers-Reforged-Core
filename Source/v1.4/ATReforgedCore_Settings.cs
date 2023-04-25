using UnityEngine;
using Verse;
using RimWorld;
using MechHumanlikes;

namespace ATReforged
{
    public class ATReforgedCore_Settings : ModSettings
    {
        // GENERAL SETTINGS
            // Settings for android gender
        public static bool androidsHaveGenders;
        public static bool androidsPickGenders;
        public static Gender androidsFixedGender;
        public static float androidsGenderRatio;

        // Settings page

        Vector2 scrollPosition = Vector2.zero;
        float cachedScrollHeight = 0;

        public void DoSettingsWindowContents(Rect inRect)
        {
            Color colorSave = GUI.color;
            TextAnchor anchorSave = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleCenter;

            bool needToScroll = cachedScrollHeight > inRect.height;
            var viewRect = new Rect(inRect);
            if (needToScroll)
            {
                viewRect.width -= 20f;
                viewRect.height = cachedScrollHeight;
                Widgets.BeginScrollView(inRect, ref scrollPosition, viewRect);
            }

            Listing_Standard listingStandard = new Listing_Standard
            {
                maxOneColumn = true
            };
            listingStandard.Begin(viewRect);

            // GENDER SETTINGS
            listingStandard.CheckboxLabeled("ATR_AndroidsHaveGenders".Translate(), ref androidsHaveGenders, tooltip:"ATR_AndroidGenderNotice".Translate());

            if (androidsHaveGenders)
            {
                listingStandard.CheckboxLabeled("ATR_AndroidsPickGenders".Translate(), ref androidsPickGenders, tooltip: "ATR_AndroidGenderNotice".Translate());
            }

            if (androidsHaveGenders && !androidsPickGenders)
            {
                bool fixedGender = androidsFixedGender == Gender.Female;
                listingStandard.CheckboxLabeled("ATR_AndroidsFixedGenderSelector".Translate(), ref fixedGender, tooltip: "ATR_AndroidGenderNotice".Translate());
                androidsFixedGender = fixedGender ? Gender.Female: Gender.Male;
            }

            if (androidsHaveGenders && androidsPickGenders)
            {
                listingStandard.SliderLabeled("ATR_AndroidsGenderRatio".Translate(), ref androidsGenderRatio, 0.0f, 1.0f, displayMult: 100);
            }
            listingStandard.GapLine();
            // Ending

            cachedScrollHeight = listingStandard.CurHeight;
            listingStandard.End();

            if (needToScroll)
            {
                Widgets.EndScrollView();
            }

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
        }

        public override void ExposeData()
        {
            base.ExposeData();

            // Gender
            Scribe_Values.Look(ref androidsHaveGenders, "ATR_androidsHaveGenders", false);
            Scribe_Values.Look(ref androidsPickGenders, "ATR_androidsPickGenders", false);
            Scribe_Values.Look(ref androidsFixedGender, "ATR_androidsFixedGender", Gender.None);
            Scribe_Values.Look(ref androidsGenderRatio, "ATR_androidsGenderRatio", 0.5f);
        }
    }

}