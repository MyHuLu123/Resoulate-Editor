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

