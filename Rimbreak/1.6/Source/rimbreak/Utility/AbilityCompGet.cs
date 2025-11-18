using RimWorld;
using System.Linq;
using Verse;

namespace rimbreak.Utility
{
    public static class AbilityCompGet
    {
        public static T GetAbilityComp<T>(Pawn pawn,AbilityDef abilityDef) where T : AbilityComp
        {
            // 从 Pawn 的能力系统中按名称获取 AbilityDef
            Ability ability = pawn.abilities?.abilities.FirstOrDefault(a => a.def == abilityDef);

            if (ability == null)
                return null; // 没有这个技能

            // 获取该技能上的指定类型的 AbilityComp
            return ability.comps.OfType<T>().FirstOrDefault();
        }
    }
}
