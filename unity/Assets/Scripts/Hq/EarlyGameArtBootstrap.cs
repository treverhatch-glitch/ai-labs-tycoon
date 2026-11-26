using AiLabs.Unity.Sim;
using UnityEngine;

namespace AiLabs.Unity.Hq
{
    /// <summary>Wires early-game art catalogs into the live garage block-out.</summary>
    public static class EarlyGameArtBootstrap
    {
        public static EarlyGameGarageDecorator AttachGarage(Transform hqRoot, SimulationHost host, HqHudView hud)
        {
            var decorator = hqRoot.GetComponent<EarlyGameGarageDecorator>();
            if (decorator == null)
                decorator = hqRoot.AddComponent<EarlyGameGarageDecorator>();
            if (host?.Simulation != null)
                decorator.Bind(host.Simulation);

            Transform canvas = hud != null ? hud.transform.parent : null;
            if (canvas != null)
                EarlyGameHudArt.DecorateCanvas(canvas, hud);

            foreach (string texId in EarlyGameTextureCatalog.TextureIds)
                EarlyGameArtPaths.LogResolve(EarlyGameTextureCatalog.PathFor(texId));

            foreach (string wispId in EarlyGameWispCatalog.WispMoodIds)
                EarlyGameArtPaths.LogResolve(EarlyGameWispCatalog.PathFor(wispId));

            if (canvas != null)
                EarlyGameWispSprite.Attach(canvas, "wisp-neutral", new Vector2(-24f, 120f));

            return decorator;
        }
    }
}