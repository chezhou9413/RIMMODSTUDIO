using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static AlienRace.AlienPartGenerator;

namespace ChezhouLib
{

    [StaticConstructorOnStartup]
    public class ChezhouLib
    {
        static Harmony harmony;

        static ChezhouLib()
        {
            harmony = new Harmony("ChezhouLib.lib");
            harmony.PatchAll(typeof(ChezhouLib).Assembly);
        }
    }
}
