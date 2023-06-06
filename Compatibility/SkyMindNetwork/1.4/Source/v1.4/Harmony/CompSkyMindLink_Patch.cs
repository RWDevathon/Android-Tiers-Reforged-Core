using HarmonyLib;
using SkyMind;
using Verse;

namespace ATReforged
{
    public class CompSkyMindLink_Patch
    {
        // Androids may not be downloaded to via normal means, as it can only be done on installation of an autonomous core.
        [HarmonyPatch(typeof(CompSkyMindLink), "MayDownloadTo")]
        public class MayDownloadTo_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn source, Pawn destination, ref bool __result)
            {
                __result = __result && !ATRCore_Utils.IsConsideredMechanicalAndroid(destination);
            }
        }
    }
}
