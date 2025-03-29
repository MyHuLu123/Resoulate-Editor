using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BoxEdit : MonoBehaviour
{
    public EditController Editor;
    private GameObject TargetBox = null;

    public Text boxid;
    public InputField boxX;
    public InputField boxY;
    public InputField boxSpeed;
    public InputField boxAngle;
    public ColorSet Color;
    void Start()
    {
        gameObject.SetActive(false);
        boxX.onEndEdit.AddListener(_xchange);
        boxY.onEndEdit.AddListener(_ychange);
        boxSpeed.onEndEdit.AddListener(_speedchange);
        boxAngle.onEndEdit.AddListener(_anglechange);
        Color.OnColorChanged += _colorchange;
    }
    private void _xchange(string x)
    {
        if (string.IsNullOrEmpty(x))
        {
            x = boxX.text = "0";
        }
        TargetBox.GetComponent<BoxProperties>().ChangeVar("x",x);
    }
    private void _ychange(string y)
    {
        if (string.IsNullOrEmpty(y))
        {
            y = boxY.text = "0";
        }
        TargetBox.GetComponent<BoxProperties>().ChangeVar("y", y);
    }
    private void _speedchange(string speed)
    {
        if (string.IsNullOrEmpty(speed))
        {
            speed = boxSpeed.text = "0";
        }
        TargetBox.GetComponent<BoxProperties>().ChangeVar("speed", speed);
    }
    private void _anglechange(string angle)
    {
        if (string.IsNullOrEmpty(angle))
        {
            angle = boxAngle.text = "0";
        }
        TargetBox.GetComponent<BoxProperties>().ChangeVar("angle", angle);
    }
    private void _colorchange(Color newcolor)
    {
        TargetBox.GetComponent<BoxProperties>().ChangeVar("color_r", newcolor.r.ToString());
        TargetBox.GetComponent<BoxProperties>().ChangeVar("color_g", newcolor.g.ToString());
        TargetBox.GetComponent<BoxProperties>().ChangeVar("color_b", newcolor.b.ToString());
        TargetBox.GetComponent<BoxProperties>().ChangeVar("color_a", newcolor.a.ToString());
    }
    public void ChangeTarget(GameObject newbox)
    {
        TargetBox = newbox;
        AddChart.BoxData GetInfo = TargetBox.GetComponent<BoxProperties>().GetBoxData();
        boxX.text = GetInfo.x.ToString();
        boxY.text = GetInfo.y.ToString();
        boxSpeed.text = GetInfo.speed.ToString();
        boxAngle.text = GetInfo.angle.ToString();
        Color.SetColor(GetInfo.color);
        boxid.text = (Editor.Box_Inst.IndexOf(TargetBox)).ToString();
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
