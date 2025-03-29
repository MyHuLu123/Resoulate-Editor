using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteEdit : MonoBehaviour
{
    public EditController Editor;

    private GameObject TargetNote = null;

    public string type;
    public Text targetbox;
    public Text timeStart;
    public Text timeEnd;
    public InputField beatStart;
    public InputField beatEnd;
    public InputField XOffset;
    public InputField YOffset;
    public InputField SpeedOffset;
    public InputField AngleOffset;

    public ColorSet NoteColor;

    private void _changeXOffset(string _xoffset)
    {
        if (string.IsNullOrEmpty(_xoffset))
        {
            _xoffset = XOffset.text = "0";
        }
        TargetNote.GetComponent<NoteProperties>().ChangeVar("xoffset", _xoffset);
    }
    private void _changeYOffset(string _yoffset)
    {
        if (string.IsNullOrEmpty(_yoffset))
        {
            _yoffset = YOffset.text = "0";
        }
        TargetNote.GetComponent<NoteProperties>().ChangeVar("yoffset", _yoffset);
    }
    private void _changeSpeedOffset(string _speedoffset)
    {
        if (string.IsNullOrEmpty(_speedoffset))
        {
            _speedoffset = SpeedOffset.text = "0";
        }
        TargetNote.GetComponent<NoteProperties>().ChangeVar("speedoffset", _speedoffset);
    }
    private void _changeAngleOffset(string _angleoffset)
    {
        if (string.IsNullOrEmpty(_angleoffset))
        {
            _angleoffset = AngleOffset.text = "0";
        }
        TargetNote.GetComponent<NoteProperties>().ChangeVar("angleoffset", _angleoffset);
    }
    private void _colorchange(Color newcolor)
    {
        TargetNote.GetComponent<NoteProperties>().ChangeVar("color_r", newcolor.r.ToString());
        TargetNote.GetComponent<NoteProperties>().ChangeVar("color_g", newcolor.g.ToString());
        TargetNote.GetComponent<NoteProperties>().ChangeVar("color_b", newcolor.b.ToString());
        TargetNote.GetComponent<NoteProperties>().ChangeVar("color_a", newcolor.a.ToString());
    }
    private void _changeBeatStart(string _beatstart)
    {
        if (string.IsNullOrEmpty(_beatstart))
        {
            _beatstart = beatStart.text = "0";
        }
        TargetNote.GetComponent<NoteProperties>().ChangeVar("beat_start", _beatstart);
        beatStart.text = TargetNote.GetComponent<NoteProperties>().GetNoteData().beat_start.ToString();
        beatEnd.text = TargetNote.GetComponent<NoteProperties>().GetNoteData().beat_end.ToString();
        timeStart.text = (float.Parse(beatStart.text) / (Editor.chart.bpm / 60)).ToString();
        timeEnd.text = (float.Parse(beatEnd.text) / (Editor.chart.bpm / 60)).ToString();
        TargetNote.GetComponent<NoteProperties>().ChangeVar("time_start", timeStart.text);
        TargetNote.GetComponent<NoteProperties>().ChangeVar("time_end", timeEnd.text);

        _changePlace();
    }
    private void _changeBeatEnd(string _beatend)
    {
        if (string.IsNullOrEmpty(_beatend))
        {
            _beatend = beatEnd.text = "0";
        }
        TargetNote.GetComponent<NoteProperties>().ChangeVar("beat_end", _beatend);
        beatStart.text = TargetNote.GetComponent<NoteProperties>().GetNoteData().beat_start.ToString();
        beatEnd.text = TargetNote.GetComponent<NoteProperties>().GetNoteData().beat_end.ToString();
        timeStart.text = (float.Parse(beatStart.text) / (Editor.chart.bpm / 60)).ToString();
        timeEnd.text = (float.Parse(beatEnd.text) / (Editor.chart.bpm / 60)).ToString();
        TargetNote.GetComponent<NoteProperties>().ChangeVar("time_start", timeStart.text);
        TargetNote.GetComponent<NoteProperties>().ChangeVar("time_end", timeEnd.text);
        _changePlace();
    }
    private void _changePlace()
    {
        Editor.Note_Inst.Remove(TargetNote);
        Editor.AddNewNote(TargetNote);
    }
    public Button delete;
    void Start()
    {
        gameObject.SetActive(false);
        beatStart.text = "0";
        beatEnd.text = "0";
        beatStart.onEndEdit.AddListener(_changeBeatStart);
        beatEnd.onEndEdit.AddListener(_changeBeatEnd);
        XOffset.onEndEdit.AddListener(_changeXOffset);
        YOffset.onEndEdit.AddListener(_changeYOffset);
        SpeedOffset.onEndEdit.AddListener(_changeSpeedOffset);
        AngleOffset.onEndEdit.AddListener(_changeAngleOffset);
        NoteColor.OnColorChanged += _colorchange;
        delete.onClick.AddListener(OnDelete);
    }
    public void ChangeTarget(GameObject newNote)
    {
        TargetNote = newNote;
        NoteProperties NoteProp = newNote.GetComponent<NoteProperties>();
        AddChart.NoteData GetInfo = NoteProp.GetNoteData();
        type = GetInfo.type;
        targetbox.text = Editor.Box_Inst.IndexOf(NoteProp.GetBox()).ToString();
        timeStart.text = GetInfo.time_start.ToString();
        timeEnd.text = GetInfo.time_end.ToString();
        beatStart.text = GetInfo.beat_start.ToString();
        beatEnd.text = GetInfo.beat_end.ToString();
        XOffset.text = GetInfo.xoffset.ToString();
        YOffset.text = GetInfo.yoffset.ToString();
        SpeedOffset.text = GetInfo.speedoffset.ToString();
        AngleOffset.text = GetInfo.angleoffset.ToString();
        NoteColor.SetColor(GetInfo.color);

        if (type != "Hold")
        {
            beatEnd.interactable = false;
        }
        else beatEnd.interactable = true;
    }
    public GameObject GetTarget()
    {
        return TargetNote;
    }
    private void OnDelete()
    {
        int _id = Editor.Note_Inst.IndexOf(TargetNote);
        Editor.Note_Inst.RemoveAt(_id);
        GameObject _targetevent = TargetNote.GetComponent<NoteProperties>().GetEvent();
        EventProperties[] _eve = _targetevent.GetComponentsInChildren<EventProperties>();
        for(int i = 0; i < _eve.Length; i++)
        {
            Editor.Event_Inst.Remove(_eve[i].gameObject);
        }
        Destroy(_targetevent);
        Destroy(TargetNote);
        
        if(_id < Editor.Note_Inst.Count && _id>=0)
        {
            ChangeTarget(Editor.Note_Inst[_id]);
        }
        else if(_id == Editor.Note_Inst.Count)
        {
            _id--;
            if(_id>=0) ChangeTarget(Editor.Note_Inst[_id]);
        }
    }
    void OnDestroy()
    {
        beatStart.onEndEdit.RemoveAllListeners();
        beatEnd.onEndEdit.RemoveAllListeners();
        XOffset.onEndEdit.RemoveAllListeners();
        YOffset.onEndEdit.RemoveAllListeners();
        SpeedOffset.onEndEdit.RemoveAllListeners();
        AngleOffset.onEndEdit.RemoveAllListeners();
        NoteColor.OnColorChanged -= _colorchange;
        delete.onClick.RemoveAllListeners();
    }
}
