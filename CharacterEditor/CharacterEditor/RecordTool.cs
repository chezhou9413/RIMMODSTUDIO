using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal static class RecordTool
{
	internal const string CO_RECORDS = "records";

	internal static Vector2 scrollPos;

	internal static int elemH;

	internal static int oldIVal;

	internal static float oldFVal;

	internal static RecordDef selectedRecord;

	internal static string GetAsSeparatedString(this RecordDef r, float val)
	{
		if (r == null)
		{
			return "";
		}
		string text = "";
		text = text + r.defName + "|";
		return text + val;
	}

	internal static string GetAllRecordsAsSeparatedString(this Pawn p)
	{
		if (!p.HasRecordsTracker() || p.GetPawnRecords().EnumerableNullOrEmpty())
		{
			return "";
		}
		string text = "";
		foreach (KeyValuePair<RecordDef, float> pawnRecord in p.GetPawnRecords())
		{
			text += pawnRecord.Key.GetAsSeparatedString(pawnRecord.Value);
			text += ":";
		}
		return text.SubstringRemoveLast();
	}

	internal static void SetRecords(this Pawn p, string s)
	{
		if (s.NullOrEmpty() || !p.HasRecordsTracker())
		{
			return;
		}
		try
		{
			string[] array = s.SplitNo(":");
			DefMap<RecordDef, float> pawnRecords = p.GetPawnRecords();
			string[] array2 = array;
			foreach (string s2 in array2)
			{
				string[] array3 = s2.SplitNo("|");
				if (array3.Length == 2)
				{
					RecordDef recordDef = DefTool.RecordDef(array3[0]);
					if (recordDef != null)
					{
						float value = array3[1].AsFloat();
						pawnRecords[recordDef] = value;
					}
				}
			}
			p.SetPawnRecords(pawnRecords);
			p.records.RecordsTickInterval(0);
		}
		catch (Exception e)
		{
			MessageTool.DebugException(e);
		}
	}

	internal static void SetPawnRecords(this Pawn p, DefMap<RecordDef, float> dic)
	{
		if (p.HasRecordsTracker())
		{
			p.records.SetMemberValue("records", dic);
		}
	}

	internal static DefMap<RecordDef, float> GetPawnRecords(this Pawn p)
	{
		return p.HasRecordsTracker() ? p.records.GetMemberValue<DefMap<RecordDef, float>>("records", null) : null;
	}

	internal static void DrawRecordCard(Rect rect, Pawn p)
	{
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		List<RecordDef> allDefsListForReading = DefDatabase<RecordDef>.AllDefsListForReading;
		List<RecordDef> list = allDefsListForReading.Where((RecordDef td) => td.type == RecordType.Time).ToList();
		List<RecordDef> list2 = allDefsListForReading.Where((RecordDef td) => td.type == RecordType.Int).ToList();
		List<RecordDef> list3 = allDefsListForReading.Where((RecordDef td) => td.type == RecordType.Float).ToList();
		int count = list.Count;
		int count2 = list2.Count;
		int count3 = list3.Count;
		int num = Mathf.Max(count, count2 + count3);
		elemH = 21;
		float num2 = (float)(num * elemH) + 50f;
		Rect outRect = default(Rect);
		((Rect)(ref outRect))._002Ector(rect);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, 0f, ((Rect)(ref outRect)).width - 16f, num2);
		Widgets.BeginScrollView(outRect, ref scrollPos, val);
		Rect val2 = val.ContractedBy(4f);
		((Rect)(ref val2)).height = num2;
		Rect rect2 = val;
		((Rect)(ref rect2)).width = ((Rect)(ref rect2)).width * 0.5f;
		Rect rect3 = val;
		((Rect)(ref rect3)).x = ((Rect)(ref rect2)).xMax;
		((Rect)(ref rect3)).width = ((Rect)(ref val)).width - ((Rect)(ref rect3)).x;
		((Rect)(ref rect2)).xMax = ((Rect)(ref rect2)).xMax - 6f;
		((Rect)(ref rect3)).xMin = ((Rect)(ref rect3)).xMin + 6f;
		DrawLeftRect(rect2, list, p);
		DrawRightRect(rect3, list2, list3, p);
		Widgets.EndScrollView();
	}

	internal static void DrawLeftRect(Rect rect, List<RecordDef> l, Pawn p)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		float curY = 0f;
		Widgets.BeginGroup(rect);
		Widgets.ListSeparator(ref curY, ((Rect)(ref rect)).width, "TimeRecordsCategory".Translate());
		foreach (RecordDef item in l)
		{
			curY += DrawRecord(8f, curY, ((Rect)(ref rect)).width - 8f, item, p);
		}
		Widgets.EndGroup();
	}

	internal static void DrawRightRect(Rect rect, List<RecordDef> li, List<RecordDef> lf, Pawn p)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		float curY = 0f;
		Widgets.BeginGroup(rect);
		Widgets.ListSeparator(ref curY, ((Rect)(ref rect)).width, "MiscRecordsCategory".Translate());
		foreach (RecordDef item in li)
		{
			curY += DrawRecord(8f, curY, ((Rect)(ref rect)).width - 8f, item, p);
		}
		foreach (RecordDef item2 in lf)
		{
			curY += DrawRecord(8f, curY, ((Rect)(ref rect)).width - 8f, item2, p);
		}
		Widgets.EndGroup();
	}

	internal static float DrawRecord(float x, float y, float w, RecordDef r, Pawn p)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		float num = w * 0.4f;
		string label = ((r.type != RecordType.Time) ? p.records.GetValue(r).ToString("0.##") : p.records.GetAsInt(r).ToStringTicksToPeriod());
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(8f, y, w, (float)elemH);
		if (Mouse.IsOver(val))
		{
			Widgets.DrawHighlight(val);
		}
		Rect rect = val;
		((Rect)(ref rect)).width = ((Rect)(ref rect)).width - num;
		Widgets.Label(rect, r.LabelCap);
		SZWidgets.ButtonInvisible(rect, delegate
		{
			selectedRecord = null;
		});
		Rect rect2 = val;
		((Rect)(ref rect2)).x = ((Rect)(ref rect)).xMax;
		((Rect)(ref rect2)).width = num;
		if (selectedRecord == r)
		{
			if (r.type == RecordType.Int)
			{
				oldIVal = p.records.GetAsInt(r);
				int num2 = SZWidgets.NumericIntBox(((Rect)(ref rect2)).x, ((Rect)(ref rect2)).y, 90f, ((Rect)(ref rect2)).height, oldIVal, 0, int.MaxValue);
				if (num2 != oldIVal)
				{
					p.SetRecordValue(r, num2);
				}
			}
			else if (r.type == RecordType.Float)
			{
				oldFVal = p.records.GetValue(r);
				float num3 = SZWidgets.NumericFloatBox(((Rect)(ref rect2)).x, ((Rect)(ref rect2)).y, 90f, ((Rect)(ref rect2)).height, oldFVal, 0f, 1E+09f);
				if (num3 != oldFVal)
				{
					p.SetRecordValue(r, num3);
				}
			}
			else if (r.type == RecordType.Time)
			{
				oldFVal = p.records.GetValue(r);
				long num4 = (long)(oldFVal / 60f);
				long num5 = SZWidgets.NumericLongBox(((Rect)(ref rect2)).x, ((Rect)(ref rect2)).y, 90f, ((Rect)(ref rect2)).height, num4, 0L, long.MaxValue);
				if (num5 != num4)
				{
					p.SetRecordValue(r, num5 * 60);
				}
			}
		}
		else
		{
			Widgets.Label(rect2, label);
			SZWidgets.ButtonInvisibleMouseOverVar(val, delegate(RecordDef record)
			{
				selectedRecord = record;
			}, r, r.description);
		}
		return ((Rect)(ref val)).height;
	}

	internal static void SetRecordValue(this Pawn p, RecordDef r, float val)
	{
		DefMap<RecordDef, float> memberValue = p.records.GetMemberValue<DefMap<RecordDef, float>>("records", null);
		if (memberValue != null)
		{
			memberValue[r] = val;
		}
	}
}
