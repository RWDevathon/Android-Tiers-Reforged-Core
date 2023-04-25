using HarmonyLib;
using System.Reflection;
using Verse;

namespace SkyMind
{
    public class ATRCore_SMN_Compatibility : Mod
    {
        public ATRCore_SMN_Compatibility(ModContentPack content) : base(content)
        {
        }
    }

    [StaticConstructorOnStartup]
    public static class ATRCore_SMN_Compatibility_PostInit
    {
        static ATRCore_SMN_Compatibility_PostInit()
        {
            new Harmony("ATRCore SMN Compatibility").PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}