using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu
{
    [RequireComponent(typeof(MultiImageEventsButton))]
    public class ButtonTransitionSpriteSwap : MonoBehaviour
    {
        [SerializeField] private List<GameObjectSwap> gameObjectSwaps;
        [SerializeField] private List<FontSwap> fontSwaps;
        [SerializeField] private List<SpriteSwap> spriteSwaps;

        private MultiImageEventsButton _button;

        private void Awake()
        {
            _button = GetComponent<MultiImageEventsButton>();
        }

        private void OnStateTransition(MultiImageEventsButton.SelectionPublicState s)
        {
            if (!enabled)
                return;

            if (gameObjectSwaps?.Count > 0)
            {
                foreach (var spriteSwap in gameObjectSwaps)
                    spriteSwap.SetForButtonState(s);
            }

            if (fontSwaps?.Count > 0)
            {
                foreach (var fontSwap in fontSwaps)
                    fontSwap.SetForButtonState(s);
            }

            if (spriteSwaps?.Count > 0)
            {
                foreach (var spriteSwap in spriteSwaps)
                    spriteSwap.SetForButtonState(s);
            }
        }

        private void OnEnable()
        {
            _button.StateChanged += OnStateTransition;
            OnStateTransition(_button.CurrentState);
        }

        private void OnDisable()
        {
            _button.StateChanged -= OnStateTransition;
        }

        #region Helper structs

        [Serializable]
        public struct GameObjectSwap : IResourceForButtonState<bool>
        {
            public GameObject gameObject;
            // font materials
            public bool normal;
            public bool highlighted;
            public bool selected;
            public bool pressed;
            public bool disabled;

            #region IResourceForButtonState
            public bool Normal => normal;
            public bool Highlighted => highlighted;
            public bool Selected => selected;
            public bool Pressed => pressed;
            public bool Disabled => disabled;

            public void SetForButtonState(MultiImageEventsButton.SelectionPublicState state)
            {
                if (!gameObject)
                    return;

                var res = IResourceForButtonState<bool>.GetForButtonState(this, state);
                gameObject.SetActive(res);
            }
            #endregion
        }

        [Serializable]
        public struct FontSwap : IResourceForButtonState<Material>
        {
            public TextMeshProUGUI text;
            // font materials
            public Material normal;
            public Material highlighted;
            public Material selected;
            public Material pressed;
            public Material disabled;

            #region IResourceForButtonState
            public readonly Material Normal => normal;
            public readonly Material Highlighted => highlighted ? highlighted : normal;
            public readonly Material Selected => selected ? selected : normal;
            public readonly Material Pressed => pressed ? pressed : normal;
            public readonly Material Disabled => disabled ? disabled : normal;

            public void SetForButtonState(MultiImageEventsButton.SelectionPublicState state)
            {
                if (!text)
                    return;

                var mat = IResourceForButtonState<Material>.GetForButtonState(this, state);
                if (!mat)
                    return;

                text.fontMaterial = mat;
            }
            #endregion
        }

        [Serializable]
        public struct SpriteSwap : IResourceForButtonState<Sprite>
        {
            public Image img;
            public Sprite normal;
            public Sprite highlighted;
            public Sprite selected;
            public Sprite pressed;
            public Sprite disabled;

            #region IResourceForButtonState
            public readonly Sprite Normal => normal;
            public readonly Sprite Highlighted => highlighted ? highlighted : normal;
            public readonly Sprite Selected => selected ? selected : normal;
            public readonly Sprite Pressed => pressed ? pressed : normal;
            public readonly Sprite Disabled => disabled ? disabled : normal;

            public void SetForButtonState(MultiImageEventsButton.SelectionPublicState state)
            {
                if (!img)
                    return;

                var sprite = IResourceForButtonState<Sprite>.GetForButtonState(this, state);
                if (!sprite)
                    return;

                img.sprite = sprite;
            }
            #endregion
        }

        private interface IResourceForButtonState<T>
        {
            T Normal { get; }
            T Highlighted { get; }
            T Selected { get; }
            T Pressed { get; }
            T Disabled { get; }

            public void SetForButtonState(MultiImageEventsButton.SelectionPublicState state);

            // function to help implementations
            public static T GetForButtonState(IResourceForButtonState<T> resource, MultiImageEventsButton.SelectionPublicState state) => state switch
            {
                MultiImageEventsButton.SelectionPublicState.Normal => resource.Normal,
                MultiImageEventsButton.SelectionPublicState.Highlighted => resource.Highlighted,
                MultiImageEventsButton.SelectionPublicState.Pressed => resource.Pressed,
                MultiImageEventsButton.SelectionPublicState.Selected => resource.Selected,
                MultiImageEventsButton.SelectionPublicState.Disabled => resource.Disabled,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null),
            };
        }
        #endregion
    }
}