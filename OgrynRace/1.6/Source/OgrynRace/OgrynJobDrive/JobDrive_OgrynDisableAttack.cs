using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace OgrynRace.OgrynJobDrive
{
    public class JobDefExtension_OgrynDisableAttack : DefModExtension
    {
        public float Damage;
        public string hediffDef;
    }
    public class JobDrive_OgrynDisableAttack : JobDriver
    {
        public float Damage;
        public string HediffDef;
        public override void Notify_Starting()
        {
            base.Notify_Starting();

            var ext = job.def.GetModExtension<JobDefExtension_OgrynDisableAttack>();
            if (ext != null)
            {
                Damage = ext.Damage;
                HediffDef = ext.hediffDef;
            }
        }
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            // 预留目标（确保不会被其他 pawn 抢）
            return this.pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOn(() => !this.job.targetA.IsValid);

            // 1) 到达目标
            var gotoToil = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return gotoToil;

            // 2) 发起 verb（一次挥砍或一次 TryStartCastOn）
            var attackToil = new Toil
            {
                initAction = () =>
                {
                    Pawn targetThing = job.targetA.Pawn;
                    if (pawn == null || targetThing == null)
                    {
                        return;
                    }
                    Verb verb = pawn.TryGetAttackVerb(targetThing);
                    if (verb != null)
                    {
                        verb.TryStartCastOn(new LocalTargetInfo(targetThing));
                        DamageInfo dinfo = new DamageInfo(
                              DamageDefOf.Blunt,
                              Damage,
                              100,
                              -1,
                              pawn
                          );
                        targetThing.TakeDamage(dinfo);
                        if (HediffDef != null)
                        {
                            HediffDef hediffDef = DefDatabase<HediffDef>.GetNamed(HediffDef);
                            Hediff hediff = HediffMaker.MakeHediff(hediffDef,targetThing);
                            targetThing.health.AddHediff(hediff);
                        }
                     
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = 60 // 给动画/攻击留出时间
            };
            yield return attackToil;
        }
    }
}
