using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

public class Level
{
    public string levelName = "New Level";

    public int levelID;

    public int version;

    public float difficulty;

    public string dateCreated;
    public string dateModified;

    public Slot[] slots;

    //public LevelSolution solution;

    [NonSerialized]
    public Dictionary<Vector3, Slot> map;

    [NonSerialized]
    public bool modified;

    [NonSerialized]
    public bool isInitialized;

    [NonSerialized]
    public bool isSolvedInEditor;

    public Level(int columns, int rows)
    {
        map = new Dictionary<Vector3, Slot>();

        dateCreated = DateTime.UtcNow.ToString();
    }

    public void AddSlot(Slot slot)
    {
        if (!map.ContainsKey(slot.position))
        {
            map.Add(slot.position, slot);
        }
    }

    public Slot GetSlot(Vector3 position)
    {
        Slot slot;
        if (map.TryGetValue(position, out slot))
        {
            return slot;
        }

        return null;
    }

    public void Initialize(bool reinitialize = false)
    {
        if (isInitialized && !reinitialize)
        {
            return;
        }

        if (map == null)
        {
            map = new Dictionary<Vector3, Slot>();
        }

        foreach (var slot in slots)
        {
            AddSlot(slot);
        }

        //Add neighbours
        foreach (var slot in map.Values)
        {
            slot.neighbours = new HashSet<Slot>();

            foreach (var direction in VectorExtensions.directions)
            {
                var pos = slot.position + direction;

                Slot neighbour;
                if (map.TryGetValue(pos, out neighbour))
                {
                    slot.AddNeighbour(neighbour); //will be checked in add neighbour
                }
            }
        }

        isInitialized = true;
    }
}