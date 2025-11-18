using AlienRace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static AlienRace.AlienPartGenerator;

namespace ChezhouLib.LibDef
{
    public class HairstyleRule:Def
    {
        public ThingDef_AlienRace raceDef;
        public List<BodyAddon> HairBodyaddon = new List<BodyAddon>();
        public List<HairPro> Hair = new List<HairPro>();
    }

    public class HairPro
    {
       public string HairProDefName;
       public List<pathSetting> PathSettings = new List<pathSetting>();
    }

    public class pathSetting
    {
        public string BodyAddonName;
        public string Path;
    }
}
