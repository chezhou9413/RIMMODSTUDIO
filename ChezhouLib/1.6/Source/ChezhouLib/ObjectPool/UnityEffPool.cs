using ChezhouLib.ALLmap;
using ChezhouLib.MonoComp;
using ChezhouLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ChezhouLib.ObjectPool
{
    public static class UnityEffPool
    {
        private static Dictionary<string, Queue<GameObject>> Effpool = new Dictionary<string, Queue<GameObject>>();

        /// <summary>
        /// 注册特效对象池
        /// </summary>
        /// <param name="key">分类Key，可留空（通常是模块名）</param>
        /// <param name="effObj">特效Prefab字典，例如 { "Explosion", prefabExplosion }</param>
        public static void Register(Dictionary<string, EffData> effObj)
        {
            foreach (var pair in effObj)
            {
                string effName = pair.Key;
                GameObject prefab = pair.Value.EffObj;
                if (!Effpool.ContainsKey(effName))
                {
                    Effpool[effName] = new Queue<GameObject>();
                    Log.Message($"[UnityEffPool] 注册特效对象池: {effName}");
                    
                    for (int i = 0; i < 3; i++)
                    {
                        GameObject go = GameObject.Instantiate(prefab);
                        if (go != null)
                        {
                            go.name = effName; // 可选：便于调试区分
                            Effpool[effName].Enqueue(go);
                            go.SetActive(false);
                            Log.Message($"[UnityEffPool] 创建预实例化对象: {effName} #{i+1}");
                        }
                        else
                        {
                            Log.Error($"[UnityEffPool] 预实例化对象创建失败: {effName} #{i+1}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取一个特效对象（自动扩容）
        /// </summary>
        public static GameObject GetEffObj(string EffName)
        {
            if (!Effpool.ContainsKey(EffName))
            {
                Log.Warning("[UnityEffPool] 未注册特效对象池: " + EffName);
                return null;
            }
            
            Queue<GameObject> queue = Effpool[EffName];
            GameObject go = null;
            
            // 优先从对象池获取
            if (queue.Count > 0)
            {
                go = queue.Dequeue();
                Log.Message($"[UnityEffPool] 从对象池获取特效: {EffName}, 剩余: {queue.Count}");
                
                // 检查从对象池获取的对象是否有效
                if (go == null || go.Equals(null))
                {
                    Log.Warning($"[UnityEffPool] 对象池中的对象无效: {EffName}, 尝试动态创建");
                    go = null; // 重置为null，让下面的逻辑处理
                }
            }
            
            // 如果对象池为空或对象无效，动态创建新实例
            if (go == null)
            {
                if (abDatabase.EffObjDataBase.ContainsKey(EffName))
                {
                    go = GameObject.Instantiate(abDatabase.EffObjDataBase[EffName].EffObj);
                    Log.Message($"[UnityEffPool] 动态创建特效实例: {EffName}");
                }
                else
                {
                    Log.Error($"[UnityEffPool] 特效数据库中没有找到: {EffName}");
                    return null;
                }
            }
            
            if (go != null) 
            { 
                // 确保对象处于正确的初始状态
                go.SetActive(false);
                Log.Message($"[UnityEffPool] 成功获取特效对象: {EffName}");
                return go;
            }
            else
            {
                Log.Error($"[UnityEffPool] 创建特效对象失败: {EffName}");
                return null;
            }
        }

        public static void Recycle(string effName, GameObject obj)
        {
            if (!Effpool.ContainsKey(effName))
            {
                Log.Warning($"[UnityEffPool] 尝试回收未注册的特效：{effName}");
                GameObject.Destroy(obj);
                return;
            }
            obj.SetActive(false);
            Effpool[effName].Enqueue(obj);
        }
    }
}
