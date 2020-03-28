using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
public class PathManager : MonoBehaviour
{
    public LineRenderer linePrefab;
    public Transform slotListTop;

    public GameObject startPrefab;
    public GameObject endPrefab;

    GameObject startIcon;
    GameObject endIcon;

    bool isMouseDown = false;
    bool isButtonPressed = false;
    bool canFillSlots = false;
        
    float lastMoveTime = 0;

    Path path;
    LineRenderer line;
    UISlot selectedSlot;

    BoardManager boardManager;

    TileMatchChecker tileMatchChecker;

    void Start()
    {
        TouchInputManager.instance.touchStart += OnTouchStart;
        TouchInputManager.instance.touchMove += OnTouchMove;
        TouchInputManager.instance.touchEnd += OnTouchEnd;

        boardManager = GameManager.instance.boardManager;

        tileMatchChecker = new TileMatchChecker();
    }

    private void OnTouchStart(Touch touch)
    {
        isMouseDown = true;
        lastMoveTime = Time.time;
    }

    private void OnTouchMove(Touch touch)
    {
        lastMoveTime = Time.time;

        if (isMouseDown && path != null)
        {            
            var lastSlot = path.GetLastPoint();

            var uiSlot = boardManager.GetUISlot(lastSlot.slot.position);
            if (uiSlot != null)
            {
                if (uiSlot.isSelected)
                {
                    return;
                }

                var touchPos = new Vector3(touch.position.x, touch.position.y, 0);
                var slotPos = Camera.main.WorldToScreenPoint(uiSlot.transform.position);
                slotPos.z = 0;

                var dir = (touchPos - slotPos).normalized;

                foreach (var neighbour in uiSlot.slot.neighbours)
                {
                    var neighbourUiSlot = boardManager.GetUISlot(neighbour.position);
                    if (neighbourUiSlot != null && neighbourUiSlot.isInteractable)
                    {
                        var neighbourDir = (neighbourUiSlot.transform.position - uiSlot.transform.position).normalized;

                        var dot = Vector3.Dot(neighbourDir, dir);

                        if (dot >= .99)
                        {
                            var gameSlot = neighbourUiSlot.GetComponent<UISlot>();
                            OnSlotEnter(gameSlot);

                            return;
                        }
                    }
                }
            }
        }
    }

    private void OnTouchEnd(Touch touch)
    {
        isMouseDown = false;
        lastMoveTime = Time.time;

        if(path != null)
        {
            var bestRecipe = tileMatchChecker.CheckPath(path);

            if(bestRecipe != null)
            {
                boardManager.DestroySlots(path);

                var lastPoint = path.GetLastPoint();

                if (bestRecipe.isDish)
                {
                    boardManager.SetNextFood(lastPoint.slot.position, bestRecipe.foodRecipe.dish);
                }
                else
                {
                    boardManager.SetNextFood(lastPoint.slot.position, bestRecipe.foodObject);
                }
            }            
        }

        ClearPath();
    }

    public void OnSlotPressed(UISlot gameSlot)
    {
        var slot = gameSlot.slot;

        selectedSlot = gameSlot;

        ClearPath();

        path = new Path(slot);
        
        line = Instantiate(linePrefab, slotListTop);
        line.positionCount += 1;
        line.SetPosition(line.positionCount - 1, gameSlot.transform.position);

        startIcon = Instantiate(startPrefab, slotListTop);
        startIcon.transform.position = gameSlot.transform.position;

        FillSlot(path.GetLastPoint());

        UpdateBoard();
    }

    public void OnSlotEnter(UISlot gameSlot)
    {
        if ((!isMouseDown && !isButtonPressed) || !canFillSlots)
        {
            return;
        }

        if (!gameSlot.isInteractable)
        {
            return;
        }

        selectedSlot = gameSlot;

        if (path != null)
        {
            var slot = gameSlot.slot;
            var previous = path.GetPreviousPoint();

            if (slot != null && previous != null && previous.slot == slot) //retracting
            {
                GoBack();
            }
            else
            {
                if (path.AddPoint(slot))
                {                    
                    var lastPoint = path.GetLastPoint();
                    //AddPoint(lastPoint.previous, lastPoint);
                    
                    line.positionCount += 1;
                    line.SetPosition(line.positionCount - 1, gameSlot.transform.position);

                    UpdateBoard();
                }
            }
        }
    }

    void FillSlot(PathSlot slot)
    {
        //var uiSlot = boardManager.GetUISlot(slot.slot.position);

        //uiSlot.SetFilled(true);
    }

    public void UpdateBoard()
    {
        UpdateFill();
    }

    public void UpdateFill()
    {
        foreach (var slot in boardManager.uiSlots.Values)
        {
            var filled = false;

            if (path != null)
            {
                filled = path.waypointsHash.Contains(slot.slot);
            }

            slot.SetFilled(filled);
        }
    }

    void GoBack()
    {
        if (path != null && path.waypoints.Count > 0)
        {
            var lastPoint = path.GetLastPoint();
            //RemovePoint(lastPoint, lastPoint.previous);

            path.GoBack();
            line.positionCount -= 1;

            UpdateBoard();
        }
    }

    public void PlayerPressClear()
    {
        if (canFillSlots)
        {            
            ClearPath();
        }
    }

    public void ClearPath()
    {
        if (line != null)
        {
            Destroy(line.gameObject);
        }

        if (startIcon != null)
        {
            Destroy(startIcon);
        }

        if (endIcon != null)
        {
            Destroy(endIcon);
        }

        path = null;
        canFillSlots = true;

        UpdateBoard();
    }
}
