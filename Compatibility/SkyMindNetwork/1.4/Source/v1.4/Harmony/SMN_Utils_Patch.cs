using HarmonyLib;
using SkyMind;
using Verse;

namespace ATReforged
{
    public class SMN_Utils_Patch
    {
        // Androids that are turned into blanks via SkyMind connection ought to have the no installed interface hediff.
        [HarmonyPatch(typeof(SMN_Utils), "TurnIntoBlank")]
        public class TurnIntoBlank_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn pawn, bool shouldRemoveSkyMindImplants)
            {
                if (ATRCore_Utils.IsConsideredMechanicalAndroid(pawn) && shouldRemoveSkyMindImplants)
                {
                    pawn.health.AddHediff(ATR_HediffDefOf.ATR_IsolatedCore, pawn.health.hediffSet.GetBrain());
                }
            }
        }
    }
}
