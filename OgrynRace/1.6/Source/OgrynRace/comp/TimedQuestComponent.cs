using OgrynRace.Ogryn;
using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OgrynRace.comp
{
    public class TimedQuestComponent : GameComponent
    {
        private List<TriggeredQuest> triggeredQuests = new List<TriggeredQuest>();

        public TimedQuestComponent(Game game) { }

        public override void FinalizeInit()
        {
            base.FinalizeInit();

            // 【修复】只在列表为空时（即新游戏开始时）才从XML加载任务
            // 加载存档时，ExposeData会先填充列表，所以这里不会执行
            if (triggeredQuests.NullOrEmpty())
            {
                LoadDefs();
            }
        }

        private void LoadDefs()
        {
            // 确保列表是全新的，防止意外的重复添加
            triggeredQuests = new List<TriggeredQuest>();
            foreach (var def in DefDatabase<TimedQuestList>.AllDefs)
            {
                foreach (var data in def.quests)
                {
                    int triggerTick = Rand.Range(data.mintick, data.maxtick);
                    triggeredQuests.Add(new TriggeredQuest
                    {
                        questDefName = data.questDefName,
                        triggerTick = triggerTick,
                        triggered = false
                    });
                }
            }
        }

        public override void GameComponentTick()
        {
            base.GameComponentTick();

            // 为了性能，不需要每个Tick都检查，可以适当增加间隔
            if (Find.TickManager.TicksGame % 2000 != 0) return; // 每2000tick检查一次

            int currentTick = Find.TickManager.TicksGame;

            foreach (var q in triggeredQuests)
            {
                if (!q.triggered && currentTick >= q.triggerTick)
                {
                    TryTriggerQuest(q);
                    q.triggered = true; // 标记为已触发
                }
            }
        }

        private void TryTriggerQuest(TriggeredQuest q)
        {
            // 优先查找 IncidentDef
            var incident = DefDatabase<IncidentDef>.GetNamedSilentFail(q.questDefName);
            if (incident != null)
            {
                // 确保有地图目标可以执行事件
                var target = Find.AnyPlayerHomeMap;
                if (target != null)
                {
                    var parms = StorytellerUtility.DefaultParmsNow(incident.category, target);
                    incident.Worker.TryExecute(parms);
                    Log.Message($"[TimedQuestComponent] Triggered Incident: {q.questDefName}");
                    return;
                }
            }

            // 否则查找 QuestScriptDef
            var questDef = DefDatabase<QuestScriptDef>.GetNamedSilentFail(q.questDefName);
            if (questDef != null)
            {
                Slate slate = new Slate();
                // 可以为任务设置一些变量，例如：
                // slate.Set("points", StorytellerUtility.DefaultThreatPointsNow(Find.CurrentMap));
                Quest quest = QuestUtility.GenerateQuestAndMakeAvailable(questDef, slate);
                QuestUtility.SendLetterQuestAvailable(quest);
                Log.Message($"[TimedQuestComponent] Triggered Quest: {q.questDefName}");
            }
        }

        public override void ExposeData()
        {
            base.ExposeData(); // 别忘了调用基类的方法
            Scribe_Collections.Look(ref triggeredQuests, "triggeredQuests", LookMode.Deep);

            // 【可选优化】如果列表为空，则在保存时将其设为null，可以节省一点点存档空间
            if (Scribe.mode == LoadSaveMode.PostLoadInit && triggeredQuests == null)
            {
                triggeredQuests = new List<TriggeredQuest>();
            }
        }

        // TriggeredQuest 类保持不变
        public class TriggeredQuest : IExposable
        {
            public string questDefName;
            public int triggerTick;
            public bool triggered;

            public void ExposeData()
            {
                Scribe_Values.Look(ref questDefName, "questDefName");
                Scribe_Values.Look(ref triggerTick, "triggerTick");
                Scribe_Values.Look(ref triggered, "triggered", false); // 给一个默认值false
            }
        }
    }
}