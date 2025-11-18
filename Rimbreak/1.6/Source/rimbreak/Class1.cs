using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace rimbreak
{
    [StaticConstructorOnStartup]
    public static class rimbreakStartup
    {
        static Harmony harmony;

        static rimbreakStartup()
        {
            Log.Message("[rimbreak] 游戏启动，开始初始化 Harmony 和添加组件");

            harmony = new Harmony("rimbreak.core");
            harmony.PatchAll(typeof(rimbreakStartup).Assembly);
        }
    }
}
