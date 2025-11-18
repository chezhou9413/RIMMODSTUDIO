using AlienRace;
using ChezhouLib.ALLmap;
using ChezhouLib.Extension;
using ChezhouLib.LibDef;
using ChezhouLib.Utils; // 假设 CompUtility 在这里
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace ChezhouLib.Comp
{
    public class HairProData : ThingComp
    {
        public string hairProName; // 这个字段现在可以被保存了

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            Pawn pawn = parent as Pawn;
            if (pawn.kindDef != null)
            {
                kindRule ext = pawn.kindDef.GetModExtension<kindRule>();
                if (ext != null)
                {
                    hairProName = ext.hairProDefName;
                    if (hairProName != null)
                    {
                        return;
                    }
                }
            }
            if (string.IsNullOrEmpty(hairProName))
            {
                // 这部分代码已经很安全了，保持原样
                HairstyleRule hairstyleRule = null;
                defmapDatabase.HairstyleRuleDataBase.TryGetValue(parent.def.defName, out hairstyleRule);

                if (hairstyleRule != null)
                {
                    HairPro hairPro = hairstyleRule.Hair.RandomElement();
                    if (hairPro != null)
                    {
                        hairProName = hairPro.HairProDefName;
                    }
                }
            }
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref hairProName, "hairProName");
        }
    }

    [HarmonyPatch(typeof(PawnComponentsUtility))]
    [HarmonyPatch(nameof(PawnComponentsUtility.CreateInitialComponents))]
    public static class Patch_PawnComponentsUtility_CreateInitialComponents
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn)
        {
            if (pawn == null || pawn.def == null)
                return;

            if (defmapDatabase.HairstyleRuleDataBase.ContainsKey(pawn.def.defName))
            {
                if (!pawn.AllComps.Any(c => c is HairProData))
                    CompUtils.TryAddComp<HairProData>(pawn);
            }
        }
    }


    [HarmonyPatch(typeof(AlienPartGenerator.ExtendedGraphicTop), nameof(AlienPartGenerator.ExtendedGraphicTop.GetPath))]
    public static class Patch_BodyAddon_GetPath2
    {
        private static readonly string[] MultiSuffixes = new[] { "_south", "_north", "_east", "_west" };

        private static bool TextureExists(string path)
        {
            try
            {
                var tex = ContentFinder<Texture2D>.Get(path, reportFailure: false);
                if (tex != null)
                    foreach (string suffix in MultiSuffixes)
                    {
                        var texDir = ContentFinder<Texture2D>.Get(path + suffix, reportFailure: false);
                        if (texDir != null)
                            return true;
                    }
                return false;
            }
            catch
            {
                return false;
            }
        }
        [HarmonyPostfix]
        public static void Postfix(AlienPartGenerator.ExtendedGraphicTop __instance, Pawn pawn, ref string __result)
        {
            if (__instance is AlienPartGenerator.BodyAddon bodyAddon)
            {
                // 您原来的所有逻辑现在都放在这个检查内部
                if (pawn == null)
                    return;
                HairstyleRule hairstyleRule = defmapDatabase.HairstyleRuleDataBase.TryGetValue(pawn.def.defName);
                if (hairstyleRule != null)
                {
                    HairProData hairProData = pawn.GetComp<HairProData>();
                    if (hairProData != null && !string.IsNullOrEmpty(hairProData.hairProName))
                    {
                        HairPro hairPro = hairstyleRule.Hair.FirstOrDefault(p => p.HairProDefName == hairProData.hairProName);
                        foreach (pathSetting setting in hairPro.PathSettings)
                        {
                            if (setting.BodyAddonName == bodyAddon.Name && !string.IsNullOrEmpty(setting.Path))
                            {
                                __result = setting.Path;
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}