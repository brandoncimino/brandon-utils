using System;
using System.Collections.Generic;

using BrandonUtils.Logging;
using BrandonUtils.Standalone.Attributes;
using BrandonUtils.Standalone.Collections;
using BrandonUtils.Standalone.Strings;

using FowlFever.Conjugal;

using UnityEngine;

public class TestPropertyDrawer : MonoBehaviour {
    [SerializeField]
    public (int, string) Tuple2;
    public Plurable       Plurable;
    public List<Plurable> PlurableList;

    public Author       Author;
    public List<Author> AuthorList;

    public Pair<DayOfWeek, Color> DayColor;

    public List<Pair<DayOfWeek, Color>> DayColorList;

    //public EnumMap<DayOfWeek, Color> DayMap;

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }

    [EditorInvocationButton]
    public void SayHi() {
        LogUtils.Log("HI");
    }
}