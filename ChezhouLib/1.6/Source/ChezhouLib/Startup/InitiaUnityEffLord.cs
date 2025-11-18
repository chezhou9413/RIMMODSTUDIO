using ChezhouLib.ALLmap;
using ChezhouLib.LibDef;
using ChezhouLib.MonoComp;
using ChezhouLib.ObjectPool;
using UnityEngine;
using Verse;

namespace ChezhouLib.Startup
{
    [StaticConstructorOnStartup]
    public static class InitiaUnityEffLord
    {
        static InitiaUnityEffLord()
        {
            foreach (UnityEffLord UnityEffDef in DefDatabase<UnityEffLord>.AllDefs)
            {
                string modRootDir = UnityEffDef.modContentPack.RootDir;
                string abPath = modRootDir + UnityEffDef.AbPath;
                AssetBundle ab = AssetBundle.LoadFromFile(abPath);
                if (ab == null)
                {
                    Log.Warning(abPath);
                    continue;
                }
                string targetPath = ("assets" + UnityEffDef.EffPrefabFolderName).ToLower();
                foreach (string name in ab.GetAllAssetNames())
                {
                    if (name.StartsWith(targetPath) && name.EndsWith(".prefab"))
                    {
                        GameObject prefab = ab.LoadAsset<GameObject>(name);
                        prefab.AddComponent<EffAutoRecycle>();
                        EffData effData = new EffData
                        {
                            ModId = UnityEffDef.ModId,
                            EffObj = prefab,
                        };
                        abDatabase.EffObjDataBase.Add(UnityEffDef.ModId + "_" + prefab.name, effData);
                    }
                }
            }
            UnityEffPool.Register(abDatabase.EffObjDataBase);
        }
    }
}
