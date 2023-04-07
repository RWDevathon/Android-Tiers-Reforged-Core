using Verse;

namespace ATReforged
{
    public class CompProperties_PawnSpawner : CompProperties
    {
        public CompProperties_PawnSpawner()
        {
            compClass = typeof(CompPawnSpawner);
        }

        public PawnKindDef pawnKind;
    }
}