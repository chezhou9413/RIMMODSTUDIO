using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ChezhouLib.ALLmap
{
    public static class abDatabase
    {
        public static Dictionary<string, AssetBundle> abDataBase = new Dictionary<string, AssetBundle>();
        public static Dictionary<string,Shader> shaderDataBase = new Dictionary<string, Shader>();
        public static Dictionary<string, EffData> EffObjDataBase = new Dictionary<string, EffData>();
    }

    public class EffData
    {
        public string ModId;
        public GameObject EffObj;
    }
}
