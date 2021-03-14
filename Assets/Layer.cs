using System;
using UnityEngine;

public class Layer
{
    Transform centerPiece;

    Transform[] orbiters = new Transform[9];

    int orbitersAdded = 0;

    public Layer(Transform piece)
    {
        centerPiece = piece;
    }

    public void AddOrbiter(Transform cube){
        orbiters[orbitersAdded] = cube;
        orbitersAdded++;
    }

    public Transform[] GetOrbiters()
    {
        return orbiters;
    }

    public Transform GetCenter(){
        return centerPiece;
    }

    internal void SnapPiecesToGrid()
    {
        foreach(var piece in GetOrbiters())
        {
            Vector3 pos = piece.position;
            Vector3 newPos = new Vector3((float) Mathf.RoundToInt(pos.x),(float) Mathf.RoundToInt(pos.y),(float) Mathf.RoundToInt(pos.z));
            piece.position = newPos;

            Vector3 euler = piece.rotation.eulerAngles;
            Vector3 newEuler = new Vector3((float) 90*Mathf.RoundToInt(pos.x/90),(float) 90*Mathf.RoundToInt(pos.y/90),(float) 90*Mathf.RoundToInt(pos.z/90));
            piece.rotation = Quaternion.Euler(newEuler.x, newEuler.y, newEuler.z);

        }
    }
}