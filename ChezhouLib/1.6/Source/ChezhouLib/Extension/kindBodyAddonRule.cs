using AlienRace;
using ChezhouLib.ALLmap;
using ChezhouLib.Comp;
using ChezhouLib.LibDef;
using ChezhouLib.Utils;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace ChezhouLib.Extension
{
    public class kindRule : DefModExtension
    {
        public string kindName;
        public string hairProDefName;
        public List<string> traitName = new List<string>();
        public List<GeneratorBodyAddonRule> bodyAddonRules = new List<GeneratorBodyAddonRule>();
    }

    public class GeneratorBodyAddonRule
    {
        public string BodyAddonName;
        public string ReplacePath;
    }

    [HarmonyPatch(typeof(PawnGenerator))]
    [HarmonyPatch(nameof(PawnGenerator.GeneratePawn), new Type[] { typeof(PawnGenerationRequest) })]
    public static class Patch_PawnGenerator_GeneratePawn
    {
        [HarmonyPostfix]
        public static void Postfix(ref Pawn __result, PawnGenerationRequest request)
        {
            try
            {
                if (__result == null || __result.kindDef == null)
                    return;

                kindRule ext = __result.kindDef.GetModExtension<kindRule>();
                if (ext == null)
                    return;
                if (ext.kindName != null)
                {
                    CompUtils.TryAddComp<DelayedNameChangerComp>(__result);
                }
                if (ext.traitName.Count > 0)
                {
                    __result.story.traits.allTraits.Clear();
                    foreach (string trait in ext.traitName)
                    {
                        TraitDef def = DefDatabase<TraitDef>.GetNamedSilentFail(trait);
                        if (def != null)
                        {
                            __result.story.traits.GainTrait(new Trait(def));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"[ChezhouLib] 修改Pawn名字时出错: {e}");
            }
        }
    }

    [HarmonyPatch(typeof(AlienPartGenerator.ExtendedGraphicTop), nameof(AlienPartGenerator.ExtendedGraphicTop.GetPath))]
    public static class Patch_BodyAddon_GetPath
    {
        [HarmonyPostfix]
        // 改动 2: 更新 __instance 的类型，并在方法内部进行安全的类型转换
        public static void Postfix(AlienPartGenerator.ExtendedGraphicTop __instance, Pawn pawn, ref string __result)
        {
            // 因为我们修补的是基类，所以需要确保当前处理的实例确实是一个 BodyAddon
            // 这个 if 判断非常重要，可以避免我们的逻辑错误地应用到其他子类上
            if (__instance is AlienPartGenerator.BodyAddon bodyAddon)
            {
                // 您原来的所有逻辑现在都放在这个检查内部
                if (pawn == null || pawn.kindDef == null)
                    return;
                HairstyleRule hairstyleRule = defmapDatabase.HairstyleRuleDataBase.TryGetValue(pawn.def.defName);
                if (hairstyleRule != null)
                {
                    if (hairstyleRule.HairBodyaddon.Any(addon => addon.Name != null && addon.Name.Contains(bodyAddon.Name)))
                    {
                        return;
                    }
                }
                kindRule ext = pawn.kindDef.GetModExtension<kindRule>();
                if (ext == null || ext.bodyAddonRules.NullOrEmpty())
                    return;
                foreach (GeneratorBodyAddonRule rule in ext.bodyAddonRules)
                {
                    // 使用我们转换后得到的 bodyAddon 变量来访问 Name 属性
                    if (bodyAddon.Name == rule.BodyAddonName)
                    {
                        // 4. 如果匹配，就用规则中的新路径覆盖原始的返回结果
                        if (!string.IsNullOrEmpty(rule.ReplacePath))
                        {
                            // 使用 ref __result，我们直接修改了方法的返回值
                            __result = rule.ReplacePath;
                            Log.Error(__result);
                            // 既然已经找到了匹配的规则并替换了，就可以提前退出了
                            return;
                        }
                        else
                        {
                            Log.Error(__result);
                        }
                    }
                }
            }
        }
    }
}
