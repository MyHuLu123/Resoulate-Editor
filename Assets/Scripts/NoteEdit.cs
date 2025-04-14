using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteEdit : MonoBehaviour
{
    public EditController Editor;
    public BoxEdit BoxEditor;

    private GameObject TargetNote = null;
    public List<NoteData> Notes_List = new List<NoteData>();
    public List<GameObject> NoteObjects = new List<GameObject>();
    public List<GameObject> TargetBoxObjects = new List<GameObject>();


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

    public void Init()
    {
        ChartData _chart = Editor.chart;
        GameObject _parent = GameObject.Find("NotesSquare");
        GameObject _p = null;
        for(int i = 0; i < _chart.notenum; i++)
        {
            NoteData _ntdt = _chart.notes[i];
            Notes_List.Add(_ntdt);
            int _box_id = _ntdt.targetbox;
            _p = _parent.transform.GetChild(_box_id).gameObject;
            TargetBoxObjects.Add(_p);
            GameObject _square = Editor.Square;
            GameObject newNote = Instantiate(_square); newNote.name = _ntdt.type;
            newNote.transform.parent = _p.transform;
            if(_ntdt.type == "Tap")
            {
                newNote.transform.localPosition = new Vector3(0, (float)_ntdt.beat_start * 20, -5);
                newNote.transform.localScale = new Vector3(6, 1, 1);
                newNote.GetComponent<SpriteRenderer>().color = new Vector4(0, 0, 1, 0.6f);
            }
            else if(_ntdt.type == "Drag")
            {
                newNote.transform.localPosition = new Vector3(0, (float)_ntdt.beat_start * 20, -5);
                newNote.transform.localScale = new Vector3(6, 1, 1);
                newNote.GetComponent<SpriteRenderer>().color = new Vector4(1, 0.92f, 0.016f, 0.6f);
            }
            else if(_ntdt.type == "Hold")
            {
                float y1 = (float)_ntdt.beat_start * 20;
                float y2 = (float)_ntdt.beat_end * 20;
                newNote.transform.localPosition = new Vector3(0, (y1 + y2) / 2, -5);
                newNote.transform.localScale = new Vector3(6, (y2 - y1), 1);
                newNote.GetComponent<SpriteRenderer>().color = new Vector4(0, 0, 1, 0.6f);
            }
            newNote.AddComponent<BoxCollider>();
            NoteObjects.Add(newNote);
           
        }
    }

    void Awake()
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
    public void Add(NoteData newNote,GameObject TargetObject,int BoxId)
    {
        Notes_List.Add(newNote);
        NoteObjects.Add(TargetObject);
        TargetBoxObjects.Add(BoxEditor.BoxObjects[newNote.targetbox]);
        ChangeTarget(NoteObjects.Count-1);
    }
    public void ChangeTarget(int index)
    {
        NoteData _ntdt = Notes_List[index];
        type = _ntdt.type;
        targetbox.text = BoxEditor.BoxObjects.IndexOf(TargetBoxObjects[index]).ToString();
        timeStart.text = _ntdt.time_start.ToString();
        timeEnd.text = _ntdt.time_end.ToString();
        beatStart.text = _ntdt.beat_start.ToString();
        beatEnd.text = _ntdt.beat_end.ToString();
        XOffset.text = _ntdt.xoffset.ToString();
        YOffset.text = _ntdt.yoffset.ToString();
        SpeedOffset.text = _ntdt.speedoffset.ToString();
        AngleOffset.text = _ntdt.angleoffset.ToString();
        NoteColor.SetColor(_ntdt.color);
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
            ChangeTarget(_id);
        }
        else if(_id == Editor.Note_Inst.Count)
        {
            _id--;
            if(_id>=0) ChangeTarget(_id);
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
