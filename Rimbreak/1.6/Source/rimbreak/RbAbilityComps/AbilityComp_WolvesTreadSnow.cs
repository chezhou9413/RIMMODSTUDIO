using HarmonyLib;
using rimbreak.DefRef;
using rimbreak.RbGizmo;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using UnityEngine;

namespace rimbreak.RbAbilityComps
{
    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch("GetGizmos")]
    public static class Patch_Pawn_GetGizmos
    {
        static void Postfix(Pawn __instance, ref IEnumerable<Gizmo> __result)
        {
            if (__instance != null && Find.Selector.NumSelected < 2)
            {
                if (__instance.abilities.GetAbility(RbAbility.lyfe_WolvesTreadSnow, true) != null)
                {
                    // 先取原有 gizmos
                    var list = new List<Gizmo>(__result);

                    // 动态修改该能力按钮的显示文本与描述（开启/关闭）
                    Ability ab = __instance.abilities.GetAbility(RbAbility.lyfe_WolvesTreadSnow, true);
                    if (ab != null)
                    {
                        var comp = ab.comps?.OfType<AbilityComp_WolvesTreadSnow>().FirstOrDefault();
                        if (comp != null)
                        {
                            for (int i = 0; i < list.Count; i++)
                            {
                                if (list[i] is Command_Ability cmd && cmd.Ability == ab)
                                {
                                    if (comp.isOpen)
                                    {
                                        cmd.defaultLabel = "关闭";
                                        cmd.defaultDesc = "关闭技能，开始回复雪息";
                                    }
                                    else
                                    {
                                        cmd.defaultLabel = "开启";
                                        cmd.defaultDesc = "开启技能，开始消耗雪息进行攻击";
                                    }
                                }
                            }
                        }
                    }

                    // 示例：只给殖民者添加
                    if (__instance.IsColonistPlayerControlled)
                    {
                        list.Add(new Gizmo_SnowBreath(__instance));
                    }
                    __result = list;
                }
            }
        }
    }
    public class CompProperties_WolvesTreadSnow : CompProperties_AbilityEffect
    {
        public int maxSnowBreath = 300;
        public int minSnowBreath = 0;
        public string UIName;
        public string UIDes;
        public CompProperties_WolvesTreadSnow()
        {
            compClass = typeof(AbilityComp_WolvesTreadSnow);
        }
    }
    public class AbilityComp_WolvesTreadSnow : AbilityComp
    {
        public int maxSnowBreath = 300;
        public int curentBreath = 300;
        public int minSnowBreath = 0;
        public bool isOpen = false;
        private int tick = 0;
        public CompProperties_WolvesTreadSnow Props => (CompProperties_WolvesTreadSnow)props;

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
                if (caster == null)
                {
                    base.CompTick();
                    return;
                }
                if (!caster.Spawned)
                {
                    base.CompTick();
                    return;
                }
                Map map = caster.Map;
                if (map == null)
                {
                    base.CompTick();
                    return;
                }

                // 关闭状态：回复雪息
                if (!isOpen)
                {
                    // 每秒恢复30点雪息
                    curentBreath = Mathf.Clamp(curentBreath + 30, minSnowBreath, maxSnowBreath);
                    base.CompTick();
                    return;
                }

                // 开启状态且雪息不足：不执行攻击
                if (curentBreath < 25)
                {
                    base.CompTick();
                    isOpen = false;
                    return;
                }

                // 在30格内筛选敌对pawn，可见性优先（带视线判定）
                IntVec3 originCell = caster.Position;
                float maxRadius = 30f;
                Pawn target = null;

                // 选择最近的一个敌对pawn作为目标，避免不稳定的随机性
                float bestDistSq = float.MaxValue;
                foreach (Pawn p in map.mapPawns.AllPawnsSpawned)
                {
                    if (p == null)
                    {
                        continue;
                    }
                    if (p == caster)
                    {
                        continue;
                    }
                    if (p.Dead || !p.Spawned)
                    {
                        continue;
                    }
                    if (!p.HostileTo(caster))
                    {
                        continue;
                    }
                    float distSq = (p.Position - originCell).LengthHorizontalSquared;
                    if (distSq > maxRadius * maxRadius)
                    {
                        continue;
                    }
                    // 需要有视线，减少穿墙发射
                    if (!GenSight.LineOfSight(originCell, p.Position, map, true))
                    {
                        continue;
                    }
                    if (distSq < bestDistSq)
                    {
                        bestDistSq = distSq;
                        target = p;
                    }
                }

                if (target != null)
                {
                    // 取自定义抛射体的ThingDef（与XML中Bullet_StardustMemory一致）
                    ThingDef projDef = DefDatabase<ThingDef>.GetNamed("Bullet_SnowBreath", false);
                    if (projDef != null && projDef.projectile != null)
                    {
                        // 生成并发射抛射体，命中意图为目标单位
                        Projectile projectile = GenSpawn.Spawn(projDef, originCell, map) as Projectile;
                        if (projectile != null)
                        {
                            projectile.Launch(caster, caster.DrawPos, target, target, ProjectileHitFlags.IntendedTarget, false, null, null);
                            curentBreath -= 25;
                            if (curentBreath < minSnowBreath)
                            {
                                curentBreath = minSnowBreath;
                            }
                        }
                    }
                }
            }
            base.CompTick();
        }
        public override void Initialize(AbilityCompProperties props)
        {
            this.props = props;
            maxSnowBreath = Props.maxSnowBreath;
            minSnowBreath = Props.minSnowBreath;
            // 初始化时将当前雪息设置为上限，避免初始为0
            curentBreath = maxSnowBreath;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref maxSnowBreath, "maxSnowBreath", 300);
            // 默认值使用上限，避免读档后重置为0导致看似未开启却在0值状态
            Scribe_Values.Look(ref curentBreath, "curentBreath", maxSnowBreath);
            Scribe_Values.Look(ref minSnowBreath, "minSnowBreath", 0);
            Scribe_Values.Look(ref isOpen, "isOpen", false);
        }
    }
}
