using UnityEngine;

public class LayerTurnData
{
    public readonly RotationDirection direction;
    public readonly Transform face;
    public readonly Layer layer;
    public readonly Vector3 layerTurningPoint;
    public readonly Vector3 layerTurningAxis;

    public LayerTurnData(RotationDirection direction, Layer layer, Vector3 layerTurningPoint, Vector3 layerTurningAxis, Transform face)
    {
        this.direction = direction;
        this.layer = layer;
        this.layerTurningPoint = layerTurningPoint;
        this.layerTurningAxis = layerTurningAxis;
        this.face = face;
    }
}