using System.Collections.Generic;
using Verse;
using RimWorld;
using MechHumanlikes;

namespace ATReforged
{
    public class Recipe_InstallAndroidInterface : Recipe_InstallMechanicalPart
    {
        // This recipe is specifically targetting the brain of an android, so we only need to check if the brain is available (a slight optimization over checking fixed body parts).
        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        { 

            BodyPartRecord targetBodyPart = pawn.health.hediffSet.GetBrain();
            if (targetBodyPart != null && pawn.health.hediffSet.HasHediff(ATR_HediffDefOf.ATR_IsolatedCore))
            {
                yield return targetBodyPart;
            }
            yield break;
        }

        // Install the part as normal, and then handle additional specifications.
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        { 
            base.ApplyOnPawn(pawn, part, billDoer, ingredients, bill);

            // There are special considerations for adding the core itself. Adding a core makes a new intelligence. Adding any core removes the "Isolated Core" hediff.
            if (recipe.addsHediff == ATR_HediffDefOf.ATR_AutonomousCore)
            {
                InitializeMind(pawn);
            }

            // Remove the isolated core hediff.
            Hediff target = pawn.health.hediffSet.GetFirstHediffOfDef(ATR_HediffDefOf.ATR_IsolatedCore);
            if (target != null)
            {
                pawn.health.RemoveHediff(target);
            }
        }

        public void InitializeMind(Pawn pawn)
        {
            PawnGenerationRequest request = new PawnGenerationRequest(pawn.kindDef, Faction.OfPlayer, forceGenerateNewPawn: true, canGeneratePawnRelations: false, allowAddictions: false, fixedBiologicalAge: 50, forceNoIdeo: true, colonistRelationChanceFactor: 0, forceBaselinerChance: 1f);
            Pawn personality = PawnGenerator.GeneratePawn(request);
            personality.story.Childhood = ATR_BackstoryDefOf.ATR_NewbootChildhood;
            personality.story.Adulthood = ATR_BackstoryDefOf.ATR_NewbootAdulthood;
            ATRCore_Utils.Duplicate(personality, pawn, false, false);
            Hediff rebootHediff = pawn.health.hediffSet.GetFirstHediffOfDef(MHC_HediffDefOf.MHC_Restarting);
            if (rebootHediff == null)
            {
                rebootHediff = HediffMaker.MakeHediff(MHC_HediffDefOf.MHC_Restarting, pawn, null);
                pawn.health.AddHediff(rebootHediff);
            }
            rebootHediff.Severity = 1;

            // Allow the player to pick a few passions and a trait for the new android, akin to child growth moments in Biotech.
            if (ModLister.BiotechInstalled)
            {
                ChoiceLetter_PersonalityShift choiceLetter = (ChoiceLetter_PersonalityShift)LetterMaker.MakeLetter(ATR_LetterDefOf.ATR_PersonalityShiftLetter);
                choiceLetter.ConfigureChoiceLetter(pawn, 3, 3, false, false);
                choiceLetter.Label = "ATR_PersonalityShiftNewboot".Translate();
                choiceLetter.OpenLetter();
            }
        }
    }
}

