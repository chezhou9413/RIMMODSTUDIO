using ChezhouLib.Utils;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace rimbreak.RbProjectile
{
    public class Projectile_SnowBreath : Projectile
    {
        // 运行期状态（保存/载入）
        private Thing mainTargetThing;
        private Vector3 basePos;          // 物理位置（用于逻辑移动/命中计算）
        private Vector3 displayPos;       // 显示位置（加入抖动/波动，驱动渲染）
        private Vector3 velocity;         // 当前速度向量（世界坐标）
        private Quaternion curRotation;   // 朝向（用于渲染）
        private float noiseSeed;          // 抖动随机种子
        private bool initialized = false; // 是否完成 Launch 初始化
        private float lastDistToTarget = float.MaxValue; // 上一帧至目标的物理距离
        private int followTicks = 0;      // 近距离跟随累计 Tick（兜底命中用）
        private Vector3 lastKnownTargetPos; // 目标最后一次已知位置（用于目标死亡后的落点）
        private bool targetLost = false;    // 是否已丢失动态目标

        // 行为常量（不映射 XML，仅内部调参；避免乱增原版不可用字段）
        private const float TurnSmoothness = 0.5f;   // 转向平滑系数（远距离时使用）
        private const float NoiseAmplitude = 0.7f;   // 显示抖动强度（Perlin 噪声）
        private const float NoiseFrequency = 10f;    // 显示抖动频率
        private const float WaveAmplitude = 0.25f;   // 显示叠加波动强度
        private const float WaveFrequency = 8f;      // 显示叠加波动频率
        private const float CloseImpactDistance = 0.5f; // 贴脸命中半径（物理/显示任意满足）
        private const float ArrivalRadius = 2.0f;    // 开始减速的半径（基于物理位置）

        public override void Launch(Thing launcher, Vector3 origin, LocalTargetInfo usedTarget,
          LocalTargetInfo intendedTarget, ProjectileHitFlags hitFlags, bool preventFriendlyFire = false,
          Thing equipment = null, ThingDef targetCoverDef = null)
        {
            base.Launch(launcher, origin, usedTarget, intendedTarget, hitFlags, preventFriendlyFire, equipment, targetCoverDef);

            // 解析目标引用（优先 intendedTarget）
            mainTargetThing = intendedTarget.HasThing ? intendedTarget.Thing :
                (usedTarget.HasThing ? usedTarget.Thing : null);

            // 初始化移动/显示/方向
            basePos = origin;
            displayPos = origin;
            Vector3 initDir = (destination - origin).Yto0().normalized;
            curRotation = Quaternion.LookRotation(initDir);
            velocity = initDir * GetBaseSpeedFromDef();
            noiseSeed = Rand.Range(0f, 10000f);
            initialized = true;

            // 记录第一帧距离用于“变糟”趋势判断
            Vector3 initTargetPos = ResolveTargetPosition().Yto0();
            lastDistToTarget = (initTargetPos - basePos).magnitude;
            followTicks = 0;
            lastKnownTargetPos = initTargetPos;
            UnityEffUtils.spawnEffProjectile("RbEfflord", "拖尾",1f, this.DrawPos, this);
        }

        protected override void Tick()
        {
            if (!initialized || landed) return;

            float dt = 1f / 60f;                                   // 物理步长（RimWorld 60FPS 基准）
            float timeSec = Find.TickManager.TicksGame * 0.016666667f; // 秒计时（用于噪声/波动）

            // 目标位置解析与丢失判定
            Vector3 targetPos;
            if (targetLost)
            {
                // 已丢失目标：继续朝最后一次已知坐标前进
                targetPos = lastKnownTargetPos;
            }
            else
            {
                targetPos = ResolveTargetPosition();
                lastKnownTargetPos = targetPos;
                if (HasLostTarget())
                {
                    // 目标死亡/消失：固定落点为最后坐标，转为命中该点
                    targetLost = true;
                    mainTargetThing = null; // 清空动态目标引用
                    destination = lastKnownTargetPos; // 设定静态目的地
                }
            }

            // 计算朝向、速度与推进
            Vector3 toTargetDir;
            float distToTarget;
            ComputeDirectionAndDistance(targetPos, out toTargetDir, out distToTarget);
            float currentSpeed = ComputeArrivalSpeed(distToTarget);
            UpdateVelocity(toTargetDir, distToTarget, currentSpeed);
            basePos += velocity * dt;

            // 显示偏移与朝向
            UpdateDisplayPosition(timeSec);
            UpdateRotation();

            // 命中检测与结算
            if (TryImpactIfNeeded(targetPos, dt, toTargetDir)) return;

            // 记录状态用于下一帧趋势判断
            lastDistToTarget = (targetPos - basePos).magnitude;
        }

        public override Vector3 ExactPosition => displayPos + Vector3.up * def.Altitude;
        public override Quaternion ExactRotation => curRotation;

        // 命中后结算：改为小范围AOE爆炸（半径1.5），不再对单体重复结算直伤
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            Map map = this.Map;
            IntVec3 pos = this.Position;

            // 调用基类以保留基础落地流程（特效/音效）
            base.Impact(hitThing, blockedByShield);

            // 生成特效：如果 hitThing 为 null，回退到投射体当前显示位置或 basePos
            Vector3 effPos;
            if (hitThing != null)
                effPos = hitThing.DrawPos;
            else if (this.Map != null)
                effPos = this.ExactPosition; // 在 Map 存在时使用物体的 ExactPosition（更准确）
            else
                effPos = basePos; // 最后回退到 basePos，不再访问 Thing/Map

            // spawnEff 的具体实现可能也会内部访问 Map/Thing；按你原来的用法调用，但位置已安全
            UnityEffUtils.spawnEff("RbEfflord", "baodian", 1f, effPos);

            // 以爆炸方式造成范围伤害：半径1.5，伤害与穿甲沿用抛射体定义
            float radius = 1.5f;
            GenExplosion.DoExplosion(
                center: pos,
                map: map,
                radius: radius,
                damType: this.DamageDef,
                instigator: launcher,
                damAmount: this.DamageAmount,
                armorPenetration: this.ArmorPenetration,
                explosionSound: SoundDefOf.BulletImpact_Ground,
                weapon: equipmentDef,
                projectile: def,
                intendedTarget: (intendedTarget.HasThing ? intendedTarget.Thing : null),
                postExplosionSpawnThingDef: null,
                postExplosionSpawnChance: 0f,
                postExplosionSpawnThingCount: 1,
                postExplosionGasType: null,
                postExplosionGasRadiusOverride: null,
                postExplosionGasAmount: 255,
                applyDamageToExplosionCellsNeighbors: false,
                preExplosionSpawnThingDef: null,
                preExplosionSpawnChance: 0f,
                preExplosionSpawnThingCount: 1,
                chanceToStartFire: 0f,
                damageFalloff: false,
                direction: null,
                ignoredThings: null,
                affectedAngle: null,
                doVisualEffects: true,
                propagationSpeed: 1f,
                excludeRadius: 0f,
                doSoundEffects: true,
                postExplosionSpawnThingDefWater: null,
                screenShakeFactor: 1f,
                flammabilityChanceCurve: null,
                overrideCells: null,
                postExplosionSpawnSingleThingDef: null,
                preExplosionSpawnSingleThingDef: null
            );
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref mainTargetThing, "mainTargetThing");
            Scribe_Values.Look(ref basePos, "basePos");
            Scribe_Values.Look(ref displayPos, "displayPos");
            Scribe_Values.Look(ref velocity, "velocity");
            Scribe_Values.Look(ref curRotation, "curRotation");
            Scribe_Values.Look(ref noiseSeed, "noiseSeed", 0f);
            Scribe_Values.Look(ref initialized, "initialized", false);
            Scribe_Values.Look(ref lastDistToTarget, "lastDistToTarget", float.MaxValue);
            Scribe_Values.Look(ref followTicks, "followTicks", 0);
        }

        // 读取原版 XML 中的速度（ThingDef.projectile.speed）。若缺失则提供保底值。
        private float GetBaseSpeedFromDef()
        {
            // 原版字段：ThingDef.projectile.speed
            float xmlSpeed = (def?.projectile != null) ? def.projectile.speed : 12f;
            return Mathf.Max(0.1f, xmlSpeed);
        }

        // 解析当前目标位置（优先动态目标，其次静态 destination）。
        private Vector3 ResolveTargetPosition()
        {
            if (mainTargetThing != null && mainTargetThing.Spawned && !mainTargetThing.Destroyed)
            {
                return mainTargetThing.TrueCenter().Yto0();
            }
            return destination.Yto0();
        }

        // 判断目标是否在运行中丢失（用于提前结束）。
        private bool HasLostTarget()
        {
            return mainTargetThing != null && (!mainTargetThing.Spawned || mainTargetThing.Destroyed);
        }

        // 计算至目标的方向与距离（基于物理位置 basePos）。
        private void ComputeDirectionAndDistance(Vector3 targetPos, out Vector3 toTargetDir, out float dist)
        {
            Vector3 toTarget = (targetPos - basePos).Yto0();
            dist = toTarget.magnitude;
            toTargetDir = (dist > 0.001f) ? toTarget.normalized : (velocity.sqrMagnitude > 0.0001f ? velocity.normalized : Vector3.forward);
        }

        // 根据到达半径做线性减速，避免近距离绕圈；速度下限留出贴脸命中空间。
        private float ComputeArrivalSpeed(float distToTarget)
        {
            float baseSpeed = GetBaseSpeedFromDef();
            if (distToTarget < ArrivalRadius)
            {
                float mapped = baseSpeed * (distToTarget / ArrivalRadius);
                return Mathf.Max(mapped, 0.8f);
            }
            return baseSpeed;
        }

        // 远距离平滑转向，近距离直接对准以提升贴脸命中率。
        private void UpdateVelocity(Vector3 toTargetDir, float distToTarget, float currentSpeed)
        {
            Vector3 desired = toTargetDir * currentSpeed;
            if (distToTarget < 3.5f)
            {
                velocity = desired;
            }
            else
            {
                velocity = Vector3.Slerp(velocity, desired, TurnSmoothness);
            }
        }

        // 计算显示偏移（Perlin 抖动 + 正弦波动），仅影响渲染不影响命中。
        private void UpdateDisplayPosition(float timeSec)
        {
            float perlinA = Mathf.PerlinNoise(noiseSeed, timeSec * NoiseFrequency) - 0.5f;
            float perlinB = Mathf.PerlinNoise(noiseSeed + 1337f, timeSec * NoiseFrequency) - 0.5f;
            Vector3 forward = velocity.normalized;
            Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
            Vector3 up = Vector3.Cross(forward, right).normalized;
            Vector3 perlinOffset = (right * perlinA + up * perlinB) * NoiseAmplitude;
            Vector3 waveOffset = (right * Mathf.Sin(timeSec * WaveFrequency + noiseSeed) +
                                 up * Mathf.Cos(timeSec * WaveFrequency * 0.8f + noiseSeed * 1.37f)) * WaveAmplitude;
            displayPos = basePos + perlinOffset + waveOffset;
        }

        // 根据速度方向更新朝向。
        private void UpdateRotation()
        {
            if (velocity.sqrMagnitude > 0.001f)
            {
                curRotation = Quaternion.LookRotation(velocity.normalized);
            }
        }

        // 命中判定整合：多条件兜底，尽量与视觉表现一致。
        private bool TryImpactIfNeeded(Vector3 targetPos, float dt, Vector3 toTargetDir)
        {
            float newDistToTarget = (targetPos - basePos).magnitude;
            float displayDistToTarget = (targetPos - displayPos).magnitude;

            float stepLen = velocity.magnitude * dt;
            bool canSnapThisStep = newDistToTarget <= stepLen + CloseImpactDistance * 0.5f;

            followTicks++;
            bool followTimeoutClose = (followTicks > 120) && (newDistToTarget < 3.0f);

            float facingDot = Vector3.Dot(velocity.sqrMagnitude > 0.0001f ? velocity.normalized : toTargetDir, toTargetDir);
            bool gettingWorse = newDistToTarget > lastDistToTarget + 0.02f;

            IntVec3 targetCell = targetPos.ToIntVec3();
            bool gridSnapHit = (this.Position == targetCell) || this.Position.AdjacentTo8Way(targetCell);

            bool shouldHit =
                newDistToTarget <= CloseImpactDistance ||
                displayDistToTarget <= CloseImpactDistance ||
                (newDistToTarget < 2.5f && (gettingWorse || facingDot < 0f)) ||
                canSnapThisStep ||
                followTimeoutClose ||
                gridSnapHit;

            if (!shouldHit) return false;

            // 将物理与显示位置吸附到目标
            basePos = targetPos;
            displayPos = targetPos;

            // 只有在确实有 Map 的时候再写 Position（否则会触发内部对 Map 的访问导致 NRE）
            if (this.Map != null)
            {
                try
                {
                    Position = targetPos.ToIntVec3();
                }
                catch
                {
                    // 保险兜底：如果写 Position 仍然异常，不要中断流程，继续 Impact（但记录日志供调试）
                    Verse.Log.Warning($"Projectile_SnowBreath: failed to set Position for projectile {this}; Map may be in bad state.");
                }
            }

            // 如果 mainTargetThing 不存在，尝试在格子找一个 pawn；但如果 Map 为 null 就直接返回 null
            Thing hitThing = mainTargetThing ?? (this.Map != null ? GridsUtility.GetFirstPawn(targetPos.ToIntVec3(), this.Map) : null);

            Impact(hitThing);
            return true;
        }
    }
}