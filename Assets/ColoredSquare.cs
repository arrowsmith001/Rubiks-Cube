using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredSquare : MonoBehaviour
{
    public SquareColor Color;


    [ExecuteInEditMode]
    public void SetColor(Material material)
    {
        GetComponent<Renderer>().sharedMaterial = material;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
