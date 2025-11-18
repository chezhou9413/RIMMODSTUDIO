using HarmonyLib;
using OgrynRace.DefRef;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OgrynRace.Patches
{

    // 对Need_Food类的NeedInterval方法进行Patch
    [HarmonyPatch(typeof(Need_Food), "NeedInterval")]
    public static class Need_Food_NeedInterval_Patch
    {
        const float TicksPerDay = 60000f;
        const float NeedUpdateInterval = 150f;

        // Postfix会在原方法执行后运行
        // __instance: 原始方法的实例对象 (即Need_Food的实例)
        // ___pawn: 原始类中的私有字段'pawn'的引用
        public static void Postfix(Need_Food __instance, Pawn ___pawn)
        {
            if (__instance != null && Find.Selector.NumSelected < 2)
            {
                if (__instance.def.defName == "Ogryn")
                {
                    // 这是我们想要设定的饥饿度减少倍率
                    // 0.5f 代表减少50%的饥饿速度
                    float hungerReductionMultiplier = ___pawn.GetStatValue(ValueDef.chezhouHungerRate);

                    // 原始的饥饿度下降值
                    float originalFall = __instance.def.fallPerDay * (NeedUpdateInterval / TicksPerDay);

                    // 我们自定义的饥饿度下降值
                    float modifiedFall = originalFall * hungerReductionMultiplier;

                    // 补偿原始方法已经减去的饥饿值，然后减去我们修改后的值
                    // 首先，将原始方法减去的饥饿度加回来
                    __instance.CurLevel += originalFall;
                    // 然后，减去我们修改后的数值
                    __instance.CurLevel -= modifiedFall;

                    // 确保饱食度不会超过其最大值 (通常是1)
                    if (__instance.CurLevel > __instance.MaxLevel)
                    {
                        __instance.CurLevel = __instance.MaxLevel;
                    }
                }
            }
        }
    }
}
