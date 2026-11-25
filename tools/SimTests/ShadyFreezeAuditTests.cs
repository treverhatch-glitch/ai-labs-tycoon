using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.SimTests.Support;
using Xunit;

namespace AiLabs.SimTests;

public class ShadyFreezeAuditTests
{
    [Fact]
    public void Start_shady_respects_home_freeze()
    {
        Game g = Game.Solo(74, c => c.ShadyFreezeAudit.Enabled = 1);
        g.Me.Bloc(g.Me.HomeBlocId).Level = BlocStanding.Freeze;
        var model = new ModelState { Id = g.State.TakeId(), Capability = 900, Status = ModelStatus.Trained };
        g.Me.Models.Add(model);
        g.Refused(Command.StartShadyOp(g.Id, "intrusion-for-hire", model.Id, -1, -1, ""), CommandError.PermitFrozen);
    }
}