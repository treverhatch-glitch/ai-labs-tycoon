using UnityEngine;

namespace AiLabs.Unity.Hq
{
    /// <summary>One room cube on the lot grid. Emission is driven later when model glow propagates through the HQ.</summary>
    public sealed class RoomBlockView : MonoBehaviour
    {
        static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

        Renderer _renderer;
        Material _material;
        Color _baseColor;
        int _glowPermille;

        public int RoomId { get; private set; }

        public void Bind(int roomId, Color baseColor)
        {
            RoomId = roomId;
            _baseColor = baseColor;
            _glowPermille = 0;
            EnsureMaterial();
            ApplyColor();
        }

        public void SetGlowPermille(int glowPermille)
        {
            _glowPermille = glowPermille;
            ApplyColor();
        }

        void EnsureMaterial()
        {
            if (_renderer == null) _renderer = GetComponent<Renderer>();
            if (_material == null)
            {
                _material = new Material(HqShaders.Lit());
                _material.EnableKeyword("_EMISSION");
                _renderer.material = _material;
            }
        }

        void ApplyColor()
        {
            if (_material == null) return;
            _material.color = _baseColor;
            _material.SetColor(EmissionColorId, HqVisualPalette.GlowEmission(_baseColor, _glowPermille));
        }

        void OnDestroy()
        {
            if (_material != null) Destroy(_material);
        }
    }
}