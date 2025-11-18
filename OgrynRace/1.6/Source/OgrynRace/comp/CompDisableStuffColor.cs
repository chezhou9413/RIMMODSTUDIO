using Verse;

namespace OgrynRace.comp
{
    /// <summary>
    /// 禁用材料染色的组件
    /// 强制装备使用自身定义的颜色，忽略材料颜色
    /// </summary>
    public class CompDisableStuffColor : ThingComp
    {
        /// <summary>
        /// 标记：这个装备禁用了材料染色
        /// </summary>
        public bool DisableStuffColor => true;
    }
}

