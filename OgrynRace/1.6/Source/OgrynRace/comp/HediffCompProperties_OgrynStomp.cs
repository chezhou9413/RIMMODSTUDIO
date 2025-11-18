using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace OgrynRace.comp
{
    public class HediffCompProperties_OgrynStomp : HediffCompProperties
    {
        public float Damage;
        public int IntervalTick;
        public int Stacks;
        public int Range;
        public string EffMote;
        public string SoundDefName; // 新增音效名

        public HediffCompProperties_OgrynStomp()
        {
            compClass = typeof(HediffComp_OgrynStomp);
        }
    }

    public class HediffComp_OgrynStomp : HediffComp
    {
        private int currentTick;
        private int currentStacks;

        public HediffCompProperties_OgrynStomp Props => (HediffCompProperties_OgrynStomp)props;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            currentTick++;

            var pawn = parent.pawn;
            if (pawn == null || !pawn.Spawned || pawn.Dead)
            {
                TryRemove();
                return;
            }

            // 如果被击倒，也停止技能
            if (pawn.Downed)
            {
                TryRemove();
                return;
            }

            // 间隔控制
            if (currentTick % Props.IntervalTick != 0)
                return;

            // ✅ 播放音效（一次）
            if (!Props.SoundDefName.NullOrEmpty())
            {
                SoundDef.Named(Props.SoundDefName)?.PlayOneShot(SoundInfo.InMap(pawn, MaintenanceType.None));
            }

            // ✅ 播放特效
            if (!Props.EffMote.NullOrEmpty())
            {
                EffecterDef effecterDef = DefDatabase<EffecterDef>.GetNamedSilentFail(Props.EffMote);
                if (effecterDef != null)
                {
                    Effecter effecter = effecterDef.Spawn();
                    IntVec3 pos = pawn.Position;
                    effecter.Trigger(new TargetInfo(pos, pawn.Map), new TargetInfo(pos, pawn.Map));
                    effecter.Cleanup();
                }
            }

            // ✅ 范围伤害
            var hostiles = GenRadial.RadialCellsAround(pawn.Position, Props.Range, true)
                .Where(c => c.InBounds(pawn.Map))
                .SelectMany(c => pawn.Map.thingGrid.ThingsListAt(c))
                .OfType<Pawn>()
                .Where(p => p != pawn && p.Spawned && !p.Dead && (p.Faction != pawn.Faction))
                .ToList();

            foreach (Pawn hostile in hostiles)
            {
                hostile.TakeDamage(new DamageInfo(DamageDefOf.Blunt, Rand.Range(Props.Damage, Props.Damage + 5), 0, -1, pawn));
            }

            currentStacks++;
            if (currentStacks >= Props.Stacks)
            {
                TryRemove();
            }
        }

        private void TryRemove()
        {
            if (parent != null && parent.pawn?.health?.hediffSet != null)
            {
                parent.pawn.health.RemoveHediff(parent);
            }
        }
    }
}
