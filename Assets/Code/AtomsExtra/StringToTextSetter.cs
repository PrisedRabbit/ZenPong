using TMPro;
using UnityAtoms;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[EditorIcon("atom-icon-delicate")]
public sealed class StringToTextSetter : MonoBehaviour, IInitializable
{
    [Tooltip("Deactivate this GameObject when text is empty")] public bool hideEmpty;
    [SerializeField][Tooltip("Optional")] private TextMeshProUGUI textMeshPro;
    [SerializeField][Tooltip("Optional")] private Text uGuiText;
    [SerializeField] private StringVariable stringVariable;

    private bool wasActive;

    [Inject]
    public void Initialize()
    {
        stringVariable.Changed.OnEvent += OnChangeValue;
        // below code nneds for OnDestroy
        wasActive = gameObject.activeSelf;
        gameObject.SetActive(true);
        gameObject.SetActive(wasActive);
    }

    private void OnChangeValue(string value)
    {
        if (textMeshPro)
            textMeshPro.text = value;
        if (uGuiText)
            uGuiText.text = value;
        if (hideEmpty)
            gameObject.SetActive(!string.IsNullOrWhiteSpace(value));
    }

    private void OnDestroy()
    {
        stringVariable.Changed.OnEvent -= OnChangeValue;
        stringVariable.Value = string.Empty;
    }
}