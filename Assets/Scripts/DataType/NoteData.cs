using System;
using UnityEngine;

[Serializable]
public struct NoteData
{
    public string type;
    public double beat_start;
    public double beat_end;
    public double time_start;
    public double time_end;
    public int targetbox;
    public int targetcover;
    public int sortorder;
    public Color color;
    public double xoffset;
    public double yoffset;
    public double speedoffset;
    public double angleoffset;
    public static bool operator ==(NoteData p1, NoteData p2)
    {
        return p1.type == p2.type && p1.beat_start == p2.beat_start && p1.beat_end == p2.beat_end && p1.xoffset == p2.xoffset && p1.yoffset == p2.yoffset && p1.targetbox == p2.targetbox && p1.color == p2.color && p1.angleoffset == p2.angleoffset && p1.speedoffset == p2.speedoffset;
    }
    public static bool operator !=(NoteData p1, NoteData p2)
    {
        return !(p1 == p2);
    }
    public override bool Equals(object obj)
    {
        if (obj is NoteData)
        {
            NoteData p = (NoteData)obj;
            return this == p;
        }
        return false;
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(type, beat_start, beat_end, targetbox);
    }
}