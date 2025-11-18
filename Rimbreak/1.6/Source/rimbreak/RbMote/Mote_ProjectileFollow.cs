using ChezhouLib.Utils;
using RimWorld;
using UnityEngine;
using Verse;

namespace rimbreak.RbMote
{
    /// <summary>
    /// 自定义Mote类，用于跟随抛射体移动
    /// 通过不断更新自身位置来实现附着效果
    /// </summary>
    public class Mote_ProjectileFollow : Mote
    {
        // 要跟随的抛射体引用
        private Projectile targetProjectile;
        
        // 位置偏移量
        private Vector3 offset = Vector3.zero;
        
        // 是否已经初始化
        private bool initialized = false;

        /// <summary>
        /// 设置要跟随的抛射体
        /// </summary>
        /// <param name="projectile">目标抛射体</param>
        /// <param name="positionOffset">位置偏移</param>
        public void SetTargetProjectile(Projectile projectile, Vector3 positionOffset = default)
        {
            targetProjectile = projectile;
            offset = positionOffset;
            initialized = true;
            
            // 立即设置初始位置
            if (targetProjectile != null)
            {
                exactPosition = targetProjectile.ExactPosition + offset;
            }
        }

        /// <summary>
        /// 重写TimeInterval方法，每帧更新位置
        /// </summary>
        /// <param name="deltaTime">时间间隔</param>
        protected override void TimeInterval(float deltaTime)
        {
            // 先调用基类方法处理mote的生命周期
            base.TimeInterval(deltaTime);
            // 如果目标抛射体存在且未销毁，更新位置
            if (targetProjectile != null && !targetProjectile.Destroyed)
            {
                // 更新精确位置
                exactPosition = targetProjectile.ExactPosition + offset;
                
                // 更新网格位置
                IntVec3 newPosition = exactPosition.ToIntVec3();
                if (newPosition != Position)
                {
                    Position = newPosition;
                }
            }
            else
            {
                // 如果目标抛射体被销毁，销毁这个mote
                Destroy();
            }
        }

        /// <summary>
        /// 重写SpawnSetup方法，确保正确初始化
        /// </summary>
        /// <param name="map">地图</param>
        /// <param name="respawningAfterLoad">是否从存档加载</param>
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            
            // 如果还没有初始化，尝试从def中获取偏移量
            if (!initialized && def?.mote?.attachedDrawOffset != null)
            {
                offset = def.mote.attachedDrawOffset;
            }
        }

        /// <summary>
        /// 获取当前跟随的目标抛射体
        /// </summary>
        public Projectile TargetProjectile => targetProjectile;

        /// <summary>
        /// 获取当前的位置偏移
        /// </summary>
        public Vector3 Offset => offset;
    }
}
