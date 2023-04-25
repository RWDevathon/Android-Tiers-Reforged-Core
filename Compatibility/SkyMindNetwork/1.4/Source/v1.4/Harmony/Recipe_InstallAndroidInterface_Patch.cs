using System;
using HarmonyLib;
using Verse;

namespace ATReforged
{
    public class Recipe_InstallAndroidInterface_Patch
    {
        // If SkyMind Network is installed, allow androids to be initialized either via SkyMind or by self-initialization.
        // Destructive Prefix to prevent original behavior - the Dialog_InitializeMind will handle that if the player wishes.
        [HarmonyPatch(typeof(Recipe_InstallAndroidInterface), "InitializeMind")]
        public class InitializeMind_Patch
        {
            [HarmonyPrefix]
            public static bool Prefix(Pawn pawn)
            {
                Find.WindowStack.Add(new Dialog_InitializeMind(pawn));
                return false;
            }
        }
    }
}
