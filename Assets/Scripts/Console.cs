using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

using Color = UnityEngine.Color;

public class Console : MonoBehaviour
{
    public int MaxLogs = 500;

    [Header("text settings")] 
    //public Color TextBg = Color.clear;
    public GUIStyle GuiStyle;

    private struct Log
    {
        public string msg;
        public float height, width;
        public LogType type;
    }

    private Dictionary<LogType, Color> logTypeColorMap = new();

    private List<Log> logHistory = new();

    void Awake()
    {
        Application.logMessageReceived += ApplicationOnlogMessageReceived;

        logTypeColorMap.Add(LogType.Log, Color.white);
        logTypeColorMap.Add(LogType.Error, Color.red);
        logTypeColorMap.Add(LogType.Warning, Color.yellow);
        logTypeColorMap.Add(LogType.Exception, Color.magenta);
        logTypeColorMap.Add(LogType.Assert, Color.blue);

        //var bgTex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        //bgTex.SetPixel(0, 0, TextBg);
        //bgTex.Apply();

        //GuiStyle.normal.background = bgTex;
        GuiStyle.wordWrap = false;

        logHistory.Add(new Log());
    }

    private void ApplicationOnlogMessageReceived(string condition, string stacktrace, LogType type)
    {
        var guiContent = new GUIContent(condition);

        var f = new Log
        {
            msg = $"{DateTime.Now:HH:mm:ss} | {condition}",
            height = GuiStyle.CalcHeight(guiContent, 40),
            width = GuiStyle.CalcSize(guiContent).x,
            type = type
        };
        logHistory.Add(f);
    }

    private Vector2 scrollPos;
    private Rect scrollbarPos = new Rect(10, 10, Screen.width, Screen.height / 3f);
    private Rect scrollbarView = new Rect(10, 10, Screen.width, Screen.height / 3f);

    void OnGUI()
    {
        //scrollPos = GUI.BeginScrollView(scrollbarPos, scrollPos, scrollbarView);

        for (int i = 0; i < logHistory.Count; i++)
        {
            if (i == 0) continue;
            var s = logHistory[i];

            //var style = guiStyle;
            //style.normal.textColor = logTypeColorMap[s.type];

            GUI.color = new Color(1f, 1f, 1f, 0.3f);
            var rectangle = new Rect(10, i * 20 + 10, 1000, 17);
            GUI.Box(rectangle, "");
            GUI.color = new Color(1f, 1f, 1f, 1f);
            GuiStyle.normal.textColor = logTypeColorMap[s.type];
            GUI.Label(rectangle, s.msg, GuiStyle);
        }
        if (logHistory.Count > MaxLogs)
            logHistory.RemoveAt(0);
        //GUI.EndScrollView();
    }
}
