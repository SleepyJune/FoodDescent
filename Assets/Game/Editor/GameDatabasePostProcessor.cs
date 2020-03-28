using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

using UnityEngine;
using UnityEditor;

using System.Linq;

class GameDatabasePostProcessor : AssetPostprocessor
{
    static GameDatabase database;

    class AssetInfo
    {
        public string[] importedAssets;
        public string[] deletedAssets;
        public string[] movedAssets;
        public string[] movedFromAssetPaths;

        public HashSet<string> importedSet = new HashSet<string>();

        public void MakeDictionary()
        {
            importedAssets.ToList().ForEach(x => importedSet.Add(x));
        }
    }

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        if (database == null)
        {
            database = (GameDatabase)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/GameDataObjects/GameDatabase.asset", typeof(GameDatabase));
        }
                
        var info = new AssetInfo()
        {
            importedAssets = importedAssets,
            deletedAssets = deletedAssets,
            movedAssets = movedAssets,
            movedFromAssetPaths = movedFromAssetPaths,
        };

        info.MakeDictionary();

        CheckModified("/Prefabs/GameDataObjects/", ref database.allObjects, "*.asset", info);

        CheckModified("/Prefabs/Units/", ref database.allPrefabs, "*.prefab", info);

        /*if (mapDatabase == null)
        {
            mapDatabase = (MapDatabase)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/MapDataObjects/MapDatabase.asset", typeof(MapDatabase));
        }

        CheckModified("/Prefabs/MapDataObjects/", ref mapDatabase.allObjects, importedAssets, deletedAssets);*/
    }

    static void CheckModified<T>(string path, ref T[] collection, string searchPattern, AssetInfo info) where T : UnityEngine.Object, IGameDataObject
    {
        bool databaseModified = false;

        foreach (string str in info.importedAssets)
        {
            //Debug.Log("Reimported Asset: " + str);

            if (str.StartsWith("Assets" + path))
            {
                databaseModified = true;
                break;
            }

        }

        if (!databaseModified)
        {
            foreach (string str in info.deletedAssets)
            {
                //Debug.Log("Deleted Asset: " + str);

                if (str.StartsWith("Assets" + path))
                {
                    databaseModified = true;
                    break;
                }

            }
        }

        if (databaseModified)
        {
            EditorHelperFunctions.GenerateFromAsset(path, ref collection, database, searchPattern);

            CheckName(collection, info);
            CheckCollection(collection, info);
        }
    }

    static void CheckName<T>(T[] collection, AssetInfo info) where T : UnityEngine.Object, IGameDataObject
    {
        foreach (var item in collection)
        {
            var path = AssetDatabase.GetAssetPath(item);

            if (info.importedSet.Contains(path))
            {
                var pathName = System.IO.Path.GetFileNameWithoutExtension(path);

                if (pathName != item.GetName())
                {
                    Debug.Log("Auto set filename: " + item.GetName());
                    AssetDatabase.RenameAsset(path, item.GetName());
                }
            }
        }
        
    }

    static void CheckCollection<T>(T[] collection, AssetInfo info) where T : UnityEngine.Object, IGameDataObject
    {
        int counter = collection.Max(i => i.GetID()) + 1;               

        HashSet<int> itemID = new HashSet<int>();
        HashSet<T> checkItems = new HashSet<T>();

        for(int i = collection.Length - 1; i >= 0; i--)
        {
            var item = collection[i];

            var path = AssetDatabase.GetAssetPath(item);

            if (info.importedSet.Contains(path))
            {
                checkItems.Add(item);
            }
        }

        foreach (var item in collection)
        {
            foreach(var checkItem in checkItems)
            {
                if(item.GetID() == checkItem.GetID() || item.GetID() == 0)
                {
                    var path = AssetDatabase.GetAssetPath(item);
                    var path2 = AssetDatabase.GetAssetPath(checkItem);

                    if(path != path2)
                    {
                        checkItem.SetID(counter);
                        counter += 1;
                        EditorUtility.SetDirty(checkItem);

                        Debug.Log("Changed id: " + checkItem.name);
                    }
                }
            }

            if (itemID.Contains(item.GetID()) || item.GetID() == 0)
            {
                if(item.GetID() == 0)
                {
                    item.SetID(counter);
                    counter += 1;
                    EditorUtility.SetDirty(item);

                    Debug.Log("Changed id: " + item.name);
                }
                else
                {
                    Debug.Log("GameDataObject with the same id: " + item.name);                
                }
            }
            else
            {
                itemID.Add(item.GetID());
            }
        }
    }

}