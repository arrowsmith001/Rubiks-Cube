using System;
using UnityEngine;

public class Layer
{
    Transform[] orbiters = new Transform[10];

    int piecesAdded = 0;

    public Layer(Transform piece)
    {
        AddOrbiter(piece);
    }

    public void AddOrbiter(Transform cube){
        orbiters[piecesAdded] = cube;
        piecesAdded++;
    }

    public Transform[] GetOrbiters()
    {
        return orbiters;
    }

    public Transform GetCenter(){
        return orbiters[0];
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

    public object GetPieceCount()
    {
        return piecesAdded + 1;
    }
}