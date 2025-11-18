using ChezhouLib.MonoComp;
using ChezhouLib.ObjectPool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ChezhouLib.Utils
{
    public static class UnityEffUtils
    {
        public static void spawnEff(string ModID,string effName, Vector3 worldPos, Thing forThing = null, float lifetime = 2f)
        {
            try
            {
                string trueEffName = effName;
                if (ModID.Length > 1) {
                    trueEffName = ModID + "_" + effName;
                };
                
                // 检查特效是否已注册
                if (!abDatabase.EffObjDataBase.ContainsKey(trueEffName))
                {
                    Log.Warning($"[UnityEffUtils] 特效未注册: {trueEffName}");
                    return;
                }
                
                GameObject effObj = UnityEffPool.GetEffObj(trueEffName);
                if (effObj == null)
                {
                    Log.Warning($"[UnityEffUtils] 无法从对象池获取特效对象: {trueEffName}，尝试直接创建");
                    
                    // 备用方案：直接从数据库创建
                    if (abDatabase.EffObjDataBase.ContainsKey(trueEffName))
                    {
                        effObj = GameObject.Instantiate(abDatabase.EffObjDataBase[trueEffName].EffObj);
                        Log.Message($"[UnityEffUtils] 直接创建特效对象: {trueEffName}");
                    }
                    else
                    {
                        Log.Error($"[UnityEffUtils] 特效数据库中也没有找到: {trueEffName}");
                        return;
                    }
                }
                
                EffAutoRecycle effAuto = effObj.GetComponent<EffAutoRecycle>();
                if (effAuto == null)
                {
                    Log.Warning($"[UnityEffUtils] 特效对象缺少EffAutoRecycle组件: {trueEffName}");
                    return;
                }
                
                // 设置特效参数
                effAuto.effName = trueEffName;
                effObj.transform.position = worldPos;
                effAuto.lifetime = lifetime;
                if (forThing != null)
                {
                    effAuto.forThing = forThing;
                }
                
                // 确保对象正确激活 - 使用双重激活确保OnEnable被调用
                effObj.SetActive(false); // 先禁用
                effObj.SetActive(true);  // 再启用，确保OnEnable被正确调用
                
                Log.Message($"[UnityEffUtils] 成功创建特效: {trueEffName} 位置: {worldPos}");
            }
            catch (System.Exception ex)
            {
                Log.Error($"[UnityEffUtils] 创建特效失败: {ex.Message}");
            }
        }
    }
}
