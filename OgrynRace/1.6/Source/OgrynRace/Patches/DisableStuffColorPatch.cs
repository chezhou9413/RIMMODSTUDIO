using HarmonyLib;
using OgrynRace.comp;
using UnityEngine;
using Verse;

namespace OgrynRace.Patches
{
    /// <summary>
    /// 禁用材料染色的Harmony补丁
    /// 当装备有CompDisableStuffColor组件时，强制返回装备定义的颜色
    /// </summary>
    [HarmonyPatch(typeof(BuildableDef), "GetColorForStuff")]
    public static class DisableStuffColorPatch
    {
        /// <summary>
        /// 拦截GetColorForStuff方法
        /// 如果装备有CompDisableStuffColor组件，返回graphicData的颜色而不是材料颜色
        /// </summary>
        static bool Prefix(ref Color __result, BuildableDef __instance, ThingDef stuff)
        {
            // 检查是否是ThingDef（装备）
            if (__instance is ThingDef thingDef)
            {
                // 如果有材料
                if (stuff != null && thingDef.MadeFromStuff)
                {
                    // 检查这个装备是否有禁用材料染色组件
                    var compProperties = thingDef.GetCompProperties<CompProperties_DisableStuffColor>();
                    if (compProperties != null)
                    {
                        // 如果有禁用材料染色组件，返回装备自身的颜色
                        if (thingDef.graphicData != null)
                        {
                            __result = thingDef.graphicData.color;
                        }
                        else
                        {
                            __result = Color.white;
                        }
                        return false; // 跳过原始方法
                    }
                }
            }
            
            return true; // 执行原始方法
        }
    }
}

