using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class ObjectSelectToolWindow : EditorWindow
{
    private SceneAsset sceneAsset = null;
    enum UIMode
    {
        CreateMode,
        Other1,
        Other2
    };

    private Dictionary<ObjectListElement, VisualElement> objectDictionary = new Dictionary<ObjectListElement, VisualElement>();
    private static readonly string CREATE_MODE_UI = "create_mode_ui";
    private VisualTreeAsset selectScrollElement = null;
    private ScrollView objectListView = null;
    
    /// <summary>
    /// Windowを開く
    /// </summary>
    [MenuItem("CustomTool/ObjectSelectTool")]
    public static void CreateWindow()
    {
        EditorWindow.GetWindow<ObjectSelectToolWindow>();
    }

    /// <summary>
    /// OnEnable
    /// </summary>
    private void OnEnable()
    {
        var asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI/ObjectSelectTool.uxml");
        asset.CloneTree(rootVisualElement);

        selectScrollElement = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI/ObjectListElement.uxml");
        objectListView = rootVisualElement.Q<ScrollView>("object_select_scroll_view");
        
        UpdateUI();
        InitializedCallback();
    }

    /// <summary>
    /// CallBack初期化
    /// </summary>
    private void InitializedCallback()
    {
        // シーン参照
        var sceneNameField = rootVisualElement.Q<ObjectField>("scene_reference");
        sceneNameField.objectType = typeof(SceneAsset);
        sceneNameField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(e =>
        {
            sceneAsset = sceneNameField.value as SceneAsset;
            UpdateUI();
            if (sceneAsset != null)
            {
                EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(sceneAsset));
            }
        });
        
        // モードボタン
        rootVisualElement.Q<Button>("create_mode").clickable.clicked += () => ChangeUIMode(UIMode.CreateMode);
        rootVisualElement.Q<Button>("other_mode1").clickable.clicked += () => ChangeUIMode(UIMode.Other1);
        rootVisualElement.Q<Button>("other_mode2").clickable.clicked += () => ChangeUIMode(UIMode.Other2);

        // リストの+ボタン、-ボタン
        rootVisualElement.Q<Button>("plus_button").clickable.clicked += AddElement;
        rootVisualElement.Q<Button>("minus_button").clickable.clicked += DeleteElement;
    }

    /// <summary>
    /// モード変更
    /// </summary>
    private void ChangeUIMode(UIMode mode)
    {
        switch (mode)
        {
            case UIMode.Other1:
                rootVisualElement.Q<IMGUIContainer>(CREATE_MODE_UI).visible = false;
                break;
            case UIMode.Other2:
                rootVisualElement.Q<IMGUIContainer>(CREATE_MODE_UI).visible = false;
                break;
            case UIMode.CreateMode:
            default:
                rootVisualElement.Q<IMGUIContainer>(CREATE_MODE_UI).visible = true;
                break;
        }
    }

    /// <summary>
    /// 要素を追加
    /// </summary>
    private void AddElement()
    {
        var element = new VisualElement();
        selectScrollElement.CloneTree(element);
        objectListView.Add(element);

        var elementData = new ObjectListElement(rootVisualElement, element);
        objectDictionary.Add(elementData, element);
    }

    /// <summary>
    /// リストの末尾から要素を削除
    /// </summary>
    private void DeleteElement()
    {
        objectListView.RemoveAt(objectListView.childCount - 1);
        objectDictionary.Remove(objectDictionary.Last().Key);
    }
    
    /// <summary>
    /// modeのUIを非表示
    /// </summary>
    private void UIModeOff()
    {
        rootVisualElement.Q<IMGUIContainer>(CREATE_MODE_UI).visible = false;
    }

    /// <summary>
    /// 生成モードのUIを更新
    /// </summary>
    private void UpdateUI()
    {
        var ui = rootVisualElement.Q<IMGUIContainer>("ui_container");
        var warning = rootVisualElement.Q<IMGUIContainer>("warning_container");
        bool isSceneAssign = sceneAsset != null;
        ui.visible = isSceneAssign;
        warning.visible = !isSceneAssign;

        if (isSceneAssign)
        {
            ChangeUIMode(UIMode.CreateMode);
        }
        else
        {
            UIModeOff();
        }
    }
}
