using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.MainMenu
{
    public class MultiImageEventsButton : Button
    {
        public class StateTransitionEvent : UnityEvent<SelectionPublicState> { }

        private StateTransitionEvent _onStateTransition;
        public StateTransitionEvent OnStateTransition
        {
            get => _onStateTransition ??= new StateTransitionEvent();
            set => _onStateTransition = value;
        }

        public event Action<SelectionPublicState> StateChanged;
        public SelectionPublicState CurrentState { get; private set; }

        private Graphic[] m_graphics;
        protected Graphic[] Graphics
        {
            get
            {
                if (m_graphics == null)
                    m_graphics = targetGraphic.transform.GetComponentsInChildren<Graphic>();
                return m_graphics;
            }
        }

        protected List<Material> FontMaterials;
        protected Color[] FontBaseColors;
        protected Color[] FontUnderlayColors;
        protected Color[] FontOutlineColors;

        private bool _hasFrocedState = false;
        private SelectionState _forcedState;

        protected List<Material> SetupFontsMaterials()
        {
            if (FontMaterials != null && FontMaterials.Any(x => !x))
                FontMaterials = null;

            if (FontMaterials != null)
                return FontMaterials;

            FontMaterials = new List<Material>();
            var textMeshPros = new List<TextMeshProUGUI>();

            targetGraphic.GetComponentsInChildren(textMeshPros);
            var textMeshTarget = targetGraphic.GetComponent<TextMeshProUGUI>();
            if (textMeshTarget)
                textMeshPros.Add(textMeshTarget);
            if (!textMeshTarget && textMeshPros.Count > 0)
                textMeshTarget = textMeshPros[0];
            if (!textMeshTarget)
                return FontMaterials;

            FontBaseColors = new Color[textMeshPros.Count];
            FontOutlineColors = new Color[textMeshPros.Count];
            FontUnderlayColors = new Color[textMeshPros.Count];

            var fontsSet = new Dictionary<Material, Material>();
            for (var i = 0; i < textMeshPros.Count; i++)
            {
                var textMesh = textMeshPros[i];
                if (fontsSet.ContainsKey(textMesh.fontMaterial))
                {
                    textMesh.fontMaterial = fontsSet[textMesh.fontMaterial];
                    continue;
                }

                var matClone = new Material(textMesh.fontMaterial);
                FontOutlineColors[i] = matClone.GetColor(OUTLINE_SHADER_PARAM);
                FontUnderlayColors[i] = matClone.GetColor(UNDERLAY_SHADER_PARAM);

                fontsSet.Add(textMesh.fontMaterial, matClone);
                textMesh.fontMaterial = matClone;
            }

            FontMaterials.AddRange(fontsSet.Select(x => x.Value));
            return FontMaterials;
        }

        public void ForceState(SelectionPublicState state)
        {
            _hasFrocedState = true;
            _forcedState = Convert(state);
            DoStateTransition(_forcedState, false);
        }

        public void ClearFrocedState()
        {
            _hasFrocedState = false;
            DoStateTransition(currentSelectionState, false);
        }

        private SelectionState Convert(SelectionPublicState state) => state switch
        {
            SelectionPublicState.Normal => SelectionState.Normal,
            SelectionPublicState.Highlighted => SelectionState.Highlighted,
            SelectionPublicState.Pressed => SelectionState.Pressed,
            SelectionPublicState.Selected => SelectionState.Selected,
            SelectionPublicState.Disabled => SelectionState.Disabled,
            _ => throw new NotImplementedException(),
        };

        private SelectionPublicState Convert(SelectionState state) => state switch
        {
            SelectionState.Normal => SelectionPublicState.Normal,
            SelectionState.Highlighted => SelectionPublicState.Highlighted,
            SelectionState.Pressed => SelectionPublicState.Pressed,
            SelectionState.Selected => SelectionPublicState.Selected,
            SelectionState.Disabled => SelectionPublicState.Disabled,
            _ => throw new NotImplementedException(),
        };

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            if (!gameObject.activeInHierarchy)
                return;
            if (_hasFrocedState && state != _forcedState)
                return;

            Color color = state switch
            {
                SelectionState.Normal => colors.normalColor,
                SelectionState.Highlighted => colors.highlightedColor,
                SelectionState.Pressed => colors.pressedColor,
                SelectionState.Selected => colors.selectedColor,
                SelectionState.Disabled => colors.disabledColor,
                _ => Color.black
            };

            if (gameObject.activeInHierarchy)
            {
                switch (transition)
                {
                    case Transition.ColorTint:
                        ColorTween(color * colors.colorMultiplier, instant);
                        break;
                    default:
                        base.DoStateTransition(state, instant);
                        break;
                }
            }

            var publicState = Convert(state);

            // обновляем публичное состояние один раз
            if (CurrentState != publicState)
            {
                CurrentState = publicState;
                OnStateTransition?.Invoke(publicState);   // UnityEvent (в инспектор)
                StateChanged?.Invoke(publicState);        // C# event (в коде)
            }
        }

        private const string OUTLINE_SHADER_PARAM = "_OutlineColor";
        private const string UNDERLAY_SHADER_PARAM = "_UnderlayColor";
        private Tween colorTween;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetupTextMeshProTween(Color targetColor, bool inst)
        {
            var mat = SetupFontsMaterials();
            if (mat.Count == 0)
                return;

            colorTween?.Kill();
            if (!inst)
            {
                var seq = DOTween.Sequence();
                for (int i = 0; i < mat.Count; i++)
                    seq.Insert(0, MaterialTween(i, targetColor, inst));
                seq.SetLink(gameObject).Play();
                colorTween = seq;
            }
            else
            {
                SetInstantColorTime(targetColor);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Tween MaterialTween(int index, Color col, bool inst)
        {
            var i = index;
            var colOutFinal = FontOutlineColors[i] * col;
            var colUndFinal = FontUnderlayColors[i] * col;

            var colOutCurr = FontMaterials[i].GetColor(OUTLINE_SHADER_PARAM);
            var colOutUnder = FontMaterials[i].GetColor(UNDERLAY_SHADER_PARAM);

            var time = 0f;
            void TimeSetter(float t)
            {
                time = t;
                var colOut = Color.Lerp(colOutCurr, colOutFinal, time);
                var colUnd = Color.Lerp(colOutUnder, colUndFinal, time);
                FontMaterials[i].SetColor(OUTLINE_SHADER_PARAM, colOut);
                FontMaterials[i].SetColor(UNDERLAY_SHADER_PARAM, colUnd);
            }

            var tweener = DOTween.To(() => time, TimeSetter, 1, inst ? 0f : colors.fadeDuration);
            tweener.SetUpdate(true);
            return tweener;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetInstantColorTime(Color col)
        {
            for (int i = 0; i < FontMaterials.Count; i++)
            {
                var colOutFinal = FontOutlineColors[i] * col;
                var colUndFinal = FontUnderlayColors[i] * col;
                FontMaterials[i].SetColor(OUTLINE_SHADER_PARAM, colOutFinal);
                FontMaterials[i].SetColor(UNDERLAY_SHADER_PARAM, colUndFinal);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ColorTween(Color targetColor, bool instant)
        {
            if (targetGraphic == null)
                return;
            if (Application.isPlaying)
                SetupTextMeshProTween(targetColor, instant);

            TweenFade(targetGraphic, targetColor, instant);
            foreach (Graphic g in Graphics)
                TweenFade(g, targetColor, instant);
        }

        private void TweenFade(Graphic target, Color targetColor, bool instant)
        {
            target.CrossFadeColor(targetColor, (!instant) ? colors.fadeDuration : 0f, true, true);
        }

        // Желательно подчистить клоны материалов, чтобы не было утечек в редакторе
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (FontMaterials != null)
            {
#if UNITY_EDITOR
                foreach (var m in FontMaterials)
                    if (m)
                        UnityEngine.Object.DestroyImmediate(m);
#else
                foreach (var m in fontMaterials) if (m) UnityEngine.Object.Destroy(m);
#endif
            }
        }

        public enum SelectionPublicState
        {
            Normal,
            Highlighted,
            Pressed,
            Selected,
            Disabled,
        }
    }
}
