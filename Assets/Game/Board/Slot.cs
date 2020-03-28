using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
public class Slot
{
    public FoodObject food;

    public Hex hexPosition;
    public Vector3 position;

    public HashSet<Slot> neighbours = new HashSet<Slot>();

    public bool isDish;

    public int foodRank = 0;

    public Slot(FoodObject food)
    {
        this.food = food;
        this.hexPosition = new Hex(0, 0);
        this.position = Vector3.zero;
        this.isDish = food.isDish;
    }

    public Slot(FoodObject food, Hex position)
    {
        this.food = food;
        this.hexPosition = position;
        this.position = position.ConvertCube();
        this.isDish = food.isDish;
    }

    public Slot(Vector3 position)
    {
        this.position = position;
    }

    public void AddNeighbour(Slot neighbour)
    {
        if (neighbour.food != null) //!neighbours.Contains(neighbour) && this != neighbour)
        {
            neighbours.Add(neighbour);
        }
    }

    public bool isNeighbour(Slot slot)
    {
        if (neighbours.Contains(slot))
        {
            return true;
        }

        return false;
    }

    public static bool operator ==(Slot value1, Slot value2)
    {
        if (object.ReferenceEquals(value1, null))
        {
            return object.ReferenceEquals(value2, null);
        }

        if (object.ReferenceEquals(value2, null))
        {
            return object.ReferenceEquals(value1, null);
        }

        return value1.position == value2.position;
    }

    public static bool operator !=(Slot value1, Slot value2)
    {
        return !(value1 == value2);
    }

    public override bool Equals(object obj)
    {
        return (obj is Slot) ? this == (Slot)obj : false;
    }

    public bool Equals(Slot other)
    {
        return this == other;
    }

    public override int GetHashCode()
    {
        return position.GetHashCode();
    }
}
