using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

public class HelperFunctions
{

}

public static class MyExtensions
{   
    public static Vector2 ToRectPos(this Vector3 worldPos, RectTransform parentRect)
    {
        return ((Vector2)worldPos).ToRectPos(parentRect);
    }

    public static Vector2 ToRectPos(this Vector2 worldPos, RectTransform parentRect)
    {
        Vector2 localPoint;
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPoint, null, out localPoint);

        return localPoint;
    }
}