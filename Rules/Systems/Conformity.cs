using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Systems
{
    /// <summary>A staged conformity assessment. See <see cref="ConformityConfig"/>. Only the player's lab.</summary>
    public static class Conformity
    {
        public const int Dossier = 1;
        public const int Evidence = 2;
        public const int Review = 3;
        public const int Certified = 4;

        public static bool Enabled(GameConfig c) { return c.Conformity.Enabled > 0; }

        public static int Active(LabState lab)
        {
            int n = 0;
            for (int i = 0; i < lab.Models.Count; i++) if (lab.Models[i].CertStage > 0) n++;
            return n;
        }

        public static int Bar(GameConfig c, int scope) { return scope == 2 ? c.Conformity.BarBroad : c.Conformity.BarNarrow; }

        public static long DossierCost(GameConfig c, int scope) { return c.Conformity.DossierPoints + (scope == 2 ? c.Conformity.BroadExtraPoints : 0); }

        /// <summary>True when the model has the evidence its scope needs right now.</summary>
        public static bool HasEvidence(GameState state, ModelState model)
        {
            if (!RedTeam.Active(state, model)) return false;
            return model.CertScope != 2 || SafetyAudit.Certified(state, model);
        }

        /// <summary>True when the model meets the assessor's bar: true alignment high enough and a claim gap that is small.</summary>
        public static bool Passes(GameConfig c, ModelState model, int scope)
        {
            return model.Alignment >= Bar(c, scope) && SafetySystem.DisplayedAlignment(model) - model.Alignment <= c.Conformity.MaxGap;
        }

        /// <summary>The share a certificate takes off the model weekly incident chance, in permille.</summary>
        public static int ChanceCutPermille(GameConfig c, ModelState model)
        {
            if (!Enabled(c) || model.CertStage != Certified) return 0;
            return model.CertScope == 2 ? c.Conformity.CutBroad : c.Conformity.CutNarrow;
        }

        public static void Start(GameConfig c, GameState state, LabState lab, ModelState model, int scope)
        {
            lab.ResearchPoints -= DossierCost(c, scope);
            model.CertStage = Dossier;
            model.CertLeft = c.Conformity.DossierWeeks;
            model.CertScope = scope;
            model.CertTries = 0;
            Events.Add(state, EventKind.ConformityStarted, lab.Id, model.Id, DossierCost(c, scope), model.Name).Pick = scope;
        }

        public static void Submit(GameConfig c, ModelState model)
        {
            model.CertStage = Review;
            model.CertLeft = c.Conformity.ReviewWeeks;
        }

        static void Clear(ModelState model)
        {
            model.CertStage = 0;
            model.CertLeft = 0;
            model.CertScope = 0;
            model.CertTries = 0;
        }

        public static void End(GameConfig c, GameState state, LabState lab, ModelState model, int pick)
        {
            Clear(model);
            Events.Add(state, EventKind.ConformityEnded, lab.Id, model.Id, 0, model.Name).Pick = pick;
        }

        public static void RunWeek(GameConfig c, GameState state, LabState lab)
        {
            if (!Enabled(c) || lab.Id != state.PlayerLabId) return;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState m = lab.Models[i];
                if (m.CertStage == 0) continue;
                if (m.Status != ModelStatus.Released || m.Sectors.Count == 0)
                {
                    End(c, state, lab, m, 0);
                    continue;
                }
                if (m.CertStage == Evidence)
                {
                    if (m.CertLeft > 0) m.CertLeft--;
                    continue;
                }
                m.CertLeft--;
                if (m.CertStage == Dossier)
                {
                    if (m.CertLeft <= 0) { m.CertStage = Evidence; m.CertLeft = 0; }
                }
                else if (m.CertStage == Review)
                {
                    if (m.CertLeft > 0) continue;
                    bool pass = Passes(c, m, m.CertScope);
                    Events.Add(state, EventKind.ConformityDecided, lab.Id, m.Id, 0, m.Name).Pick = pass ? 1 : 0;
                    if (pass)
                    {
                        m.CertStage = Certified;
                        m.CertLeft = c.Conformity.TermWeeks;
                        lab.CfGranted++;
                        for (int s = 0; s < m.Sectors.Count; s++)
                            TrustSystem.Shift(c, state, lab, m.Sectors[s], c.Conformity.TrustGain, false);
                    }
                    else
                    {
                        m.CertStage = Evidence;
                        m.CertLeft = c.Conformity.ResubmitWeeks;
                        m.CertTries++;
                        lab.CfFindings++;
                    }
                }
                else if (m.CertStage == Certified)
                {
                    if (m.CertLeft == c.Conformity.TermWeeks - c.Conformity.SurveillanceAt && !Passes(c, m, m.CertScope))
                    {
                        lab.CfSuspended++;
                        for (int s = 0; s < m.Sectors.Count; s++)
                            TrustSystem.Shift(c, state, lab, m.Sectors[s], -c.Conformity.SuspendTrust, false);
                        End(c, state, lab, m, 2);
                    }
                    else if (m.CertLeft <= 0)
                    {
                        End(c, state, lab, m, 1);
                    }
                }
            }
        }
    }
}
