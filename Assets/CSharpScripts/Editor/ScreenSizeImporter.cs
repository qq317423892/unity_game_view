using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;


public class GameViewUtils
{
    static object gameViewSizesInstance;
    static MethodInfo getGroup;
    static PropertyInfo groupType;
    private static int screenIndex = 16; // Because have 16 indexes in my list.
    private static int gameViewProfilesCount;
    private static List<ViewInfo> totalList = new List<ViewInfo>();
    struct ViewInfo
    {
        public string name;
        public int width;
        public int height;
        public ViewInfo(string name, int width, int height) 
        {
            this.name = name;
            this.width = width;
            this.height = height;
        }
    }

    static GameViewUtils()
    {
        var sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
        var singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        var instanceProp = singleType.GetProperty("instance");
        getGroup = sizesType.GetMethod("GetGroup");
        gameViewSizesInstance = instanceProp.GetValue(null, null);

        groupType = sizesType.GetProperty("currentGroupType");

        InitScreenSize();
    }

    static void InitScreenSize()
    {
        totalList.Add(new ViewInfo("standard", 1624, 750));
        totalList.Add(new ViewInfo("iPhoneXr", 1792, 828));
        totalList.Add(new ViewInfo("iPhone6",      1136, 640));
        totalList.Add(new ViewInfo("iPhone6",      1334, 750));
        totalList.Add(new ViewInfo("iPhone6 plus", 2208, 1242));
        totalList.Add(new ViewInfo("iPhoneX[s]",   2436, 1125));
        totalList.Add(new ViewInfo("iPhoneXs Max", 2688, 1242));
        totalList.Add(new ViewInfo("iPad",         1024, 768));
        totalList.Add(new ViewInfo("iPad 2/Mini", 1024, 768));
        totalList.Add(new ViewInfo("iPad 2048x1536",  2048, 1536));
        totalList.Add(new ViewInfo("iPad pro", 2732, 2048));
        totalList.Add(new ViewInfo("Android 1280 * 720", 1280, 720));
        totalList.Add(new ViewInfo("Android 1920 * 1080", 1920, 1080));
        totalList.Add(new ViewInfo("Android 2160 * 1080", 2160, 1080));
        totalList.Add(new ViewInfo("Android 2960 * 1440", 2160, 1080));
    }

    private enum GameViewSizeType
    {
        AspectRatio, FixedResolution
    }

    private static void SetSize(int index)
    {
        var gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        var gvWnd = EditorWindow.GetWindow(gvWndType);
        var SizeSelectionCallback = gvWndType.GetMethod("SizeSelectionCallback",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        SizeSelectionCallback.Invoke(gvWnd, new object[] { index, null });
    }

    static object GetGroup(GameViewSizeGroupType type)
    {
        return getGroup.Invoke(gameViewSizesInstance, new object[] { (int)type });
    }

    static object GetCurrentGroupType()
    {
        return groupType.GetValue(gameViewSizesInstance, null);
    }

    [MenuItem("Tools/游戏窗口工具/一键导入横版")]
    private static void AddCustmerGameViewSize()
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        Type gameViewType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize");
        Type gameViewProvider = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizesMenuItemProvider");
        Type gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        Type fizeGroupType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizeGroup");
        EditorWindow gvWnd = EditorWindow.GetWindow(gvWndType);

        FieldInfo sizeGroupField = gameViewProvider.GetField("m_GameViewSizeGroup", flags);
        FieldInfo customField = fizeGroupType.GetField("m_Custom", flags);

        object[] viewParam = new object[1];
        viewParam[0] = GetCurrentGroupType();

        object obj = Activator.CreateInstance(gameViewProvider, viewParam);

        object sizeGroup = sizeGroupField.GetValue(obj);
        var custom = (IList)customField.GetValue(sizeGroup);
        custom.Clear();

        for (int i = 0; i < totalList.Count; ++i)
        {
            ViewInfo view = totalList[i];
            object[] parameters = new object[4];
            parameters[0] = 1;
            parameters[1] = view.width;
            parameters[2] = view.height;
            parameters[3] = view.name;

            object gameView = Activator.CreateInstance(gameViewType, parameters);

            object[] addParam = new object[1];
            addParam[0] = gameView;

            
            var addFunc = gameViewProvider.GetMethod("Add", flags);
            addFunc.Invoke(obj, addParam);
        }
    }

    [MenuItem("Tools/游戏窗口工具/一键导入竖版")]
    private static void AddCustmerVerGameViewSize()
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        Type gameViewType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize");
        Type gameViewProvider = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizesMenuItemProvider");
        Type gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        Type fizeGroupType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizeGroup");
        EditorWindow gvWnd = EditorWindow.GetWindow(gvWndType);

        FieldInfo sizeGroupField = gameViewProvider.GetField("m_GameViewSizeGroup", flags);
        FieldInfo customField = fizeGroupType.GetField("m_Custom", flags);

        object[] viewParam = new object[1];
        viewParam[0] = GetCurrentGroupType();

        object obj = Activator.CreateInstance(gameViewProvider, viewParam);

        object sizeGroup = sizeGroupField.GetValue(obj);
        var custom = (IList)customField.GetValue(sizeGroup);
        custom.Clear();

        for (int i = 0; i < totalList.Count; ++i)
        {
            ViewInfo view = totalList[i];
            object[] parameters = new object[4];
            parameters[0] = 1;
            parameters[1] = view.height;
            parameters[2] = view.width;
            parameters[3] = view.name;

            object gameView = Activator.CreateInstance(gameViewType, parameters);

            object[] addParam = new object[1];
            addParam[0] = gameView;


            var addFunc = gameViewProvider.GetMethod("Add", flags);
            addFunc.Invoke(obj, addParam);
        }
    }
}