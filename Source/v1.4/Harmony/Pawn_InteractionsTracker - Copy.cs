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
                // There are special considerations for removing the core (brain) itself. Removing an autonomous core is murder. Removing any core applies the "Isolated Core" hediff and removes the SkyMind comp.
                if (pawn.health.hediffSet.GetBrain() == part)
                {
                    // Removing an interface always disconnects a pawn from the SkyMind network. This needs to disconnect surrogates, controllers, and those currently in mind operations.
                    // This will ensure all appropriate comps and interactions are taken care of before continuing
                    Utils.gameComp.DisconnectFromSkyMind(pawn);

                    // Surrogates are already handled via disconnecting from the SkyMind or are already blank. Autonomous intelligences must be murdered and made blank.
                    if (pawn.Name != Utils.GetBlank().Name)
                    {
                        Utils.Duplicate(Utils.GetBlank(), pawn, isTethered: false);
                        pawn.guest?.SetGuestStatus(Faction.OfPlayer);
                        if (pawn.playerSettings != null)
                            pawn.playerSettings.medCare = MedicalCareCategory.Best;

                        // Send a message about the removed intelligence
                        Messages.Message("ATR_InterfaceRemoved".Translate(), MessageTypeDefOf.NegativeEvent);
                    }

                    // Clean up and apply the appropriate hediff. Apply Isolated core before no host is applied to ensure the pawn doesn't become capable of moving for a tick.
                    pawn.health.AddHediff(ATR_HediffDefOf.ATR_IsolatedCore, pawn.health.hediffSet.GetBrain());
                    Hediff targetHediff = pawn.health.hediffSet.GetFirstHediffOfDef(ATR_HediffDefOf.ATR_NoController);
                    if (targetHediff != null)
                        pawn.health.RemoveHediff(targetHediff);
                }
            }
        }
    }
}