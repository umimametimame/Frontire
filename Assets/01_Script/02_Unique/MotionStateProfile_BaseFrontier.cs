using AYellowpaper.SerializedCollections;
using System;
using UnityEngine;


public class MotionStateProfile_BaseFrontier<T> : MotionStateProfile<T> where T : Enum
{
    public SerializedDictionary<MotionParameter,float> parameterDic = new SerializedDictionary<MotionParameter,float>();


    public enum MotionParameter
    {
        Power,
        Speed,
    }
}
