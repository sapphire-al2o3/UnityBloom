using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rot : MonoBehaviour
{
    void Update()
    {
		transform.Rotate(0, Time.deltaTime * 100.0f, 0);
    }
}
