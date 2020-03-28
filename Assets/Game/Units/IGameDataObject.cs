using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public interface IGameDataObject
{
    int GetID();
    void SetID(int id);

    string GetName();
}
