using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;

namespace AiLabs.Rules.Sim
{
    public static partial class CommandProcessor
    {
        static CommandResult KnowledgeGate(GameConfig c, GameState state, LabState lab, Command cmd, out ModelState model)
        {
            model = null;
            if (lab.Id != state.PlayerLabId) return Fail(CommandError.InvalidArgument, "Only the player's lab keeps owners and documentation.");
            if (!Knowledge.Enabled(c)) return Fail(CommandError.InvalidArgument, "Nobody keeps owners of models in this game.");
            model = lab.Model(cmd.TargetId);
            if (model == null) return Fail(CommandError.UnknownId, "The lab has no model " + cmd.TargetId + ".");
            if (model.Status == ModelStatus.Retired) return Fail(CommandError.WrongModelStatus, model.Name + " is retired.");
            return CommandResult.Done(-1);
        }

        static CommandResult AssignOwner(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            ModelState model;
            CommandResult gate = KnowledgeGate(c, state, lab, cmd, out model);
            if (!gate.Ok) return gate;
            if (cmd.Count == 0)
            {
                if (model.OwnerId == 0) return Fail(CommandError.InvalidArgument, model.Name + " has no owner.");
                model.OwnerId = 0;
                return CommandResult.Done(-1);
            }
            StaffMember person = Knowledge.Person(lab, cmd.Count);
            if (person == null) return Fail(CommandError.UnknownId, "Nobody on the payroll has the id " + cmd.Count + ".");
            RoleFunction f = c.Role(person.RoleId).Function;
            if (f != RoleFunction.Research && f != RoleFunction.Engineering && f != RoleFunction.Safety) return Fail(CommandError.InvalidArgument, "Only a researcher, engineer or safety person can own a model.");
            if (model.OwnerId == person.Id) return Fail(CommandError.InvalidArgument, "That person already owns " + model.Name + ".");
            if (Knowledge.Owned(lab, person.Id) >= c.Knowledge.MaxOwned) return Fail(CommandError.InvalidArgument, "That person already owns " + c.Knowledge.MaxOwned + " models.");
            Knowledge.Own(c, state, lab, model, person.Id);
            return CommandResult.Done(-1);
        }

        static CommandResult WriteDocs(GameConfig c, GameState state, LabState lab, Command cmd)
        {
            ModelState model;
            CommandResult gate = KnowledgeGate(c, state, lab, cmd, out model);
            if (!gate.Ok) return gate;
            if (model.OwnerId == 0) return Fail(CommandError.MissingPrerequisite, model.Name + " needs an owner to write its documentation.");
            if (model.DocLevel >= c.Knowledge.MaxDocs) return Fail(CommandError.InvalidArgument, model.Name + " already has all the documentation there is.");
            if (lab.Cash < c.Knowledge.DocCost) return NeedCash(lab, c.Knowledge.DocCost);
            Knowledge.WriteDocs(c, state, lab, model);
            return CommandResult.Done(-1);
        }
    }
}
