using rimbreak.Utility;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace rimbreak.RbProjectile
{

    public class Projectile_PaidJustice : Projectile
    {
        private Mote attachedMote = null;
        private static readonly List<Thing> tmpThings = new List<Thing>(16);

        public override void Launch(Thing launcher, Vector3 origin, LocalTargetInfo usedTarget, LocalTargetInfo intendedTarget, ProjectileHitFlags hitFlags, bool preventFriendlyFire = false, Thing equipment = null, ThingDef targetCoverDef = null)
        {
            // 调用基类Launch方法
            base.Launch(launcher, origin, usedTarget, intendedTarget, hitFlags, preventFriendlyFire, equipment, targetCoverDef);
            // 在Launch后创建特效
            CreateMoteEffects();
        }

        public void LaunchWithMote(Thing launcher, Vector3 origin, LocalTargetInfo target, Thing equipment = null)
        {
            // 使用新的 RimWorld 1.6 Launch 方法签名
            base.Launch(
                launcher,
                origin,
                target,       // usedTarget
                target,       // intendedTarget
                ProjectileHitFlags.All,
                false,
                equipment
            );
            // 创建特效
            CreateMoteEffects();
        }

        private void CreateMoteEffects()
        {
            // 创建附着特效
            attachedMote = MoteUtility.TryAttachMote(this, this.Map, "PaidJusticeMoteEff", 1.0f);
        }

        protected override void Tick()
        {
            base.Tick();

            // 更新附着Mote的位置和旋转，让它跟随抛射体
            if (attachedMote != null && !attachedMote.Destroyed)
            {
                attachedMote.exactPosition = this.ExactPosition;
                attachedMote.exactRotation = this.ExactRotation.eulerAngles.y;
            }

            // 飞行过程中持续对自身所在格与左右相邻格造成伤害
            TryApplyTrailDamage();
        }

        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {

            // 销毁附着的Mote
            if (attachedMote != null && !attachedMote.Destroyed)
            {
                attachedMote.Destroy();
                attachedMote = null;
            }
            base.Impact(hitThing, blockedByShield);
        }

        // 对抛射体当前位置以及左右相邻格应用伤害
        private void TryApplyTrailDamage()
        {
            Map map = this.Map;
            if (map == null)
            {
                return;
            }

            // 当前格
            IntVec3 centerCell = this.Position;
            if (!centerCell.InBounds(map))
            {
                return;
            }

            // 基于朝向的左右方向向量（水平右手系：左 = (-fz, fx)）
            Vector3 forward = this.ExactRotation * Vector3.forward;
            Vector3 leftDir = new Vector3(-forward.z, 0f, forward.x);
            Vector3 rightDir = new Vector3(forward.z, 0f, -forward.x);

            // 将方向向量转换为相邻格偏移
            IntVec3 leftOffset = new IntVec3(Mathf.RoundToInt(Mathf.Clamp(leftDir.x, -1f, 1f)), 0, Mathf.RoundToInt(Mathf.Clamp(leftDir.z, -1f, 1f)));
            IntVec3 rightOffset = new IntVec3(Mathf.RoundToInt(Mathf.Clamp(rightDir.x, -1f, 1f)), 0, Mathf.RoundToInt(Mathf.Clamp(rightDir.z, -1f, 1f)));

            IntVec3 leftCell = centerCell + leftOffset;
            IntVec3 rightCell = centerCell + rightOffset;

            // 对三个格子依次应用伤害
            ApplyDamageAtCell(centerCell);
            if (leftCell.InBounds(map))
            {
                ApplyDamageAtCell(leftCell);
            }
            if (rightCell.InBounds(map))
            {
                ApplyDamageAtCell(rightCell);
            }
        }

        // 在单个格子内对所有可伤害目标造成一次伤害
        private void ApplyDamageAtCell(IntVec3 cell)
        {
            Map map = this.Map;
            if (map == null)
            {
                return;
            }

            // 组装伤害信息（沿用抛射体伤害与角度）
            float angle = this.ExactRotation.eulerAngles.y;
            bool instigatorGuilty = !(this.launcher is Pawn pawn) || !pawn.Drafted;
            DamageInfo damageInfo = new DamageInfo(
                DamageDefOf.Cut,
                this.DamageAmount,
                this.ArmorPenetration,
                angle,
                this.launcher,
                null,
                this.EquipmentDef,
                DamageInfo.SourceCategory.ThingOrUnknown,
                this.intendedTarget.Thing,
                instigatorGuilty
            );
            damageInfo.SetWeaponQuality(this.equipmentQuality);
            var things = map.thingGrid.ThingsListAtFast(cell);
            if (things == null || things.Count == 0)
            {
                return;
            }
            tmpThings.Clear();
            for (int i = 0; i < things.Count; i++)
            {
                Thing t = things[i];
                if (t != null)
                {
                    tmpThings.Add(t);
                }
            }
            for (int i = 0; i < tmpThings.Count; i++)
            {
                Thing thing = tmpThings[i];
                Pawn pawn1 = thing as Pawn;
                if (pawn1 != null)
                {
                    // 仅对非友方生物造成伤害
                    if (pawn1.Faction != null && this.launcher is Pawn launcherPawn && pawn1.Faction == launcherPawn.Faction)
                    {
                        continue;
                    }
                }
                if (thing == null)
                {
                    continue;
                }
                if (thing == this)
                {
                    continue;
                }
                if (thing.Destroyed)
                {
                    continue;
                }

                // 施加伤害
                thing.TakeDamage(damageInfo);
            }
        }
    }
}
