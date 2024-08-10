using BNG;
using System.Collections;
using UnityEngine;

namespace Core.ItemColorization
{
    /// <summary>
    /// Дополнение для <see cref="BNG.Grabber"/>, меняющее цвет предмету при его подборе.
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]
    public sealed class GrabbableColorizable : GrabbableEvents
    {
        [SerializeField] private Color grabColor;
        [SerializeField] private float colorizationSpeed = 2f;

        private Coroutine _colorizeCoroutine;
        private MeshRenderer _renderer;

        private float _animationTime = 1f;
        private Color _normalColor;

        private void Start()
        {
            _renderer = GetComponent<MeshRenderer>();
            _normalColor = _renderer.material.color;
        }

        public override void OnGrab(Grabber grabber)
        {
            ColorizeTo(grabColor);
        }

        public override void OnRelease()
        {
            ColorizeTo(_normalColor);
        }

        /// <summary>
        /// Покрасить предмет в указанный цвет.
        /// </summary>
        /// <param name="color">Цвет для покраски.</param>
        private void ColorizeTo(Color color)
        {
            if (_colorizeCoroutine != null)
            {
                StopCoroutine(_colorizeCoroutine);
            }

            _colorizeCoroutine = StartCoroutine(ColorizeToRoutine(color));
        }

        private IEnumerator ColorizeToRoutine(Color color)
        {
            Material rendererMaterial = _renderer.material;
            Color startingColor = color == _normalColor ? grabColor : _normalColor;
            
            // Вычисление нужно в случае, если предыдущая анимация была прервана
            _animationTime = 1f - _animationTime;
            while (_animationTime <= 1f)
            {
                rendererMaterial.color = Color.Lerp(
                    startingColor, color, _animationTime);

                _animationTime += Time.deltaTime * colorizationSpeed;
                yield return null;
            }

            _animationTime = 1f;
            _colorizeCoroutine = null;
        }
    }
}