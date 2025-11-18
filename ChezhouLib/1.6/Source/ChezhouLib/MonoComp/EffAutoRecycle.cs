using ChezhouLib.ObjectPool;
using RimWorld;
using System.Collections;
using UnityEngine;
using Verse;

namespace ChezhouLib.MonoComp
{
    public class EffAutoRecycle : MonoBehaviour
    {
        public string effName;          // 特效名（与对象池键一致）
        public float lifetime = 2f;     // 固定时长（forThing 为空时使用）
        public Thing forThing;          // 绑定的 RimWorld Thing 对象

        private bool isRecycling = false;

        private void OnEnable()
        {
            if (forThing != null)
            {
                StartCoroutine(FollowThing());
            }
            else
            {
                StartCoroutine(RecycleAfterTime());
            }
        }

        private IEnumerator FollowThing()
        {
            while (forThing != null && forThing.Spawned && !forThing.Destroyed)
            {
                // 将特效的世界坐标与Thing同步
                Vector3 worldPos = forThing.DrawPos; // RimWorld中 Thing 的世界位置
                transform.position = worldPos;

                yield return null; // 每帧更新
            }

            // Thing消失或销毁后，执行回收
            Recycle();
        }

        private IEnumerator RecycleAfterTime()
        {
            yield return new WaitForSeconds(lifetime);
            Recycle();
        }

        private void Recycle()
        {
            if (isRecycling)
                return; // 防止多次回收

            isRecycling = true;
            forThing = null;
            UnityEffPool.Recycle(effName, gameObject);
        }
    }
}
