using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteProperties : MonoBehaviour
{
    private GameObject TargetBox;
    private GameObject TargetEvent;
    private GameObject TargetCover;
    private AddChart.NoteData NoteData = new AddChart.NoteData();

    public GameObject GetBox()
    {
        return TargetBox;
    }
    public GameObject GetEvent()
    {
        return TargetEvent;
    }
    public GameObject GetCover()
    {
        return TargetCover;
    }
    public AddChart.NoteData GetNoteData()
    {
        return NoteData;
    }
    public void ChangeVar(string var_type, string val)
    {
        switch (var_type)
        {
            case "beat_start":
                if(double.TryParse(val, out NoteData.beat_start))
                {
                    if (NoteData.type != "Hold")
                    {
                        var V = gameObject.transform.localPosition;
                        V.y = (float)NoteData.beat_start * 20;
                        gameObject.transform.localPosition = V;
                    }
                    else
                    {
                        if (NoteData.beat_end <= NoteData.beat_start) NoteData.beat_end = NoteData.beat_start + 0.001;
                        var y1 = (float)NoteData.beat_start * 20;
                        var y2 = (float)NoteData.beat_end * 20;
                        var V = gameObject.transform.localPosition;
                        V.y = (y1 + y2) / 2;
                        var S = gameObject.transform.localScale;
                        S.y = y2 - y1;
                        gameObject.transform.localPosition = V;
                        gameObject.transform.localScale = S;
                    }
                    //NoteData.time_start = (NoteData.time_start / (Editor.chart.bpm / 60));
                }
                break;
            case "beat_end":
                if(double.TryParse(val, out NoteData.beat_end))
                {
                    if(NoteData.type == "Hold")
                    {
                        if (NoteData.beat_end <= NoteData.beat_start) NoteData.beat_end = NoteData.beat_start + 0.001;
                        var y1 = (float)NoteData.beat_start * 20;
                        var y2 = (float)NoteData.beat_end * 20;
                        var V = gameObject.transform.localPosition;
                        V.y = (y1 + y2) / 2;
                        var S = gameObject.transform.localScale;
                        S.y = y2 - y1;
                        gameObject.transform.localPosition = V;
                        gameObject.transform.localScale = S;
                    }
                }
                break;
            case "time_start":
                double.TryParse(val, out NoteData.time_start);
                break;
            case "time_end":
                double.TryParse(val, out NoteData.time_end);
                break;
            case "color_r":
                float.TryParse(val, out NoteData.color.r);
                break;
            case "color_g":
                float.TryParse(val, out NoteData.color.g);
                break;
            case "color_b":
                float.TryParse(val, out NoteData.color.b);
                break;
            case "color_a":
                float.TryParse(val, out NoteData.color.a);
                break;
            case "xoffset":
                double.TryParse(val, out NoteData.xoffset);
                break;
            case "yoffset":
                double.TryParse(val, out NoteData.yoffset);
                break;
            case "speedoffset":
                double.TryParse(val, out NoteData.speedoffset);
                break;
            case "angleoffset":
                double.TryParse(val, out NoteData.angleoffset);
                break;
        }
    }
    public void ChangeBox(GameObject newbox)
    {
        TargetBox = newbox;
    }
    public void ChangeCover(GameObject newcover)
    {
        TargetCover = newcover;
    }
    public void ChangeEvent(GameObject newevent)
    {
        TargetEvent = newevent;
    }
    public void NewNote()
    {
        NoteData = new AddChart.NoteData();
    }
    public void NewNote(AddChart.NoteData newnote)
    {
        NoteData = newnote;
    }
}
