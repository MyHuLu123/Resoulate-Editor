using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventProperties : MonoBehaviour
{
    private GameObject TargetObject;
    private AddChart.EventData EventData = new AddChart.EventData();
    public void ChangeObject(GameObject obj)
    {
        TargetObject = obj;
    }
    public GameObject GetObject()
    {
        return TargetObject;
    }
    public AddChart.EventData GetEventData()
    {
        return EventData;
    }
    public void ChangeVar(string var_type,string val)
    {
        switch (var_type)
        {
            case "time":
                double.TryParse(val, out EventData.time);
                break;
            case "beat":
                double.TryParse(val, out EventData.beat);
                break;
            case "endbeat":
                double.TryParse(val, out EventData.endbeat);
                if(EventData.endbeat <= EventData.beat)
                {
                    EventData.endbeat = EventData.beat + 0.01;
                }
                break;
            case "during":
                double.TryParse(val, out EventData.during);
                break;
            case "start":
                double.TryParse(val, out EventData.start);
                break;
            case "end":
                double.TryParse(val, out EventData.end);
                break;
            case "first":
                double.TryParse(val, out EventData.first);
                break;
            case "last":
                double.TryParse(val, out EventData.last);
                break;
            case "Ease":
                EventData.Ease = val;
                break;
            case "Tween":
                EventData.Tween = val;
                break;
        }
    }
    public void NewEvent()
    {
        EventData = new AddChart.EventData();
    }
    public void NewEvent(AddChart.EventData newevent)
    {
        EventData = newevent;
    }
}
