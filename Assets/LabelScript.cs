using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabelScript : MonoBehaviour
{
    public RectTransform canvasRect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // public void SetText(Text text){
    //     GetComponent<Text>() = text;
    // }

    Transform target;

    // Update is called once per frame
    void LateUpdate()
    {
        if(target == null) return;

        Vector3 center = target.GetComponent<BoxCollider>().bounds.center;
        float m = 2.5f;

        // Calculate *screen* position (note, not a canvas/recttransform position)
        Vector2 canvasPos;
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(new Vector3(m*center.x,m*center.y,m*center.z));
        
        // Convert screen position to Canvas / RectTransform space <- leave camera null if Screen Space Overlay
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, null, out canvasPos);
        
        // Set
        transform.localPosition = canvasPos;
    }

    internal void SetTransform(Transform target)
    {
        this.target = target;
    }

    internal void SetText(string v)
    {
        GetComponent<Text>().text = v;
    }
}
