using Verse;
using HarmonyLib;
using System;

namespace ATReforged
{
    public class PawnGenerator_Patch
    {
        // Prefix pawn generation for androids so they have the appropriate gender. This will allow vanilla pawn gen to handle various details like name and body type automatically.
        [HarmonyPatch(typeof(PawnGenerator), "GeneratePawn")]
        [HarmonyPatch(new Type[] { typeof(PawnGenerationRequest) }, new ArgumentType[] { ArgumentType.Normal })]
        public class GeneratePawn_Prefix
        {
            [HarmonyPrefix]
            public static bool Prefix(ref PawnGenerationRequest request)
            {
                ThingDef thingDef = request.KindDef?.race;
                if (thingDef != null && Utils.IsConsideredMechanicalAndroid(thingDef))
                {
                    request.FixedGender = Utils.GenerateGender(request.KindDef);
                }
                // This prefix will always allow vanilla pawn gen to continue
                return true;
            }
        }

        // Patch pawn generation for androids so they have an autonomous core if needed.
        [HarmonyPatch(typeof(PawnGenerator), "GeneratePawn")]
        [HarmonyPatch(new Type[] { typeof(PawnGenerationRequest)}, new ArgumentType[] { ArgumentType.Normal })]
        public class GeneratePawn_Patch
        {
            [HarmonyPostfix]
            public static void Listener(ref Pawn __result)
            {
                try
                {
                    // All androids that need cores (by def extension) receive one by default. Cases where that is not desired can remove it there.
                    if (Utils.IsConsideredMechanicalAndroid(__result) && __result.def.GetModExtension<ATR_AndroidExtension>()?.needsCoreAsAndroid == true)
                    {
                        __result.health.AddHediff(ATR_HediffDefOf.ATR_AutonomousCore, __result.health.hediffSet.GetBrain());
                    }
                }
                catch(Exception ex)
                {
                    Log.Error("[ATR] PawnGenerator.GeneratePawn " + ex.Message + " " + ex.StackTrace);
                }
            }
        }
    }
}