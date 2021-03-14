using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SquareColor{
    none, red, white, orange, green, blue, yellow
}
public enum CubeFace{
    front, back, left, right, up, down
}

public class Piece : MonoBehaviour
{
    public string GetColors(){
        string output = "";
        if(frontColor != SquareColor.none) output += frontColor.ToString() + ", ";
        if(backColor != SquareColor.none) output += backColor.ToString() + ", ";
        if(leftColor != SquareColor.none) output += leftColor.ToString() + ", ";
        if(rightColor != SquareColor.none) output += rightColor.ToString() + ", ";
        if(upColor != SquareColor.none) output += upColor.ToString() + ", ";
        if(downColor != SquareColor.none) output += downColor.ToString() + ", ";
        return output;
    }

    Dictionary<CubeFace, Transform> faceMap = new Dictionary<CubeFace, Transform>();

    Transform front;
    Transform back;
    Transform left;
    Transform right;
    Transform up;
    Transform down;

    public SquareColor frontColor;
    public SquareColor backColor;
    public SquareColor leftColor;
    public SquareColor rightColor;
    public SquareColor upColor;
    public SquareColor downColor;

    public void SetMaterial(CubeFace faceType, Material mat){
        faceMap[faceType].GetComponent<MeshRenderer>().sharedMaterial = mat;
    }


    // Start is called before the first frame update

    void Start()
    {

    }

    bool isInitialized = false;

    // Update is called once per frame
    public void Initialize()
    {
        //if(isInitialized) return;

        front = transform.Find("Front");
        back = transform.Find("Back");
        left = transform.Find("Left");
        right = transform.Find("Right");
        up = transform.Find("Up");
        down = transform.Find("Down");

        faceMap.Clear();
        faceMap.Add(CubeFace.front, front);
        faceMap.Add(CubeFace.back, back);
        faceMap.Add(CubeFace.left, left);
        faceMap.Add(CubeFace.right, right);
        faceMap.Add(CubeFace.up, up);
        faceMap.Add(CubeFace.down, down);

        //isInitialized = true;
    }
}
