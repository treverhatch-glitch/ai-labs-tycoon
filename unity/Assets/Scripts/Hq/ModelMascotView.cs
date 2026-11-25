using AiLabs.Rules.State;
using UnityEngine;

namespace AiLabs.Unity.Hq
{
    /// <summary>One model mascot blob; mood and glow come from rules-derived presentation.</summary>
    public sealed class ModelMascotView : MonoBehaviour
    {
        public int ModelId { get; private set; }
        Renderer _renderer;
        Material _material;
        ModelMascotPresentation.MascotLook _look;
        float _phase;

        public void Bind(int modelId, ModelMascotPresentation.MascotLook look, Vector3 localPos)
        {
            ModelId = modelId;
            _look = look;
            transform.localPosition = localPos;
            transform.localScale = Vector3.one * look.Scale * 0.35f;
            _phase = modelId * 0.37f;
            EnsureMaterial(look.Body, look.Glow);
        }

        public void SetLook(ModelMascotPresentation.MascotLook look)
        {
            _look = look;
            transform.localScale = Vector3.one * look.Scale * 0.35f;
            EnsureMaterial(look.Body, look.Glow);
        }

        void EnsureMaterial(Color body, Color glow)
        {
            if (_renderer == null) _renderer = GetComponent<Renderer>();
            if (_material == null)
            {
                _material = new Material(HqShaders.Lit());
                _material.EnableKeyword("_EMISSION");
                _renderer.material = _material;
            }
            _material.color = body;
            _material.SetColor("_EmissionColor", glow);
        }

        void Update()
        {
            float y = Mathf.Sin((Time.time + _phase) * _look.BobSpeed) * 0.08f;
            var p = transform.localPosition;
            p.y = 0.55f + y;
            transform.localPosition = p;
        }

        void OnDestroy()
        {
            if (_material != null) Destroy(_material);
        }
    }
}