using System;
using UnityEngine;

[Serializable]
public struct EventData
{
    public string Object;
    public int id;
    public string variable;

    public double time;//这两个变量控制开始时间和持续时间
    public double beat;
    public double endbeat;
    public double during;
    public double start;//这两个变量控制初始值和结束值
    public double end;
    public double first;//这两个变量控制曲线范围（连续区间）
    public double last;
    public string Tween;
    public string Ease;
}
