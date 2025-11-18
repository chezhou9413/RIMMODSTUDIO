using AlienRace;
using ChezhouLib.AlienRaceCondition;
using ChezhouLib.comp;
using System.Collections.Generic;
using Verse;
using static AlienRace.AlienPartGenerator;

namespace ChezhouLib.Startup
{
    [StaticConstructorOnStartup]
    public static class InitialBodyAddonApparel
    {
        static InitialBodyAddonApparel()
        {
            foreach (ThingDef apparelDef in DefDatabase<ThingDef>.AllDefs)
            {
                var compProps = apparelDef.GetCompProperties<ApparelAddBodyAddonCompProperties>();
                if (compProps == null || compProps.bodyAddonDefs.Count < 1 || compProps.targetRaces.Count < 1)
                {
                    continue;
                }
                foreach (BodyAddon addon in compProps.bodyAddonDefs)
                {
                    addon.conditions.Add(new Condition_HasApparel
                    {
                        apparelDefs = new List<string> { apparelDef.defName }, // 关联到这个装备 Def
                        requireAll = true,
                        invert = false
                    });

                    // 5. 遍历这个装备指定的所有“目标种族”
                    foreach (ThingDef raceDef in compProps.targetRaces)
                    {
                        // 6. 检查这个种族 Def 是不是外星种族
                        if (raceDef is ThingDef_AlienRace alienRaceDef)
                        {
                            // 7. 【安全检查】确保 alienPartGenerator 及其列表存在
                            if (alienRaceDef.alienRace.generalSettings.alienPartGenerator == null)
                            {
                                alienRaceDef.alienRace.generalSettings.alienPartGenerator = new AlienPartGenerator();
                            }
                            if (alienRaceDef.alienRace.generalSettings.alienPartGenerator.bodyAddons == null)
                            {
                                alienRaceDef.alienRace.generalSettings.alienPartGenerator.bodyAddons = new List<BodyAddon>();
                            }
                            var partGenerator = alienRaceDef.alienRace.generalSettings.alienPartGenerator;
                            if (partGenerator.offsetDefaultsDictionary == null)
                            {
                                // 手动调用 AlienRace 的方法来填充偏移量列表
                                partGenerator.GenericOffsets();
                                // 手动填充字典
                                partGenerator.offsetDefaultsDictionary = new Dictionary<string, OffsetNamed>();
                                foreach (OffsetNamed offsetDefault in partGenerator.offsetDefaults)
                                {
                                    partGenerator.offsetDefaultsDictionary[offsetDefault.name] = offsetDefault;
                                }
                            }

                            // 现在，安全地从字典中查找偏移量对象
                            if (partGenerator.offsetDefaultsDictionary.TryGetValue(addon.defaultOffset, out var offsetNamed))
                            {
                                // 【修复】将找到的偏移量对象(offsets)赋值给 addon 的 defaultOffsets 字段
                                addon.defaultOffsets = offsetNamed.offsets;
                            }
                            else
                            {
                                if (partGenerator.offsetDefaultsDictionary.TryGetValue("Center", out var centerOffset))
                                {
                                    addon.defaultOffsets = centerOffset.offsets;
                                }
                                else
                                {
                                    continue; // 跳过这个 Addon 的注入
                                }
                            }
                            List<BodyAddon> raceAddonList = alienRaceDef.alienRace.generalSettings.alienPartGenerator.bodyAddons;
                            if (!raceAddonList.Contains(addon))
                            {
                                raceAddonList.Add(addon);
                            }
                        }
                        else
                        {
                            Log.Warning($"[InitialBodyAddonApparel] 装备 {apparelDef.defName} 尝试向 {raceDef.defName} 注入 BodyAddon, 但 {raceDef.defName} 不是一个 ThingDef_AlienRace。已跳过。");
                        }
                    }
                }
            }
        }
    }
}
