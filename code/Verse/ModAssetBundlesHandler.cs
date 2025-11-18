using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Verse;

public class ModAssetBundlesHandler
{
	private ModContentPack mod;

	public List<AssetBundle> loadedAssetBundles = new List<AssetBundle>();

	public static readonly string[] TextureExtensions = new string[4] { ".png", ".psd", ".jpg", ".jpeg" };

	public static readonly string[] AudioClipExtensions = new string[3] { ".wav", ".mp3", ".ogg" };

	public static readonly string[] ShaderExtensions = new string[1] { ".shader" };

	public const string LinuxBundleSuffix = "_linux";

	public const string MacBundleSuffix = "_mac";

	public const string WindowsBundleSuffix = "_win";

	private string BundleSuffixForCurrentOs
	{
		get
		{
			switch (Application.platform)
			{
			case RuntimePlatform.LinuxPlayer:
			case RuntimePlatform.LinuxEditor:
				return "_linux";
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.OSXPlayer:
				return "_mac";
			case RuntimePlatform.WindowsPlayer:
			case RuntimePlatform.WindowsEditor:
				return "_win";
			default:
				throw new NotSupportedException($"Unsupported platform for asset bundle loading: {Application.platform}");
			}
		}
	}

	public ModAssetBundlesHandler(ModContentPack mod)
	{
		this.mod = mod;
	}

	public void ReloadAll(bool hotReload = false)
	{
		string text = default(string);
		FileInfo fileInfo = default(FileInfo);
		foreach (KeyValuePair<string, FileInfo> item in ModContentPack.GetAllFilesForMod(mod, GenFilePaths.ContentPath<AssetBundle>(), IsAcceptableExtension))
		{
			item.Deconstruct(ref text, ref fileInfo);
			FileInfo fileInfo2 = fileInfo;
			string bundleNameWithoutOsSpecifier = GetBundleNameWithoutOsSpecifier(fileInfo2);
			if (bundleNameWithoutOsSpecifier != null && !(fileInfo2.Name.Replace(bundleNameWithoutOsSpecifier, "") == BundleSuffixForCurrentOs))
			{
				continue;
			}
			AssetBundle assetBundle = AssetBundle.LoadFromFile(fileInfo2.FullName);
			if (assetBundle != null)
			{
				if (!loadedAssetBundles.Contains(assetBundle))
				{
					loadedAssetBundles.Add(assetBundle);
				}
			}
			else
			{
				Log.Error("Could not load asset bundle at " + fileInfo2.FullName);
			}
		}
	}

	private bool IsAcceptableExtension(string extension)
	{
		if (!extension.NullOrEmpty())
		{
			return false;
		}
		return true;
	}

	private string GetBundleNameWithoutOsSpecifier(FileInfo file)
	{
		string name = file.Name;
		if (name.EndsWith("_linux"))
		{
			string text = name;
			int length = "_linux".Length;
			return text.Substring(0, text.Length - length);
		}
		if (name.EndsWith("_mac"))
		{
			string text = name;
			int length = "_mac".Length;
			return text.Substring(0, text.Length - length);
		}
		if (name.EndsWith("_win"))
		{
			string text = name;
			int length = "_win".Length;
			return text.Substring(0, text.Length - length);
		}
		return null;
	}

	public void ClearDestroy()
	{
		LongEventHandler.ExecuteWhenFinished(delegate
		{
			for (int i = 0; i < loadedAssetBundles.Count; i++)
			{
				loadedAssetBundles[i].Unload(unloadAllLoadedObjects: true);
			}
			loadedAssetBundles.Clear();
		});
	}
}
