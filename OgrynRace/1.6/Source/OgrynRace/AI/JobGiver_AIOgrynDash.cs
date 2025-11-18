using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace OgrynRace.AI
{
    // 让AI自动寻找敌人并施放 OGR_Ability_OgrynDashAbility 的 JobGiver
    public class JobGiver_AIOgrynDash : ThinkNode_JobGiver
    {
        private static readonly HashSet<Pawn> ActiveDashTargets = new HashSet<Pawn>();
        // 定义一个事件。Action<Pawn> 是一个委托，表示这个事件在触发时
        // 会传递一个 Pawn 类型的参数（也就是完成冲刺的那个 Pawn）。
        public static event Action<Pawn> OnOgrynDashCompleted;

        // 触发事件的方法。我们添加一个空值检查（?.），
        // 确保在没有任何对象订阅事件时调用它不会出错。
        public static void RaiseOgrynDashCompleted(Pawn pawn)
        {
            if (pawn == null) return;

            // 1. 从集合中移除该 Pawn (清除自身)
            // HashSet.Remove 方法在目标不存在时不会报错，所以这很安全。
            ActiveDashTargets.Remove(pawn);
            OnOgrynDashCompleted?.Invoke(pawn);
        }

        // 运行期获取，避免在Defs尚未加载时静态初始化为null
        private static AbilityDef GetOgrynDashAbilityDef()
        {
            return DefDatabase<AbilityDef>.GetNamed("OGR_Ability_OgrynDashAbility");
        }

        // 主入口：尝试给当前Pawn生成一个施放技能的Job
        protected override Job TryGiveJob(Pawn pawn)
        {
            // 基本防御：无能力定义或Pawn无能力系统直接返回
            AbilityDef abilityDef = GetOgrynDashAbilityDef();
            if (abilityDef == null)
            {
                return null;
            }
            if (pawn.Faction == null)
            {
                return null;
            }
            if (pawn.Faction.IsPlayer)
            {
                return null;
            }
            // 3️⃣ 限定种族（通过 defName 判断；可换成更严格的基因或 ThingDef 检查）
            if (!pawn.def.defName.Equals("Ogryn", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            // 确保人形单位具备能力系统；若缺失则初始化
            if (pawn.abilities == null)
            {
                try
                {
                    pawn.abilities = new Pawn_AbilityTracker(pawn);
                }
                catch
                {
                    return null;
                }
            }
            if (pawn.abilities == null)
            {
                return null;
            }

            // 取得该技能的实例，确认冷却与可施放性
            Ability ability = pawn.abilities.GetAbility(abilityDef);

            // 检查是否可以施放（包括冷却/资源/姿态等）
            if (!CanCastNow(pawn, ability))
            {
                return null;
            }
            Pawn target = FindUnlockedEnemyTarget(pawn, ability);
            if (target == null)
                return null;

            // 锁定目标（防止别的AI选中同一个敌人）
            ActiveDashTargets.Add(target);

            // 直接下达自定义冲刺Job，绕过“施放能力”Job对AI可能的限制
            Job dashJob = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("Ogryn_Dash_Job", true), target);
            // 为防连续刷屏，在这里手动进入冷却（与动词内部一致）
            try
            {
                int cd = ability.def?.cooldownTicksRange.TrueMin == 0 && ability.def?.cooldownTicksRange.TrueMax == 0
                    ? 0
                    : ability.def.cooldownTicksRange.RandomInRange;
                if (cd > 0)
                {
                    ability.StartCooldown(cd);
                }
            }
            catch
            {
                // 忽略冷却异常，仍然返回Job
                Log.Message("[欧格林AI] 冷却处理异常，已忽略");
            }
            return dashJob;
        }

        // 判断能力当前是否可施放
        private static bool CanCastNow(Pawn caster, Ability ability)
        {
            // 早退：死亡/下落/被束缚等不可行动状态
            if (caster.Dead || caster.Downed || caster.InMentalState)
            {
                return false;
            }

            // 核心校验：能力不在冷却、能量/条件满足等
            try
            {
                // 注意：GizmoDisabled 仅用于玩家UI的禁用原因，不应限制AI
                // AI只需确认能力存在、可施放且无冷却
                if (ability == null)
                {
                    return false;
                }

                if (!ability.CanCast)
                {
                    return false;
                }

                // 冷却为0即可
                if (ability.CooldownTicksRemaining > 0)
                {
                    return false;
                }

                return true;
            }
            catch
            {
                Log.Message("[欧格林AI] 可施放校验异常");
                return false;
            }
        }
        // 查找“未被锁定”的敌人
        private static Pawn FindUnlockedEnemyTarget(Pawn pawn, Ability ability)
        {
            if (pawn?.Map == null)
                return null;

            float range = ability?.def?.verbProperties?.range ?? 30f;

            // 先过滤可攻击的敌方单位
            var candidates = pawn.Map.mapPawns.AllPawnsSpawned
                .Where(t =>
                    t is Pawn p &&
                    p.HostileTo(pawn) &&
                    p.Spawned &&
                    !p.Dead &&
                    !p.Downed &&
                    !ActiveDashTargets.Contains(p) && //未被锁定
                    pawn.Position.DistanceTo(p.Position) <= range
                ).Cast<Pawn>()
                .ToList();

            if (candidates.Count == 0)
                return null;

            // 随机化选择，避免多个AI选择同一个敌人
            return candidates.RandomElement();
        }
    }
}


