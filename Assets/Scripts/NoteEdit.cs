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

    public int noteId = -1;

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
        if (noteId == -1) return;
        NoteData _ntdt = Notes_List[noteId];
        _ntdt.xoffset = double.Parse(_xoffset);
        Notes_List[noteId] = _ntdt;
    }
    private void _changeYOffset(string _yoffset)
    {
        if (string.IsNullOrEmpty(_yoffset))
        {
            _yoffset = YOffset.text = "0";
        }
        if (noteId == -1) return;
        NoteData _ntdt = Notes_List[noteId];
        _ntdt.yoffset = double.Parse(_yoffset);
        Notes_List[noteId] = _ntdt;
    }
    private void _changeSpeedOffset(string _speedoffset)
    {
        if (string.IsNullOrEmpty(_speedoffset))
        {
            _speedoffset = SpeedOffset.text = "0";
        }
        if (noteId == -1) return;
        NoteData _ntdt = Notes_List[noteId];
        _ntdt.speedoffset = double.Parse(_speedoffset);
        Notes_List[noteId] = _ntdt;
    }
    private void _changeAngleOffset(string _angleoffset)
    {
        if (string.IsNullOrEmpty(_angleoffset))
        {
            _angleoffset = AngleOffset.text = "0";
        }
        if (noteId == -1) return;
        NoteData _ntdt = Notes_List[noteId];
        _ntdt.angleoffset = double.Parse(_angleoffset);
        Notes_List[noteId] = _ntdt;
    }
    private void _colorchange(Color newcolor)
    {
        if (noteId == -1) return;
        NoteData _ntdt = Notes_List[noteId];
        _ntdt.color = newcolor;
        Notes_List[noteId] = _ntdt;
    }
    private void _changeBeatStart(string _beatstart)
    {
        if (string.IsNullOrEmpty(_beatstart))
        {
            _beatstart = beatStart.text = "0";
        }
        if (noteId == -1) return;
        NoteData _ntdt = Notes_List[noteId];
        _ntdt.beat_start = double.Parse(_beatstart);
        _ntdt.time_start = _ntdt.beat_start * 60 / Editor.chart.bpm;
        if(_ntdt.type == "Hold" && _ntdt.beat_end < _ntdt.beat_start)
        {
            _ntdt.beat_end = _ntdt.beat_start + 0.25;
            _ntdt.time_end = _ntdt.beat_end * 60 / Editor.chart.bpm;
        }
        Notes_List[noteId] = _ntdt;
        beatStart.text = _ntdt.beat_start.ToString();
        beatEnd.text = _ntdt.beat_end.ToString();
        timeStart.text = _ntdt.time_start.ToString();
        timeEnd.text = _ntdt.time_end.ToString();
        _changePlace();
    }
    private void _changeBeatEnd(string _beatend)
    {
        if (string.IsNullOrEmpty(_beatend))
        {
            _beatend = beatEnd.text = "0";
        }
        if (noteId == -1) return;
        NoteData _ntdt = Notes_List[noteId];
        _ntdt.beat_end = double.Parse(_beatend);
        _ntdt.time_end = _ntdt.beat_end * 60 / Editor.chart.bpm;
        if (_ntdt.type == "Hold" && _ntdt.beat_end < _ntdt.beat_start)
        {
            _ntdt.beat_start = _ntdt.beat_end - 0.25;
            _ntdt.time_start = _ntdt.beat_start * 60 / Editor.chart.bpm;
        }
        beatStart.text = _ntdt.beat_start.ToString();
        beatEnd.text = _ntdt.beat_end.ToString();
        timeStart.text = _ntdt.time_start.ToString();
        timeEnd.text = _ntdt.time_end.ToString();
        _changePlace();
    }
    private void _changePlace()
    {
        NoteData _ntdt = Notes_List[noteId];
        GameObject _ntobj = NoteObjects[noteId];
        GameObject _targetbox = TargetBoxObjects[noteId];
        Notes_List.Remove(_ntdt);
        NoteObjects.Remove(_ntobj);
        TargetBoxObjects.Remove(_targetbox);
        Add(_ntdt, _ntobj, BoxEditor.BoxObjects.IndexOf(_targetbox));
        noteId = Notes_List.IndexOf(_ntdt);
        _changeObjPos(noteId);
    }
    private void _changeObjPos(int index)
    {
        NoteData _ntdt = Notes_List[index];
        GameObject _ntobj = NoteObjects[index];
        if(_ntdt.type == "Hold")
        {
            float y1 = (float)_ntdt.beat_start * 20;
            float y2 = (float)_ntdt.beat_end * 20;
            _ntobj.transform.localPosition = new Vector3(0, (y1 + y2) / 2, -5);
            _ntobj.transform.localScale = new Vector3(6, (y2 - y1), 1);
        }
        else
        {
            _ntobj.transform.localPosition = new Vector3(0, (float)_ntdt.beat_start * 20, -5);
        }
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
    private bool _compareNote(NoteData newNote, NoteData orgNote)
    {
        if (newNote.beat_start != orgNote.beat_start)
        {
            return newNote.beat_start < orgNote.beat_start;
        }
        if (newNote.beat_end != orgNote.beat_end)
        {
            return newNote.beat_end < orgNote.beat_end;
        }
        if (newNote.type == "Drag") return true;
        if (newNote.type == "Hold" && orgNote.type != "Drag") return true;
        if (newNote.type == "Tap" && orgNote.type != "Drag" && orgNote.type != "Hold") return true;
        return false;
    }
    public void Add(NoteData newNote,GameObject TargetObject,int BoxId)
    {
        int _left = 0, _right = Notes_List.Count - 1;
        while(_left <= _right)
        {
            int _mid = (_left + _right) / 2;
            NoteData _now_ntdt = Notes_List[_mid];
            if(_compareNote(newNote, _now_ntdt))
            {
                _right = _mid - 1;
            }
            else
            {
                _left = _mid + 1;
            }
        }
        if (_left < 0) _left = 0;
        else if (_left >= Notes_List.Count) _left = Notes_List.Count;
        Notes_List.Insert(_left, newNote);
        NoteObjects.Insert(_left,TargetObject);
        TargetBoxObjects.Insert(_left,BoxEditor.BoxObjects[newNote.targetbox]);
        ChangeTarget(_left);
    }
    public void ChangeTarget(int index)
    {
        noteId = index;
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
        Editor.CameraMoveTarget(NoteObjects[index]);
    }
    public GameObject GetTarget()
    {
        return NoteObjects[noteId];
    }
    public int GetId()
    {
        return noteId;
    }
    public void Delete(int index)
    {
        Notes_List.RemoveAt(index);
        Destroy(NoteObjects[index]);
        NoteObjects.RemoveAt(index);
        TargetBoxObjects.RemoveAt(index);
    }

    private void OnDelete()
    {
        Delete(noteId);
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
