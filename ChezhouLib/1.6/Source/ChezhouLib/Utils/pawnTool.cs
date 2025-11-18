using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ChezhouLib.Utils
{
    public static class pawnTool
    {
        public static List<Pawn> GetAllPawnsOfRace(string raceDefname)
        {
            // 获取地图上、世界上所有Pawn
            return PawnsFinder.All_AliveOrDead.Where(p => p.def.defName == raceDefname).ToList();
        }

        public static bool HasApparelByDefName(Pawn pawn, string defName)
        {
            return pawn.apparel?.WornApparel?.Any(a => a.def.defName == defName) ?? false;
        }
    }
}
