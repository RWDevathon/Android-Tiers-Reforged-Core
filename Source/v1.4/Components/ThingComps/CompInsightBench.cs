using Verse;
using RimWorld;
using System.Collections.Generic;

namespace ATReforged
{
    public class CompInsightBench : ThingComp
    {
        public ATR_ServerType ATR_ServerType
        {
            get
            {
                return serverMode;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look(ref serverMode, "ATR_serverMode", ATR_ServerType.SkillServer);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            building = (Building)parent;
            networkConnection = parent.GetComp<CompSkyMind>();
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (!parent.GetComp<CompPowerTrader>().PowerOn || networkConnection?.connected == false)
                yield break;

            // Generate button to switch server mode based on which servermode the server is currently in.
            switch (serverMode)
            {
                case ATR_ServerType.None:
                    yield break;
                case ATR_ServerType.SkillServer:
                    yield return new Command_Action
                    { // In Skill Mode, can switch to Security
                        icon = ATRCore_Textures.SkillIcon,
                        defaultLabel = "ATR_SkillMode".Translate(),
                        defaultDesc = "ATR_SkillModeDesc".Translate(),
                        action = delegate ()
                        {
                            serverMode = ATR_ServerType.SecurityServer;
                        }
                    };
                    break;
                case ATR_ServerType.SecurityServer:
                    yield return new Command_Action
                    { // In Security Mode, can switch to Hacking
                        icon = ATRCore_Textures.SecurityIcon,
                        defaultLabel = "ATR_SecurityMode".Translate(),
                        defaultDesc = "ATR_SecurityModeDesc".Translate(),
                        action = delegate ()
                        {
                            serverMode = ATR_ServerType.HackingServer;
                        }
                    };
                    break;
                case ATR_ServerType.HackingServer:
                    yield return new Command_Action
                    { // In Hacking Mode, can switch to Skill
                        icon = ATRCore_Textures.HackingIcon,
                        defaultLabel = "ATR_HackingMode".Translate(),
                        defaultDesc = "ATR_HackingModeDesc".Translate(),
                        action = delegate ()
                        {
                            serverMode = ATR_ServerType.SkillServer;
                        }
                    };

                    // Servers in hacking mode allow access to the hacking menu for deploying a hack.
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
                    break;
                default:
                    yield return new Command_Action
                    { // In an illegal Mode, can switch to Skill
                        icon = ATRCore_Textures.SkillIcon,
                        defaultLabel = "ATR_SwitchToSkillMode".Translate(),
                        defaultDesc = "ATR_SwitchToSkillModeDesc".Translate(),
                        action = delegate ()
                        {
                            serverMode = ATR_ServerType.SkillServer;
                        }
                    };
                    break;
            }
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);

            // No building can be connected to the network when despawned.
            if (networkConnection?.connected == true)
            {
                ATRCore_Utils.gameComp.DisconnectFromSkyMind(building);
            }
        }

        private CompSkyMind networkConnection;
        private Building building;
        private ATR_ServerType serverMode = ATR_ServerType.SkillServer;
    }
}