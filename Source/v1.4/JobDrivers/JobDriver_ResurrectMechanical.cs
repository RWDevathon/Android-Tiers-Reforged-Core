using System;
using System.Collections.Generic;
using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;
using MechHumanlikes;

namespace ATReforged
{
    public class JobDriver_ResurrectMechanical : JobDriver_Resurrect
    {
        private Corpse Corpse => (Corpse)job.GetTarget(TargetIndex.A).Thing;

        private Thing Item => job.GetTarget(TargetIndex.B).Thing;

        private Mote warmupMote;

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch).FailOnDespawnedOrNull(TargetIndex.B).FailOnDespawnedOrNull(TargetIndex.A);
            yield return Toils_Haul.StartCarryThing(TargetIndex.B);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnDespawnedOrNull(TargetIndex.A);
            Toil toil = Toils_General.Wait(1200);
            toil.WithProgressBarToilDelay(TargetIndex.A);
            toil.FailOnDespawnedOrNull(TargetIndex.A);
            toil.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            toil.tickAction = delegate
            {
                CompUsable compUsable = Item.TryGetComp<CompUsable>();
                if (compUsable != null && warmupMote == null && compUsable.Props.warmupMote != null)
                {
                    warmupMote = MoteMaker.MakeAttachedOverlay(Corpse, compUsable.Props.warmupMote, Vector3.zero);
                }

                warmupMote?.Maintain();
            };
            yield return toil;
            yield return Toils_General.Do(new Action(Resurrect));
        }

        // Resurrect the targetted pawn if it is legal to do so.
        private void Resurrect()
        { 
            Pawn innerPawn = Corpse.InnerPawn;

            // Legal to resurrect pawns with this if they are considered mechanical.
            if (MHC_Utils.IsConsideredMechanical(innerPawn))
            {
                // Drone Resurrection Kits may only resurrect drone units (simple-minded) or animal units.
                if (Item.def.defName == "ATR_DroneResurrectorKit" && MHC_Utils.IsConsideredMechanicalSapient(innerPawn))
                {
                    Messages.Message("ATR_ResurrectionFailedDroneOnly".Translate(innerPawn).CapitalizeFirst(), innerPawn, MessageTypeDefOf.RejectInput, true);
                    return;
                }

                // Apply the long reboot (24 hours) to the pawn. This will ensure hostile units can be safely captured, and that friendly units can't be reactivated mid-combat.
                Hediff rebootHediff = HediffMaker.MakeHediff(MHC_HediffDefOf.MHC_Restarting, innerPawn);
                innerPawn.health.AddHediff(rebootHediff);

                bool shouldbeBlank = false;
                // Androids have a special consideration attached: if the Core is destroyed, then the dead pawn is blank upon resurrection.
                if (ATRCore_Utils.IsConsideredMechanicalAndroid(innerPawn) && MHC_Utils.IsConsideredMechanicalSapient(innerPawn) && innerPawn.health.hediffSet.GetBrain() == null)
                {
                    shouldbeBlank = true;
                }

                // This kit executes a full resurrection which removes all negative hediffs.
                ResurrectionUtility.Resurrect(innerPawn);
                SoundDefOf.MechSerumUsed.PlayOneShot(SoundInfo.InMap(innerPawn));

                // If the pawn should be blank, replace their autonomous core with an isolated core to represent that.
                if (shouldbeBlank)
                {
                    ATRCore_Utils.Duplicate(ATRCore_Utils.GetBlank(), innerPawn, false, false);
                    innerPawn.health.AddHediff(ATR_HediffDefOf.ATR_IsolatedCore, innerPawn.health.hediffSet.GetBrain());
                    Hediff target = innerPawn.health.hediffSet.GetFirstHediffOfDef(ATR_HediffDefOf.ATR_AutonomousCore);
                    if (target != null)
                    {
                        innerPawn.health.RemoveHediff(target);
                    }
                    innerPawn.guest?.SetGuestStatus(Faction.OfPlayer);
                    if (innerPawn.playerSettings != null)
                        innerPawn.playerSettings.medCare = MedicalCareCategory.Best;
                }

                // Notify successful resurrection and destroy the used kit.
                Messages.Message("ATR_ResurrectionSuccessful".Translate(innerPawn).CapitalizeFirst(), innerPawn, MessageTypeDefOf.PositiveEvent, true);
                Item.SplitOff(1).Destroy(DestroyMode.Vanish);
            }
            else
                Messages.Message("ATR_ResurrectionFailedInvalidPawn".Translate(innerPawn).CapitalizeFirst(), innerPawn, MessageTypeDefOf.RejectInput, true);
        }
    }
}
