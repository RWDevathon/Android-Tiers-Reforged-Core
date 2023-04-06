using Verse;
using HarmonyLib;

namespace ATReforged
{
    public class Pawn_Patch
    {
        // Some things explode when they die and are destroyed instantly.
        [HarmonyPatch(typeof(Pawn), "Kill")]
        public static class Kill_Patch
        {
            [HarmonyPrefix]
            public static bool Listener(ref Pawn __instance, DamageInfo? dinfo, Hediff exactCulprit = null)
            {
                ATR_DetonateOnIncapacitation detonationExtension = __instance.def.GetModExtension<ATR_DetonateOnIncapacitation>();
                if (detonationExtension == null)
                {
                    return true;
                }

                // Save details and destroy before doing the explosion to avoid the damage hitting the pawn, killing them again.
                if (!__instance.Destroyed)
                {
                    IntVec3 tempPos = __instance.Position;
                    Map tempMap = __instance.Map;
                    __instance.Destroy();
                    GenExplosion.DoExplosion(tempPos, tempMap, detonationExtension.explosionRadius, detonationExtension.damageType, __instance, detonationExtension.damageAmount, postExplosionSpawnThingDef: detonationExtension.itemToSpawnOnDetonation, postExplosionSpawnChance: detonationExtension.itemToSpawnOnDetonation != null ? 1 : 0, postExplosionSpawnThingCount: detonationExtension.itemCountToSpawn);
                    return false;
                }
                return true;
            }
        }
    }
}