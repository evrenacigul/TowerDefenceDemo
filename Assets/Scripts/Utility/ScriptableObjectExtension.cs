using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public static class ScriptableObjectExtension
    {
        public static T Clone<T>(this T scriptableObject) where T : ScriptableObject
        {
            if (scriptableObject == null)
            {
                return (T)ScriptableObject.CreateInstance(typeof(T));
            }

            T instance = Object.Instantiate(scriptableObject);
            instance.name = scriptableObject.name;
            return instance;
        }
    }
}