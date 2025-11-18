using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ChezhouLib.Utils
{
     public static class CompUtils
    {
        public static T TryAddComp<T>(this ThingWithComps thing) where T : ThingComp
        {
            // 1. 检查是否已经有这个组件了，避免重复添加
            if (thing.GetComp<T>() != null)
            {
                return null;
            }

            // 2. 创建一个新的组件实例
            T newComp = (T)System.Activator.CreateInstance(typeof(T));

            // 3. 关键：将组件的parent设置为当前Thing
            newComp.parent = thing;

            // 4. 将新组件添加到Thing的组件列表中
            thing.AllComps.Add(newComp);

            // 5. 关键：手动调用初始化函数
            // 首先需要一个CompProperties，我们可以动态创建一个
            CompProperties props = new CompProperties(typeof(T));
            newComp.Initialize(props);

            // 如果组件有PostSpawnSetup，并且Thing已经在地图上，也应该调用它
            if (thing.Spawned)
            {
                newComp.PostSpawnSetup(false);
            }

            return newComp;
        }
    }
}
