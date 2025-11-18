using Verse;

namespace OgrynRace.comp
{
    /// <summary>
    /// 禁用材料染色的组件属性
    /// 当装备有这个组件时，会强制使用装备自身的颜色，忽略材料的颜色
    /// </summary>
    public class CompProperties_DisableStuffColor : CompProperties
    {
        public CompProperties_DisableStuffColor()
        {
            compClass = typeof(CompDisableStuffColor);
        }
    }
}

