using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityAtoms;
using UnityEngine;
using UnityEngine.UI;

[EditorIcon("atom-icon-delicate")]
public sealed class IntToTextSetter : MonoBehaviour
{
    #pragma warning disable 0649
    
    [SerializeField][Tooltip("Optional")] private TextMeshProUGUI textMeshPro;
    [SerializeField][Tooltip("Optional")] private Text uGuiText;
    [SerializeField] private IntVariable intVariable;

    private void Awake()
    {
        intVariable.Changed.OnEvent += OnChangeValue;
    }

    private void OnChangeValue(int value)
    {
        if (textMeshPro)
            textMeshPro.text = value.ToString();
        if (uGuiText)
            uGuiText.text = value.ToString();
    }

    private void OnDestroy()
    {
        intVariable.Changed.OnEvent -= OnChangeValue;
    }
}