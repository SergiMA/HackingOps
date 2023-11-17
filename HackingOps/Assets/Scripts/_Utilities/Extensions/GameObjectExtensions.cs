using UnityEngine;

namespace HackingOps.Utilities.Extensions
{
    public static class GameObjectExtensions
    {
        public static GameObject FindOrCreate(this GameObject g, string objectName)
        {
            GameObject objectFound = GameObject.Find(objectName);

            if (objectFound != null)
                return objectFound;

            GameObject newObject = new GameObject(objectName);
            newObject.name = objectName;

            return newObject;
        }

        public static T GetOrAdd<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (!component) component = gameObject.AddComponent<T>();
            return component;
        }
    }
}