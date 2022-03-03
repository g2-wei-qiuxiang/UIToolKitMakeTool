using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public class SelectObjectWindow : EditorWindow
{
    /// <summary>
    /// 開く
    /// </summary>
    /// <param name="path"></param>
    /// <param name="searchPattern"></param>
    /// <param name="onSetCallback"></param>
    public static void Open(string path, string searchPattern, Action<Object> onSetCallback)
    {
        string uxmlPath = "Assets/Editor/UI/SelectObjectWindow.uxml";
        var asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
        var window = EditorWindow.GetWindow<SelectObjectWindow>();
        asset.CloneTree(window.rootVisualElement);
        window.Initialize(path, searchPattern, onSetCallback);
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void Initialize(string path, string searchPattern, Action<Object> onSetCallback)
    {
        rootVisualElement.Q<Label>("path").text = "フォルダパス：" + path;
        rootVisualElement.Q<Label>("type").text = "ファイルタイプ：" + searchPattern;

        // フォルタ内の全オブジェクトを取得
        path = path.Replace("Assets/", "");
        var assets = FindAllAsset<Object>(Path.Combine(Application.dataPath, path), searchPattern);

        // ボタン群作成
        var listView = rootVisualElement.Q<ScrollView>();
        foreach (var asset in assets)
        {
            var button = new Button(() =>
            {
                onSetCallback?.Invoke(asset);
                Close();
            });
            button.text = asset.name;
            listView.Add(button);
        }
    }
    
    /// <summary>
    /// フォルダの全アセットを取得
    /// </summary>
    /// <param name="directoryPath"></param>
    /// <param name="searchPattern"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private IReadOnlyList<T> FindAllAsset<T>(string directoryPath, string searchPattern) where T : UnityEngine.Object
    {
        List<T> assets = new List<T>();
        var fileNames = Directory.GetFiles(directoryPath, searchPattern, SearchOption.TopDirectoryOnly);

        foreach (var fileName in fileNames)
        {
            var path = "Assets/" + fileName.Replace(Application.dataPath, "");
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
            {
                assets.Add(asset);
            }
        }

        return assets;
    }
}
