using Verse;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using MechHumanlikes;

namespace ATReforged
{
    public class ATR_PawnGroupMakerUtility_Patch
    {
        // Mechanical members of android factions will always spawn with the stasis pill hediff effect.
        [HarmonyPatch(typeof(PawnGroupMakerUtility), "GeneratePawns")]
        public class GeneratePawns_Patch
        {
            [HarmonyPostfix]
            public static void Listener(PawnGroupMakerParms parms, bool warnOnZeroResults, ref IEnumerable<Pawn> __result)
            {
                // If the faction in question is not an android faction, do not provide them the hediff.
                if (!parms.faction?.def.GetModExtension<ATR_FactionAndroidExtension>()?.membersShouldBeAndroids ?? true)
                {
                    return;
                }

                List<Pawn> modifiedPawns = __result.ToList();
                foreach (Pawn member in modifiedPawns)
                {
                    // This only applies to mechanical units in the faction, in the event there are mixed organic/mechanical groups.
                    if (!MHC_Utils.IsConsideredMechanical(member))
                    {
                        continue;
                    }

                    Hediff stasisHediff = member.health.hediffSet.GetFirstHediffOfDef(ATR_HediffDefOf.ATR_StasisPill);
                    if (stasisHediff == null)
                    {
                        member.health.AddHediff(HediffMaker.MakeHediff(ATR_HediffDefOf.ATR_StasisPill, member));
                    }
                    else if (stasisHediff != null)
                    {
                        stasisHediff.Severity = 1f;
                    }
                }
                __result = modifiedPawns;
            }
        }
    }
}