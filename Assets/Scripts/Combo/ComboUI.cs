using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine.Localization.Settings;

public class ComboUI : MonoBehaviour
{

    [SerializeField] private List<ComboFacePanelInfo> comboFacePanelsInfo;
    [SerializeField] private List<ComboStyle> comboStyles;

    private readonly Queue<ComboFacePanelInfo> queueFaces = new();

    [SerializeField] private CanvasGroup combo;
    [SerializeField] private Image glowCombo;
    [SerializeField] private TextMeshProUGUI comboText;

    [SerializeField] private float faceAppearDuration = 0.3f;

    [SerializeField] private float moveUpDistance = 0.1f;
    [SerializeField] private float durationComboEffect = 1f;

    private int _currentFaceIndex;

    private ComboFacePanelInfo _facePanel;

    private Sequence _seqComboValue;

    private readonly Vector2 _targetSize = new(492f, 520f);

    private string _comboStr;

    public void Setup()
    {
        _comboStr = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "combo_x");

        foreach (var item in comboFacePanelsInfo)
        {
            queueFaces.Enqueue(item);
        }
    }

    public void SetupFace(int faceNumb, NumericalDiceInfo numericalDiceInfo)
    {
        _currentFaceIndex++;
        if (_currentFaceIndex >= comboFacePanelsInfo.Count-1)
        {
            _facePanel = queueFaces.Dequeue();
            _facePanel.panelTransform?.DOKill();
            _facePanel.panelTransform?.DOSizeDelta(Vector2.zero, faceAppearDuration);
            queueFaces.Enqueue(_facePanel);

            _facePanel = queueFaces.Dequeue();
            SetFaceStyle(_facePanel, numericalDiceInfo);
            _facePanel.panelTransform.SetAsLastSibling();
            _facePanel.faceNumb.SetText("{0}", faceNumb);
            _facePanel.panelTransform?.DOKill();
            _facePanel.panelTransform?.DOSizeDelta(_targetSize, faceAppearDuration);
            queueFaces.Enqueue(_facePanel);
        }
        else
        {
            _facePanel = queueFaces.Dequeue();
            SetFaceStyle(_facePanel, numericalDiceInfo);
            _facePanel.faceNumb.SetText("{0}", faceNumb);
            _facePanel.panelTransform?.DOKill();
            _facePanel.panelTransform?.DOSizeDelta(_targetSize, faceAppearDuration);
            queueFaces.Enqueue(_facePanel);
        }
    }

    private void SetFaceStyle(ComboFacePanelInfo facePanel, NumericalDiceInfo numericalDiceInfo)
    {
        facePanel.faceIcon.sprite = numericalDiceInfo.faceSprite;
        facePanel.faceNumb.fontMaterial = numericalDiceInfo.numbMat;
        facePanel.faceNumb.color = numericalDiceInfo.numbColor;
    }

    public void SetComboValue(int numberCombo)
    {
        ComboStyle comboStyle = comboStyles.Find(info => info.countCombo == numberCombo);
        comboStyle ??= comboStyles[^1];

        glowCombo.color = comboStyle.glowColor;
        comboText.color = comboStyle.comboTextColor;

        comboText.SetText(_comboStr, numberCombo);

        combo.transform.localPosition = Vector2.zero;
        combo.alpha = 0;

        _seqComboValue?.Kill();
        _seqComboValue = DOTween.Sequence();
        _seqComboValue.Append(combo?.DOFade(1f, durationComboEffect / 4))
            .Append(combo.transform?.DOMoveY(moveUpDistance, durationComboEffect * 1.5f).SetRelative().SetEase(Ease.OutQuad))
            .Append(combo?.DOFade(0f, durationComboEffect / 4));
    }

    private void OnLocaleChanged(UnityEngine.Localization.Locale _)
    {
        _comboStr = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "combo_x");
    }

    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        _seqComboValue?.Kill();

        foreach (var p in comboFacePanelsInfo)
        {
            p?.panelTransform?.DOKill();
        }
    }

    [Serializable]
    public class ComboFacePanelInfo
    {
        public RectTransform panelTransform;

        public Image faceIcon;

        public TextMeshProUGUI faceNumb;
    }

    [Serializable]
    public class ComboStyle
    {
        public int countCombo;

        public Color glowColor;
        public Color comboTextColor;
    }

}
