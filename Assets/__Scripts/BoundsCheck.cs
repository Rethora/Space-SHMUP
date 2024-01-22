using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsCheck : MonoBehaviour
{
    [System.Flags]
    public enum eScreenLocs
    {
        onScreen = 0,  // 0000 in binary
        offRight = 1,  // 0001 in binary
        offLeft = 2,  // 0010 in binary
        offUp = 4,  // 0100 in binary
        offDown = 8   // 1000 in binary
    }

    public enum eType { center, inset, outset };

    [Header("Inscribed")]
    public eType boundsType = eType.center;
    public float radius = 1f;
    public bool keepOnScreen = true;

    [Header("Dynamic")]
    public eScreenLocs screenLocs = eScreenLocs.onScreen;
    public float camWidth;
    public float camHeight;

    void Awake()
    {
        camHeight = Camera.main.orthographicSize;
        camWidth = camHeight * Camera.main.aspect;
    }

    void LateUpdate()
    {
        // Find the checkRadius that will enable center, inset, or outset
        float checkRadius = 0;
        if (boundsType == eType.inset) checkRadius = -radius;
        if (boundsType == eType.outset) checkRadius = radius;

        Vector3 pos = transform.position;
        screenLocs = eScreenLocs.onScreen;

        float hBound = camWidth + checkRadius;
        float vBound = camHeight + checkRadius;

        // Restrict the X position to camWidth
        if (pos.x > hBound)
        {
            pos.x = hBound;
            screenLocs |= eScreenLocs.offRight;
        }
        if (pos.x < -hBound)
        {
            pos.x = -hBound;
            screenLocs |= eScreenLocs.offLeft;
        }

        // Restrict the Y position to camHeight
        if (pos.y > vBound)
        {
            pos.y = vBound;
            screenLocs |= eScreenLocs.offUp;
        }
        if (pos.y < -vBound)
        {
            pos.y = -vBound;
            screenLocs |= eScreenLocs.offDown;
        }

        if (keepOnScreen && !onScreen)
        {
            transform.position = pos;
            screenLocs = eScreenLocs.onScreen;
        }
    }

    public bool onScreen
    {
        get
        {
            return (screenLocs == eScreenLocs.onScreen);
        }
    }

    public bool LocIs(eScreenLocs checkLoc)
    {
        if (checkLoc == eScreenLocs.onScreen) return onScreen;
        return ((screenLocs & checkLoc) == checkLoc);
    }
}
