using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
//using OrbItUtils;

[ExecuteInEditMode]
public class MetaData : MonoBehaviour
{
    private static MetaData _instance;
    public static MetaData instance { get { if (_instance == null) _instance = (MetaData)FindObjectOfType(typeof(MetaData)); return _instance; } }

    //public string author;
    public string levelName;

    void OnValidate()
    {
        if (levelName == "")
            Debug.LogError("levelname is null");
    }

    void Awake()
    {
        _instance = this;
    }

}
