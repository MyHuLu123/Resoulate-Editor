using System;
using UnityEngine;

[Serializable]
public struct ChartData
{
    public float bpm;
    //public double[][] bpmlist;
    public int boxnum;
    public BoxData[] boxes;
    public int notenum;
    public NoteData[] notes;
    public int eventnum;
    public EventData[] events;
    public int covernum;
    public CoverData[] covers;
    public int coverpartnum;
    public CoverPartData[] coverparts;
    public int imagenum;
    public ImageData[] images;
    public int spritenum;
    public SpriteData sprites;
    public double time_offset;
    public double beat_set;
}