using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoverEdit : MonoBehaviour
{
    public EditController Editor = null;
    public TMP_InputField CoverId = null;
    public TMP_InputField CoverPartId = null;
    private int CoverIdMax = -1;
    public List<int> CoverPartIdMax = new List<int>();
    private int chosen_coverId = -1;
    private int chosen_coverPartId = -1;
    public Button AddCover = null;
    public Button AddCoverPart = null;
    public Button DelCover = null;
    public Button DelCoverPart = null;
    public TMP_InputField CoverFront = null;
    public TMP_InputField CoverBack = null;
    public TMP_Text CoverIdRangeText;
    public TMP_Text CoverPartIdRangeText;
    public TMP_Dropdown CoverSpriteChoose;
    public ColorSet Cover_Color;

    public Toggle UsePolygonColliderToggle;
    public TMP_InputField CoverPartX;
    public TMP_InputField CoverPartY;
    public TMP_InputField CoverPartXscale;
    public TMP_InputField CoverPartYscale;
    public TMP_InputField CoverPartAngle;


    public List<AddChart.CoverData> covers = new List<AddChart.CoverData>();
    public List<List<AddChart.CoverPartData>> coverParts = new List<List<AddChart.CoverPartData>>();


    public void CoverLoadPart()
    {
        for(int i = 0; i < covers.Count; i++)
        {
            var _cov = covers[i];
            _cov.SpriteMaskNum = coverParts[i].Count;
            _cov.CoverParts = coverParts[i].ToArray();
            covers[i] = _cov;
        }
    }
    public void Init()
    {
        for (int i = 0; i < Editor.chart.covernum; i++)
        {
            AddChart.CoverData _cov = Editor.chart.covers[i];
            coverParts.Add(new List<AddChart.CoverPartData>());
            CoverPartIdMax.Add(_cov.SpriteMaskNum-1);
            covers.Add(_cov);
            for(int j = 0; j < _cov.SpriteMaskNum; j++)
            {
                coverParts[i].Add(_cov.CoverParts[j]);
                
            }
        }
        CoverIdMax = covers.Count - 1;
        _changerangeshow();
    }
    private void Awake()
    {
        CoverId.text = CoverPartId.text = "-1";
        CoverPartIdRangeText.text = CoverIdRangeText.text = "[-1,-1]";
        AddCover.onClick.AddListener(_addcover);
        CoverId.onEndEdit.AddListener(_changetargetcover);
        CoverPartAngle.text = CoverPartX.text = CoverPartY.text = "0";
        CoverPartXscale.text = CoverPartYscale.text = "1";
        AddCoverPart.onClick.AddListener(_addcoverpart);
        CoverPartId.onEndEdit.AddListener(_changetargetcoverpart);
        CoverBack.text = "0";
        CoverFront.text = "4";


        CoverPartX.onEndEdit.AddListener(_changeContentPartVal);
        CoverPartY.onEndEdit.AddListener(_changeContentPartVal);
        CoverPartXscale.onEndEdit.AddListener(_changeContentPartVal);
        CoverPartYscale.onEndEdit.AddListener(_changeContentPartVal);
        CoverPartAngle.onEndEdit.AddListener(_changeContentPartVal);
        CoverFront.onEndEdit.AddListener(_changeFront);
        CoverBack.onEndEdit.AddListener(_changeBack);
        CoverSpriteChoose.onValueChanged.AddListener(_changeChooseSprite);
        Cover_Color.OnColorChanged += _colorchange;

    }
    private void _colorchange(Color _col)
    {
        if(chosen_coverId != -1)
        {
            AddChart.CoverData _codt = covers[chosen_coverId];
            _codt.color = _col;
            covers[chosen_coverId] = _codt;
        }
    }
    private void _changeFront(string _val)
    {
        int _front = int.Parse(_val);
        int _back = int.Parse(CoverBack.text);
        if (_front < _back + 4) _front = _back + 4;
        CoverFront.text = _front.ToString();
        if (chosen_coverId != -1)
        {
            AddChart.CoverData _codt = covers[chosen_coverId];
            _codt.groupFront = _front;
            covers[chosen_coverId] = _codt;
        }
    }
    private void _changeBack(string _val)
    {
        int _back = int.Parse(_val);
        int _front = int.Parse(CoverFront.text);
        if (_back > _front-4) _back = _front - 4;
        CoverBack.text = _back.ToString();
        if (chosen_coverId != -1)
        {
            AddChart.CoverData _codt = covers[chosen_coverId];
            _codt.groupBack = _back;
            covers[chosen_coverId] = _codt;
        }
    }
    private void _changetargetcover(string _text)
    {
        Debug.Log(_text);
        if (string.IsNullOrEmpty(_text))
        {
            CoverId.text = "-1";
            chosen_coverId = -1;
            _changerangeshow();
            return;
        }
        int _id = int.Parse(_text);
        if (_id > CoverIdMax)
        {
            CoverId.text = CoverIdMax.ToString();
        }
        else if(_id < -1)
        {
            CoverId.text = "-1";
        }
        if(int.Parse(CoverId.text) != chosen_coverId)
        {
            CoverPartId.text = "-1";
            chosen_coverPartId = -1;
            chosen_coverId = _id;
            _changerangeshow();
        }
        if(chosen_coverId != -1)
        {
            CoverFront.text = covers[chosen_coverId].groupFront.ToString();
            CoverBack.text = covers[chosen_coverId].groupBack.ToString();
            Cover_Color.SetColor(covers[chosen_coverId].color);
        }
        
    }
    private void _changetargetcoverpart(string _text)
    {
        if (string.IsNullOrEmpty(_text) || chosen_coverId==-1)
        {
            CoverPartId.text = "-1";
            chosen_coverPartId = -1;
            return;
        }
         
        int _id = int.Parse(_text);
        if (_id > CoverPartIdMax[chosen_coverId])
        {
            CoverPartId.text = CoverPartIdMax[chosen_coverId].ToString();
        }
        else if (_id < -1)
        {
            CoverPartId.text = "-1";
        }
        chosen_coverPartId = int.Parse(CoverPartId.text);
        if(chosen_coverPartId != -1)
        {
            var _part = coverParts[chosen_coverId][chosen_coverPartId];
            CoverPartX.text = _part.x.ToString();
            CoverPartY.text = _part.y.ToString();
            CoverPartXscale.text = _part.xscale.ToString();
            CoverPartYscale.text = _part.yscale.ToString();
            CoverPartAngle.text = _part.angle.ToString();

        }
    }
    private void _changerangeshow()
    {
        CoverIdRangeText.text = "[-1," + CoverIdMax.ToString() + "]";
        if (chosen_coverId != -1)
        {
            CoverPartIdRangeText.text = "[-1," + CoverPartIdMax[chosen_coverId].ToString() + "]";
        }
        else CoverPartIdRangeText.text = "[-1,-1]";
    }
    private void _changeChooseSprite(int val)
    {
        if (chosen_coverPartId == -1) return;
        var _part = coverParts[chosen_coverId][chosen_coverPartId];
        _part.spriteName = CoverSpriteChoose.options[val].text;
        coverParts[chosen_coverId][chosen_coverPartId] = _part;
    }
    private void _changeContentPartVal(string _str)
    {
        if (chosen_coverPartId == -1) return;
        var _part = coverParts[chosen_coverId][chosen_coverPartId];
        _part.x = double.Parse(CoverPartX.text);
        _part.y = double.Parse(CoverPartY.text);
        _part.xscale = double.Parse(CoverPartXscale.text);
        _part.yscale = double.Parse(CoverPartYscale.text);
        _part.angle = double.Parse(CoverPartAngle.text);
        coverParts[chosen_coverId][chosen_coverPartId] = _part;
    }
    private void _addcover()
    {
        AddChart.CoverData _new_cov = new AddChart.CoverData();
        _new_cov.groupFront = int.Parse(CoverFront.text);
        _new_cov.groupBack = int.Parse(CoverBack.text);
        _new_cov.color = Cover_Color.GetColor();
        covers.Add(_new_cov);
        coverParts.Add(new List<AddChart.CoverPartData>());
        CoverPartIdMax.Add(-1);
        chosen_coverId = ++CoverIdMax;

        _changerangeshow();
        CoverId.text = chosen_coverId.ToString();
        CoverPartId.text = "-1";
    }
    private void _addcoverpart()
    {
        if (chosen_coverId == -1) return;
        AddChart.CoverPartData _newpart = new AddChart.CoverPartData();
        _newpart.spriteName = CoverSpriteChoose.options[CoverSpriteChoose.value].text;
        _newpart.x = double.Parse(CoverPartX.text);
        _newpart.y = double.Parse(CoverPartY.text);
        _newpart.angle = double.Parse(CoverPartAngle.text);
        _newpart.xscale = double.Parse(CoverPartXscale.text);
        _newpart.yscale = double.Parse(CoverPartYscale.text);
        coverParts[chosen_coverId].Add(_newpart);
        chosen_coverPartId = coverParts[chosen_coverId].Count - 1;
        CoverPartId.text = chosen_coverPartId.ToString();
        CoverPartIdMax[chosen_coverId] = chosen_coverPartId;
        _changerangeshow();
    }
    private void OnDestroy()
    {
        AddCover.onClick.RemoveAllListeners();
        AddCoverPart.onClick.RemoveAllListeners();
        CoverId.onEndEdit.RemoveAllListeners();
        CoverPartId.onEndEdit.RemoveAllListeners();
        Cover_Color.OnColorChanged -= _colorchange;
    }
}
