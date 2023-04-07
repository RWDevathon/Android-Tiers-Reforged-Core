using Verse;
using RimWorld;

namespace ATReforged
{
    public class Alert_FullSkillServers : Alert
    {
        public Alert_FullSkillServers()
        {
            defaultLabel = "ATR_AlertFullSkillServers".Translate();
            defaultExplanation = "ATR_AlertFullSkillServersDesc".Translate();
            defaultPriority = AlertPriority.Medium;
        }


        public override AlertReport GetReport()
        {
            if (!ATReforgedCore_Settings.receiveSkillAlert || ATRCore_Utils.gameComp.GetPointCapacity(ATR_ServerType.SkillServer) <= 0)
                return false;

            if (ATRCore_Utils.gameComp.GetPoints(ATR_ServerType.SkillServer) >= ATRCore_Utils.gameComp.GetPointCapacity(ATR_ServerType.SkillServer) * 0.9f)
            {
                return true;
            }
            return false;
        }
    }
}
