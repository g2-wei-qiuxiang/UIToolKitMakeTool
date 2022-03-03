using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;

public class ObjectListElement : WindowElementBase
{
    private VisualElement element = null;
    private ObjectField folder = null;
    private Button selectButton = null;
    private Button setButton = null;
    private GameObject prefab = null;
    
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="root"></param>
    /// <param name="element"></param>
    public ObjectListElement(VisualElement root, VisualElement element) : base(root)
    {
        this.element = element;
        folder = element.Q<ObjectField>("folder");
        selectButton = element.Q<Button>("select_button");
        setButton = element.Q<Button>("set_button");

        folder.objectType = typeof(DefaultAsset);
        selectButton.clickable.clicked += () =>
        {
            if (folder.value == null)
            {
                EditorUtility.DisplayDialog("フォルダを指定してください", "フォルダをアサインしてください", "ok");
                return;
            }
            
            SelectObjectWindow.Open(AssetDatabase.GetAssetPath(folder.value), "*.prefab", prefabAssets =>
            {
                prefab = prefabAssets as GameObject;
                if (prefab != null)
                {
                    selectButton.text = prefab.name;
                }
            });
        };
        setButton.clickable.clicked += () =>
        {
            if (prefab == null)
            {
                EditorUtility.DisplayDialog("ファイルを指定してください", "ファイル(prefab)をアサインしてください", "ok");
                return;
            }

            PrefabUtility.InstantiatePrefab(prefab);
        };
    }
}
