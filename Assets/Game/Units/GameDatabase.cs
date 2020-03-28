using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(menuName = "Game/GameDatabase")]

public class GameDatabase : ScriptableObject
{
    public GameDataObject[] allObjects = new GameDataObject[0];

    public GameDataPrefab[] allPrefabs = new GameDataPrefab[0];
}