using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ColorSet : MonoBehaviour
{
    public Image colorDisplayImage;
    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;
    public Slider alphaSlider;
    public InputField redInput;
    public InputField greenInput;
    public InputField blueInput;
    public InputField alphaInput;
    // Start is called before the first frame update

    public delegate void OnValueChangedDelegate(Color newColor);
    public event OnValueChangedDelegate OnColorChanged;
    void Start()
    {
        redInput.onValueChanged.AddListener(ColorChangeInput);
        redInput.contentType = InputField.ContentType.IntegerNumber;
        blueInput.onValueChanged.AddListener(ColorChangeInput);
        blueInput.contentType = InputField.ContentType.IntegerNumber;
        greenInput.onValueChanged.AddListener(ColorChangeInput);
        greenInput.contentType = InputField.ContentType.IntegerNumber;
        alphaInput.onValueChanged.AddListener(ColorChangeInput);
        alphaInput.contentType = InputField.ContentType.IntegerNumber;

        redSlider.onValueChanged.AddListener(ColorChangeSlider);
        blueSlider.onValueChanged.AddListener(ColorChangeSlider);
        greenSlider.onValueChanged.AddListener(ColorChangeSlider);
        alphaSlider.onValueChanged.AddListener(ColorChangeSlider);
    }
    private void ChangeColorSprite()
    {
        if (colorDisplayImage != null)
        {
            colorDisplayImage.color = new Color(
                redSlider.value,
                greenSlider.value,
                blueSlider.value,
                alphaSlider.value
            );
            OnColorChanged?.Invoke(colorDisplayImage.color);
        }
    }
    private void ColorChangeInput(string change)   //当InputField的内容改变时，修改slider
    {
        float val;
        if (float.TryParse(redInput.text, out val))
        {
            redSlider.value = (val/255);
        }
        if (float.TryParse(blueInput.text, out val))
        {
            blueSlider.value = val/255;
        }
        if (float.TryParse(greenInput.text, out val))
        {
            greenSlider.value = val/255;
        }
        if (float.TryParse(alphaInput.text, out val))
        {
            alphaSlider.value = val/255;
        }
        ChangeColorSprite();
    }
    private void ColorChangeSlider(float change)   //当slider的内容改变时，修改InputField
    {
        redInput.text = ((int)(redSlider.value*255)).ToString();
        blueInput.text = ((int)(blueSlider.value*255)).ToString();
        greenInput.text = ((int)(greenSlider.value*255)).ToString();
        alphaInput.text = ((int)(alphaSlider.value*255)).ToString();
        ChangeColorSprite();
    }
    public void SetColor(Color col)
    {
        redSlider.value = col.r; redInput.text = ((int)(col.r*255)).ToString();
        blueSlider.value = col.b; blueInput.text = ((int)(col.b * 255)).ToString();
        greenSlider.value = col.g; greenInput.text = ((int)(col.g * 255)).ToString();
        alphaSlider.value = col.a; alphaInput.text = ((int)(col.a * 255)).ToString();
    }
    public Color GetColor()
    {
        return colorDisplayImage.color;
    }
    void OnDestroy()
    {
        redInput.onValueChanged.RemoveAllListeners();
        blueInput.onValueChanged.RemoveAllListeners();
        greenInput.onValueChanged.RemoveAllListeners();
        alphaInput.onValueChanged.RemoveAllListeners();
        redSlider.onValueChanged.RemoveAllListeners();
        blueSlider.onValueChanged.RemoveAllListeners();
        greenSlider.onValueChanged.RemoveAllListeners();
        alphaSlider.onValueChanged.RemoveAllListeners();
    }
}
