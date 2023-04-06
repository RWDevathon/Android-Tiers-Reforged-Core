using Verse;
using RimWorld;
using System;
using MechHumanlikes;

namespace ATReforged
{
    public class CompSpawnerGeneric : ThingComp
    {
        public CompProperties_GenericSpawner Spawnprops
        {
            get
            {
                return props as CompProperties_GenericSpawner;
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
        public void SpawnPawn()
        {
            try
            {
                PawnGenerationRequest request = new PawnGenerationRequest(Spawnprops.pawnKind, Faction.OfPlayer, PawnGenerationContext.NonPlayer, forceGenerateNewPawn: true, canGeneratePawnRelations: false, allowFood: false, allowAddictions: false, fixedBiologicalAge: 0, fixedChronologicalAge: 0, fixedIdeo: null, forceNoIdeo: true, forceBaselinerChance: 1f);
                Pawn pawn = PawnGenerator.GeneratePawn(request);

                // Pawns may sometimes spawn with apparel somewhere in the generation process. Ensure they don't actually spawn with any - if they even can have apparel.
                pawn.apparel?.DestroyAll();

                // If the pawn is a sapient android, they will spawn blank and must be handled appropriately.
                if (Utils.IsConsideredMechanicalAndroid(pawn) && MHC_Utils.IsConsideredMechanicalSapient(pawn))
                {
                    Utils.Duplicate(Utils.GetBlank(), pawn, false, false);
                    pawn.health.AddHediff(ATR_HediffDefOf.ATR_IsolatedCore, pawn.health.hediffSet.GetBrain());
                    Hediff target = pawn.health.hediffSet.GetFirstHediffOfDef(ATR_HediffDefOf.ATR_AutonomousCore);
                    if (target != null)
                        pawn.health.RemoveHediff(target);
                    pawn.playerSettings.medCare = MedicalCareCategory.NormalOrWorse;
                    pawn.guest.SetGuestStatus(Faction.OfPlayer);
                    Messages.Message("ATR_NewbootAndroidCreated".Translate(), MessageTypeDefOf.PositiveEvent);

                    // If this is the player's first constructed android, send a letter in case they don't understand how they work.
                    if (!Utils.gameComp.hasBuiltAndroid)
                    {
                        Find.LetterStack.ReceiveLetter("ATR_FirstBlankAndroidCreated".Translate(), "ATR_FirstBlankAndroidCreatedDesc".Translate(), LetterDefOf.NeutralEvent);
                        Utils.gameComp.hasBuiltAndroid = true;
                    }
                }
                else if (MHC_Utils.IsConsideredMechanicalDrone(pawn) && !Utils.gameComp.hasBuiltDrone)
                {
                    Find.LetterStack.ReceiveLetter("ATR_FirstDroneCreated".Translate(), "ATR_FirstDroneCreatedDesc".Translate(), LetterDefOf.NeutralEvent);
                    Utils.gameComp.hasBuiltDrone = true;
                }

                GenSpawn.Spawn(pawn, parent.Position, parent.Map);
            }
            catch (Exception ex)
            {
                Log.Warning("[ATR] Error occured while generating/spawning a created pawn. This will leave a dummy Thing in its place! " + ex.Message + ex.StackTrace);
            }
        }
    }
}