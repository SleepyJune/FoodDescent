using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Path
{
    public List<PathSlot> waypoints;

    public HashSet<Slot> waypointsHash;

    public PathSlot lastPoint;
    public PathSlot startPoint;

    public Path(Slot startPoint)
    {
        waypoints = new List<PathSlot>();
        waypointsHash = new HashSet<Slot>();

        var pathSlot = new PathSlot(startPoint);

        this.startPoint = pathSlot;
        this.lastPoint = pathSlot;

        waypoints.Add(pathSlot);
        waypointsHash.Add(startPoint);
    }

    public Path(Path pathClone)
    {
        waypoints = new List<PathSlot>();
        waypoints.AddRange(pathClone.waypoints);

        waypointsHash = new HashSet<Slot>(pathClone.waypointsHash);
        
        startPoint = pathClone.startPoint;
        lastPoint = pathClone.lastPoint;
    }

    public bool AddPoint(Slot slot)
    {
        var pathSlot = new PathSlot(slot);

        if (!waypointsHash.Contains(slot))
        {
            if (!lastPoint.slot.isNeighbour(slot))
            {                
                return false;
            }
          
            pathSlot.previous = lastPoint;

            lastPoint.next = pathSlot;

            waypoints.Add(pathSlot);
            waypointsHash.Add(pathSlot.slot);

            lastPoint = pathSlot;

            return true;
        }

        return false;
    }

    public bool GoBack()
    {
        return RemovePoint(GetLastPoint());
    }

    public bool RemovePoint(PathSlot pathSlot)
    {
        if (waypointsHash.Contains(pathSlot.slot))
        {
            waypoints.Remove(pathSlot);
            waypointsHash.Remove(pathSlot.slot);
            lastPoint = waypoints.LastOrDefault();

            return true;
        }

        return false;
    }

    public PathSlot GetLastPoint()
    {
        return lastPoint;
    }

    public PathSlot GetPreviousPoint()
    {
        return lastPoint.previous;
    }

    public bool isSolution(Level level)
    {
        return waypoints.Count == level.slots.Length;
    }
}
