using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interpolator
{
    public abstract float getValue(float t);
}

public class AccelerateDecelerateInterpolator : Interpolator
{
    public override float getValue(float t)
    {
        return Mathf.Clamp01((Mathf.Cos((t+1)*Mathf.PI))/2 + 0.5f);
    }
}
