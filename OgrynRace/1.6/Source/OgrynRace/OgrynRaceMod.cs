using HarmonyLib;
using OgrynRace.DefRef;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OgrynRace
{
    [StaticConstructorOnStartup]
    public static class OgrynRaceStartup
    {
        static Harmony harmony;
        static OgrynRaceStartup()
        {
            harmony = new Harmony("chezhou.Race.OgrynRace");
            harmony.PatchAll(typeof(OgrynRaceStartup).Assembly);
        }
    }
}
