using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WindowElementBase
{
    protected VisualElement root = null;
    protected WindowElementBase(VisualElement root)
    {
        this.root = root;
    }
}
