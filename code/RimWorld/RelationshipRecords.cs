using System.Collections.Generic;
using Verse;

namespace RimWorld;

public sealed class RelationshipRecords : IExposable
{
	private Dictionary<int, RelationshipRecord> records = new Dictionary<int, RelationshipRecord>();

	private static readonly HashSet<int> toRemove = new HashSet<int>();

	private List<int> tmpKeys;

	private List<RelationshipRecord> tmpValues;

	public IReadOnlyDictionary<int, RelationshipRecord> Records => records;

	public RelationshipRecord GetOrCreateRecord(Pawn pawn)
	{
		if (records.TryGetValue(pawn.thingIDNumber, out var value))
		{
			return value;
		}
		return CreateRecord(pawn);
	}

	public RelationshipRecord CreateRecord(Pawn pawn)
	{
		RelationshipRecord relationshipRecord = new RelationshipRecord(pawn.thingIDNumber, pawn.gender, pawn.Name.ToStringShort);
		records.Add(relationshipRecord.ID, relationshipRecord);
		return relationshipRecord;
	}

	public RelationshipRecord GetRecord(int id)
	{
		return records[id];
	}

	public int CleanupUnusedRecords()
	{
		toRemove.Clear();
		int num = default(int);
		RelationshipRecord relationshipRecord = default(RelationshipRecord);
		foreach (KeyValuePair<int, RelationshipRecord> record in records)
		{
			record.Deconstruct(ref num, ref relationshipRecord);
			int item = num;
			if (relationshipRecord.References.Count <= 1)
			{
				toRemove.Add(item);
			}
		}
		foreach (int item2 in toRemove)
		{
			foreach (Pawn reference in records[item2].References)
			{
				reference.relations.RemoveRelation(records[item2]);
			}
			records.Remove(item2);
		}
		int count = toRemove.Count;
		toRemove.Clear();
		return count;
	}

	public void ExposeData()
	{
		Scribe_Collections.Look(ref records, "records", LookMode.Value, LookMode.Deep, ref tmpKeys, ref tmpValues);
	}
}
