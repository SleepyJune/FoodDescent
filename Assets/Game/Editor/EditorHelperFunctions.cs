using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

using UnityEngine;
using UnityEditor;

public static class EditorHelperFunctions
{
    public static void DeleteAllSubAssets(UnityEngine.Object obj)
    {
        var path = AssetDatabase.GetAssetPath(obj);
        var assets = AssetDatabase.LoadAllAssetsAtPath(path);

        foreach (var asset in assets)
        {
            if (asset.name != obj.name)
            {
                GameObject.DestroyImmediate(asset, true);
            }
        }
    }

    public static void GenerateFromAsset<T>(string path, ref T[] collection, Object target, string assetType = "*.asset")
    {
        DeleteAllSubAssets(target);

        List<T> newList = new List<T>();

        var files = Directory.GetFiles(Application.dataPath + path, assetType, SearchOption.AllDirectories);
        foreach (var file in files)
        {
            string assetPath = "Assets" + file.Replace(Application.dataPath, "").Replace('\\', '/');

            T asset = (T)(object)AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));
            if (asset != null)
            {
                newList.Add(asset);
            }
        }

        collection = newList.ToArray();
        EditorUtility.SetDirty(target);
    }

    public static void Generate<T>(ref T[] collection, Object target)
    {
        List<T> newList = new List<T>();

        var types = System.AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(T));

        var newObject = new GameObject();
        newObject.name = typeof(T).ToString();

        foreach (var type in types)
        {
            var newComponent = newObject.AddComponent(type);
            newList.Add((T)((object)newComponent));
        }

        newObject.transform.parent = ((MonoBehaviour)target).transform;

        collection = newList.ToArray();
        EditorUtility.SetDirty(target);
    }

    public static void GenerateFromVariable<T>(string path, ref T[] collection, ref string[] nameCollection, Object target, string variableName, string assetType = "*.asset")
    {
        List<T> newList = new List<T>();
        List<string> nameList = new List<string>();

        var files = Directory.GetFiles(Application.dataPath + path, assetType, SearchOption.AllDirectories);
        foreach (var file in files)
        {
            string assetPath = "Assets" + file.Replace(Application.dataPath, "").Replace('\\', '/');

            Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));
            if (asset != null)
            {
                var targetField = asset.GetType().GetField(variableName);

                if (targetField != null)
                {
                    var targetValue = (T)targetField.GetValue(asset);
                    newList.Add(targetValue);
                    nameList.Add(asset.name);
                }
            }
        }

        collection = newList.ToArray();
        nameCollection = nameList.ToArray();
        EditorUtility.SetDirty(target);
    }

    /*public static void GenerateNames(string path, ref string[] collection, Object target, string assetType = "*.asset")
    {
        List<string> newList = new List<string>();

        var files = Directory.GetFiles(Application.dataPath + path, assetType, SearchOption.AllDirectories);
        foreach (var file in files)
        {
            string assetPath = "Assets" + file.Replace(Application.dataPath, "").Replace('\\', '/');

            Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));
            if (asset != null)
            {
                newList.Add(asset.name);
            }
        }

        collection = newList.ToArray();
        EditorUtility.SetDirty(target);
    }*/
}

public static class ReflectionHelpers
{
    public static System.Type[] GetAllDerivedTypes(this System.AppDomain aAppDomain, System.Type aType)
    {
        var result = new List<System.Type>();
        var assemblies = aAppDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.IsSubclassOf(aType))
                    result.Add(type);
            }
        }
        return result.ToArray();
    }
}
