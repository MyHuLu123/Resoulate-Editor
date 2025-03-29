using TMPro;
using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AddChart : MonoBehaviour
{
    public TMP_InputField _folder;
    public TMP_InputField _file;
    public TMP_InputField _bpm;
    public GetMusicFile _musicfile;
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
            return p1.type == p2.type && p1.beat_start == p2.beat_start && p1.beat_end == p2.beat_end && p1.xoffset==p2.xoffset && p1.yoffset==p2.yoffset && p1.targetbox==p2.targetbox && p1.color==p2.color && p1.angleoffset==p2.angleoffset && p1.speedoffset==p2.speedoffset;
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
            return HashCode.Combine(type, beat_start,beat_end,targetbox);
        }

    };
    [Serializable]
    public struct BoxData
    {
        public double x;
        public double y;
        public double speed;
        public Color color;
        public double angle;

        public int sortorder;
        public int targetcover;
    };
    [Serializable]
    public struct EventData
    {
        public string Object;
        public int id;
        public string variable;

        public double time;//这两个变量控制开始时间和持续时间
        public double beat;
        public double endbeat;
        public double during;
        public double start;//这两个变量控制初始值和结束值
        public double end;
        public double first;//这两个变量控制曲线范围（连续区间）
        public double last;
        public string Tween;
        public string Ease;
    };
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
    [Serializable]
    public struct CoverPartData
    {
        public string spriteName;
        public double x;
        public double y;
        public double xscale;
        public double yscale;
        public double angle;
        public bool usePolygonCollider;
    }
    [Serializable]
    public struct ImageData
    {
        public string ImageName;
        public string filterMode;

    }
    public struct SpriteData
    {
        public string spriteName;
        public int sortorder;
        public double x;
        public double y;
        public double xscale;
        public double yscale;
        public double angle;
        public Color color;

    }
    [Serializable]
    public struct ChartData
    {
        /*
        public string name;
        public string composer;
        public string chart;
        public string illustrate;
        public string type;
        public string level;
        */
        public float bpm;
        public double[][] bpmlist;
        public int boxnum;
        public BoxData[] boxes;
        public int notenum;
        public NoteData[] notes;
        public int eventnum;
        public EventData[] events;
        public int covernum;
        public CoverData[] covers;
        public int imagenum;
        public ImageData[] images;
        public int spritenum;
        public SpriteData sprites;
        public double time_offset;
        public double beat_set;

    };
    private void Awake()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    // Update is called once per frame
    private void OnClick()
    {
        ChartData data = new ChartData();
        if (_musicfile._path.Length > 0)
        {
            var path = Application.persistentDataPath + "/LoadLevel.txt";
            if (!float.TryParse(_bpm.text, out data.bpm))
            {
                Debug.LogError("Wrong BPM!");
                return;
            }     
            /*
            data.name = _name.text;
            data.composer = _composer.text;
            data.chart = _chart.text;
            data.illustrate = _illustrate.text;
            data.type = _type.text;
            data.level = _level.text;
            */
            data.boxnum = data.notenum = data.eventnum = 0;
            string jsonData = JsonUtility.ToJson(data);

            string FolderName = _folder.text;
            string FileName = _file.text + ".json";
            string FolderPath = Path.Combine(Application.persistentDataPath, FolderName);
            string FilePath = Path.Combine(FolderPath, FileName);
            string pMusicPath = "";
            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXEditor)
            {
                pMusicPath = @"file://" + _musicfile._path;
            }
            else if(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                pMusicPath = @"file:///" + _musicfile._path;
            }
            string _extension = Path.GetExtension(_musicfile._path).ToLower();
            string MusicPath = FolderPath + "/" + Path.GetFileName(_musicfile._path);
            switch (_extension)
            {
                case ".mp3":
                    MusicPath += ".mp3";
                    break;
                case ".wav":
                    MusicPath += ".wav";
                    break;
                case ".ogg":
                    MusicPath += ".ogg";
                    break;
            }
            if (!string.IsNullOrEmpty(FolderPath) && !Directory.Exists(FolderPath))
            {
                string[] contents = {FolderName};
                File.WriteAllLines(path, contents);
                Directory.CreateDirectory(FolderPath);
                WWW ww = new WWW(pMusicPath);
                while (!ww.isDone) { }
                var buffer = ww.bytes;
                if (File.Exists(MusicPath))
                    File.Delete(MusicPath);
                var ws = File.Create(MusicPath);
                ws.Write(buffer, 0, buffer.Length);
                ws.Close();
                ww.Dispose();
                File.WriteAllText(FilePath, jsonData);
                Debug.Log("Saved!Path:"+FolderPath);
                var HistoryPath = Application.persistentDataPath + "/NowFiles.txt";
                File.AppendAllLines(HistoryPath,contents);
                SceneManager.LoadScene("edit");
            }
            else
            {
                Debug.LogError("Failed!");
                return;
            }
        }
        else
        {
            Debug.LogError("Warning!No Music Selected!");
        }
    }
}

