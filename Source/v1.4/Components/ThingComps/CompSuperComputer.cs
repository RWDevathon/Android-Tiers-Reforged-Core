using System.Collections.Generic;
using System.Text;
using Verse;

namespace ATReforged
{
    public class CompSuperComputer : ThingComp
    {
        public CompProperties_SuperComputer Props
        {
            get
            {
                return (CompProperties_SuperComputer)props;
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            building = (Building)parent;

            // The server lists need to know how much storage and point generation exists for each server mode. This adds it to all three types.
            if (!respawningAfterLoad)
                ATRCore_Utils.gameComp.AddServer(building);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            // Servers in hacking mode allow access to the hacking menu for deploying a hack. Supercomputers always enable hacking mode, so they always have the gizmo to open the menu.
            if (ATReforgedCore_Settings.playerCanHack)
            {
                yield return new Command_Action
                {
                    icon = ATRCore_Textures.HackingWindowIcon,
                    defaultLabel = "ATR_HackingWindow".Translate(),
                    defaultDesc = "ATR_HackingWindowDesc".Translate(),
                    action = delegate ()
                    {
                        Find.WindowStack.Add(new Dialog_HackingWindow());
                    }
                };
            }
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder ret = new StringBuilder();

            ret.AppendLine("ATR_SkillServersSynthesis".Translate(ATRCore_Utils.gameComp.GetPoints(ATR_ServerType.SkillServer), ATRCore_Utils.gameComp.GetPointCapacity(ATR_ServerType.SkillServer)))
               .AppendLine("ATR_SecurityServersSynthesis".Translate(ATRCore_Utils.gameComp.GetPoints(ATR_ServerType.SecurityServer), ATRCore_Utils.gameComp.GetPointCapacity(ATR_ServerType.SecurityServer)))
               .AppendLine("ATR_HackingServersSynthesis".Translate(ATRCore_Utils.gameComp.GetPoints(ATR_ServerType.HackingServer), ATRCore_Utils.gameComp.GetPointCapacity(ATR_ServerType.HackingServer)))
               .AppendLine("ATR_SkillProducedPoints".Translate(Props.passivePointGeneration))
               .AppendLine("ATR_SecurityProducedPoints".Translate(Props.passivePointGeneration))
               .AppendLine("ATR_HackingProducedPoints".Translate(Props.passivePointGeneration))
               .AppendLine("ATR_SkillSlotsAdded".Translate(Props.pointStorage))
               .AppendLine("ATR_SecuritySlotsAdded".Translate(Props.pointStorage))
               .Append("ATR_HackingSlotsAdded".Translate(Props.pointStorage));
            return ret.Append(base.CompInspectStringExtra()).ToString();
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);

            // The server lists need to know how much storage exists for each server mode. This removes it from all three types.
            ATRCore_Utils.gameComp.RemoveServer(building);
        }

        private Building building = null;
    }
}
