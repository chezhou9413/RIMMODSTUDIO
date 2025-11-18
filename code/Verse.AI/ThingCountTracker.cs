using System.Collections.Generic;
using RimWorld;

namespace Verse.AI;

public class ThingCountTracker : IExposable
{
	public class PawnCount : IExposable
	{
		public Pawn pawn;

		public int count;

		public void ExposeData()
		{
			Scribe_References.Look(ref pawn, "pawn");
			Scribe_Values.Look(ref count, "count", 0);
		}
	}

	private Dictionary<ThingDef, List<PawnCount>> stuffCount = new Dictionary<ThingDef, List<PawnCount>>();

	public IHaulEnroute parent;

	private List<ThingDef> tmpThings;

	private List<List<PawnCount>> tmpCount;

	public IReadOnlyDictionary<ThingDef, List<PawnCount>> ReadOnlyPairs => stuffCount;

	public Thing ParentThing => (Thing)parent;

	public ThingCountTracker()
	{
	}

	public ThingCountTracker(IHaulEnroute parent)
	{
		this.parent = parent;
	}

	public void Add(Pawn pawn, ThingDef stuff, int amount)
	{
		List<PawnCount> value;
		if (amount <= 0)
		{
			Log.Error($"{pawn.LabelShort} tried to add a negative value to the tracker on {((Thing)parent).Label} ({stuff.label} {amount})");
		}
		else if (stuffCount.TryGetValue(stuff, out value))
		{
			foreach (PawnCount item in value)
			{
				if (item.pawn == pawn)
				{
					item.count += amount;
					return;
				}
			}
			stuffCount[stuff].Add(new PawnCount
			{
				pawn = pawn,
				count = amount
			});
		}
		else
		{
			stuffCount[stuff] = new List<PawnCount>
			{
				new PawnCount
				{
					pawn = pawn,
					count = amount
				}
			};
		}
	}

	public void CopyReservations(ThingCountTracker other)
	{
		ThingDef thingDef = default(ThingDef);
		List<PawnCount> list = default(List<PawnCount>);
		foreach (KeyValuePair<ThingDef, List<PawnCount>> item in other.stuffCount)
		{
			item.Deconstruct(ref thingDef, ref list);
			ThingDef stuff = thingDef;
			foreach (PawnCount item2 in list)
			{
				Add(item2.pawn, stuff, item2.count);
			}
		}
	}

	public int Get(ThingDef stuff, Pawn excludePawn = null)
	{
		if (stuffCount.TryGetValue(stuff, out var value))
		{
			int num = 0;
			for (int num2 = value.Count - 1; num2 >= 0; num2--)
			{
				PawnCount pawnCount = value[num2];
				if (pawnCount.pawn != excludePawn)
				{
					num += pawnCount.count;
				}
			}
			return num;
		}
		return 0;
	}

	public void ReleaseFor(Pawn pawn)
	{
		ThingDef thingDef = default(ThingDef);
		List<PawnCount> list = default(List<PawnCount>);
		foreach (KeyValuePair<ThingDef, List<PawnCount>> item in stuffCount)
		{
			item.Deconstruct(ref thingDef, ref list);
			List<PawnCount> list2 = list;
			for (int num = list2.Count - 1; num >= 0; num--)
			{
				if (list2[num].pawn == pawn)
				{
					list2.RemoveAt(num);
				}
			}
		}
	}

	public bool CanCleanup()
	{
		ThingDef thingDef = default(ThingDef);
		List<PawnCount> list = default(List<PawnCount>);
		foreach (KeyValuePair<ThingDef, List<PawnCount>> item in stuffCount)
		{
			item.Deconstruct(ref thingDef, ref list);
			foreach (PawnCount item2 in list)
			{
				if (item2.count > 0)
				{
					return false;
				}
			}
		}
		return true;
	}

	public void InterruptEnroutePawns(Pawn exclude)
	{
		ThingDef thingDef = default(ThingDef);
		List<PawnCount> list = default(List<PawnCount>);
		foreach (KeyValuePair<ThingDef, List<PawnCount>> item in stuffCount)
		{
			item.Deconstruct(ref thingDef, ref list);
			List<PawnCount> list2 = list;
			for (int i = 0; i < list2.Count; i++)
			{
				Pawn pawn = list2[i].pawn;
				if (exclude == pawn)
				{
					continue;
				}
				for (int num = pawn.jobs.jobQueue.Count - 1; num >= 0; num--)
				{
					Job job = pawn.jobs.jobQueue[num].job;
					if (job.targetB == ParentThing && !job.targetQueueB.NullOrEmpty())
					{
						if (pawn.mindState.priorityWork.WorkGiver != null && ParentThing.OccupiedRect().Contains(pawn.mindState.priorityWork.Cell))
						{
							pawn.mindState.priorityWork.Clear();
						}
						pawn.jobs.jobQueue.Extract(job);
					}
					else if (job.targetB == ParentThing)
					{
						job.SetTarget(TargetIndex.B, null);
					}
					else if (job.targetQueueB != null && job.targetQueueB.Contains(ParentThing))
					{
						job.targetQueueB.Remove(ParentThing);
					}
				}
				if (pawn.CurJob.targetB == ParentThing && !pawn.CurJob.targetQueueB.NullOrEmpty())
				{
					if (pawn.mindState.priorityWork.WorkGiver != null && ParentThing.OccupiedRect().Contains(pawn.mindState.priorityWork.Cell))
					{
						pawn.mindState.priorityWork.Clear();
					}
					pawn.jobs.EndCurrentJob(JobCondition.InterruptForced, startNewJob: false);
				}
				else if (pawn.CurJob.targetB == ParentThing)
				{
					pawn.CurJob.SetTarget(TargetIndex.B, null);
				}
				else if (pawn.CurJob.targetQueueB != null && pawn.CurJob.targetQueueB.Contains(ParentThing))
				{
					pawn.CurJob.targetQueueB.Remove(ParentThing);
				}
			}
			list2.Clear();
		}
	}

	public void ExposeData()
	{
		Scribe_References.Look(ref parent, "parent");
		Scribe_Collections.Look(ref stuffCount, "stuffCount", LookMode.Def, LookMode.Deep, ref tmpThings, ref tmpCount);
	}
}
