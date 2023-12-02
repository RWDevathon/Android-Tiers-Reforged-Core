using Verse;
using HarmonyLib;
using RimWorld;
using MechHumanlikes;
using System.Collections.Generic;

namespace ATReforged
{
    public class Recipe_RemoveMechanicalPart_Patch
    {
        // Androids need to have special behavior done if their core is removed.
        [HarmonyPatch(typeof(Recipe_RemoveMechanicalPart), "ApplyOnPawn")]
        public class ApplyOnPawn_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
            {
                // There are special considerations for removing the core for androids. Removing an autonomous core is murder. Removing any core applies the "Isolated Core" hediff.
                if (ATRCore_Utils.IsConsideredMechanicalAndroid(pawn) && pawn.health.hediffSet.GetBrain() == part)
                {
                    // Autonomous intelligences must be murdered and made blank.
                    if (pawn.Name != ATRCore_Utils.GetBlank().Name)
                    {
                        ATRCore_Utils.Duplicate(ATRCore_Utils.GetBlank(), pawn, isTethered: false);
                        pawn.guest?.SetGuestStatus(Faction.OfPlayer);
                        if (pawn.playerSettings != null)
                            pawn.playerSettings.medCare = MedicalCareCategory.Best;

                        // Send a message about the removed intelligence
                        Messages.Message("ATR_InterfaceRemoved".Translate(), MessageTypeDefOf.NegativeEvent);
                    }

                    // Removing a core applies the isolated Core Hediff.
                    pawn.health.AddHediff(ATR_HediffDefOf.ATR_IsolatedCore, pawn.health.hediffSet.GetBrain());
                }
            }
        }
    }
}