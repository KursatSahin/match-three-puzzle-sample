﻿using UnityEngine;

namespace Utils
{
    public static class Utils
    {
        public static bool HasComponent<T> (this GameObject obj)
        {
            return obj.GetComponent(typeof(T)) != null;
        }
    }
}