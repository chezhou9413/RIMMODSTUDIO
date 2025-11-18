using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace OgrynRace.AI
{
    public class JobGiver_AutoOgrynStomp : ThinkNode_JobGiver
    {
        public float radius = 4f;  // 检测范围
        public string abilityDefName = "OGR_Ability_DestructiveStomp"; // 技能Def名

        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.Faction == null)
            {
                return null;
            }
            if (pawn.Faction.IsPlayer)
            {
                return null;
            }
            if (!pawn.def.defName.Equals("Ogryn", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            // 2️⃣ 查找周围敌人
            var enemies = GenRadial.RadialDistinctThingsAround(pawn.Position, pawn.Map, radius, true).OfType<Pawn>().Where(p => !p.Dead && p.Spawned && GenHostility.HostileTo(p, pawn)).ToList();
            if (enemies.Count < 3)
                return null; // 敌人太少，不释放技能

            // 3️⃣ 检查技能是否存在
            Ability ability = pawn.abilities?.AllAbilitiesForReading
                ?.FirstOrDefault(a => a.def.defName == abilityDefName);

            if (ability == null)
            {
                Log.Warning($"[{pawn}] 没有找到技能 {abilityDefName}");
                return null;
            }

            // 4️⃣ 检查技能是否可用
            if (!ability.CanCast)
                return null;

            // 5️⃣ 选择目标（比如最近的敌人）
            var target = enemies
                .OrderBy(e => e.Position.DistanceTo(pawn.Position))
                .FirstOrDefault();

            if (target == null)
                return null;

            // 生成施法任务
            ability.QueueCastingJob(new LocalTargetInfo(pawn), LocalTargetInfo.Invalid);
            return null;
        }
    }
}
