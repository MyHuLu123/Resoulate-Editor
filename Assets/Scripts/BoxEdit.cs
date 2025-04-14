using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoxEdit : MonoBehaviour
{
    public EditController Editor;
    public CoverEdit CoverEditor;
    private GameObject TargetBox = null;

    public List<GameObject> TargetCoverObject = new List<GameObject>();
    public List<BoxData> Boxes_List = new List<BoxData>();
    public List<GameObject> BoxObjects = new List<GameObject>();

    private int _now_id = -1;

    public InputField boxX;
    public InputField boxY;
    public InputField boxSpeed;
    public InputField boxAngle;
    public TMP_InputField ID;
    public ColorSet Color;
    public TMP_Text IDRange;
    void Start()
    {
        boxX.onEndEdit.AddListener(_xchange);
        boxY.onEndEdit.AddListener(_ychange);
        boxSpeed.onEndEdit.AddListener(_speedchange);
        boxAngle.onEndEdit.AddListener(_anglechange);
        ID.onEndEdit.AddListener(_idchange);
        Color.OnColorChanged += _colorchange;
    }
    public void Init()
    {
        ChartData _chart = Editor.chart;
        GameObject _parent = GameObject.Find("NotesSquare");
        for (int i = 0; i < _chart.boxnum; i++)
        {
            Boxes_List.Add(_chart.boxes[i]);
            GameObject _new_box = new GameObject("Box_"+i.ToString());
            _new_box.transform.parent = _parent.transform;
            _new_box.transform.localPosition = new Vector3(0, 0, 0);
            BoxObjects.Add(_new_box);
            int _cover_id = _chart.boxes[i].targetcover;
            if (_cover_id == -1) TargetCoverObject.Add(CoverEditor.Object_None);
            else TargetCoverObject.Add(CoverEditor.CoverObject[_chart.boxes[i].targetcover]);
        }
        ID.text = "-1";_now_id = -1;
    }
    private void _idchange(string val)
    {
        if (string.IsNullOrEmpty(val))
        {
            ChangeTarget(-1);
            return;
        }
        ChangeTarget(int.Parse(val));
    }
    private void _xchange(string x)
    {
        if (string.IsNullOrEmpty(x))
        {
            x = boxX.text = "0";
        }
        if (_now_id == -1) return;
        BoxData _boxdata = Boxes_List[_now_id];
        _boxdata.x = double.Parse(x);
        Boxes_List[_now_id] = _boxdata;
    }
    private void _ychange(string y)
    {
        if (string.IsNullOrEmpty(y))
        {
            y = boxY.text = "0";
        }
        if (_now_id == -1) return;
        BoxData _boxdata = Boxes_List[_now_id];
        _boxdata.y = double.Parse(y);
        Boxes_List[_now_id] = _boxdata;
    }
    private void _speedchange(string speed)
    {
        if (string.IsNullOrEmpty(speed))
        {
            speed = boxSpeed.text = "0";
        }
        if (_now_id == -1) return;
        BoxData _boxdata = Boxes_List[_now_id];
        _boxdata.speed = double.Parse(speed);
        Boxes_List[_now_id] = _boxdata;
    }
    private void _anglechange(string angle)
    {
        if (string.IsNullOrEmpty(angle))
        {
            angle = boxAngle.text = "0";
        }
        if (_now_id == -1) return;
        BoxData _boxdata = Boxes_List[_now_id];
        _boxdata.angle = double.Parse(angle);
        Boxes_List[_now_id] = _boxdata;
    }
    private void _colorchange(Color newcolor)
    {
        if (_now_id == -1) return;
        BoxData _boxdata = Boxes_List[_now_id];
        _boxdata.color = newcolor;
        Boxes_List[_now_id] = _boxdata;
    }
    public void ChangeTarget(GameObject newbox)
    {
        _now_id = BoxObjects.IndexOf(newbox);
        ID.text = _now_id.ToString();
        if (_now_id == -1)
        {
            Debug.Log("Doesn't exist Box Object");
            return;
        }
        BoxData GetInfo = Boxes_List[_now_id]; 
        boxX.text = GetInfo.x.ToString();
        boxY.text = GetInfo.y.ToString();
        boxSpeed.text = GetInfo.speed.ToString();
        boxAngle.text = GetInfo.angle.ToString();
        Color.SetColor(GetInfo.color);
        return;
    }
    public void ChangeTarget(int index)
    {
        if(index < 0 || index >= Boxes_List.Count)
        {
            _now_id = -1;
            ID.text = _now_id.ToString();
            return;
        }
        _now_id = index;
        ID.text = _now_id.ToString();
        BoxData GetInfo = Boxes_List[_now_id];
        boxX.text = GetInfo.x.ToString();
        boxY.text = GetInfo.y.ToString();
        boxSpeed.text = GetInfo.speed.ToString();
        boxAngle.text = GetInfo.angle.ToString();
        Color.SetColor(GetInfo.color);
        return;
    }
    void OnDestroy()
    {
        boxX.onEndEdit.RemoveAllListeners();
        boxY.onEndEdit.RemoveAllListeners();
        boxSpeed.onEndEdit.RemoveAllListeners();
        boxAngle.onEndEdit.RemoveAllListeners();
        Color.OnColorChanged -= _colorchange;
    }
}
