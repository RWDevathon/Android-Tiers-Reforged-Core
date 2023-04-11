using Verse;
using HarmonyLib;
using MechHumanlikes;

namespace ATReforged
{
    public class MHC_Utils_Patch
    {
        // Surrogates and coreless androids are also treated as non humanlike intelligences.
        [HarmonyPatch(typeof(MHC_Utils), "IsConsideredNonHumanlike")]
        public class IsConsideredNonHumanlike_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn pawn, ref bool __result)
            {
                __result = __result || ATRCore_Utils.IsSurrogate(pawn) || (pawn.def.GetModExtension<ATR_PawnExtension>()?.isAndroid ?? false && pawn.health.hediffSet.HasHediff(ATR_HediffDefOf.ATR_IsolatedCore));
            }
        }
    }
}