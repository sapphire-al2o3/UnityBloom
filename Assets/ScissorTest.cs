using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScissorTest : MonoBehaviour
{
    private void Start()
    {
        enabled = false;
    }

    public Rect scissorRect = new Rect(0, 0, 1, 1);

    public static void SetScissorRect(Camera cam, Rect r)
    {
        if (r.x < 0)
        {
            r.width += r.x;
            r.x = 0;
        }

        if (r.y < 0)
        {
            r.height += r.y;
            r.y = 0;
        }

        r.width = Mathf.Min(1 - r.x, r.width);
        r.height = Mathf.Min(1 - r.y, r.height);

        cam.rect = new Rect(0, 0, 1, 1);
        cam.ResetProjectionMatrix();
        Matrix4x4 m = cam.projectionMatrix;
        cam.rect = r;
        Matrix4x4 m1 = Matrix4x4.TRS(new Vector3(r.x, r.y, 0), Quaternion.identity, new Vector3(r.width, r.height, 1));
        float iw = 1.0f / r.width;
        float ih = 1.0f / r.height;
        Matrix4x4 m2 = Matrix4x4.TRS(new Vector3(iw - 1, 1 / ih - 1, 0), Quaternion.identity, new Vector3(iw, ih, 1));
        Matrix4x4 m3 = Matrix4x4.TRS(new Vector3(-r.x * 2 * iw, -r.y * 2 * ih, 0), Quaternion.identity, Vector3.one);
        cam.projectionMatrix = m3 * m2 * m;
    }

    void OnPreRender()
    {
        var camera = GetComponent<Camera>();
        SetScissorRect(camera, scissorRect);
    }
}
