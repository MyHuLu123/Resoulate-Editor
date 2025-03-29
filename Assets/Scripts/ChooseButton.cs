using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseButton : MonoBehaviour
{
    void Start()
    {
        Toggle T = this.GetComponent<Toggle>();
        T.onValueChanged.AddListener(ChooseChosen);
    }
    private void ChooseChosen(bool val)
    {
        Image image = this.GetComponent<Image>();
        if (!val)
        {
            image.color = new Vector4(1,1,1,1);
        }
        else image.color = new Vector4((float)0.5, (float)0.5, (float)0.5, 1);
    }
    private void Update()
    {
        if(GetComponent<Toggle>().enabled == false && GetComponent<Image>().color.r != 0.3)
        {
            GetComponent<Image>().color = new Vector4(0.3f, 0.3f, 0.3f, 1);
        }
        else if (GetComponent<Toggle>().enabled == true && GetComponent<Image>().color.r == 0.3)
        {
            GetComponent<Image>().color = new Vector4(1, 1, 1, 1);
        }
    }
}
