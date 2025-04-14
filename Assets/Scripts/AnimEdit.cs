using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class AnimEdit : MonoBehaviour
{
    public EditController Editor;
    public string Object = null;
    public Dropdown Tween;
    public Dropdown Ease;
    public InputField start_val;
    public InputField end_val;
    public InputField start_beat;
    public InputField end_beat;
    public GameObject AnimObject;
    public Button Delete;
    public Material LineMaterial;

    public TMP_Dropdown EventType;
    public TMP_InputField _ID1;
    public TMP_InputField _ID2;

    public enum AnimObjectType
    {
        Box,
        Note,
        Cover,
        CoverPart,
        setting
    }
    public enum BoxVar
    {
        x,
        y,
        angle,
        speed,
        color_r,
        color_g,
        color_b,
        color_a
    }
    
    private GameObject targetObject = null;
    private string targetType = null;
    private GameObject ChoosingShower = null;

    private int index1 = -1;
    private int pointCount = 200;

    private List<GameObject> EventTypes = new List<GameObject>();
    private List<List<GameObject>> EventTracks = new List<List<GameObject>>();

    private List<string> variable_type = new List<string>();
    private List<List<string>> variable_use = new List<List<string>>();
    private Dictionary<string, int> variable_type_map = new Dictionary<string, int>();
    private Dictionary<string, Dictionary<string,int>> variable_map = new Dictionary<string, Dictionary<string, int>>();
    private Dictionary<string, int> Tween_map = new Dictionary<string, int>();
    private Dictionary<string, int> Ease_map = new Dictionary<string, int>();
    private List<List<EventData>> ObjectEvents = new List<List<EventData>>();
    private List<List<GameObject>> EventShower = new List<List<GameObject>>();
    private LineRenderer AnimLine = new LineRenderer();
    private GameObject AnimLineObject;
    private double NowAnimStartBeat;
    private double NowAnimStartTime;
    private GameObject NowLine = null;
    private EventData NowEventData;
    private EventData ChoosingEvent;
    // Start is called before the first frame update
    private GameObject SetText(string _text)
    {
        TextMeshPro textObject = new GameObject("TextObject").AddComponent<TextMeshPro>();
        textObject.text = _text;
        textObject.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        textObject.fontSize = 30;
        textObject.color = Color.white;
        textObject.alignment = TextAlignmentOptions.Center;
        return textObject.gameObject;
    }
    private GameObject SetTrack()
    {
        GameObject NewTrack = Instantiate(Editor.Square);
        NewTrack.transform.localScale = new Vector3(10, 600, 1);
        NewTrack.GetComponent<SpriteRenderer>().color = new Vector4(0, 0, 0, 0.6f);
        NewTrack.AddComponent<BoxCollider>();

        return NewTrack;
    }
    private void GetObjects()
    {
        Transform _type = transform.Find("Type"); EventType = _type.GetComponent<TMP_Dropdown>();
        Transform _id1 = transform.Find("ID1"); _ID1 = _id1.GetComponent<TMP_InputField>(); _ID1.text = "-1";
        Transform _id2 = transform.Find("ID2"); _ID2 = _id2.GetComponent<TMP_InputField>(); _ID2.text = "-1";
        EventType.onValueChanged.AddListener(typechanger);
        _ID1.onEndEdit.AddListener(idchanger);
        _ID2.onEndEdit.AddListener(idchanger);
    }
    private void TrackInit()
    {
        variable_type = new List<string>
        {
            "Box","Note","Cover"
        };

        List<string> _var_box = new List<string>
        {
            "x","y","angle","speed","color_r","color_g","color_b","color_a"
        };
        variable_use.Add(_var_box);
        List<string> _var_note = new List<string>
        {
            "xoffset","yoffset","angleoffset","speedoffset","color_r","color_g","color_b","color_a","x_scale","y_scale"
        };
        variable_use.Add(_var_note);
        List<string> _var_cover = new List<string>
        {
            "x","y","angle","x_scale","y_scale","color_r","color_g","color_b","color_a"
        };
        variable_use.Add(_var_cover);

        for(int i = 0; i < variable_use.Count; i++)
        {
            variable_type_map[variable_type[i]] = i;
            GameObject _current_type = new GameObject(variable_type[i]); _current_type.transform.parent = AnimObject.transform;
            GameObject _track_collection = new GameObject("Tracks"); _track_collection.transform.parent = _current_type.transform;
            EventTypes.Add(_current_type);
            EventTracks.Add(new List<GameObject>());
            variable_map[variable_type[i]] = new Dictionary<string, int>();
            for (int j = 0; j < variable_use[i].Count; j++)
            {
                variable_map[variable_type[i]][variable_use[i][j]] = j;
                GameObject _current_track = new GameObject(variable_use[i][j]); _current_track.transform.parent = _track_collection.transform;
                _current_track.transform.localPosition = new Vector3(j * 15, 0, -5);

                GameObject _track = SetTrack(); _track.transform.parent = _current_track.transform; _track.transform.localPosition = new Vector3(0, 0, 0); _track.AddComponent<FixOnCamera>().fixY = true;
                GameObject _text = SetText(variable_use[i][j]);_text.transform.parent = _current_track.transform; _text.transform.localPosition = new Vector3(0, 0, 0); _text.AddComponent<FixOnCamera>().fixY = true;

                EventTracks[i].Add(_current_track);
            }
            _current_type.SetActive(false);
        }
    }
    private void EventInit()
    {
        ChartData _chart = Editor.chart;
        EventData[] _event_data = _chart.events;

        for(int i = 0; i < _chart.boxnum; i++)
        {
            BoxData _box = _chart.boxes[i];
            GameObject _box_collect = new GameObject("Box_events"); _box_collect.transform.SetParent(EventTypes[0].transform);
            for (int j = 0; j < variable_use[0].Count; j++)
            {
                GameObject _empty_track = new GameObject(variable_use[0][j]); _empty_track.transform.parent = _box_collect.transform;
                _empty_track.transform.localPosition = new Vector3(j * 15, 0, -5);
                
            }
            Debug.Log("now index:" + i.ToString());
            Editor.Box_Inst[i].GetComponent<BoxProperties>().ChangeEvent(_box_collect);
            _box_collect.SetActive(false);
        }
        for (int i = 0; i < _chart.notenum; i++)
        {
            NoteData _note = _chart.notes[i];
            GameObject _note_collect = new GameObject("Note_events"); _note_collect.transform.SetParent(EventTypes[1].transform);
            for (int j = 0; j < variable_use[1].Count; j++)
            {
                GameObject _empty_track = new GameObject(variable_use[1][j]); _empty_track.transform.parent = _note_collect.transform;
                _empty_track.transform.localPosition = new Vector3(j * 15, 0, -5);
            }
            Editor.Note_Inst[i].GetComponent<NoteProperties>().ChangeEvent(_note_collect);
            _note_collect.SetActive(false);
        }
        for (int i = 0; i < _chart.covernum; i++)
        {
            CoverData _cover = _chart.covers[i];
            GameObject _cover_collect = new GameObject("Cover_events"); _cover_collect.transform.SetParent(EventTypes[2].transform);
            for (int j = 0; j < variable_use[2].Count; j++)
            {
                GameObject _empty_track = new GameObject(variable_use[2][j]); _empty_track.transform.parent = _cover_collect.transform;
                _empty_track.transform.localPosition = new Vector3(j * 15, 0, -5);

            }
            _cover_collect.SetActive(false);
        }

        for (int i = 0; i < _event_data.Length; i++)
        {
            EventData _event = _event_data[i];
            float _ypos = (float)(_event.endbeat + _event.beat) * 10;
            float _height = (float)(_event.endbeat - _event.beat) * 20;
            int _id1 = variable_type_map[_event.Object];
            int _id2 = variable_map[_event.Object][_event.variable];
            GameObject Shower = new GameObject("EventLine");
            Shower.transform.parent = EventTypes[_id1].transform.GetChild(1+ _event.id).GetChild(_id2);
            Shower.transform.localPosition = new Vector3(0, _ypos, -10);
            Shower.transform.localScale = new Vector3(10, _height, 1);
            Shower.AddComponent<BoxCollider>();
            Shower.AddComponent<EventProperties>().NewEvent(_event);
            if(_event.Object == "Box")
            {
                Shower.GetComponent<EventProperties>().ChangeObject(Editor.Box_Inst[_event.id]);
            }
            else if (_event.Object == "Note")
            {
                Shower.GetComponent<EventProperties>().ChangeObject(Editor.Note_Inst[_event.id]);
            }

            SpriteRenderer S = Shower.AddComponent<SpriteRenderer>();
            S.color = new Vector4(1, 1, 1, 0.5f); S.sprite = Editor.Square.GetComponent<SpriteRenderer>().sprite;
            LineRenderer L = Shower.AddComponent<LineRenderer>();
            L.material = LineMaterial;
            L.positionCount = 100;
            L.startWidth = 0.3f;
            L.endWidth = 0.3f;
            for (int j = 0; j < 100; j++)
            {
                L.SetPosition(j, new Vector3(Shower.transform.position.x - 5 + 10 * (1.0f * j / 100f), Shower.transform.position.y - _height / 2 + _height * (float)AnimationCollection.AnimationGet(_event.Tween, _event.Ease, 1.0 * j / 100), Shower.transform.position.z-3));
            }

            Editor.Event_Inst.Add(Shower);
        }
        
    }
    public void Init()
    {
        TrackInit();
        EventInit();
    }
    public void ChangeTarget(GameObject _obj)
    {
        if(targetObject != null)
        {
            if (targetObject.GetComponent<BoxProperties>() != null)
            {
                EventTypes[0].SetActive(false);
                targetObject.GetComponent<BoxProperties>().GetEvent().SetActive(false);
                
            }
            else if(targetObject.GetComponent<NoteProperties>() != null)
            {
                EventTypes[1].SetActive(false);
                targetObject.GetComponent<NoteProperties>().GetEvent().SetActive(false);
            }
        }
        targetObject = _obj;
        if(_obj != null)
        {
            if (_obj.GetComponent<BoxProperties>() != null)
            {
                EventTypes[0].SetActive(true);
                _obj.GetComponent<BoxProperties>().GetEvent().SetActive(true);
                Object = targetType = "Box"; EventType.value = 0;
                _ID1.text = Editor.Box_Inst.IndexOf(_obj).ToString();
            }
            else if (_obj.GetComponent<NoteProperties>() != null)
            {
                EventTypes[1].SetActive(true);
                _obj.GetComponent<NoteProperties>().GetEvent().SetActive(true);
                Object = targetType = "Note"; EventType.value = 1;
                _ID1.text = Editor.Note_Inst.IndexOf(_obj).ToString();
            }
        }
        
    }
    private void typechanger(int x)
    {
        TargetObjChange();
    }
    private void idchanger(string x)
    {
        TargetObjChange();
    }
    private void TargetObjChange()
    {
        if(EventType.value == 0)//Box
        {
            if (int.Parse(_ID1.text) < 0) _ID1.text = "0";
            else if (int.Parse(_ID1.text) >= Editor.Box_Inst.Count) _ID1.text = (Editor.Box_Inst.Count - 1).ToString();
            ChangeTarget(Editor.Box_Inst[int.Parse(_ID1.text)]);
        }
        else if(EventType.value == 1)
        {
            if (int.Parse(_ID1.text) < 0) _ID1.text = "0";
            else if (int.Parse(_ID1.text) >= Editor.Note_Inst.Count) _ID1.text = (Editor.Note_Inst.Count - 1).ToString();
            ChangeTarget(Editor.Note_Inst[int.Parse(_ID1.text)]);
        }
    }
    public GameObject AddBox()
    {
        GameObject _box_collect = new GameObject("Box_events"); _box_collect.transform.SetParent(EventTypes[0].transform);
        for (int j = 0; j < variable_use[0].Count; j++)
        {
            GameObject _empty_track = new GameObject(variable_use[0][j]); _empty_track.transform.parent = _box_collect.transform;
            _empty_track.transform.localPosition = new Vector3(j * 15, 0, -5);

        }
        _box_collect.SetActive(false);

        return _box_collect;
    }
    public GameObject AddNote()
    {
        GameObject _note_collect = new GameObject("Note_events"); _note_collect.transform.SetParent(EventTypes[1].transform);
        for (int j = 0; j < variable_use[1].Count; j++)
        {
            GameObject _empty_track = new GameObject(variable_use[1][j]); _empty_track.transform.parent = _note_collect.transform;
            _empty_track.transform.localPosition = new Vector3(j * 15, 0, -5);
        }
        _note_collect.SetActive(false);

        return _note_collect;
    }
    void Start()
    {
        AnimLineObject = new GameObject();
        AnimLineObject.transform.position = new Vector3(0,0,0);
        Tween.onValueChanged.AddListener(_tween_change);
        Ease.onValueChanged.AddListener(_ease_change);
        start_val.onEndEdit.AddListener(_startval_change);
        end_val.onEndEdit.AddListener(_endval_change);
        start_beat.onEndEdit.AddListener(_startbeat_change);
        end_beat.onEndEdit.AddListener(_endbeat_change);
        EventType.onValueChanged.AddListener(typechanger);
        _ID1.onEndEdit.AddListener(idchanger);
        _ID2.onEndEdit.AddListener(idchanger);
        AnimLine = AnimLineObject.AddComponent<LineRenderer>();
        AnimLine.positionCount = pointCount;
        float width = 10;
        float height = 10;
        for (int i = 0; i < pointCount; i++)
        {
            float p = i * (float)1.0 / pointCount;
            AnimLine.SetPosition(i, new Vector3(p * width, p * height, -10));
        }
        for(int i = 0; i < Tween.options.Count; i++)
        {
            Tween_map[Tween.options[i].text] = i;
        }
        for (int i = 0; i < Ease.options.Count; i++)
        {
            Ease_map[Ease.options[i].text] = i;
        }
        Delete.onClick.AddListener(OnDelete);
        //GetObjects();
        start_val.text = "0";
        end_val.text = "0";
        start_beat.text = "0";
        end_beat.text = "0";

    }
    private void LoadEvent(EventData _event)
    {
        Tween.value = Tween_map[_event.Tween];
        Ease.value = Ease_map[_event.Ease];
        start_val.text = _event.start.ToString();
        end_val.text = _event.end.ToString();
        start_beat.text = _event.beat.ToString();
        end_beat.text = _event.endbeat.ToString();
    }
    private void OnDelete()
    {
        if (ChoosingShower != null)
        {
            Editor.Event_Inst.Remove(ChoosingShower);
            Destroy(ChoosingShower);
            ChoosingShower = null;
        }
    }

    private EventData CreateEvent(int track_id,double beat)
    {
        EventData new_evdt = new EventData();
        new_evdt.Object = Object;
        new_evdt.variable = variable_use[EventType.value][track_id];
        new_evdt.Tween = Tween.options[Tween.value].text;
        new_evdt.Ease = Ease.options[Ease.value].text;
        new_evdt.time = beat / (1.0 * Editor.chart.bpm / 60);
        new_evdt.during = 0;
        new_evdt.beat = beat;
        new_evdt.endbeat = beat;
        new_evdt.start = float.Parse(start_val.text);
        new_evdt.end = float.Parse(end_val.text);
        return new_evdt;
    }
    private GameObject CreateShower(int track_id,double beat)
    {
        GameObject Shower = new GameObject("EventLine");
        if(EventType.value == 0)
        {
            Shower.transform.parent = targetObject.GetComponent<BoxProperties>().GetEvent().transform.GetChild(track_id).transform;
        }
        else if(EventType.value == 1)
        {
            Shower.transform.parent = targetObject.GetComponent<NoteProperties>().GetEvent().transform.GetChild(track_id).transform;
        }
        Shower.transform.localPosition = new Vector3(0, (float)beat * 20, -10);
        Shower.transform.localScale = new Vector3(10,1,1);
        Shower.AddComponent<BoxCollider>();
        SpriteRenderer S = Shower.AddComponent<SpriteRenderer>();
        S.color = new Vector4(1, 1, 1, 0.5f); S.sprite = Editor.Square.GetComponent<SpriteRenderer>().sprite;
        LineRenderer L = Shower.AddComponent<LineRenderer>();
        L.material = LineMaterial;
        L.positionCount = 100;
        L.startWidth = 0.3f;
        L.endWidth = 0.3f;
        for (int j = 0; j < 100; j++)
        {
            L.SetPosition(j, new Vector3(Shower.transform.position.x - 5 + 10 * (1.0f * j / 100f), Shower.transform.position.y - 0.5f + (float)AnimationCollection.AnimationGet(Tween.options[Tween.value].text, Ease.options[Ease.value].text, 1.0 * j / 100), Shower.transform.position.z - 10));
        }
        return Shower;
    }
    private bool SearchShower(GameObject ev_shower,out int track_id,out int eve_id)
    {
        for(int i = 0; i < EventShower.Count; i++)
        {
            for(int j = 0; j < EventShower[i].Count; j++)
            {
                if(ev_shower == EventShower[i][j])
                {
                    track_id = i;eve_id = j;
                    return true;
                }
            }
        }
        track_id = -1;
        eve_id = -1;
        return false;
    }

    //----------------------------------//修改监听器
    private void _shower_change(GameObject _shower)
    {
        EventData _eve = _shower.GetComponent<EventProperties>().GetEventData();
        float ypos = 10 * ((float)(_eve.beat + _eve.endbeat));
        float yscale = 20 * ((float)(_eve.endbeat - _eve.beat));
        _shower.transform.localPosition = new Vector3(0, ypos, -10);
        _shower.transform.localScale = new Vector3(10, yscale, 1);
        LineRenderer _l = _shower.GetComponent<LineRenderer>();
        for (int j = 0; j < 100; j++)
        {
            _l.SetPosition(j, new Vector3(_shower.transform.position.x - 5 + 10 * (1.0f * j / 100f), _shower.transform.position.y - yscale / 2 + yscale * (float)AnimationCollection.AnimationGet(Tween.options[Tween.value].text, Ease.options[Ease.value].text, 1.0 * j / 100), _shower.transform.position.z - 10));
        }
    }
    private void _tween_change(int val)
    {
        if(Editor.Choose.isOn && ChoosingShower != null)
        {
            ChoosingShower.GetComponent<EventProperties>().ChangeVar("Tween",Tween.options[val].text);
            _shower_change(ChoosingShower);
        }
    }
    private void _ease_change(int val)
    {
        if (Editor.Choose.isOn && ChoosingShower != null)
        {
            ChoosingShower.GetComponent<EventProperties>().ChangeVar("Ease", Ease.options[val].text);
            _shower_change(ChoosingShower);
        }
    }
    private void _startval_change(string val)
    {
        if (Editor.Choose.isOn && ChoosingShower != null)
        {
            ChoosingShower.GetComponent<EventProperties>().ChangeVar("start", val);
        }
    }
    private void _endval_change(string val)
    {
        if (Editor.Choose.isOn && ChoosingShower != null)
        {
            ChoosingShower.GetComponent<EventProperties>().ChangeVar("end", val);
        }
    }
    private void _startbeat_change(string val)
    {
        if (Editor.Choose.isOn && ChoosingShower != null)
        {
            ChoosingShower.GetComponent<EventProperties>().ChangeVar("beat", val);
            _shower_change(ChoosingShower);
        }
    }
    private void _endbeat_change(string val)
    {
        if (Editor.Choose.isOn && ChoosingShower != null)
        {
            ChoosingShower.GetComponent<EventProperties>().ChangeVar("endbeat", val);
            _shower_change(ChoosingShower);
        }
    }


    //----------------------------------//
    void Update()
    {
        if (Editor.Choose.isOn)
        {
            if (Input.GetMouseButtonDown(0) && gameObject.activeSelf && !EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 ClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Default")))
                {
                    GameObject _eve = hit.collider.gameObject;
                    if(_eve.GetComponent<EventProperties>() != null)
                    {
                        ChoosingShower = _eve;
                        LoadEvent(_eve.GetComponent<EventProperties>().GetEventData());
                    }
                }
            }       
        }
        else
        {
            ChoosingShower = null;
            if (Input.GetMouseButtonDown(0) && gameObject.activeSelf && !EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 ClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Default")))
                {
                    GameObject HitTrack = hit.collider.transform.parent.gameObject;
                    int _id = EventTracks[EventType.value].IndexOf(HitTrack);
                    if (_id != -1)
                    {
                        Debug.Log(_id);
                        double beat = 1.0 * (int)(ClickPos.y * Editor.NowBeatSum / 20 + 0.5) / Editor.NowBeatSum;
                        double time = beat / (1.0 * Editor.chart.bpm / 60);
                        NowAnimStartTime = time;
                        NowAnimStartBeat = beat;
                        NowEventData = CreateEvent(_id, beat);
                        NowLine = CreateShower(_id, beat);
                    }

                }
            }
            else if (Input.GetMouseButton(0) && gameObject.activeSelf && NowLine != null)
            {
                Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                double beat = 1.0 * (int)(MousePos.y * Editor.NowBeatSum / 20 + 0.5) / Editor.NowBeatSum;
                double time = beat / (1.0 * Editor.chart.bpm / 60);
                NowEventData.during = time - NowEventData.time;
                NowEventData.endbeat = beat;
                float ypos = 10 * ((float)(beat + NowEventData.beat));
                float yscale = 20 * ((float)(beat - NowEventData.beat));
                NowLine.transform.localPosition = new Vector3(0, ypos, -10);
                NowLine.transform.localScale = new Vector3(10, yscale, 1);
                LineRenderer L = NowLine.GetComponent<LineRenderer>();
                for (int j = 0; j < 100; j++)
                {
                    L.SetPosition(j, new Vector3(NowLine.transform.position.x - 5 + 10 * (1.0f * j / 100f), NowLine.transform.position.y - yscale / 2 + yscale * (float)AnimationCollection.AnimationGet(Tween.options[Tween.value].text, Ease.options[Ease.value].text, 1.0 * j / 100), NowLine.transform.position.z - 10));
                }

            }
            else if (Input.GetMouseButtonUp(0) && gameObject.activeSelf && NowLine != null)
            {
                NowLine.AddComponent<EventProperties>().NewEvent(NowEventData);
                NowLine.GetComponent<EventProperties>().ChangeObject(targetObject);
                Editor.AddNewEvent(NowLine);
                NowLine = null;
            }
        }
    }
    void OnDestroy()
    {
        Tween.onValueChanged.RemoveAllListeners();
        Ease.onValueChanged.RemoveAllListeners();
        start_val.onEndEdit.RemoveAllListeners();
        end_val.onEndEdit.RemoveAllListeners();
        start_beat.onEndEdit.RemoveAllListeners();
        end_beat.onEndEdit.RemoveAllListeners();
        Delete.onClick.RemoveAllListeners();
    }
    /*
    private void Tracks()
    {
        if (AnimTracks.Count != 0)
        {
            for(int i=0;i< AnimTracks.Count; i++)
            {
                Destroy(AnimTracks[i]);
                Destroy(AnimTMPText[i]);
            }
            AnimTracks.Clear(); AnimTMPText.Clear();
        }
        for(int i = 0; i < variable_use.Count; i++)//按照给定变量数量创建轨道
        {
            GameObject NewText = SetText(i);
            GameObject NewTrack = SetTrack(i);

            ObjectEvents.Add(new List<AddChart.EventData>());
            EventShower.Add(new List<GameObject>());

            AnimTracks.Add(NewTrack);
            AnimTMPText.Add(NewText);
        }
    }
    private void AddEvents()
    {
        switch (Object)
        {
            case "Box":
                foreach(AddChart.EventData evdt in Editor.Box_Events[index1])
                {
                    int track_id = variable_map[evdt.variable];
                    ObjectEvents[track_id].Add(evdt);
                    EventShower[track_id].Add(CreateShower(evdt));
                }
                break;
            case "Note":
                foreach (AddChart.EventData evdt in Editor.Note_Events[index1][index2])
                {
                    int track_id = variable_map[evdt.variable];
                    ObjectEvents[track_id].Add(evdt);
                    EventShower[track_id].Add(CreateShower(evdt));
                }
                break;
        }

    }
    /*
    public void LoadInfo(string obj,int id1,int id2)
    {
        index1 = id1;
        index2 = id2;
        for(int i = 0; i < EventShower.Count; i++)
        {
            foreach (GameObject gmj in EventShower[i])
            {
                Destroy(gmj);
            }
        }
        variable_map.Clear();
        ObjectEvents.Clear();
        EventShower.Clear();
        Object = obj;
        switch (obj)
        {
            case "Note":
                variable_use = new List<string> {"xoffset","yoffset","speedoffset","angleoffset"};
                for(int i = 0; i < variable_use.Count; i++)
                {
                    variable_map[variable_use[i]] = i;
                    ObjectEvents.Add(new List<AddChart.EventData>());
                }
                break;
            case "Box":
                variable_use = new List<string> {"x", "y", "speed", "angle"};
                for (int i = 0; i < variable_use.Count; i++)
                {
                    variable_map[variable_use[i]] = i;
                    ObjectEvents.Add(new List<AddChart.EventData>());
                }
                break;
        }
        Tracks();
        AddEvents();

    }
    public void UpdateInfo()
    {
        switch (Object)
        {
            case "Box"://打击箱
                Editor.Box_Events[index1].Clear();
                for (int i=0;i< AnimTracks.Count; i++)
                {
                    for(int j=0;j< ObjectEvents[i].Count; j++)
                    {
                        Editor.Box_Events[index1].Add(ObjectEvents[i][j]);
                    }
                }
                break;
            case "Note":
                Editor.Note_Events[index1][index2].Clear();
                for (int i = 0; i < AnimTracks.Count; i++)
                {
                    for (int j = 0; j < ObjectEvents[i].Count; j++)
                    {
                        Editor.Note_Events[index1][index2].Add(ObjectEvents[i][j]);
                    }
                }
                break;
        }
        variable_map.Clear();
        ObjectEvents.Clear();
    }
    */
}
