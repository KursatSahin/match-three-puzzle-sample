using System.Collections.Generic;
using UnityEngine;

namespace Core.Utils
{
    public static class ZUtils
    {
        public static bool HasComponent<T> (this GameObject obj)
        {
            return obj.GetComponent(typeof(T)) != null;
        }
    }
}