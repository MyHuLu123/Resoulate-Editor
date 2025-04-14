using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxProperties : MonoBehaviour
{
    private GameObject TargetEvent;
    private GameObject TargetCover;
    private BoxData BoxData = new BoxData();
    public BoxProperties()
    {
        TargetEvent = null;
        TargetCover = null;
        BoxData = new BoxData();
    }
    public GameObject GetEvent()
    {
        return TargetEvent;
    }
    public BoxData GetBoxData()
    {
        return BoxData;
    }
    public void ChangeVar(string var_type, string val)
    {
        switch (var_type)
        {
            case "x":
                double.TryParse(val, out BoxData.x);
                break;
            case "y":
                double.TryParse(val, out BoxData.y);
                break;
            case "speed":
                double.TryParse(val, out BoxData.speed);
                break;
            case "angle":
                double.TryParse(val, out BoxData.angle);
                break;
            case "color_r":
                float.TryParse(val, out BoxData.color.r);
                break;
            case "color_g":
                float.TryParse(val, out BoxData.color.g);
                break;
            case "color_b":
                float.TryParse(val, out BoxData.color.b);
                break;
            case "color_a":
                float.TryParse(val, out BoxData.color.a);
                break;
        }
    }
    public void ChangeCover(GameObject newcover)
    {
        TargetCover = newcover;
    }
    public void ChangeEvent(GameObject newevent)
    {
        TargetEvent = newevent;
    }
    public void NewBox()
    {
        BoxData = new BoxData();
    }
    public void NewBox(BoxData boxdata)
    {
        BoxData = boxdata;
    }
}
