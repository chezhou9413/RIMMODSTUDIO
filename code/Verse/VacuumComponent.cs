using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;

namespace Verse;

[StaticConstructorOnStartup]
public class VacuumComponent : MapComponent, ICellBoolGiver, IDisposable
{
	private class GroupData : IFullPoolable
	{
		public readonly Dictionary<Room, HashSet<IntVec3>> adjacent = new Dictionary<Room, HashSet<IntVec3>>();

		public HashSet<IntVec3> directWarnings;

		public bool hasDirectPathToVacuum;

		public void Reset()
		{
			Room room = default(Room);
			HashSet<IntVec3> hashSet = default(HashSet<IntVec3>);
			foreach (KeyValuePair<Room, HashSet<IntVec3>> item in adjacent)
			{
				item.Deconstruct(ref room, ref hashSet);
				HashSet<IntVec3> hashSet2 = hashSet;
				hashSet2.Clear();
				SimplePool<HashSet<IntVec3>>.Return(hashSet2);
			}
			adjacent.Clear();
			directWarnings = null;
			hasDirectPathToVacuum = false;
		}
	}

	private CellBoolDrawer drawerInt;

	private bool dirty = true;

	private const int UpdateInterval = 250;

	private const float VacuumDeltaPerSecondFactor = 1.0416666f;

	private const float VacuumFactorPerHundredCells = 0.5f;

	private const float DirectVacuumPenaltyPerHundredCells = 0.1f;

	private const float WarningLevel = 0.5f;

	private readonly Dictionary<Room, GroupData> roomGroups = new Dictionary<Room, GroupData>();

	private static readonly Material IntWarningMat = MatLoader.LoadMat("Map/OxygenOverlay/Warning");

	private static readonly Material IntVacuumOverlayMat0 = MatLoader.LoadMat("Map/OxygenOverlay/VacuumOverlay");

	private static readonly Material IntVacuumOverlayTransitionMat0 = MatLoader.LoadMat("Map/OxygenOverlay/VacuumOverlayTransition");

	private static readonly Material IntVacuumOverlayMat1 = MatLoader.LoadMat("Map/OxygenOverlay/VacuumOverlay2");

	private static readonly Material IntVacuumOverlayTransitionMat1 = MatLoader.LoadMat("Map/OxygenOverlay/VacuumOverlayTransition2");

	private static readonly int VacuumPercent = Shader.PropertyToID("_VacuumPercent");

	private readonly List<Matrix4x4> vacuumBoxMatrices = new List<Matrix4x4>();

	private readonly List<float> vacuumBoxIntensities = new List<float>();

	private MaterialPropertyBlock vacuumBoxPropBlock;

	private readonly Dictionary<IntVec3, float> vacuumedCells = new Dictionary<IntVec3, float>();

	private readonly List<Matrix4x4> vacuumTransitionMatrices = new List<Matrix4x4>();

	private readonly List<float> vacuumTransitionIntensities = new List<float>();

	private readonly Dictionary<Room, float> roomVacuums = new Dictionary<Room, float>();

	private readonly HashSet<IntVec3> warningCells = new HashSet<IntVec3>();

	private readonly List<Matrix4x4> warningMatrices = new List<Matrix4x4>();

	private MaterialPropertyBlock vacuumTransitionPropBlock;

	private static readonly HashSet<Room> visited = new HashSet<Room>();

	private static readonly Dictionary<Room, HashSet<IntVec3>> exteriorCells = new Dictionary<Room, HashSet<IntVec3>>();

	private static readonly Queue<Room> frontier = new Queue<Room>();

	private CellBoolDrawer Drawer => drawerInt ?? (drawerInt = new CellBoolDrawer(this, map.Size.x, map.Size.z, 3600));

	private Material WarningMat => IntWarningMat;

	private Material VacuumOverlayMat0 => IntVacuumOverlayMat0;

	private Material VacuumOverlayTransitionMat0 => IntVacuumOverlayTransitionMat0;

	private Material VacuumOverlayMat1 => IntVacuumOverlayMat1;

	private Material VacuumOverlayTransitionMat1 => IntVacuumOverlayTransitionMat1;

	private bool ActiveOnMap
	{
		get
		{
			if (ModLister.OdysseyInstalled)
			{
				return map.Biome.inVacuum;
			}
			return false;
		}
	}

	public Color Color => Color.white;

	public VacuumComponent(Map map)
		: base(map)
	{
		if (Scribe.mode == LoadSaveMode.Inactive)
		{
			InitializeIfRequired();
		}
	}

	private void InitializeIfRequired()
	{
		if (ActiveOnMap)
		{
			map.events.RegionsRoomsChanged += Dirty;
			map.events.DoorOpened += delegate
			{
				Dirty();
			};
			map.events.DoorClosed += delegate
			{
				Dirty();
			};
			map.events.TerrainChanged += delegate
			{
				Dirty();
			};
			map.events.RoofChanged += delegate
			{
				Dirty();
			};
		}
	}

	public override void FinalizeInit()
	{
		if (ActiveOnMap)
		{
			RebuildData();
		}
	}

	public override void MapComponentTick()
	{
		if (ActiveOnMap && map.IsHashIntervalTick(250))
		{
			if (dirty)
			{
				RebuildData();
			}
			ExchangeRoomVacuum();
			UpdateVacuumOverlay();
		}
	}

	private bool HasDirectPathToVacuum(Room room, out HashSet<IntVec3> warning)
	{
		frontier.Enqueue(room);
		Room room2 = default(Room);
		Room room3 = default(Room);
		HashSet<IntVec3> hashSet = default(HashSet<IntVec3>);
		while (frontier.TryDequeue(ref room2))
		{
			visited.Add(room2);
			if (room2.ExposedToSpace)
			{
				warning = exteriorCells[room2];
				visited.Clear();
				exteriorCells.Clear();
				frontier.Clear();
				return true;
			}
			if (!roomGroups.TryGetValue(room2, out var value))
			{
				continue;
			}
			foreach (KeyValuePair<Room, HashSet<IntVec3>> item in value.adjacent)
			{
				item.Deconstruct(ref room3, ref hashSet);
				Room room4 = room3;
				HashSet<IntVec3> hashSet2 = hashSet;
				if (visited.Contains(room4) || frontier.Contains(room4))
				{
					continue;
				}
				foreach (IntVec3 item2 in hashSet2)
				{
					Building edifice = item2.GetEdifice(map);
					if (edifice == null || edifice is Building_Door { Open: not false })
					{
						exteriorCells[room4] = hashSet2;
						frontier.Enqueue(room4);
						break;
					}
				}
			}
		}
		visited.Clear();
		exteriorCells.Clear();
		frontier.Clear();
		warning = null;
		return false;
	}

	private void ExchangeRoomVacuum()
	{
		warningCells.Clear();
		warningMatrices.Clear();
		Room room = default(Room);
		GroupData groupData = default(GroupData);
		HashSet<IntVec3> hashSet = default(HashSet<IntVec3>);
		foreach (KeyValuePair<Room, GroupData> roomGroup in roomGroups)
		{
			roomGroup.Deconstruct(ref room, ref groupData);
			Room room2 = room;
			GroupData groupData2 = groupData;
			roomVacuums[room2] = room2.UnsanitizedVacuum;
			foreach (KeyValuePair<Room, HashSet<IntVec3>> item in groupData2.adjacent)
			{
				item.Deconstruct(ref room, ref hashSet);
				Room room3 = room;
				roomVacuums[room3] = room3.UnsanitizedVacuum;
			}
		}
		foreach (KeyValuePair<Room, GroupData> roomGroup2 in roomGroups)
		{
			roomGroup2.Deconstruct(ref room, ref groupData);
			Room room4 = room;
			GroupData groupData3 = groupData;
			if (groupData3.adjacent.Count == 0 || room4.IsDoorway)
			{
				continue;
			}
			float num = roomVacuums[room4];
			int num2 = room4.CellCount;
			float num3 = (float)room4.CellCount * num;
			int num4 = room4.CellCount;
			if (room4.ExposedToSpace)
			{
				continue;
			}
			foreach (KeyValuePair<Room, HashSet<IntVec3>> item2 in groupData3.adjacent)
			{
				item2.Deconstruct(ref room, ref hashSet);
				Room room5 = room;
				HashSet<IntVec3> hashSet2 = hashSet;
				num2 += room5.CellCount;
				num3 += (float)room5.CellCount * roomVacuums[room5];
				if (!room5.ExposedToSpace)
				{
					num4 += room5.CellCount;
					continue;
				}
				foreach (IntVec3 item3 in hashSet2)
				{
					if (warningCells.Add(item3))
					{
						warningMatrices.Add(Matrix4x4.Translate(item3.ToVector3ShiftedWithAltitude(AltitudeLayer.MapDataOverlay)));
					}
				}
			}
			float num5 = Mathf.Clamp(num3 / (float)num2, -0.1f, 1f);
			if (groupData3.hasDirectPathToVacuum)
			{
				if (room4.Vacuum >= 1f)
				{
					continue;
				}
				foreach (IntVec3 directWarning in groupData3.directWarnings)
				{
					if (warningCells.Add(directWarning))
					{
						warningMatrices.Add(Matrix4x4.Translate(directWarning.ToVector3ShiftedWithAltitude(AltitudeLayer.MapDataOverlay)));
					}
				}
				float num6 = Mathf.Min(0.1f / ((float)num4 / 100f), 1f);
				num5 = Mathf.Clamp(num5 + num6, -0.1f, 1f);
				float num7 = Mathf.Min(0.5f / ((float)num4 / 100f), 1f);
				float num8 = (num5 - num) * 1.0416666f * num7;
				room4.Vacuum = num + num8;
			}
			else if (!Mathf.Approximately(num, num5))
			{
				float num9 = Mathf.Min(0.5f / ((float)num4 / 100f), 1f);
				float num10 = (num5 - num) * 1.0416666f * num9;
				room4.Vacuum = num + num10;
			}
		}
		roomVacuums.Clear();
		Drawer.SetDirty();
	}

	private void RebuildData()
	{
		using (ProfilerBlock.Scope("VacuumComponent.RebuildData"))
		{
			dirty = false;
			ClearBuffer();
			MergeRoomsIntoGroups();
			RegenerateVacuumOverlay();
		}
	}

	public void Dirty()
	{
		dirty = true;
		SetDrawerDirty();
	}

	private void ClearBuffer()
	{
		Room room = default(Room);
		GroupData item = default(GroupData);
		foreach (KeyValuePair<Room, GroupData> roomGroup in roomGroups)
		{
			roomGroup.Deconstruct(ref room, ref item);
			FullPool<GroupData>.Return(item);
		}
		roomGroups.Clear();
	}

	private void MergeRoomsIntoGroups()
	{
		foreach (Room allRoom in map.regionGrid.AllRooms)
		{
			if (allRoom.ExposedToSpace)
			{
				continue;
			}
			GroupData orAddGroup = GetOrAddGroup(allRoom);
			if (allRoom.IsDoorway)
			{
				Building_Door door = allRoom.Cells.First().GetDoor(map);
				if (door == null || !door.ExchangeVacuum)
				{
					continue;
				}
				CellRect cellRect = door.OccupiedRect();
				foreach (IntVec3 item in cellRect)
				{
					for (int i = 0; i < 4; i++)
					{
						IntVec3 intVec = item + GenAdj.CardinalDirections[i];
						if (cellRect.Contains(intVec))
						{
							continue;
						}
						Room room = intVec.GetRoom(map);
						if (room != null && room != allRoom)
						{
							if (!orAddGroup.adjacent.TryGetValue(room, out var value))
							{
								HashSet<IntVec3> hashSet = (orAddGroup.adjacent[room] = SimplePool<HashSet<IntVec3>>.Get());
								value = hashSet;
							}
							value.Add(item);
						}
					}
				}
				continue;
			}
			foreach (IntVec3 borderCell in allRoom.BorderCells)
			{
				Building edifice = borderCell.GetEdifice(map);
				if (edifice == null || !edifice.ExchangeVacuum)
				{
					continue;
				}
				CellRect cellRect2 = edifice.OccupiedRect();
				foreach (IntVec3 item2 in cellRect2)
				{
					for (int j = 0; j < 4; j++)
					{
						IntVec3 intVec2 = item2 + GenAdj.CardinalDirections[j];
						if (cellRect2.Contains(intVec2))
						{
							continue;
						}
						Room room2 = intVec2.GetRoom(map);
						if (room2 != null && room2 != allRoom)
						{
							if (!orAddGroup.adjacent.TryGetValue(room2, out var value2))
							{
								HashSet<IntVec3> hashSet = (orAddGroup.adjacent[room2] = SimplePool<HashSet<IntVec3>>.Get());
								value2 = hashSet;
							}
							value2.Add(borderCell);
						}
					}
				}
			}
		}
		Room room3 = default(Room);
		GroupData groupData = default(GroupData);
		foreach (KeyValuePair<Room, GroupData> roomGroup in roomGroups)
		{
			roomGroup.Deconstruct(ref room3, ref groupData);
			Room room4 = room3;
			GroupData groupData2 = groupData;
			if (HasDirectPathToVacuum(room4, out var warning))
			{
				groupData2.hasDirectPathToVacuum = true;
				groupData2.directWarnings = warning;
			}
		}
	}

	private GroupData GetOrAddGroup(Room room)
	{
		if (roomGroups.TryGetValue(room, out var value))
		{
			return value;
		}
		GroupData groupData = FullPool<GroupData>.Get();
		roomGroups[room] = groupData;
		return groupData;
	}

	public void SetDrawerDirty()
	{
		Drawer.SetDirty();
	}

	private void RegenerateVacuumOverlay()
	{
		vacuumBoxMatrices.Clear();
		vacuumBoxIntensities.Clear();
		vacuumedCells.Clear();
		Room room = default(Room);
		GroupData groupData = default(GroupData);
		foreach (KeyValuePair<Room, GroupData> roomGroup in roomGroups)
		{
			roomGroup.Deconstruct(ref room, ref groupData);
			Room room2 = room;
			room2.vacuumOverlayDrawn = RoomVacuumVisible(room2);
			room2.vacuumOverlayRects = 0;
			if (!room2.vacuumOverlayDrawn)
			{
				continue;
			}
			float vacuum = room2.Vacuum;
			foreach (IntVec3 cell in room2.Cells)
			{
				vacuumedCells.Add(cell, vacuum);
			}
			foreach (Region region in room2.Regions)
			{
				foreach (CellRect item4 in region.EnumerateRectangleCover())
				{
					Vector3 centerVector = item4.CenterVector3;
					centerVector.y = AltitudeLayer.Gas.AltitudeFor();
					Matrix4x4 item = Matrix4x4.TRS(centerVector, Quaternion.identity, new Vector3(item4.Width, 1f, item4.Height));
					room2.vacuumOverlayRects++;
					vacuumBoxMatrices.Add(item);
					vacuumBoxIntensities.Add(vacuum);
				}
			}
		}
		vacuumTransitionMatrices.Clear();
		vacuumTransitionIntensities.Clear();
		IntVec3 intVec = default(IntVec3);
		float num = default(float);
		foreach (KeyValuePair<IntVec3, float> vacuumedCell in vacuumedCells)
		{
			vacuumedCell.Deconstruct(ref intVec, ref num);
			IntVec3 intVec2 = intVec;
			float item2 = num;
			for (int i = 0; i < 4; i++)
			{
				IntVec3 key = intVec2 + GenAdj.CardinalDirections[i];
				if (!vacuumedCells.ContainsKey(key))
				{
					Matrix4x4 item3 = Matrix4x4.TRS(key.ToVector3ShiftedWithAltitude(AltitudeLayer.Gas), new Rot4(i).AsQuat, Vector3.one);
					vacuumTransitionMatrices.Add(item3);
					vacuumTransitionIntensities.Add(item2);
				}
			}
		}
	}

	private void UpdateVacuumOverlay()
	{
		int num = 0;
		Room room = default(Room);
		GroupData groupData = default(GroupData);
		foreach (KeyValuePair<Room, GroupData> roomGroup in roomGroups)
		{
			roomGroup.Deconstruct(ref room, ref groupData);
			Room room2 = room;
			if (RoomVacuumVisible(room2) != room2.vacuumOverlayDrawn)
			{
				RegenerateVacuumOverlay();
				break;
			}
			for (int i = 0; i < room2.vacuumOverlayRects; i++)
			{
				vacuumBoxIntensities[num++] = room2.Vacuum;
			}
		}
	}

	private static bool RoomVacuumVisible(Room room)
	{
		if (!room.ExposedToSpace && room.UnsanitizedVacuum > 0.5f)
		{
			if (room.IsDoorway)
			{
				return room.Door.Open;
			}
			return true;
		}
		return false;
	}

	public override void MapComponentDraw()
	{
		if (!map.Biome.inVacuum)
		{
			return;
		}
		if (Find.PlaySettings.showVacuumOverlay && !WorldComponent_GravshipController.GravshipRenderInProgess)
		{
			Drawer.MarkForDraw();
			if (warningMatrices.Count > 0)
			{
				Graphics.DrawMeshInstanced(MeshPool.plane08, 0, WarningMat, warningMatrices);
			}
		}
		using (ProfilerBlock.Scope("VacuumComponent.Drawer.CellBoolDrawerUpdate()"))
		{
			Drawer.CellBoolDrawerUpdate();
		}
		if (WorldComponent_GravshipController.CutsceneInProgress)
		{
			return;
		}
		if (vacuumBoxMatrices.Count > 0)
		{
			if (vacuumBoxPropBlock == null)
			{
				vacuumBoxPropBlock = new MaterialPropertyBlock();
			}
			vacuumBoxPropBlock.Clear();
			if (vacuumBoxIntensities.Count > 0)
			{
				vacuumBoxPropBlock.SetFloatArray(VacuumPercent, vacuumBoxIntensities);
			}
			Graphics.DrawMeshInstanced(MeshPool.plane10, 0, VacuumOverlayMat0, vacuumBoxMatrices, vacuumBoxPropBlock);
			Graphics.DrawMeshInstanced(MeshPool.plane10, 0, VacuumOverlayMat1, vacuumBoxMatrices, vacuumBoxPropBlock);
		}
		if (vacuumTransitionMatrices.Count > 0)
		{
			if (vacuumTransitionPropBlock == null)
			{
				vacuumTransitionPropBlock = new MaterialPropertyBlock();
			}
			vacuumTransitionPropBlock.Clear();
			if (vacuumTransitionIntensities.Count > 0)
			{
				vacuumTransitionPropBlock.SetFloatArray(VacuumPercent, vacuumTransitionIntensities);
			}
			Graphics.DrawMeshInstanced(MeshPool.plane10, 0, VacuumOverlayTransitionMat0, vacuumTransitionMatrices, vacuumTransitionPropBlock);
			Graphics.DrawMeshInstanced(MeshPool.plane10, 0, VacuumOverlayTransitionMat1, vacuumTransitionMatrices, vacuumTransitionPropBlock);
		}
	}

	public bool GetCellBool(int index)
	{
		IntVec3 c = map.cellIndices.IndexToCell(index);
		Building edifice = c.GetEdifice(map);
		if (!c.Fogged(map))
		{
			return edifice?.ExchangeVacuum ?? true;
		}
		return false;
	}

	public Color GetCellExtraColor(int index)
	{
		float vacuum = map.cellIndices.IndexToCell(index).GetVacuum(map);
		if (vacuum < 0.5f)
		{
			return Color.Lerp(Color.green, Color.yellow, Mathf.InverseLerp(0f, 0.5f, vacuum));
		}
		return Color.Lerp(Color.yellow, Color.red, Mathf.InverseLerp(0.5f, 1f, vacuum));
	}

	public override void ExposeData()
	{
		if (Scribe.mode == LoadSaveMode.PostLoadInit)
		{
			InitializeIfRequired();
		}
	}

	public void Dispose()
	{
		ClearBuffer();
	}
}
