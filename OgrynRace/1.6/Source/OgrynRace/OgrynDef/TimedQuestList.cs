
using System.Collections.Generic;
using System.IO;
using Verse;

namespace OgrynRace.Ogryn
{
    public class TimedQuestList : Def
    {
        public List<TimedQuestData> quests;
    }

    public class TimedQuestData : IExposable
    {
        public string questDefName;
        public int mintick;
        public int maxtick;

        public void ExposeData()
        {
            // 保存/读取每个字段
            Scribe_Values.Look(ref questDefName, "questDefName");
            Scribe_Values.Look(ref mintick, "mintick");
            Scribe_Values.Look(ref maxtick, "maxtick");
        }
    }
}
