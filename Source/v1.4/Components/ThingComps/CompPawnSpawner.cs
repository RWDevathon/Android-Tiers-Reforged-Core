using Verse;
using RimWorld;
using System;
using MechHumanlikes;

namespace ATReforged
{
    public class CompPawnSpawner : ThingComp
    {
        public CompProperties_PawnSpawner Props
        {
            get
            {
                return props as CompProperties_PawnSpawner;
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            SpawnPawn();
            parent.Destroy();
        }

        // Fail-safe if an error should happen to occur at spawn, attempt to do it again repeatedly somewhat infrequently until success.
        public override void CompTick()
        {
            if (Find.TickManager.TicksGame % 339 == 0)
            {
                SpawnPawn();
                parent.Destroy();
            }
        }

        // Generate and spawn the created pawn.
        public virtual Pawn SpawnPawn()
        {
            try
            {
                PawnGenerationRequest request = new PawnGenerationRequest(Props.pawnKind, Faction.OfPlayer, PawnGenerationContext.NonPlayer, forceGenerateNewPawn: true, canGeneratePawnRelations: false, allowFood: false, allowAddictions: false, fixedBiologicalAge: 0, fixedChronologicalAge: 0, fixedIdeo: null, forceNoIdeo: true, forceBaselinerChance: 1f);
                Pawn pawn = PawnGenerator.GeneratePawn(request);

                // Pawns may sometimes spawn with apparel somewhere in the generation process. Ensure they don't actually spawn with any - if they even can have apparel.
                pawn.apparel?.DestroyAll();

                // If the pawn is a sapient android, they will spawn blank and must be handled appropriately.
                if (ATRCore_Utils.IsConsideredMechanicalAndroid(pawn) && MHC_Utils.IsConsideredMechanicalSapient(pawn))
                {
                    ATRCore_Utils.Duplicate(ATRCore_Utils.GetBlank(), pawn, false, false);
                    pawn.health.AddHediff(ATR_HediffDefOf.ATR_IsolatedCore, pawn.health.hediffSet.GetBrain());
                    Hediff target = pawn.health.hediffSet.GetFirstHediffOfDef(ATR_HediffDefOf.ATR_AutonomousCore);
                    if (target != null)
                        pawn.health.RemoveHediff(target);
                    pawn.playerSettings.medCare = MedicalCareCategory.NormalOrWorse;
                    pawn.guest.SetGuestStatus(Faction.OfPlayer);
                    Messages.Message("ATR_NewbootAndroidCreated".Translate(), MessageTypeDefOf.PositiveEvent);

                    // If this is the player's first constructed android, send a letter in case they don't understand how they work.
                    if (!ATRCore_Utils.gameComp.hasBuiltAndroid)
                    {
                        Find.LetterStack.ReceiveLetter("ATR_FirstBlankAndroidCreated".Translate(), "ATR_FirstBlankAndroidCreatedDesc".Translate(), LetterDefOf.NeutralEvent);
                        ATRCore_Utils.gameComp.hasBuiltAndroid = true;
                    }
                }
                else if (MHC_Utils.IsConsideredMechanicalDrone(pawn) && !ATRCore_Utils.gameComp.hasBuiltDrone)
                {
                    Find.LetterStack.ReceiveLetter("ATR_FirstDroneCreated".Translate(), "ATR_FirstDroneCreatedDesc".Translate(), LetterDefOf.NeutralEvent);
                    ATRCore_Utils.gameComp.hasBuiltDrone = true;
                }

                GenSpawn.Spawn(pawn, parent.Position, parent.Map);

                // If the pawn was successfully spawned, return it.
                return pawn;
            }
            catch (Exception ex)
            {
                Log.Warning("[ATR] Error occured while generating/spawning a created pawn. This will leave a dummy Thing in its place! " + ex.Message + ex.StackTrace);
                return null;
            }
        }
    }
}