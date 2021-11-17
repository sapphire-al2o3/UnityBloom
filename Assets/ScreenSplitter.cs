using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSplitter : MonoBehaviour
{
    [SerializeField]
    ScissorTest leftScreen;

    [SerializeField]
    ScissorTest rightScreen;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            float x = Input.mousePosition.x;
            float w = Screen.width;
            float h = Screen.height;

            if (x < 0)
            {
                x = 0;
            }
            else if (x > w)
            {
                x = w;
            }

            Rect r = new Rect(0.0f, 0.0f, 0.0f, 1.0f);
            if (x > 0)
            {
                r.width = x / w;
                leftScreen.enabled = true;
                leftScreen.scissorRect = r;
            }
            if (x < w)
            {
                r.x = r.width;
                r.width = (w - x) / w;
                rightScreen.enabled = true;
                rightScreen.scissorRect = r;
            }
        }
    }
}
