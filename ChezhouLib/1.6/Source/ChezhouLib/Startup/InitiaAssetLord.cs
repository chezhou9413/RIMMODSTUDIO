using ChezhouLib.ALLmap;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Verse;

namespace ChezhouLib.Startup
{
    public class ChezhouLib : Mod
    {
        public ChezhouLib(ModContentPack content) : base(content)
        {
        }
    }

    [StaticConstructorOnStartup]
    public static class InitiaAssetLord
    {
        static InitiaAssetLord()
        {
            try
            {
                string abPath = Path.Combine(
                    LoadedModManager.GetMod<ChezhouLib>().Content.RootDir,
                    "1.6",
                    "AssetBundles"
                );

                if (!Directory.Exists(abPath))
                    return;

                string[] abFiles = Directory.GetFiles(abPath, "*.ab", SearchOption.AllDirectories);
                if (abFiles.Length == 0)
                    return;

                List<AssetBundle> loadedBundles = new List<AssetBundle>();

                foreach (string file in abFiles)
                {
                    try
                    {
                        AssetBundle bundle = AssetBundle.LoadFromFile(file);
                        if (bundle != null)
                        {
                            loadedBundles.Add(bundle);
                            string[] allAssetNames = bundle.GetAllAssetNames();

                            foreach (string assetPath in allAssetNames)
                            {
                                if (assetPath.EndsWith(".shader", StringComparison.OrdinalIgnoreCase))
                                {
                                    Shader shader = bundle.LoadAsset<Shader>(assetPath);
                                    if (shader != null)
                                    {
                                        string key = Path.GetFileNameWithoutExtension(assetPath).ToUpper();
                                        abDatabase.shaderDataBase[key] = shader;
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        [StaticConstructorOnStartup]
        [HarmonyPatch(typeof(ShaderDatabase))]
        [HarmonyPatch(nameof(ShaderDatabase.LoadShader), new Type[] { typeof(string) })]
        public static class ShaderDatabase_LoadShader_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(ref Shader __result, string shaderPath)
            {
                try
                {
                    if (shaderPath.Contains("Custom"))
                    {
                        string[] parts = shaderPath.Split('/');
                        if (parts.Length > 1 &&
                            abDatabase.shaderDataBase.TryGetValue(parts[1].ToUpper(), out var shader) &&
                            shader != null)
                        {
                            __result = shader;
                        }
                    }
                }
                catch { }
            }
        }

        [StaticConstructorOnStartup]
        [HarmonyPatch(typeof(ShaderDatabase))]
        public static class ShaderDatabase_TryLoadShader_Patch
        {
            static MethodBase TargetMethod()
            {
                return AccessTools.Method(typeof(ShaderDatabase), "TryLoadShader",
                    new Type[] { typeof(string), typeof(Shader).MakeByRefType() });
            }

            [HarmonyPostfix]
            public static void Postfix(string shaderPath, ref Shader result, ref bool __result)
            {
                try
                {
                    if (shaderPath.Contains("Custom"))
                    {
                        string[] parts = shaderPath.Split('/');
                        if (parts.Length > 1 &&
                            abDatabase.shaderDataBase.TryGetValue(parts[1].ToUpper(), out var customShader) &&
                            customShader != null)
                        {
                            result = customShader;
                            __result = true;
                        }
                    }
                }
                catch { }
            }
        }
    }
}
