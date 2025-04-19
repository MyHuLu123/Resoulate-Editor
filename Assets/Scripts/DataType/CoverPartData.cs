using System;
using UnityEngine;

[Serializable]
public struct CoverPartData
{
    public string spriteName;
    public int targetCover;
    public double x;
    public double y;
    public double xscale;
    public double yscale;
    public double angle;
    public bool usePolygonCollider;
}
