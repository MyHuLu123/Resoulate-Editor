using System;
using UnityEngine;

[Serializable]
public struct CoverData
{
    public int id;
    //public string spriteName;
    public int groupFront;
    public int groupBack;
    public int SpriteMaskNum;
    public Color color;
    public CoverPartData[] CoverParts;
}
