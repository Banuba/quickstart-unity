using System;
using System.Collections.Generic;
using UnityEngine;

namespace BNB
{
    [Serializable]
    public class ResourcesJSON
    {
        public List<string> resources = new List<string>();

        public static ResourcesJSON CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<ResourcesJSON>(jsonString);
        }

        public string SaveToString()
        {
            return JsonUtility.ToJson(this);
        }
    }
}
