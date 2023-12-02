using HarmonyLib;
using System.Reflection;
using Verse;
using UnityEngine;
using RimWorld;

namespace ATReforged
{
    public class ATReforgedCore : Mod
    {
        public static ATReforgedCore_Settings settings;
        public static ATReforgedCore ModSingleton { get; private set; }

        public ATReforgedCore(ModContentPack content) : base(content)
        {
            ModSingleton = this;
            new Harmony("ATReforgedCore").PatchAll(Assembly.GetExecutingAssembly());
        }
        
        // Handles the localization for the mod's name in the list of mods in the mod settings page.
        public override string SettingsCategory()
        {
            return "ATRCore_ModTitle".Translate();
        }

        // Handles actually displaying this mod's settings.
        public override void DoSettingsWindowContents(Rect inRect)
        {
            settings.DoSettingsWindowContents(inRect);
            base.DoSettingsWindowContents(inRect);
        }
    }

    [StaticConstructorOnStartup]
    public static class ATReforgedCore_PostInit
    {
        static ATReforgedCore_PostInit()
        {
            ATReforgedCore.settings = ATReforgedCore.ModSingleton.GetSettings<ATReforgedCore_Settings>();


            // Must dynamically modify some ThingDefs based on certain qualifications.
            foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
            {
                // Check race to see if the thingDef is for a Pawn.
                if (thingDef.race != null && thingDef.GetModExtension<ATR_PawnExtension>()?.isAndroid == true)
                {
                    ATRCore_Utils.cachedAndroidRaces.Add(thingDef);
                }
            }
        }
    }
}