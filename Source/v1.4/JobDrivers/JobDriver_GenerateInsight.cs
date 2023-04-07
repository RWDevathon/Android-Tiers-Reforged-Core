using System.Collections.Generic;
using Verse.AI;
using RimWorld;
using Verse;

namespace ATReforged
{
    public class JobDriver_GenerateInsight : JobDriver
    {
        private const int JobEndInterval = 4000;

        private Building_ResearchBench ResearchBench => (Building_ResearchBench)TargetThingA;
        private CompInsightBench CompInsightBench => ResearchBench.GetComp<CompInsightBench>();

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (pawn.Reserve(ResearchBench, job, 1, -1, null, errorOnFailed))
            {
                if (ResearchBench.def.hasInteractionCell)
                {
                    return pawn.ReserveSittableOrSpot(ResearchBench.InteractionCell, job, errorOnFailed);
                }
                return true;
            }
            return false;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
            Toil generateInsight = ToilMaker.MakeToil("MakeNewToils");
            generateInsight.tickAction = delegate
            {
                Pawn actor = generateInsight.actor;
                float pointsGenerated = 0.008f;
                pointsGenerated *= actor.GetStatValue(StatDefOf.ResearchSpeed);
                pointsGenerated *= TargetThingA.GetStatValue(StatDefOf.ResearchSpeedFactor);
                ATRCore_Utils.gameComp.ChangeServerPoints(pointsGenerated, CompInsightBench.ATR_ServerType);
                actor.skills.Learn(SkillDefOf.Intellectual, 0.1f);
                actor.GainComfortFromCellIfPossible(chairsOnly: true);
            };
            generateInsight.FailOn(() => ResearchBench.GetComp<CompSkyMind>()?.connected != true);
            generateInsight.FailOn(() => CompInsightBench == null);
            generateInsight.FailOn(() => CompInsightBench.ATR_ServerType == ATR_ServerType.None);
            generateInsight.FailOn(() => ATRCore_Utils.gameComp.GetPointCapacity(CompInsightBench.ATR_ServerType) <= ATRCore_Utils.gameComp.GetPoints(CompInsightBench.ATR_ServerType));
            generateInsight.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
            generateInsight.WithEffect(EffecterDefOf.Research, TargetIndex.A);
            generateInsight.WithProgressBar(TargetIndex.A, () => ATRCore_Utils.gameComp.GetPoints(CompInsightBench.ATR_ServerType) / ATRCore_Utils.gameComp.GetPointCapacity(CompInsightBench.ATR_ServerType));
            generateInsight.defaultCompleteMode = ToilCompleteMode.Delay;
            generateInsight.defaultDuration = JobEndInterval;
            generateInsight.activeSkill = () => SkillDefOf.Intellectual;
            yield return generateInsight;
            yield return Toils_General.Wait(2);
        }
    }
}