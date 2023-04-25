using Verse;
using HarmonyLib;
using RimWorld.QuestGen;
using RimWorld;
using System.Collections.Generic;
using System;

namespace ATReforged
{
    public class QuestGen_Pawns_Patch
    {
        // Quest pawns for android factions should use their faction's default pawn kind def. This prefix modifies the pawn kind and allows the code to resume.
        [HarmonyPatch(typeof(QuestGen_Pawns), "GeneratePawn")]
        [HarmonyPatch(new Type[] { typeof(Quest), typeof(PawnKindDef), typeof(Faction), typeof(bool), typeof(IEnumerable<TraitDef>), typeof(float), typeof(bool), typeof(Pawn), typeof(float), typeof(float), typeof(bool), typeof(bool), typeof(DevelopmentalStage), typeof(bool) })]
        public class GeneratePawn_Patch
        {
            [HarmonyPrefix]
            public static bool Prefix(ref PawnKindDef kindDef, Faction faction)
            {
                if (kindDef == PawnKindDefOf.SpaceRefugee || kindDef == PawnKindDefOf.Refugee)
                {
                    // Factionless quest pawns for android faction players should be androids.
                    if (faction == null && Faction.OfPlayer.def.GetModExtension<ATR_FactionAndroidExtension>() is ATR_FactionAndroidExtension playerFactionExtension && playerFactionExtension.membersShouldBeAndroids)
                    {
                        kindDef = Faction.OfPlayer.def.basicMemberKind;
                    }
                    // Quest pawns that are members of an android faction should be androids.
                    else if (faction != null && faction.def.GetModExtension<ATR_FactionAndroidExtension>() is ATR_FactionAndroidExtension factionExtension && factionExtension.membersShouldBeAndroids)
                    {
                        kindDef = faction.def.basicMemberKind;
                    }
                }
                return true;
            }
        }
    }
}