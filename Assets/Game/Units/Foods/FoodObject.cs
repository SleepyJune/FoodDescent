using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Food Object")]
public class FoodObject : GameDataObject
{
    public Sprite icon;

    public int energy;

    [System.NonSerialized]
    public bool isDish = false;
    
    /*public ExitGames.Client.Photon.Hashtable EventObject()
    {
        var eventData = new ExitGames.Client.Photon.Hashtable();

        eventData.Add("id", id);
        eventData.Add("energy", energy);

        return eventData;
    }*/
}
