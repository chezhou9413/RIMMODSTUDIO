using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace CharacterEditor;

internal class Capturer
{
	internal bool bUpdateGraphics;

	internal bool bNude;

	internal bool bHats;

	private int iCurrentRotation;

	private List<Rot4> lRotation;

	private RenderTexture image;

	private const int imageW = 200;

	private const int imageH = 280;

	internal int renderW = 500;

	internal int renderH = 700;

	private Vector2 v2 = new Vector2(200f, 280f);

	internal RenderTextureFormat renderTextureFormat = (RenderTextureFormat)0;

	internal Pawn Pawn { get; set; }

	internal Capturer()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		image = null;
		lRotation = new List<Rot4>
		{
			Rot4.South,
			Rot4.West,
			Rot4.North,
			Rot4.East
		};
		iCurrentRotation = 0;
		bNude = true;
		bHats = true;
		bUpdateGraphics = false;
	}

	internal void RotateAndCapture(Pawn pawn)
	{
		iCurrentRotation = lRotation.NextOrPrevIndex(iCurrentRotation, next: true, random: false);
		UpdatePawnGraphic(pawn);
	}

	internal void ToggleNudeAndCapture(Pawn pawn)
	{
		bNude = !bNude;
		UpdatePawnGraphic(pawn);
	}

	internal void ToggleHatAndCapture(Pawn pawn)
	{
		bHats = !bHats;
		UpdatePawnGraphic(pawn);
	}

	internal void UpdatePawnGraphic(Pawn pawn)
	{
		pawn.SetDirty();
		bUpdateGraphics = true;
		image = GetRenderTexture(pawn, fromCache: false);
	}

	internal RenderTexture GetRenderTexture(Pawn pawn, bool fromCache)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected O, but got Unknown
		if (pawn == null)
		{
			return null;
		}
		if (fromCache)
		{
			return PortraitsCache.Get(pawn, v2, lRotation[iCurrentRotation]);
		}
		if ((Object)(object)image == (Object)null)
		{
			image = new RenderTexture(renderW, renderH, 32, renderTextureFormat);
		}
		if (bUpdateGraphics)
		{
			PrepareForRender(pawn);
			Render(pawn);
			bUpdateGraphics = false;
		}
		return image;
	}

	internal void ChangeRenderTextureParamter(int resolution, bool isARGB)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		renderW = resolution;
		renderH = (int)((double)renderW * 1.4);
		renderTextureFormat = (RenderTextureFormat)((!isARGB) ? 9 : 0);
		if ((Object)(object)image != (Object)null)
		{
			image.Release();
		}
		image = new RenderTexture(renderW, renderH, 32, renderTextureFormat);
	}

	internal static void PrepareForRender(Pawn pawn)
	{
		try
		{
			PortraitsCache.SetDirty(pawn);
			pawn.Drawer.renderer.EnsureGraphicsInitialized();
		}
		catch
		{
		}
	}

	internal void Render(Pawn pawn)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		float angle = 0f;
		Vector3 positionOffset = default(Vector3);
		if (pawn.Dead || pawn.Downed)
		{
			angle = 85f;
			positionOffset.x -= 0.18f;
			positionOffset.z -= 0.18f;
		}
		try
		{
			PawnCacheCameraManager.PawnCacheRenderer.RenderPawn(pawn, image, Vector3.zero, 1f, angle, lRotation[iCurrentRotation], renderHead: true, renderHeadgear: true, renderClothes: true, portrait: true, positionOffset);
		}
		catch (Exception ex)
		{
			Log.Error(ex.StackTrace);
		}
	}

	internal void DrawPawnImage(Pawn pawn, int x, int y)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		image = GetRenderTexture(pawn, fromCache: false);
		if ((Object)(object)image != (Object)null)
		{
			GUI.DrawTexture(new Rect((float)x, (float)y, 200f, 280f), (Texture)(object)image, (ScaleMode)2);
		}
	}
}
