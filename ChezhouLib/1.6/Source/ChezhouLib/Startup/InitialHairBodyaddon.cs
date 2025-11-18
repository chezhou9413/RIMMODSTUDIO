using AlienRace;
using ChezhouLib.ALLmap;
using ChezhouLib.LibDef;
using System.Collections.Generic;
using UnityEngine; // 确保引用了 UnityEngine
using Verse;
using static AlienRace.AlienPartGenerator;

namespace ChezhouLib.Startup
{
    [StaticConstructorOnStartup]
    public static class InitialHairBodyaddon
    {
        static InitialHairBodyaddon()
        {
            foreach (HairstyleRule HairstyleRuleDef in DefDatabase<HairstyleRule>.AllDefs)
            {
                defmapDatabase.HairstyleRuleDataBase.Add(HairstyleRuleDef.raceDef.defName, HairstyleRuleDef);
                foreach (BodyAddon addon in HairstyleRuleDef.HairBodyaddon)
                {
                    ThingDef_AlienRace raceDef = HairstyleRuleDef.raceDef;
                    if (raceDef == null) continue;
                    if (raceDef.alienRace.generalSettings.alienPartGenerator == null)
                        raceDef.alienRace.generalSettings.alienPartGenerator = new AlienPartGenerator();

                    if (raceDef.alienRace.generalSettings.alienPartGenerator.bodyAddons == null)
                        raceDef.alienRace.generalSettings.alienPartGenerator.bodyAddons = new List<BodyAddon>();

                    var partGenerator = raceDef.alienRace.generalSettings.alienPartGenerator;
                    if (partGenerator.offsetDefaultsDictionary == null)
                    {
                        partGenerator.GenericOffsets();
                        partGenerator.offsetDefaultsDictionary = new Dictionary<string, OffsetNamed>();
                        foreach (OffsetNamed offsetDefault in partGenerator.offsetDefaults)
                        {
                            partGenerator.offsetDefaultsDictionary[offsetDefault.name] = offsetDefault;
                        }
                    }

                    if (partGenerator.offsetDefaultsDictionary.TryGetValue(addon.defaultOffset, out var offsetNamed))
                    {
                        addon.defaultOffsets = offsetNamed.offsets;
                    }
                    else if (partGenerator.offsetDefaultsDictionary.TryGetValue("Center", out var centerOffset))
                    {
                        addon.defaultOffsets = centerOffset.offsets;
                    }
                    else
                    {
                        continue;
                    }

                    List<BodyAddon> raceAddonList = raceDef.alienRace.generalSettings.alienPartGenerator.bodyAddons;

                    if (!raceAddonList.Contains(addon))
                    {
                        raceAddonList.Add(addon);
                    }
                }
                
            }
        }
    }
}