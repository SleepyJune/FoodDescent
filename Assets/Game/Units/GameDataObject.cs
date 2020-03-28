using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class GameDataObject : ScriptableObject, IGameDataObject
{
    public new string name;

    public int id;

    public int GetID()
    {
        return id;
    }

    public void SetID(int id)
    {
        this.id = id;
    }

    public string GetName()
    {
        return name;
    }
}