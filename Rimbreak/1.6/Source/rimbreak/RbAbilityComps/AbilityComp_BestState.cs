using HarmonyLib;
using rimbreak.DefRef;
using rimbreak.RbGizmo;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace rimbreak.RbAbilityComps
{
    [HarmonyPatch(typeof(StatWorker), nameof(StatWorker.GetValueUnfinalized))]
    public static class StatWorker_GetValueUnfinalized_Patch
    {
        public static void Postfix(StatWorker __instance, StatRequest req, ref float __result)
        {
            if (req.HasThing && req.Thing is Pawn pawn)
            {
                // 使用 Harmony 的 Traverse 访问受保护的 stat 字段
                StatDef statDef = Traverse.Create(__instance).Field("stat").GetValue<StatDef>();
                
                if (statDef == StatDefOf.RangedWeapon_Cooldown || statDef == StatDefOf.AimingDelayFactor)
                {
                    AbilityComp_BestState comp = pawn.abilities?.GetAbility(RbAbility.fenny_BestState, true)?.comps?.OfType<AbilityComp_BestState>().FirstOrDefault();
                    if (comp != null)
                    {                      
                        float reduction = comp.curentFavor * 0.01f;
                        __result *= (1f - reduction);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Thing), nameof(Thing.TakeDamage))]
    public static class Thing_TakeDamage_Patch
    {
        public static void Prefix(Thing __instance, ref DamageInfo dinfo)
        {
            if (__instance is Pawn victim)
            {
                Thing instigator = dinfo.Instigator;

                if (instigator != null)
                {
                    if (instigator is Pawn attackerPawn)
                    {
                        AbilityComp_BestState comp = attackerPawn.abilities?.GetAbility(RbAbility.fenny_BestState, true)?.comps?.OfType<AbilityComp_BestState>().FirstOrDefault();
                        if(comp != null)
                        {
                            if (comp.curentFavor < comp.maxFavor)
                            {
                                comp.curentFavor += 1.2f;
                                if(comp.curentFavor > comp.maxFavor)
                                {
                                    comp.curentFavor = comp.maxFavor;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch("GetGizmos")]
    public static class Patch_BestState_GetGizmos
    {
        static void Postfix(Pawn __instance, ref IEnumerable<Gizmo> __result)
        {
            if (__instance != null && Find.Selector.NumSelected < 2)
            {
                if (__instance.abilities.GetAbility(RbAbility.fenny_BestState, true) != null)
                {
                    // 先取原有 gizmos
                    var list = new List<Gizmo>(__result);
                    Hediff BriarCrown = __instance.health.hediffSet.GetFirstHediffOfDef(RbHediffDef.RB_BriarCrown);
                    if (BriarCrown != null)
                    {
                        if (__instance.IsColonistPlayerControlled)
                        {
                            list.Add(new Gizmo_Favor(__instance));
                        }
                        __result = list;
                    }
                }
            }
        }
    }

    public class CompProperties_BestState : CompProperties_AbilityEffect
    {
        public float maxFavor = 80;
        public float minFavor = 0;
        public string UIName;
        public string UIDes;
        public CompProperties_BestState()
        {
            compClass = typeof(AbilityComp_BestState);
        }
    }
    public class AbilityComp_BestState : AbilityComp
    {
        public float maxFavor = 80;
        public float curentFavor = 80;
        public float minFavor = 0;
        private float tick = 0;
        public CompProperties_BestState Props => (CompProperties_BestState)props;

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            // 注意：CompGetGizmosExtra 仅用于追加“额外”Gizmo，
            // 绝不能去调用 parent.GetGizmos()（会导致递归与重复绘制）。
            yield break;
        }
        public override void CompTick()
        {
            tick++;
            // 每60tick执行一次：根据开启状态进行攻击或回复
            if (tick % 60 == 0)
            {
                // 拿到施法者
                Pawn caster = parent?.pawn;
                Hediff BriarCrownHediff = caster.health.hediffSet.GetFirstHediffOfDef(RbHediffDef.RB_BriarCrown);
                if (BriarCrownHediff != null)
                {
                    if (curentFavor > minFavor)
                    {
                        curentFavor -= 0.5f;
                    }
                    if (curentFavor <= 0)
                    {
                        caster.health.RemoveHediff(BriarCrownHediff);
                    }
                }
            }
            base.CompTick();
        }
        public override void Initialize(AbilityCompProperties props)
        {
            this.props = props;
            maxFavor = Props.maxFavor;
            minFavor = Props.minFavor;
            // 初始化时将当前雪息设置为上限，避免初始为0
            curentFavor = maxFavor;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref maxFavor, "maxFavor", 80);
            // 默认值使用上限，避免读档后重置为0导致看似未开启却在0值状态
            Scribe_Values.Look(ref curentFavor, "curentFavor", maxFavor);
            Scribe_Values.Look(ref minFavor, "minFavor", 0);
        }
    }
}
