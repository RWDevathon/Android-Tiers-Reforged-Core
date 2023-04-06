using MechHumanlikes;
using RimWorld;
using Verse;

namespace ATReforged
{
    public class ThoughtWorker_PaintedColor_Favorite : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (p.story?.favoriteColor.HasValue == true && MHC_Utils.IsConsideredMechanicalSapient(p))
            {
                return p.story?.favoriteColor.Value == p.story?.SkinColorBase;
            }
            return false;
        }
    }
}
