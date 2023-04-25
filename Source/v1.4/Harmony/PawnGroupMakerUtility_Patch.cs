using Verse;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;

namespace ATReforged
{
    public class PawnGroupMakerUtility_Patch
    {
        // Androids spawned in other faction groups will always spawn with the stasis pill hediff effect.
        [HarmonyPatch(typeof(PawnGroupMakerUtility), "GeneratePawns")]
        public class GeneratePawns_Patch
        {
            [HarmonyPostfix]
            public static void Listener(PawnGroupMakerParms parms, bool warnOnZeroResults, ref IEnumerable<Pawn> __result)
            {
                // Generated androids in proper groups will always receive the Stasis Hediff to reduce their power consumption significantly.
                foreach (Pawn member in __result)
                {
                    Hediff stasisHediff = member.health.hediffSet.GetFirstHediffOfDef(ATR_HediffDefOf.ATR_StasisPill);
                    if (member.def.GetModExtension<ATR_PawnExtension>()?.isAndroid ?? false && stasisHediff == null)
                    {
                        member.health.AddHediff(HediffMaker.MakeHediff(ATR_HediffDefOf.ATR_StasisPill, member));
                    }
                    else if (stasisHediff != null)
                    {
                        stasisHediff.Severity = 1f;
                    }
                }
            }
        }
    }
}