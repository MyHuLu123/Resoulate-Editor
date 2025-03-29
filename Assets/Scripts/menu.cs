using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using SFB;
using System.IO.Compression;

public class menu : MonoBehaviour
{
    public List<string> FileInfo = new List<string>();
    public Button ChartImport;
    public Dropdown History;
    public Button Enter;
    void Start()
    {
        History.onValueChanged.AddListener(choose);
        Enter.onClick.AddListener(enter);
        ChartImport.onClick.AddListener(_import);
        string[] Info = null;
        var path = Application.persistentDataPath + "/LoadLevel.txt";
        if (!File.Exists(path))
        {
            FileStream fs = File.Create(path);
            fs.Close();
        }
        path = Application.persistentDataPath + "/NowFiles.txt";
        if (!File.Exists(path))
        {
            FileStream fs = File.Create(path);
            fs.Close();
        }
        else
        {
            Info = File.ReadAllLines(path);
        }
        if (Info == null) return;
        if (Info.Length == 0) Enter.interactable = false;
        else
        {
            Enter.interactable = true;
        }
        string _now = File.ReadAllText(Application.persistentDataPath + "/LoadLevel.txt");
        bool _select = false;
        for (int i = 0; i < Info.Length; i ++)
        {
            FileInfo.Add(Info[i]);
            History.options.Add(new Dropdown.OptionData(Info[i]));
            if (Info[i] == _now) {
                History.value = i;
                _select = true;
            }
        }
        if(!_select && FileInfo.Count > 0)
        {
            History.value = 0;
            File.WriteAllText(Application.persistentDataPath + "/LoadLevel.txt", FileInfo[0]);
        }
    }
    private void choose(int val)
    {
        var path = Application.persistentDataPath + "/LoadLevel.txt";
        File.WriteAllText(path, FileInfo[val]);

    }
    private void _import()
    {
        var title = "Chart Zip Select";
        var extensions = new[] {
            new ExtensionFilter("ZIP","zip")
        };
        string[] paths = StandaloneFileBrowser.OpenFilePanel(title, "", extensions, true);
        Debug.Log("filesnum:"+paths.Length);
        
        for(int i = 0; i < paths.Length; i++)
        {
            string _str = paths[i];
            string _name = Path.GetFileNameWithoutExtension(_str);
            if (File.Exists(_str))
            {
                Debug.Log(_str + "\n" + _name);
                Directory.CreateDirectory(Application.persistentDataPath + "/" + _name);
                ZipFile.ExtractToDirectory(_str, Application.persistentDataPath + "/" + _name);
                History.options.Add(new Dropdown.OptionData(_name));
                if(History.options.Count == 1)
                {
                    File.WriteAllText(Application.persistentDataPath + "/LoadLevel.txt", _name);
                    History.value = 0;
                    Enter.interactable = true;

                }

                FileInfo.Add(_name);
                File.AppendAllText(Application.persistentDataPath + "/NowFiles.txt", _name + "\n");
                Debug.Log("Success!");
            }
            
        }
    }
    private void enter()
    {
        SceneManager.LoadScene("edit");
    }
}
