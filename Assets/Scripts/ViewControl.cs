using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewControl : MonoBehaviour
{
    // Start is called before the first frame update
    //public Button Edit;
    public EditController EditController;
    public Camera ViewCamera;

    public GameObject ViewCanvas;
    public GameObject ViewObjects;
    public GameObject EditCanvas;
    public GameObject EditObjects;

    public GameObject Box;
    public GameObject Tap;
    public GameObject Drag;
    public GameObject Hold;
    public GameObject Cover;

    public Toggle Play;

    public float time;

    private List<GameObject> BoxObjects = new List<GameObject>();
    private List<GameObject> NoteObjects = new List<GameObject>();
    private List<GameObject> CoverObjects = new List<GameObject>();
    private List<AddChart.EventData> Events = new List<AddChart.EventData>();

    public Text MusicLength;
    public Slider SongSlider;
    private float wholeLength;
    private AudioClip music;

    private int height;
    private int width;
    public float time_offset;
    public float time_tobeat;
    private float beat_set;
    private float start_time;
    public void Init()
    {
        for (int i=0;i< BoxObjects.Count; i++)
        {
            Destroy(BoxObjects[i]);
        }
        BoxObjects.Clear();
        NoteObjects.Clear();
        for(int i = 0; i < CoverObjects.Count; i++)
        {
            Destroy(CoverObjects[i]);
        }
        CoverObjects.Clear();
        Events.Clear();

        AddChart.ChartData chart = EditController.chart;
        ViewCamera.orthographicSize = 160;
        width = 160; height = 160 * Screen.height / Screen.width;
        ViewCamera.transform.position = new Vector3(0, 0, 80);
        Vector2 v = ViewCamera.ViewportToWorldPoint(new Vector2(0, 0));
        Vector2 v1 = ViewCamera.ViewportToWorldPoint(new Vector2(1, 1));
        Vector2 delta = v1 - v;
        float.TryParse(EditController.TimeOffset.text, out time_offset);
        float.TryParse(EditController.BeatOffset.text, out beat_set);
        wholeLength = Mathf.Max(EditController.songLength, EditController.songLength - time_offset + BeatToTime(beat_set)) + 2f;
        start_time = wholeLength - EditController.songLength;
        time_tobeat = wholeLength - (EditController.songLength - time_offset + BeatToTime(beat_set));
        time = 0;
        music = EditController.song;
        GetComponent<AudioSource>().clip = music;

        for (int i = 0; i < chart.boxnum; i++)
        {
            GameObject Obj = Instantiate(Box);
            Obj.AddComponent<ViewBoxInfo>().speed = chart.boxes[i].speed;
            Obj.transform.parent = ViewObjects.transform;
            Obj.transform.localPosition = new Vector3((float)chart.boxes[i].x,(float)chart.boxes[i].y, 0);
            Obj.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, (float)chart.boxes[i].angle));
            Obj.transform.localScale = new Vector3(1, 1, 1);
            Obj.transform.GetChild(0).GetComponent<SpriteRenderer>().color = chart.boxes[i].color;
            BoxObjects.Add(Obj);
        }
        for (int i = 0; i < chart.notenum; i++)
        {
            AddChart.NoteData ntdt = chart.notes[i];
            //NoteIdInBox.Add(NoteObjects[ntdt.targetbox].Count);
            if (ntdt.type == "Tap")
            {
                GameObject note = Instantiate(Tap);
                note.transform.parent = BoxObjects[ntdt.targetbox].transform;
                ViewNoteInfo V = note.GetComponent<ViewNoteInfo>();
                V.time_start = ntdt.time_start;
                V.speedoffset = ntdt.speedoffset;
                V.targetbox = ntdt.targetbox;
                V.notecolor = ntdt.color;
                V.ViewController = this;
                note.transform.localPosition = new Vector3((float)ntdt.xoffset, (float)ntdt.yoffset, 0);
                note.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, (float)ntdt.angleoffset));
                note.transform.localScale = new Vector3(1, 1, 0);
                note.transform.GetChild(0).localPosition = new Vector3(0, (float)ntdt.time_start * (float)(ntdt.speedoffset + chart.boxes[ntdt.targetbox].speed), -1);
                note.transform.GetChild(0).GetComponent<SpriteRenderer>().color = ntdt.color;
                NoteObjects.Add(note);
            }
            else if (ntdt.type == "Drag")
            {
                GameObject note = Instantiate(Drag);
                note.transform.parent = BoxObjects[ntdt.targetbox].transform;
                ViewNoteInfo V = note.GetComponent<ViewNoteInfo>();
                V.time_start = ntdt.time_start;
                V.speedoffset = ntdt.speedoffset;
                V.targetbox = ntdt.targetbox;
                V.notecolor = ntdt.color;
                V.ViewController = this;
                note.transform.localPosition = new Vector3((float)ntdt.xoffset, (float)ntdt.yoffset, 0);
                note.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, (float)ntdt.angleoffset));
                note.transform.localScale = new Vector3(1, 1, 0);
                note.transform.GetChild(0).localPosition = new Vector3(0, (float)ntdt.time_start * (float)(ntdt.speedoffset + chart.boxes[ntdt.targetbox].speed), -1);
                note.transform.GetChild(0).GetComponent<SpriteRenderer>().color = ntdt.color;
                NoteObjects.Add(note);
            }
            else if (ntdt.type == "Hold")
            {
                GameObject note = Instantiate(Hold);
                note.transform.parent = BoxObjects[ntdt.targetbox].transform;
                ViewNoteInfo V = note.GetComponent<ViewNoteInfo>();
                V.time_start = ntdt.time_start;
                V.time_end = ntdt.time_end;
                V.speedoffset = ntdt.speedoffset;
                V.targetbox = ntdt.targetbox;
                V.notecolor = ntdt.color;
                V.ViewController = this;
                note.transform.localPosition = new Vector3((float)ntdt.xoffset, (float)ntdt.yoffset, 0);
                note.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, (float)ntdt.angleoffset));
                note.transform.localScale = new Vector3(1, 1, 0);
                note.transform.GetChild(0).localPosition = new Vector3(0, (float)ntdt.time_start * (float)(ntdt.speedoffset + chart.boxes[ntdt.targetbox].speed), 1f);
                note.transform.GetChild(2).localPosition = new Vector3(0, (float)ntdt.time_end * (float)(ntdt.speedoffset + chart.boxes[ntdt.targetbox].speed), 1f);

                note.transform.GetChild(1).localPosition = (note.transform.GetChild(0).localPosition + note.transform.GetChild(2).localPosition) / 2;
                note.transform.GetChild(3).localPosition = new Vector3(note.transform.GetChild(3).localPosition.x, note.transform.GetChild(1).localPosition.y, 0.5f);
                note.transform.GetChild(4).localPosition = new Vector3(note.transform.GetChild(4).localPosition.x, note.transform.GetChild(1).localPosition.y, 0.5f);

                note.transform.GetChild(1).localScale = new Vector3((float)1.359375, note.transform.GetChild(2).localPosition.y - note.transform.GetChild(0).localPosition.y, 1);
                note.transform.GetChild(3).localScale = new Vector3((float)0.02586207, note.transform.GetChild(1).localScale.y, 1);
                note.transform.GetChild(4).localScale = new Vector3((float)0.02586207, note.transform.GetChild(1).localScale.y, 1);
                for (int k = 0; k < 5; k++)
                {
                    if (k == 3 || k == 4)
                    {
                        note.transform.GetChild(k).GetComponent<SpriteRenderer>().color = Color.black;
                    }
                    else if (k == 1)
                    {
                        var col = ntdt.color;
                        col.a = col.a * (float)0.6;
                        note.transform.GetChild(k).GetComponent<SpriteRenderer>().color = col;
                    }
                    else note.transform.GetChild(k).GetComponent<SpriteRenderer>().color = ntdt.color;
                }
                NoteObjects.Add(note);
            }
        }
        for (int i = 0; i < chart.eventnum; i++)
        {
            Events.Add(chart.events[i]);
        }
        for(int i = 0; i < chart.covernum; i++)
        {
            Debug.Log(SortingLayer.layers.Length);
            AddChart.CoverData _codt = chart.covers[i];
            GameObject Obj = new GameObject("Cover");
            Obj.transform.parent = ViewObjects.transform;
            Obj.transform.localPosition = new Vector3(0, 0, 0);
            GameObject _background = new GameObject("BackGround");
            _background.transform.parent = Obj.transform;
            _background.transform.localPosition = new Vector3(0, 0, 0);
            _background.transform.localScale = new Vector3(1000, 1000, 1);
            SpriteRenderer _SR =_background.AddComponent<SpriteRenderer>();
            _SR.color = _codt.color; _SR.sprite = EditController.SpriteGet("Square");
            _SR.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            _SR.sortingLayerID = SortingLayer.NameToID("GameDisplay");
            _SR.sortingOrder = _codt.groupBack + 1;
            foreach (AddChart.CoverPartData _copdt in _codt.CoverParts)
            {
                GameObject _part_obj = new GameObject("CoverPart");
                _part_obj.transform.parent = Obj.transform;
                _part_obj.transform.localPosition = new Vector3((float)_copdt.x,(float)_copdt.y,0);
                _part_obj.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, (float)_copdt.angle));
                _part_obj.transform.localScale = new Vector3((float)_copdt.xscale,(float)_copdt.yscale,1);
                SpriteMask _sprmask = _part_obj.AddComponent<SpriteMask>();
                _sprmask.backSortingLayerID = SortingLayer.NameToID("GameDisplay");
                _sprmask.frontSortingLayerID = SortingLayer.NameToID("GameDisplay");
                _sprmask.isCustomRangeActive = true;
                _sprmask.frontSortingOrder = _codt.groupFront;
                _sprmask.backSortingOrder = _codt.groupBack;
                _sprmask.sprite = EditController.SpriteGet(_copdt.spriteName);

            }
            CoverObjects.Add(Obj);
        }

    }

    void Start()
    {
        //Edit.onClick.AddListener(EditMode);
        SongSlider.onValueChanged.AddListener(SongLength);
        Play.onValueChanged.AddListener(PlayOrPause);
    }
    private float BeatToTime(float beat)
    {
        return beat * 60f /EditController.chart.bpm;
    }
    public void PlayOrPause(bool val)
    {
        if (val)
        {
            if (!GetComponent<AudioSource>().isPlaying && time<=wholeLength && time>=start_time)
            {
                GetComponent<AudioSource>().time = time-start_time;
                GetComponent<AudioSource>().Play();
            }
        }
        else
        {
            GetComponent<AudioSource>().Pause();
        }
    }
    public void EditMode()
    {
        EditController.gameObject.SetActive(true);
        EditCanvas.SetActive(true);
        EditObjects.SetActive(true);

        ViewCanvas.SetActive(false);
        ViewObjects.SetActive(false);
    }
    /*
    public void LoadView()
    {
        ViewCamera.orthographicSize = (float)160;
        width = 160;height = 160 * Screen.height / Screen.width;
        ViewCamera.transform.position = new Vector3(0, 0, 0);
        Vector2 v = ViewCamera.ViewportToWorldPoint(new Vector2(0,0));
        Vector2 v1 = ViewCamera.ViewportToWorldPoint(new Vector2(1, 1));
        Vector2 delta = v1 - v;
        float.TryParse(EditController.TimeOffset.text, out time_offset);
        float.TryParse(EditController.BeatOffset.text, out beat_set);
        wholeLength = Mathf.Max(EditController.songLength,EditController.songLength-time_offset+BeatToTime(beat_set))+2f;
        start_time = wholeLength - EditController.songLength;
        time_tobeat = wholeLength - (EditController.songLength - time_offset + BeatToTime(beat_set));
        time = 0;
        music = EditController.song;
        GetComponent<AudioSource>().clip = music;
        foreach (GameObject obj in BoxObjects)
        {
            DestroyImmediate(obj);
        }
        foreach (List<GameObject> ls in NoteObjects)
        {
            foreach(GameObject obj in ls)
            {
                DestroyImmediate(obj);
            }
            ls.Clear();
        }
        // Çå¿ÕÁÐ±í
        BoxObjects.Clear();
        NoteObjects.Clear();
        Events.Clear();
        NoteIdInBox.Clear();
        BoxObjects = new List<GameObject>();
        NoteObjects = new List<List<GameObject>>();
        Events = new List<AddChart.EventData>();
        NoteIdInBox = new List<int>();
        AddChart.ChartData chart = EditController.chart;
        for(int i = 0; i < chart.boxnum; i++)
        {
            GameObject Obj = Instantiate(Box);
            Obj.AddComponent<ViewBoxInfo>().speed = chart.boxes[i].speed;
            Obj.transform.parent = ViewObjects.transform;
            Obj.transform.position = new Vector3(v.x+ delta.x/2 + (float)chart.boxes[i].x,v.y+ delta.y/2+(float)chart.boxes[i].y, 0);
            Obj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, (float)chart.boxes[i].angle));
            Obj.transform.localScale = new Vector3(15,15,0);
            Obj.transform.GetChild(0).GetComponent<SpriteRenderer>().color = chart.boxes[i].color;
            BoxObjects.Add(Obj);
            NoteObjects.Add(new List<GameObject>());
        }
        for(int i = 0; i < chart.notenum; i++)
        {
            AddChart.NoteData ntdt = chart.notes[i];
            NoteIdInBox.Add(NoteObjects[ntdt.targetbox].Count);
            if (ntdt.type == "Tap")
            {
                GameObject note = Instantiate(Tap);
                note.transform.parent = BoxObjects[ntdt.targetbox].transform;
                ViewNoteInfo V = note.GetComponent<ViewNoteInfo>();
                V.time_start = ntdt.time_start;
                V.speedoffset = ntdt.speedoffset;
                V.targetbox = ntdt.targetbox;
                V.notecolor = ntdt.color;
                V.ViewController = this;
                note.transform.localPosition = new Vector3((float)ntdt.xoffset, (float)ntdt.yoffset, 0);
                note.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, (float)ntdt.angleoffset));
                note.transform.localScale = new Vector3(1, 1, 0);
                note.transform.GetChild(0).localPosition = new Vector3(0, (float)ntdt.time_start*(float)(ntdt.speedoffset+chart.boxes[ntdt.targetbox].speed),-1);
                note.transform.GetChild(0).GetComponent<SpriteRenderer>().color = ntdt.color;
                NoteObjects[ntdt.targetbox].Add(note);
            }
            else if (ntdt.type == "Drag")
            {
                GameObject note = Instantiate(Drag);
                note.transform.parent = BoxObjects[ntdt.targetbox].transform;
                ViewNoteInfo V = note.GetComponent<ViewNoteInfo>();
                V.time_start = ntdt.time_start;
                V.speedoffset = ntdt.speedoffset;
                V.targetbox = ntdt.targetbox;
                V.notecolor = ntdt.color;
                V.ViewController = this;
                note.transform.localPosition = new Vector3((float)ntdt.xoffset, (float)ntdt.yoffset, 0);
                note.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, (float)ntdt.angleoffset));
                note.transform.localScale = new Vector3(1, 1, 0);
                note.transform.GetChild(0).localPosition = new Vector3(0, (float)ntdt.time_start * (float)(ntdt.speedoffset + chart.boxes[ntdt.targetbox].speed), -1);
                note.transform.GetChild(0).GetComponent<SpriteRenderer>().color = ntdt.color;
                NoteObjects[ntdt.targetbox].Add(note);
            }
            else if(ntdt.type == "Hold")
            {
                GameObject note = Instantiate(Hold);
                note.transform.parent = BoxObjects[ntdt.targetbox].transform;
                ViewNoteInfo V = note.GetComponent<ViewNoteInfo>();
                V.time_start = ntdt.time_start;
                V.time_end = ntdt.time_end;
                V.speedoffset = ntdt.speedoffset;
                V.targetbox = ntdt.targetbox;
                V.notecolor = ntdt.color;
                V.ViewController = this;
                note.transform.localPosition = new Vector3((float)ntdt.xoffset, (float)ntdt.yoffset, 0);
                note.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, (float)ntdt.angleoffset));
                note.transform.localScale = new Vector3(1, 1, 0);
                note.transform.GetChild(0).localPosition = new Vector3(0,(float)ntdt.time_start * (float)(ntdt.speedoffset + chart.boxes[ntdt.targetbox].speed), 1f);
                note.transform.GetChild(2).localPosition = new Vector3(0,(float)ntdt.time_end * (float)(ntdt.speedoffset + chart.boxes[ntdt.targetbox].speed), 1f);
                
                note.transform.GetChild(1).localPosition = (note.transform.GetChild(0).localPosition + note.transform.GetChild(2).localPosition) / 2;
                note.transform.GetChild(3).localPosition = new Vector3(note.transform.GetChild(3).localPosition.x, note.transform.GetChild(1).localPosition.y, 0.5f);
                note.transform.GetChild(4).localPosition = new Vector3(note.transform.GetChild(4).localPosition.x, note.transform.GetChild(1).localPosition.y, 0.5f);

                note.transform.GetChild(1).localScale = new Vector3((float)1.359375, note.transform.GetChild(2).localPosition.y- note.transform.GetChild(0).localPosition.y, 1);
                note.transform.GetChild(3).localScale = new Vector3((float)0.02586207, note.transform.GetChild(1).localScale.y,1);
                note.transform.GetChild(4).localScale = new Vector3((float)0.02586207, note.transform.GetChild(1).localScale.y,1);
                for (int k = 0; k < 5; k++)
                {
                    if(k == 3 || k == 4)
                    {
                        note.transform.GetChild(k).GetComponent<SpriteRenderer>().color = Color.black;
                    }
                    else if (k == 1)
                    {
                        var col = ntdt.color;
                        col.a = col.a * (float)0.6;
                        note.transform.GetChild(k).GetComponent<SpriteRenderer>().color = col;
                    }
                    else note.transform.GetChild(k).GetComponent<SpriteRenderer>().color = ntdt.color;
                }
                NoteObjects[ntdt.targetbox].Add(note);
            }
        }
        for(int i = 0; i < chart.eventnum; i++)
        {
            Events.Add(chart.events[i]);
        }
        Debug.Log("Events num:"+Events.Count);
    }
    */
    public void SongLength(float val)
    {
        time = val * wholeLength;
        MusicLength.text = "Time:" + (time-start_time).ToString();

        Vector2 v = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        Vector2 v1 = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
        Vector2 delta = v1 - v;
        AddChart.ChartData chart = EditController.chart;
        for (int i = 0; i < BoxObjects.Count; i++)
        {
            GameObject Obj = BoxObjects[i];
            Obj.GetComponent<ViewBoxInfo>().speed = chart.boxes[i].speed;
            Obj.transform.localPosition = new Vector3((float)chart.boxes[i].x,(float)chart.boxes[i].y, 0);
            Obj.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, (float)chart.boxes[i].angle));
            Obj.transform.localScale = new Vector3(15, 15, 0);
            Obj.transform.GetChild(0).GetComponent<SpriteRenderer>().color = chart.boxes[i].color;
        }
        for (int i = 0; i < NoteObjects.Count; i++)
        {
            AddChart.NoteData ntdt = chart.notes[i];
            //int id2 = chart.notes[i].targetbox; int id3 = NoteIdInBox[i];
            GameObject note = NoteObjects[i];
            string T = note.GetComponent<ViewNoteInfo>().type;
            note.transform.localPosition = new Vector3((float)ntdt.xoffset, (float)ntdt.yoffset, 0);
            note.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, (float)ntdt.angleoffset));
            note.transform.localScale = new Vector3(1, 1, 0);
            if (T == "Tap" || T == "Drag")
            {
                float nowTime = Mathf.Max((float)note.GetComponent<ViewNoteInfo>().time_start + time_tobeat - time, 0);
                note.transform.GetChild(0).localPosition = new Vector3(0, nowTime * (float)(note.transform.parent.GetComponent<ViewBoxInfo>().speed + note.GetComponent<ViewNoteInfo>().speedoffset), 0);
            }
            else if (T == "Hold")
            {
                float nowTime = Mathf.Max((float)note.GetComponent<ViewNoteInfo>().time_start + time_tobeat - time, 0);
                float endTime = Mathf.Max((float)note.GetComponent<ViewNoteInfo>().time_end + time_tobeat - time, 0);
                float speed = (float)(note.transform.parent.GetComponent<ViewBoxInfo>().speed + note.GetComponent<ViewNoteInfo>().speedoffset);
                note.transform.GetChild(0).localPosition = new Vector3(0, nowTime * speed, -1);
                note.transform.GetChild(2).localPosition = new Vector3(0, endTime * speed, -1);

                note.transform.GetChild(1).localPosition = new Vector3(0, (endTime + nowTime) * speed / 2, 0);
                note.transform.GetChild(3).localPosition = new Vector3(0.6679688f, (endTime + nowTime) * speed / 2, -0.05f);
                note.transform.GetChild(4).localPosition = new Vector3(-0.6679688f, (endTime + nowTime) * speed / 2, -0.05f);

                note.transform.GetChild(1).localScale = new Vector3(1.359375f, (endTime - nowTime) * speed, 1);
                note.transform.GetChild(3).localScale = new Vector3(0.02586207f, note.transform.GetChild(1).localScale.y, 1);
                note.transform.GetChild(4).localScale = new Vector3(0.02586207f, note.transform.GetChild(1).localScale.y, 1);
            }
        }
        foreach(AddChart.EventData eve in Events)
        {
            if (time >= eve.time + time_tobeat)
            {
                switch (eve.Object)
                {
                    case "Box":
                        switch (eve.variable)
                        {
                            case "x":
                                Vector3 pos = new Vector3((float)eve.end, BoxObjects[eve.id].transform.localPosition.y, BoxObjects[eve.id].transform.localPosition.z);
                                BoxObjects[eve.id].transform.localPosition = pos;
                                break;
                            case "y":
                                Vector3 poss = new Vector3(BoxObjects[eve.id].transform.localPosition.x, (float)eve.end, BoxObjects[eve.id].transform.localPosition.z);
                                BoxObjects[eve.id].transform.localPosition = poss;
                                break;
                            case "speed":
                                BoxObjects[eve.id].GetComponent<ViewBoxInfo>().speed = eve.end;
                                break;
                            case "angle":
                                BoxObjects[eve.id].transform.localRotation = Quaternion.Euler(new Vector3(0, 0, (float)eve.end));
                                break;

                        }
                        break;
                    case "Note":
                        switch (eve.variable)
                        {
                            case "xoffset":
                                Vector3 pos = new Vector3((float)eve.end, NoteObjects[eve.id].transform.localPosition.y, NoteObjects[eve.id].transform.localPosition.z);
                                NoteObjects[eve.id].transform.localPosition = pos;
                                break;
                            case "yoffset":
                                Vector3 poss = new Vector3(NoteObjects[eve.id].transform.localPosition.x, (float)eve.end, NoteObjects[eve.id].transform.localPosition.z);
                                NoteObjects[eve.id].transform.localPosition = poss;
                                break;
                            case "speedoffset":
                                NoteObjects[eve.id].GetComponent<ViewNoteInfo>().speedoffset = (float)eve.end;
                                break;
                            case "angleoffset":
                                NoteObjects[eve.id].transform.rotation = Quaternion.Euler(new Vector3(0, 0, (float)eve.end));
                                break;
                        }

                        break;
                }
            }
            if(time - time_tobeat >= eve.time && time - time_tobeat <= eve.time + eve.during)
            {
                double eve_val = (time - time_tobeat - eve.time) / eve.during;
                double change_val = AnimationCollection.AnimationGet(eve.Tween,eve.Ease,eve_val);
                double variable_val = (eve.end - eve.start) * change_val + eve.start;
                switch (eve.Object)
                {
                    case "Box":
                        switch (eve.variable)
                        {
                            case "x":
                                Vector3 pos = new Vector3((float)variable_val, BoxObjects[eve.id].transform.localPosition.y, BoxObjects[eve.id].transform.localPosition.z);
                                BoxObjects[eve.id].transform.localPosition = pos;
                                break;
                            case "y":
                                Vector3 poss = new Vector3(BoxObjects[eve.id].transform.localPosition.x,(float)variable_val, BoxObjects[eve.id].transform.localPosition.z);
                                BoxObjects[eve.id].transform.localPosition = poss;
                                break;
                            case "speed":
                                BoxObjects[eve.id].GetComponent<ViewBoxInfo>().speed = variable_val;
                                break;
                            case "angle":
                                BoxObjects[eve.id].transform.localRotation = Quaternion.Euler(new Vector3(0, 0, (float)variable_val));
                                break;
                            
                        }
                        break;
                    case "Note":
                        switch (eve.variable)
                        {
                            case "xoffset":
                                Vector3 pos  = new Vector3((float)variable_val, NoteObjects[eve.id].transform.localPosition.y, NoteObjects[eve.id].transform.localPosition.z);
                                NoteObjects[eve.id].transform.localPosition = pos;
                                break;
                            case "yoffset":
                                Vector3 poss = new Vector3(NoteObjects[eve.id].transform.localPosition.x, (float)variable_val, NoteObjects[eve.id].transform.localPosition.z);
                                NoteObjects[eve.id].transform.localPosition = poss;
                                break;
                            case "speedoffset":
                                NoteObjects[eve.id].GetComponent<ViewNoteInfo>().speedoffset = (float)variable_val;
                                break;
                            case "angleoffset":
                                NoteObjects[eve.id].transform.rotation = Quaternion.Euler(new Vector3(0, 0, (float)variable_val));
                                break;
                        }
                
                        break;
                }
            }
        }

    }
    // Update is called once per frame
    void Update()
    {
        if (Play.isOn)
        {
            time += Time.deltaTime;
            SongSlider.value = time / wholeLength;
            SongLength(SongSlider.value);
            if( time >= start_time && !GetComponent<AudioSource>().isPlaying)
            {
                GetComponent<AudioSource>().time = time - start_time;
                GetComponent<AudioSource>().Play();
            }
        }
        SongLength(SongSlider.value);
    }
}
