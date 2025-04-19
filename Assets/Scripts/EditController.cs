using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.IO;
using System.IO.Compression;
using TMPro;
using System;
using UnityEngine.EventSystems;
using SFB;

public class EditController : MonoBehaviour
{
    public Slider CameraSlider;
    public float MinCameraSize;
    public float MaxCameraSize;


    public GameObject Square;
    public GameObject Border;
    public GameObject EditCanvas;
    public GameObject ViewCanvas;
    public GameObject EditObject;
    public GameObject ViewObject;
    public GameObject AnimObject;
    public GameObject Edits;

    public Scrollbar ShowBar;
    public Dropdown BeatType;
    public InputField PerBeat;
    public InputField BeatSum;
    public InputField TimeOffset;
    public TMP_InputField BeatOffset;
    public Toggle TapAndHold;
    public Toggle Drag;

    public Button AddBox;
    public Button View;
    public Button Save;
    public Toggle AddNote;
    public Toggle Choose;
    public Toggle Anim;
    public Toggle AutoSave;
    public Button Quit;
    public Button ImageImport;
    public Button Export;
    public Button EditCover;
    public GameObject ImageEdit;
    public RawImage ImageShower;
    public TMP_Dropdown ImageSelect;
    public Image SelectImageShower;

    public BoxEdit BoxObject;
    public NoteEdit NoteEditor;
    public AnimEdit AnimEditor;
    public CoverEdit CoverEditor;

    public ChartData chart;

    private string[] Info;
    private string Folderpath;

    //public List<GameObject> Box_Inst = new List<GameObject>();
    //public List<GameObject> Note_Inst = new List<GameObject>();
    //public List<GameObject> Event_Inst = new List<GameObject>();
    public List<Sprite> Sprite_Images = new List<Sprite>();

    public GameObject NowChosenBox = null;
    public GameObject NowChosenNote = null;
    public GameObject NowChosenEvent = null;

    [HideInInspector] public GameObject Borders;
    [HideInInspector] public GameObject BoxBorder;
    [HideInInspector] public GameObject NotesSquare;
    private List<GameObject> Lines = new List<GameObject>();
    private List<GameObject> BoxLines = new List<GameObject>();
    //[HideInInspector] public List<BoxData> Boxes;
    //[HideInInspector] public List<List<NoteData>> Notes = new List<List<NoteData>>();
    //[HideInInspector] public List<List<List<EventData>>> Note_Events = new List<List<List<EventData>>>();
    //[HideInInspector] public List<List<EventData>> Box_Events = new List<List<EventData>>();

    //[HideInInspector] public List<EventData> Events_all = new List<EventData>();
    //[HideInInspector] public List<List<GameObject>> Notes_WatchAble = new List<List<GameObject>>();

    private Vector3 MouseStartPos;
    private Vector3 NowMousePos;

    [HideInInspector] public float songLength;
    [HideInInspector] public int beats;
    [HideInInspector] public int NowPerBeat = 2;
    [HideInInspector] public int NowBeatSum = 4;

    private Toggle NowNoteType;
    public AudioClip song;

    private bool Chosen = false;
    private bool NoteTimeChosen = false;
    private int ChooseBox = -1;
    private double NowNoteStartTime;
    private double NowNoteStartBeat;
    private GameObject NowNote;

    private string chartpath;

    void Start()
    {
        //导入音乐
        string path = Application.persistentDataPath;
        if (!File.Exists(path + "/LoadLevel.txt"))
        {
            Debug.LogError("Cannot Find the chart file");
        }
        Info = File.ReadAllLines(path + "/LoadLevel.txt");
        Folderpath = path + "/" + Info[0];
        chartpath = Directory.GetFiles(Folderpath, "*.json", SearchOption.AllDirectories)[0];
        //chartpath = Folderpath + "/" + Info[1];
        string spritepath = Folderpath + "/Sprites";
        if (File.Exists(chartpath))
        {
            string jsonData = File.ReadAllText(chartpath);
            Debug.Log(jsonData);
            Debug.Log(JsonUtility.ToJson(new ChartData()));
            chart = JsonUtility.FromJson<ChartData>(jsonData);
        }
        else
        {
            Debug.LogError("Couldn't Find chart file!");
        }
        if (!Directory.Exists(spritepath))
        {
            Directory.CreateDirectory(spritepath);
        }
        Debug.Log(chart.notenum);
        StartCoroutine(GetAudioDuration());
        StartCoroutine(GetImages());
        //-------------------------------------------------------------//
        //-------------------------------------------------------------//
        BoxBorder = new GameObject("BoxBorder");
        BoxBorder.transform.parent = EditObject.transform;
        //Box分割线
        

        Borders = new GameObject("Borders");
        NotesSquare = new GameObject("NotesSquare");
        NotesSquare.transform.position = Borders.transform.position = new Vector3(0, 0, 0);
        NotesSquare.transform.parent = Borders.transform.parent = EditObject.transform;
        //-------------------------------------------------------------//
        //添加已有的box,note,event数据
        CoverEditor.Init();//Box依附在Cover上
        BoxObject.Init();
        NoteEditor.Init();
        AnimEditor.Init();
        TimeOffset.text = "0";
        //-------------------------------------------------------------//
        //添加按钮函数
        TimeOffset.text = chart.time_offset.ToString();
        BeatOffset.text = chart.beat_set.ToString();

        BeatType.onValueChanged.AddListener(ChangeBeatType);
        Anim.onValueChanged.AddListener(AnimShow);
        AddBox.onClick.AddListener(ChangeAddBox);
        View.onClick.AddListener(ViewMode);
        Save.onClick.AddListener(SaveChart);
        Quit.onClick.AddListener(QuitEditor);
        
        ImageImport.onClick.AddListener(_import_image);
        ShowBar.onValueChanged.AddListener(ChangeShowBar);
        Export.onClick.AddListener(_export_chart);
        EditCover.onClick.AddListener(_cover_edit);
        ImageSelect.onValueChanged.AddListener(_change_sprite_select);

        //-------------------------------------------------------------//
    }
    public Sprite SpriteGet(string _name)
    {
        for (int i = 0;i< Sprite_Images.Count; i++)
        {
            if (Sprite_Images[i].name == _name) return Sprite_Images[i];
        }
        return null;
    }
    public void CameraMoveTarget(GameObject Target)
    {
        Vector3 _world_pos = Target.transform.position;
        float screenWidth = 1920f;
        float screenCenterX = screenWidth / 2f;
        float targetScreenX = 600f;
        float offsetX = ((targetScreenX - screenCenterX) / screenWidth) * (Camera.main.orthographicSize * Camera.main.aspect * 2f);
        FixOnCamera _fix;
        float _pos_x = _world_pos.x - offsetX;
        float _pos_y = _world_pos.y;
        if (Target.TryGetComponent<FixOnCamera>(out _fix))
        {
            if (_fix.fixX) _pos_x = Camera.main.transform.position.x;
            if (_fix.fixY) _pos_y = Camera.main.transform.position.y;
        }

        Camera.main.transform.position = new Vector3(
            _pos_x,
            _pos_y,
            Camera.main.transform.position.z
        );
    }
    private void _import_image()
    {
        BoxObject.gameObject.SetActive(false);
        NoteEditor.gameObject.SetActive(false);
        var title = "Image Select";
        var extensions = new[] {
            new ExtensionFilter("Image File","png","jpg","jpeg")
        };
        var paths = StandaloneFileBrowser.OpenFilePanel(title, "", extensions, false);
        string _path = null;
        if (paths.Length > 0)
        {
            _path = paths[0];
        }
        if(_path != null)
        {
            StartCoroutine(AddImages(_path));
            AddBox.interactable = AddNote.interactable = Anim.interactable = ImageImport.interactable = false;
        }
    }
    private void _change_sprite_select(int _index)
    {
        Sprite _spr = Sprite_Images[_index];
        SelectImageShower.sprite = _spr;
        float scalerate = Math.Min(200f / _spr.texture.width, 200f / _spr.texture.height);
        SelectImageShower.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(_spr.texture.width * scalerate, _spr.texture.height * scalerate);
    }
    private void ChangeShowBar(float k)
    {
        int _starty = -415;
        int _endy = 100;
        RectTransform _rect = Edits.GetComponent<RectTransform>();
        _rect.localPosition = new Vector3(0,_starty*1.0f+(_endy-_starty)*k,0);
    }
    //-------------------------------------------------------------//
    //预览模式
    private void ViewMode()
    {
        SaveEdit();
    }
    //-------------------------------------------------------------//
    private void AnimShow(bool k)
    {
        if (k == false)
        {
            if(AnimEditor.gameObject.activeSelf == true)
            {
                AnimEditor.gameObject.SetActive(false);
                AnimObject.gameObject.SetActive(false);
                if(AnimEditor.Object == "Note")
                {
                    NoteEditor.gameObject.SetActive(true);
                }
                else if (AnimEditor.Object == "Box")
                {
                    BoxObject.gameObject.SetActive(true);
                }
                else
                {
                    CoverEditor.gameObject.SetActive(true);
                }
                BoxBorder.gameObject.SetActive(true);
                ImageImport.enabled = true;
            }
        }
        else
        {
            BoxBorder.gameObject.SetActive(false);
            AnimEditor.gameObject.SetActive(true);
            AnimObject.SetActive(true);
            AddNote.isOn = false;
            if (BoxObject.gameObject.activeSelf == true)
            {
                BoxObject.gameObject.SetActive(false);
                AnimEditor.ChangeTarget("Box",NowChosenBox);
            }
            else if (NoteEditor.gameObject.activeSelf == true)
            {
                NoteEditor.gameObject.SetActive(false);
                AnimEditor.ChangeTarget("Note",NoteEditor.GetTarget());
            }
            else if(CoverEditor.gameObject.activeSelf == true)
            {
                CoverEditor.gameObject.SetActive(false);
                if(CoverEditor.chosen_coverId != -1)
                {
                    if(CoverEditor.chosen_coverPartId != -1)
                    {
                        AnimEditor.ChangeTarget("CoverPart",CoverEditor.CoverObject[CoverEditor.chosen_coverId].transform.GetChild(CoverEditor.chosen_coverPartId).gameObject);
                    }
                    else
                    {
                        AnimEditor.ChangeTarget("Cover", CoverEditor.CoverObject[CoverEditor.chosen_coverId]);
                    }
                }
            }
            ImageImport.enabled = false;
        }

    }
    private void _cover_edit()
    {
        AnimEditor.gameObject.SetActive(false);
        AnimObject.gameObject.SetActive(false);
        BoxObject.gameObject.SetActive(false);
        NoteEditor.gameObject.SetActive(false);

        CoverEditor.gameObject.SetActive(true);
        
    }
    //-------------------------------------------------------------//
    //修改节奏线
    private void ChangeBeatType(int index)
    {
        string type = BeatType.options[index].text;
        switch (type)
        {
            case "2/4":
                AdjustLines(2, 4);
                NowPerBeat = 2;
                NowBeatSum = 4;
                break;
            case "3/4":
                AdjustLines(3, 4);
                NowPerBeat = 3;
                NowBeatSum = 4;
                break;
            case "4/4":
                AdjustLines(4, 4);
                NowPerBeat = 4;
                NowBeatSum = 4;
                break;
            case "6/8":
                AdjustLines(6, 8);
                NowPerBeat = 6;
                NowBeatSum = 8;
                break;
            case "others":
                string text_perbeat = PerBeat.text;
                string text_beatsum = BeatSum.text;
                int val_perbeat, val_beatsum;
                if (!int.TryParse(text_perbeat, out val_perbeat) && val_perbeat<=0)
                {
                    Debug.LogError("Wrong PerBeat!");
                    BeatType.value = 0;
                    AdjustLines(2, 4);
                    return;
                }
                if (!int.TryParse(text_beatsum, out val_beatsum) && val_beatsum<=0)
                {
                    Debug.LogError("Wrong BeatSum!");
                    BeatType.value = 0;
                    AdjustLines(2, 4);
                    return;
                }
                AdjustLines(val_perbeat, val_beatsum);
                NowPerBeat = val_perbeat;
                NowBeatSum = val_beatsum;
                break;
        }
    }
    private void ChangedBorder(GameObject line,int i,int perbeat,int beatsum)
    {
        line.transform.position = new Vector3(0, i * 20 / beatsum, 1);
        line.transform.parent = Borders.transform;
        SpriteRenderer spr = line.GetComponentInChildren<SpriteRenderer>();
        if (i % perbeat == 0)
        {
            spr.color = new Color32(0, 44, 154, 255);
        }
        else
        {
            spr.color = Color.black;
        }
        TextMesh text = line.GetComponentInChildren<TextMesh>();
        if (text != null)
        {
            int whole = i / beatsum;
            int left = i % beatsum;
            // 修改文本
            string T = whole.ToString();
            if (left != 0)
            {
                T += " + " + left.ToString() + "/" + beatsum.ToString();
       
            }
            text.text = T;
            if(i % perbeat == 0)
            {
                text.color = new Color32(0, 44, 154, 255);
            }
            else
            {
                text.color = Color.black;
            }
        }
    }
    private void AdjustLines(int perbeat,int beatsum)
    {
        int L = Lines.Count;
        int newsum = beats * beatsum;
        if(newsum <= L)
        {
            for(int i = 0; i < newsum; i++)
            {
                ChangedBorder(Lines[i], i, perbeat, beatsum);
            }
            for (int i = newsum; i < L; i++)
            {
                Destroy(Lines[i]);
            }
            Lines.RemoveRange(newsum,L-newsum);
        }
        else
        {
            for(int i = 0; i < newsum; i++)
            {
                if (i < L)
                {
                    ChangedBorder(Lines[i], i, perbeat, beatsum);
                }
                else
                {
                    GameObject line = Instantiate(Border);
                    FixOnCamera O = line.AddComponent<FixOnCamera>();
                    O.fixX = true;
                    ChangedBorder(line, i, perbeat, beatsum);
                    Lines.Add(line);
                }
            
            }
        }
    }
    //-------------------------------------------------------------//
    //获取音频文件
    IEnumerator GetAudioDuration()
    {
        // 使用UnityWebRequest下载MP3文件
        string pMusicPath = "";
        string[] _path_mp3 = Directory.GetFiles(Folderpath, "*.mp3", SearchOption.AllDirectories);
        string[] _path_wav = Directory.GetFiles(Folderpath, "*.wav", SearchOption.AllDirectories);
        string[] _path_ogg = Directory.GetFiles(Folderpath, "*.ogg", SearchOption.AllDirectories);
        if (_path_mp3.Length > 0)
        {
            pMusicPath = _path_mp3[0];
        }
        else if (_path_wav.Length > 0)
        {
            pMusicPath = _path_wav[0];
        }
        else if (_path_ogg.Length > 0)
        {
            pMusicPath = _path_ogg[0];
        }
        if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
        {
            pMusicPath = @"file://" + pMusicPath;
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            pMusicPath = @"file:///" + pMusicPath;
        }
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(pMusicPath, AudioType.MPEG);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.responseCode != 200)
        {
            Debug.LogError("Error: " + www.error);
            yield break;
        }

        // 获取AudioClip
       song = DownloadHandlerAudioClip.GetContent(www);
       if(song != null)
        {
            songLength = song.length;
            float length = songLength * chart.bpm / 60 + 1;
            beats = Mathf.RoundToInt(length);
            int beatsum = 4;int perbeat = 2;
            for (int i = 0; i < beats*4; i++)
            {
                GameObject line = Instantiate(Border);
                FixOnCamera O = line.AddComponent<FixOnCamera>();
                O.fixX = true;
                ChangedBorder(line, i, perbeat, beatsum);
                Lines.Add(line);
            }
        }
        ViewCanvas.GetComponent<ViewControl>().Init();
    }
    //获取图片文件
    IEnumerator GetImages()
    {
        string[] _spr_texture = Directory.GetFiles(Folderpath + "/Sprites","*.png",SearchOption.AllDirectories);
        Debug.Log(_spr_texture.Length);
        foreach(string _path in _spr_texture)
        {
            string _Url = "";
            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXEditor)
            {
                _Url = @"file://" + _path;
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                _Url = @"file:///" + _path;
            }
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(_Url);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                Texture2D _texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                string _name = Path.GetFileNameWithoutExtension(_Url);
                Debug.Log("贴图数量："+chart.imagenum);
                for (int i = 0; i < chart.imagenum; i++)
                {
                    ImageData _img = chart.images[i];
                    if (_name == _img.ImageName)
                    {
                        if (_img.filterMode == "Point") _texture.filterMode = FilterMode.Point;
                        else if (_img.filterMode == "Bilinear") _texture.filterMode = FilterMode.Bilinear;
                        else if (_img.filterMode == "Trilinear") _texture.filterMode = FilterMode.Trilinear;
                        Sprite _sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(0.5f, 0.5f));
                        _sprite.name = _img.ImageName;
                        Sprite_Images.Add(_sprite);
                    }
                }
            }
        }
        _change_sprite_select(0);
        for (int i = 0; i < Sprite_Images.Count; i++)
        {
            ImageSelect.options.Add(new TMP_Dropdown.OptionData(Sprite_Images[i].name));
            ImageSelect.options[ImageSelect.options.Count - 1].image = Sprite_Images[i];
            Debug.Log(Sprite_Images[i].name);
        }
    }
    IEnumerator AddImages(string _str)
    {
        string _Url = "";
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXEditor)
        {
            _Url = @"file://" + _str;
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            _Url = @"file:///" + _str;
        }
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(_Url);
        yield return request.SendWebRequest();
        string _name = Path.GetFileNameWithoutExtension(_Url);
        Debug.Log(_name);
        if(request.result == UnityWebRequest.Result.Success)
        {
            ImageEdit.SetActive(true);
            Texture2D _texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            ImageShower.texture = _texture;
            int _wid = _texture.width;int _hei = _texture.height;
            float scalerate = Math.Min((float)400/ _wid, (float)400 / _wid);
            ImageShower.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(_wid * scalerate, _hei * scalerate);
            ImageEdit.transform.GetChild(1).GetComponent<TMP_InputField>().text = "256";
            ImageEdit.transform.GetChild(2).GetComponent<TMP_InputField>().text = _name;
            ImageEdit.transform.GetChild(3).GetComponent<TMP_Dropdown>().value = 1;
            ImageEdit.transform.GetChild(4).GetComponent<TMP_Text>().text = "width:" + _wid.ToString();
            ImageEdit.transform.GetChild(5).GetComponent<TMP_Text>().text = "height:" + _hei.ToString();
        }
        ImageEdit.transform.GetChild(6).GetComponent<Button>().onClick.AddListener(_cancle_image);
        ImageEdit.transform.GetChild(7).GetComponent<Button>().onClick.AddListener(_add_image);
    }
    private void _cancle_image()
    {
        ImageEdit.SetActive(false);
        AddBox.interactable = AddNote.interactable = Anim.interactable = ImageImport.interactable = true;
    }
    private void _add_image()
    {
        Texture2D _tex = (Texture2D)ImageShower.texture;
        switch (ImageEdit.transform.GetChild(3).GetComponent<TMP_Dropdown>().value)
        {
            case 0:
                _tex.filterMode = FilterMode.Point;
                break;
            case 1:
                _tex.filterMode = FilterMode.Bilinear;
                break;
            case 2:
                _tex.filterMode = FilterMode.Trilinear;
                break;
        }
        //_tex.name = ImageEdit.transform.GetChild(2).GetComponent<TMP_InputField>().text;
        Sprite _sprite = Sprite.Create(_tex, new Rect(0, 0, _tex.width, _tex.height), new Vector2(0.5f, 0.5f));
        _sprite.name = ImageEdit.transform.GetChild(2).GetComponent<TMP_InputField>().text;
        Sprite_Images.Add(_sprite);
        ImageEdit.SetActive(false);
        AddBox.interactable = AddNote.interactable = Anim.interactable = ImageImport.interactable = true;

        byte[] _bytes = _tex.EncodeToPNG();
        File.WriteAllBytes(Folderpath + "/Sprites/"+_sprite.name+".png",_bytes);
        SaveEdit();

    }
    private void _export_chart()
    {
        SaveChart();
        var title = "选择文件夹";
        string[] _selectedFolders = StandaloneFileBrowser.OpenFolderPanel("选择文件夹", "", false);
        if (_selectedFolders.Length > 0)
        {
            string _select = _selectedFolders[0];
            Debug.Log("Selected Folder: " + _select);
            if (Directory.Exists(_select))
            {
                Debug.Log("Folder exists: " + _select);
                ZipFile.CreateFromDirectory(Folderpath, _select+"/"+ Info[0] + ".zip");
            }
            else
            {
                Debug.Log("folder path wrong");
            }
        }
        else
        {
            Debug.Log("No folder selected.");
        }
    }

    //-------------------------------------------------------------//
    //添加打击箱
    private void ChangeAddBox()
    {
        BoxObject.Add();
    }
    //-------------------------------------------------------------//
    //检测点击事件
    private void CheckPresses()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (AnimEditor.gameObject.activeSelf == false)
                {
                    Vector3 ClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    int id = (int)(ClickPos.x / 8);
                    if (id >= 0 && id < BoxObject.Boxes_List.Count)
                    {
                        if (BoxObject.BoxObjects[id] == NowChosenBox && AddNote.isOn)
                        {

                            //BoxObject.gameObject.SetActive(false); BoxObject.UpdateInfo();
                            double beat = 1.0 * (int)(ClickPos.y * NowBeatSum / 20 + 0.5) / NowBeatSum;
                            double time = beat / (1.0 * chart.bpm / 60);
                            if (Drag.isOn)
                            {
                                NoteData newDrag = new NoteData();
                                newDrag.angleoffset = newDrag.speedoffset = newDrag.xoffset = newDrag.yoffset = 0;
                                newDrag.type = "Drag";
                                newDrag.targetbox = ChooseBox;
                                newDrag.color = Color.white;
                                newDrag.beat_start = beat;
                                newDrag.time_start = time;

                                GameObject newDragWatchAble = Instantiate(Square);
                                newDragWatchAble.transform.parent = NowChosenBox.transform;
                                newDragWatchAble.transform.localPosition = new Vector3(0, (float)beat * 20, -5);
                                newDragWatchAble.transform.localScale = new Vector3(6, 1, 1);
                                newDragWatchAble.GetComponent<SpriteRenderer>().color = new Vector4(1, (float)0.92, (float)0.016, (float)0.6);
                                newDragWatchAble.AddComponent<BoxCollider>();

                                NowChosenNote = newDragWatchAble;
                                NoteEditor.Add(newDrag, newDragWatchAble,id);
                                NoteEditor.gameObject.SetActive(true);
                                //NoteEditor.LoadInfo(ChooseBox, Notes[ChooseBox].Count - 1);
                            }
                            else if (TapAndHold.isOn)
                            {
                                NoteTimeChosen = true;
                                NowNoteStartTime = time;
                                NowNoteStartBeat = beat;
                                NowNote = Instantiate(Square); NowNote.transform.parent = NowChosenBox.transform;
                                NowNote.transform.localPosition = new Vector3(0, (float)beat * 20, -5);
                                NowNote.transform.localScale = new Vector3(6, 1, 1);
                                NowNote.GetComponent<SpriteRenderer>().color = new Vector4(0, 0, 1, (float)0.6);
                            }
                        }
                        else if (Choose.isOn)
                        {
                            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                            if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Default")))
                            {
                                GameObject _hit = hit.collider.gameObject;
                                int _note_id = NoteEditor.NoteObjects.IndexOf(_hit);
                                if(_note_id != -1)
                                {
                                    NoteEditor.ChangeTarget(_note_id);
                                    BoxObject.gameObject.SetActive(false);
                                    NoteEditor.gameObject.SetActive(true);
                                }
                            }
                            else
                            {
                                NowChosenBox = BoxObject.BoxObjects[id];
                                Chosen = true;
                                ChooseBox = id;
                                BoxObject.gameObject.SetActive(true);
                                NoteEditor.gameObject.SetActive(false);
                                BoxObject.ChangeTarget(id);
                            }

                        }
                        else
                        {
                            NowChosenBox = BoxObject.BoxObjects[id];
                            ChooseBox = id;
                            Chosen = true;
                            NoteEditor.gameObject.SetActive(false);
                            BoxObject.gameObject.SetActive(true);
                            BoxObject.ChangeTarget(id);
                        }

                    }
                    else
                    {
                        Chosen = false;
                        ChooseBox = -1;
                        NowChosenBox = null;
                        BoxObject.gameObject.SetActive(false);
                        NoteEditor.gameObject.SetActive(false);

                    }
                }
            }
        }
        else if (Input.GetMouseButton(0) && NoteTimeChosen && AddNote.isOn) 
        {
            Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            double beat = 1.0 * (int)(MousePos.y * NowBeatSum / 20 + 0.5) / NowBeatSum;
            double time = beat / (1.0 * chart.bpm / 60);
            if (beat-NowNoteStartBeat<=0.01)
            {
                NowNote.transform.localPosition = new Vector3(0, (float)NowNoteStartBeat*20, -5);
                NowNote.transform.localScale = new Vector3(6, 1, 1);
            }
            else
            {
                NowNote.transform.localPosition = new Vector3(0, ((float)(NowNoteStartBeat +beat) * 20) / 2, -5);
                NowNote.transform.localScale = new Vector3(6, (float)(beat-NowNoteStartBeat) * 20, 1);
            }
        }
        else if(Input.GetMouseButtonUp(0) && NoteTimeChosen && AddNote.isOn)
        {
            //Debug.Log(ChooseBox);Debug.Log(Notes_WatchAble.Count);
            NoteTimeChosen = false;
            Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            double beat = 1.0 * (int)(MousePos.y * NowBeatSum / 20 + 0.5) / NowBeatSum;
            double time = beat / (1.0 * chart.bpm / 60);
            if (beat - NowNoteStartBeat <= 0.01)
            {
                NoteData newTap = new NoteData();
                newTap.angleoffset = newTap.speedoffset = newTap.xoffset = newTap.yoffset = 0;
                newTap.type = "Tap";
                newTap.targetbox = ChooseBox;
                newTap.color = Color.white;
                newTap.beat_start = beat;
                newTap.time_start = time;
                NowNote.AddComponent<BoxCollider>();
                NoteEditor.Add(newTap, NowNote, ChooseBox);
                NoteEditor.gameObject.SetActive(true);
                BoxObject.gameObject.SetActive(false);
            }
            else
            {
                NoteData newHold = new NoteData();
                newHold.angleoffset = newHold.speedoffset = newHold.xoffset = newHold.yoffset = 0;
                newHold.type = "Hold";
                newHold.targetbox = ChooseBox;
                newHold.color = Color.white;
                newHold.beat_start = NowNoteStartBeat;newHold.beat_end = beat;
                newHold.time_start = NowNoteStartTime;newHold.time_end = time;
                NowNote.AddComponent<BoxCollider>();
                NoteEditor.Add(newHold, NowNote, ChooseBox);
                NoteEditor.gameObject.SetActive(true);
                BoxObject.gameObject.SetActive(false);
                //NoteEditor.LoadInfo(ChooseBox, Notes[ChooseBox].Count - 1);
            }
            NowNote = null;
        }
    }
    private void SaveEdit()//存储小规模的修改
    {
        chart.time_offset = double.Parse(TimeOffset.text);
        chart.beat_set = double.Parse(BeatOffset.text);
        chart.boxnum = BoxObject.Boxes_List.Count;
        chart.boxes = new BoxData[chart.boxnum];
        for (int i=0;i< chart.boxnum; i++)
        {
            chart.boxes[i] = BoxObject.Boxes_List[i];
        }
        chart.notenum = NoteEditor.Notes_List.Count;

        chart.notes = new NoteData[chart.notenum];
        for (int i = 0; i < chart.notenum; i++)
        {
            chart.notes[i] = NoteEditor.Notes_List[i];
        }

        chart.events = AnimEditor.GetEventArray();
        chart.eventnum = chart.events.Length;
        chart.imagenum = Sprite_Images.Count - 8;
        chart.images = new ImageData[chart.imagenum];
        for (int i=8;i< Sprite_Images.Count; i++)
        {
            Texture2D _tex = Sprite_Images[i].texture;
            chart.images[i - 8].ImageName = Sprite_Images[i].name;
            if(_tex.filterMode == FilterMode.Point)
            {
                chart.images[i - 8].filterMode = "Point";
            }
            else if (_tex.filterMode == FilterMode.Bilinear)
            {
                chart.images[i - 8].filterMode = "Bilinear";
            }
            else if (_tex.filterMode == FilterMode.Trilinear)
            {
                chart.images[i - 8].filterMode = "Trilinear";
            }
        }
        chart.covernum = CoverEditor.covers.Count;
        chart.covers = CoverEditor.covers.ToArray();
        chart.coverpartnum = CoverEditor.coverParts.Count;
        chart.coverparts = CoverEditor.coverParts.ToArray();
        ViewCanvas.GetComponent<ViewControl>().Init();
    }
    public void SaveChart()//保存修改到本地
    {
        SaveEdit();
        string jsonData = JsonUtility.ToJson(chart,true);
        File.WriteAllText(chartpath,jsonData);
    }
    private void QuitEditor()
    {
        if (AutoSave.isOn) SaveChart();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
    void Update()
    {
        CheckPresses();
        Camera.main.orthographicSize = MinCameraSize + (MaxCameraSize - MinCameraSize) * CameraSlider.value;
        if (Input.GetMouseButtonDown(1))
        {
            NowMousePos = MouseStartPos = Input.mousePosition;
        }
        if (Input.GetMouseButton(1))
        {
            NowMousePos = Input.mousePosition;
            Vector3 DeltaPos = (NowMousePos - MouseStartPos);
            Camera.main.transform.position += new Vector3(-DeltaPos.x*Camera.main.orthographicSize/300, -DeltaPos.y * Camera.main.orthographicSize / 300, 0);
            MouseStartPos = NowMousePos;
        }

    }
}
