using System;
using UnityEngine;

[Serializable]
public struct BoxData
{
    public double x;
    public double y;
    public double speed;
    public Color color;
    public double angle;

    public int sortorder;
    public int targetcover;
}