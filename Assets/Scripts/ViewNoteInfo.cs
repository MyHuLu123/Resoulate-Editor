using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewNoteInfo : MonoBehaviour
{
    public int targetbox;
    public string type;
    public double time_start;
    public double time_end;
    public double speedoffset;
    public Color notecolor;
    public ViewControl ViewController;
    void Update()
    {
        if(type == "Tap" || type == "Drag")
        {
            if(ViewController.time - ViewController.time_tobeat > time_start)
            {
                SpriteRenderer spr = transform.GetChild(0).GetComponent<SpriteRenderer>();
                spr.color = new Vector4(0, 0, 0, 0);
            }
            else
            {
                transform.GetChild(0).GetComponent<SpriteRenderer>().color = notecolor;
            }
        }
        else
        {
            if (ViewController.time - ViewController.time_tobeat > time_end)
            {
                for (int k = 0; k < 5; k++)
                {
                    transform.GetChild(k).GetComponent<SpriteRenderer>().color = new Vector4(0, 0, 0, 0);
                }
            }
            else
            {
                for (int k = 0; k < 5; k++)
                {
                    if (k == 3 || k == 4)
                    {
                        transform.GetChild(k).GetComponent<SpriteRenderer>().color = Color.black;
                    }
                    else if (k == 1)
                    {
                        var col = notecolor;
                        col.a = col.a * (float)0.6;
                        transform.GetChild(k).GetComponent<SpriteRenderer>().color = col;
                    }
                    else transform.GetChild(k).GetComponent<SpriteRenderer>().color = notecolor;
                }
            }
        }
    }
}
