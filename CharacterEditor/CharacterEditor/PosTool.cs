using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal static class PosTool
{
	internal static void DeletePawnsInCell(this IntVec3 cell)
	{
		cell.FirstPawnFromCell()?.Delete();
	}

	internal static Pawn FirstPawnFromCell(this IntVec3 cell)
	{
		Pawn pawn = cell.FirstFromCell<Pawn>();
		if (pawn != null)
		{
			return pawn;
		}
		return cell.FirstFromCell<Corpse>()?.InnerPawn;
	}

	internal static Pawn FirstPawnInCellArea(this IntVec3 pos)
	{
		Pawn pawn = pos.FirstPawnFromCell();
		if (pawn != null)
		{
			return pawn;
		}
		List<IntVec3> list = new List<IntVec3>();
		list.Add(pos);
		list.Add(new IntVec3(pos.x, pos.y, pos.z));
		list.Add(new IntVec3(pos.x, pos.y, pos.z + 1));
		list.Add(new IntVec3(pos.x, pos.y, pos.z - 1));
		list.Add(new IntVec3(pos.x + 1, pos.y, pos.z));
		list.Add(new IntVec3(pos.x + 1, pos.y, pos.z + 1));
		list.Add(new IntVec3(pos.x + 1, pos.y, pos.z - 1));
		list.Add(new IntVec3(pos.x - 1, pos.y, pos.z));
		list.Add(new IntVec3(pos.x - 1, pos.y, pos.z + 1));
		list.Add(new IntVec3(pos.x - 1, pos.y, pos.z - 1));
		foreach (IntVec3 item in list)
		{
			pawn = item.FirstPawnFromCell();
			if (pawn != null)
			{
				break;
			}
		}
		return pawn;
	}

	internal static T FirstFromCell<T>(this IntVec3 cell)
	{
		if (Find.CurrentMap == null || !cell.InBounds(Find.CurrentMap))
		{
			return default(T);
		}
		return Find.CurrentMap.thingGrid.ThingsAt(cell).ToList().OfType<T>()
			.FirstOrFallback();
	}

	internal static Pawn FirstPawnFromSelector(this Selector selector)
	{
		if (selector != null && selector.FirstSelectedObject != null)
		{
			object firstSelectedObject = selector.FirstSelectedObject;
			if (firstSelectedObject.GetType() == typeof(Pawn))
			{
				return firstSelectedObject as Pawn;
			}
			if (firstSelectedObject.GetType() == typeof(Corpse))
			{
				return (firstSelectedObject as Corpse).InnerPawn;
			}
		}
		return null;
	}

	internal static ThingDef FirstThingFromSelector(this Selector selector)
	{
		if (selector != null && selector.FirstSelectedObject != null)
		{
			object firstSelectedObject = selector.FirstSelectedObject;
			if (firstSelectedObject.GetType() == typeof(ThingDef))
			{
				return firstSelectedObject as ThingDef;
			}
		}
		return null;
	}

	internal static void CheckAndSetScrollPos<T>(this Vector2 scrollPos, List<T> l, T tSelected, float elemenH, float maxH)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		if (tSelected == null || l.NullOrEmpty())
		{
			return;
		}
		int num = l.IndexOf(tSelected);
		if (num >= 0)
		{
			Vector2 val = default(Vector2);
			((Vector2)(ref val))._002Ector(scrollPos.x, (float)num * elemenH);
			float num2 = val.y - scrollPos.y;
			if (num2 < 0f || num2 > maxH)
			{
				scrollPos.x = val.x;
				scrollPos.y = val.y;
			}
		}
	}

	internal static void CheckAndSetScrollPos<T>(this Vector2 scrollPos, HashSet<T> l, T tSelected, float elemenH, float maxH)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		if (tSelected == null || l.NullOrEmpty())
		{
			return;
		}
		int num = l.FirstIndexOf(delegate(T y)
		{
			ref T reference = ref tSelected;
			object obj = y;
			return reference.Equals(obj);
		});
		if (num >= 0)
		{
			Vector2 val = default(Vector2);
			((Vector2)(ref val))._002Ector(scrollPos.x, (float)num * elemenH);
			float num2 = val.y - scrollPos.y;
			if (num2 < 0f || num2 > maxH)
			{
				scrollPos.x = val.x;
				scrollPos.y = val.y;
			}
		}
	}
}
