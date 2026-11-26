using AiLabs.Rules.Core;

namespace AiLabs.Rules.State
{
    public sealed class EndingRecord : ISync
    {
        public int Tick;
        public string EndingId = "";
        public int LabId;
        public string Choice = "";
        public int ChoiceTick;

        public void Sync(Syncer s)
        {
            s.Int("tick", ref Tick);
            s.Str("endingId", ref EndingId);
            s.Int("labId", ref LabId);
            s.Str("choice", ref Choice);
            s.Int("choiceTick", ref ChoiceTick);
        }
    }

    public sealed class ShadyOp : ISync
    {
        public int Id;
        public string CardId = "";
        public int ModelId;
        public int TargetLabId;
        public string TargetRef = "";
        public int AuxId;
        public int StartTick;
        public int ResolveTick;

        public void Sync(Syncer s)
        {
            s.Int("id", ref Id);
            s.Str("cardId", ref CardId);
            s.Int("modelId", ref ModelId);
            s.Int("targetLabId", ref TargetLabId);
            s.Str("targetRef", ref TargetRef);
            s.Int("auxId", ref AuxId);
            s.Int("startTick", ref StartTick);
            s.Int("resolveTick", ref ResolveTick);
        }
    }
}