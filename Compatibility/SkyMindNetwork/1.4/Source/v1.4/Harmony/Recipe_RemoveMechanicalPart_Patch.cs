using System.Collections.Generic;
using HarmonyLib;
using MechHumanlikes;
using RimWorld;
using SkyMind;
using Verse;

namespace ATReforged
{
    public class Recipe_RemoveMechanicalPart_Patch
    {
        // With SkyMind Network, removing an android's core needs to disconnect it and remove the No Controller hediff.
        [HarmonyPatch(typeof(Recipe_RemoveMechanicalPart), "ApplyOnPawn")]
        public class ApplyOnPawn_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
            {
                // There are special considerations for removing the core for androids. Removing an autonomous core is murder. Removing any core applies the "Isolated Core" hediff.
                if (pawn.def.GetModExtension<ATR_PawnExtension>()?.isAndroid == true && pawn.health.hediffSet.GetBrain() == part)
                {
                    SMN_Utils.gameComp.DisconnectFromSkyMind(pawn);

                    if (pawn.health.hediffSet.GetFirstHediffOfDef(SMN_HediffDefOf.SMN_NoController) is Hediff target)
                    {
                        pawn.health.RemoveHediff(target);
                    }
                }
            }
        }
    }
}
