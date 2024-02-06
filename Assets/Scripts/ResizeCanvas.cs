using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeCanvas : MonoBehaviour
{

    private void OnRectTransformDimensionsChange()
    {
        foreach(HalfScreenWidthCamera hswc in FindObjectsOfType<HalfScreenWidthCamera>())
            hswc.Adjust();
    }
}
