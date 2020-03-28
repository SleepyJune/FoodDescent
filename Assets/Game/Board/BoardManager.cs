using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
public class BoardManager : MonoBehaviour
{
    public Dictionary<Vector3, UISlot> uiSlots;

    public Transform slotParent;
    public UISlot uiSlotPrefab;

    float xMin = 9999, xMax = 0, yMin = 9999, yMax = 0;
    float slotSize = 40;

    public Dictionary<Vector3, UISlot> destroyedSlots;

    void Start()
    {
        uiSlots = new Dictionary<Vector3, UISlot>();
        destroyedSlots = new Dictionary<Vector3, UISlot>();

        MakeRandomBoard();
    }

    public void ClearBoard()
    {
        uiSlots = new Dictionary<Vector3, UISlot>();
        destroyedSlots = new Dictionary<Vector3, UISlot>();

        foreach (Transform child in slotParent)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void MakeRandomBoard()
    {
        ClearBoard();

        var gridColumns = 7;
        var gridRows = 7;

        var maxWidth = 9999;
        var maxHeight = 9999;

        slotSize = uiSlotPrefab.GetComponent<RectTransform>().sizeDelta.x / 2f;

        int xHalf = (int)Math.Floor(gridColumns / 2f);

        for (int row = (int)Math.Round(-gridRows / 2f); row < gridRows; row++)
        {
            for (int column = (int)Math.Round(-gridColumns / 2f); column < gridColumns; column++)
            {
                var hex = new Hex(column, row);

                var worldPos = hex.GetWorldPos(slotSize);

                if (worldPos.x < 0 || worldPos.y < 0 || worldPos.x > maxWidth || worldPos.y > maxHeight)
                {
                    continue;
                }

                //trim top right
                if(hex.col > xHalf && hex.row >= xHalf + gridColumns - hex.col)
                {
                    continue;
                }

                //trim bottom left
                if(hex.col < xHalf && gridColumns - hex.row >= gridColumns + hex.col - 1)
                {
                    continue;
                }

                //trim bottom right
                if (hex.col > xHalf && (gridColumns - hex.col - xHalf) - hex.row >= gridColumns - hex.col - 1)
                {
                    continue;
                }

                var randomFood = GameManager.instance.foodManager.GenerateRandomFood();

                var slot = new Slot(randomFood, hex);

                var newUISlot = Instantiate(uiSlotPrefab, slotParent);
                newUISlot.Initialize(slot, slotSize);

                uiSlots.Add(slot.position, newUISlot);

                SetBoardDimension(newUISlot);
            }
        }

        SetNeighbours();

        AdjustBoard();
    }

    void SetNeighbours()
    {
        //Add neighbours
        foreach (var uiSlot in uiSlots.Values)
        {
            var slot = uiSlot.slot;

            slot.neighbours = new HashSet<Slot>();

            foreach (var direction in VectorExtensions.directions)
            {
                var pos = uiSlot.slot.position + direction;

                UISlot neighbour;
                if (uiSlots.TryGetValue(pos, out neighbour))
                {
                    slot.AddNeighbour(neighbour.slot); //will be checked in add neighbour
                }
            }
        }
    }


    void SetBoardDimension(UISlot slot)
    {
        float buffer = slotSize + 40;

        xMin = Math.Min(xMin, slot.transform.localPosition.x - buffer);
        xMax = Math.Max(xMax, slot.transform.localPosition.x + buffer);
        yMin = Math.Min(yMin, slot.transform.localPosition.y - buffer);
        yMax = Math.Max(yMax, slot.transform.localPosition.y + buffer);
    }

    float GetBoardScaling()
    {
        float minScaling = .6f;
        float maxScaling = 99f;

        float maxColSize = xMax - xMin;
        float maxRowSize = yMax - yMin;

        float xScaling = 1080 / (maxColSize);
        float yScaling = 1920 / (maxRowSize);
                
        float boardScaling = Math.Min(xScaling, yScaling);
        boardScaling = Math.Max(minScaling, boardScaling);
        boardScaling = Math.Min(maxScaling, boardScaling);

        return boardScaling;
    }
    public void AdjustBoard()
    {
        var slotParent = this.slotParent.parent;

        var boardScaling = GetBoardScaling();

        slotParent.localScale = new Vector3(boardScaling, boardScaling, boardScaling);

        float xCenter = xMin + (xMax - xMin) / 2;
        float yCenter = yMin + (yMax - yMin) / 2;

        float xStart = -(xCenter) * boardScaling;
        float yStart = -(yCenter) * boardScaling;


        //450, 900 - 150, 324

        slotParent.localPosition = new Vector3(xStart, yStart, 0);

        //Debug.Log()

    }

    public UISlot GetUISlot(Vector3 pos)
    {
        return uiSlots[pos];
    }

    public void DestroySlots(Path path)
    {
        foreach (var point in path.waypoints)
        {
            var slot = GetUISlot(point.slot.position);
            if (slot != null)
            {
                slot.DestroySlot();
                //destroyedSlots.Add(slot.slot.position, slot);
            }
        }
    }

    public void SetNextFood(Vector3 position, FoodObject food)
    {
        var slot = GetUISlot(position);
        if (slot != null)
        {
            slot.SetNextFood(food);
        }
    }


}
