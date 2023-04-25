using System.Collections.Generic;
using Verse;
using RimWorld;
using MechHumanlikes;
using SkyMind;

namespace ATReforged
{
    public class Recipe_InstallAndroidReceiver : Recipe_InstallMechanicalPart
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

            Hediff hediff = HediffMaker.MakeHediff(recipe.addsHediff, pawn, part);
            SMN_Utils.TurnIntoSurrogate(pawn, hediff, part, false);

            // Remove the isolated core hediff.
            Hediff target = pawn.health.hediffSet.GetFirstHediffOfDef(ATR_HediffDefOf.ATR_IsolatedCore);
            if (target != null)
            {
                pawn.health.RemoveHediff(target);
            }
        }
    }
}

