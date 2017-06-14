using CK.Core;

namespace CK.Crs
{
    public interface ICommandDecorator
    {
        int Order { get; }

        void OnCommandExecuting( CommandContext ctx );

        void OnException( CommandContext ctx );

        void OnCommandExecuted( CommandContext ctx );
    }
}