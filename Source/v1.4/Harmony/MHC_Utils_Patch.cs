using Verse;
using HarmonyLib;
using MechHumanlikes;

namespace ATReforged
{
    public class MHC_Utils_Patch
    {
        // Coreless androids are treated as non humanlike intelligences.
        [HarmonyPatch(typeof(MHC_Utils), "IsConsideredNonHumanlike")]
        public class IsConsideredNonHumanlike_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn pawn, ref bool __result)
            {
                Log.Warning(pawn.LabelShort + " " + ATRCore_Utils.IsConsideredMechanicalAndroid(pawn).ToString() + " " + pawn.health.hediffSet.HasHediff(ATR_HediffDefOf.ATR_IsolatedCore).ToString());
                __result = __result || (ATRCore_Utils.IsConsideredMechanicalAndroid(pawn) && pawn.health.hediffSet.HasHediff(ATR_HediffDefOf.ATR_IsolatedCore));
            }
        }
    }
}