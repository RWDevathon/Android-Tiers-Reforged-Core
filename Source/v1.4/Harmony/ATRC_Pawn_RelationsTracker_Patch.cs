using Verse;
using HarmonyLib;
using RimWorld;

namespace ATReforged
{
    public class ATRC_Pawn_RelationsTracker_Patch
    {
        // Androids should be significantly less likely to initiate or accept relationships for balancing purposes.
        [HarmonyPatch(typeof(Pawn_RelationsTracker), "LovinAgeFactor")]
        public static class LovinAgeFactor_Patch
        {
            [HarmonyPrefix]
            public static bool Listener(Pawn otherPawn, ref float __result, ref Pawn ___pawn)
            {
                bool isAndroid = ATRCore_Utils.IsConsideredMechanicalAndroid(___pawn);
                bool otherisAndroid = ATRCore_Utils.IsConsideredMechanicalAndroid(otherPawn);
                // If exactly one of them is an android, quarter the relationship factor.
                if (isAndroid != otherisAndroid)
                {
                    __result = 0.25f;
                    return false;
                }
                // If both are androids, slightly reduce relationship factor.
                else if (isAndroid && otherisAndroid)
                {
                    __result = 0.75f;
                    return false;
                }
                // If neither are androids, allow default behavior to continue.
                return true;
            }
        }
    }
}