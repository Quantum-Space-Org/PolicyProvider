using Quantum.Command.Pipeline;

namespace Quantum.PolicyProvider
{
    public class SkipAuthorizationStage : IAmAPipelineStage
    {
        public override async Task Process<TCommand>(TCommand command, StageContext context)
        {
            await CheckAccessRights(command);

            await GoToSuccessorStage(command, context);
        }

        private Task CheckAccessRights<TCommand>(TCommand command)
            where TCommand : IAmACommand
        {
            return Task.CompletedTask;
        }
    }
}