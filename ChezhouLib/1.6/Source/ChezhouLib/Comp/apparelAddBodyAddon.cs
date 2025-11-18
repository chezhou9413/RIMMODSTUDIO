using AlienRace;
using ChezhouLib.AlienRaceCondition;
using ChezhouLib.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;
using static AlienRace.AlienPartGenerator; // 确保这行 using 存在

namespace ChezhouLib.comp
{
    public class ApparelAddBodyAddonCompProperties : CompProperties
    {
        // 在 XML 中定义这个装备会添加哪些 BodyAddon
        public List<BodyAddon> bodyAddonDefs = new List<BodyAddon>();

        // 在 XML 中定义这些 Addon 只对哪些种族生效
        public List<ThingDef> targetRaces = new List<ThingDef>();

        // 【把这个构造函数加回来！】
        public ApparelAddBodyAddonCompProperties()
        {
            // 告诉 RimWorld 你的 ThingComp 类的名字
            compClass = typeof(ApparelAddBodyAddonComp);
        }
    }

    public class ApparelAddBodyAddonComp : ThingComp
    {

    }
}