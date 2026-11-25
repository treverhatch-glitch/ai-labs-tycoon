using System.Collections.Generic;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using UnityEngine;

namespace AiLabs.Unity.Hq
{
    /// <summary>Mascot row above the lot; moods from LabQueries.Mood.</summary>
    public sealed class ModelMascotRow : MonoBehaviour
    {
        Simulation _simulation;
        readonly Dictionary<int, ModelMascotView> _views = new Dictionary<int, ModelMascotView>();

        public void Bind(Simulation simulation)
        {
            _simulation = simulation;
            Rebuild();
        }

        public void Rebuild()
        {
            Clear();
            if (_simulation == null) return;
            LabState lab = _simulation.Player;
            int n = 0;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState model = lab.Models[i];
                if (model.Status == ModelStatus.Retired) continue;
                ModelMood mood = LabQueries.Mood(_simulation.Config, lab, model, _simulation.Tick);
                var look = ModelMascotPresentation.ForMood(mood);

                var blob = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                blob.name = "Mascot_" + model.Id;
                blob.transform.SetParent(transform, false);
                var col = blob.GetComponent<Collider>();
                if (col != null) Destroy(col);

                float x = (n - lab.Models.Count * 0.5f) * 0.55f + 3.5f;
                var view = blob.AddComponent<ModelMascotView>();
                view.Bind(model.Id, look, new Vector3(x, 0.55f, -0.6f));
                _views[model.Id] = view;
                n++;
            }
        }

        void Clear()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                Destroy(transform.GetChild(i).gameObject);
            _views.Clear();
        }
    }
}