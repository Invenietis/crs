#if DNX451 || NET46
using System.Transactions;

namespace CK.Infrastructure.Commands
{
    public sealed class TransactionAttribute : HandlerAttributeBase
    {
        public IsolationLevel IsolationLevel { get; set; }

        public TransactionAttribute( IsolationLevel level = IsolationLevel.ReadCommitted )
        {
            IsolationLevel = level;
            Order = -100;
        }

        internal const string Key =  "CK:Infrastructure:Commands:TransactionAttribute";

        public override void OnCommandExecuting( CommandExecutionContext ctx )
        {
            TransactionScope scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel },
                TransactionScopeAsyncFlowOption.Enabled );

            var item = new Item { Scope = scope, ShouldRollback = false };
            ctx.Items.Add( Key, item );
        }

        public override void OnException( CommandExecutionContext ctx )
        {
            Item item = Item.GetFrom( ctx );
            item.ShouldRollback = true;
        }

        public override void OnCommandExecuted( CommandExecutionContext ctx )
        {
            Item item = Item.GetFrom( ctx );
            if( item != null )
            {
                if( item.ShouldRollback == false )
                {
                    item.Scope.Complete();
                }
                item.Scope.Dispose();
                item.Scope = null;
            }
        }

        internal class Item
        {
            public bool ShouldRollback { get; set; }

            public TransactionScope Scope { get; set; }

            internal static Item GetFrom( CommandExecutionContext ctx )
            {
                Item i = ctx.Items[Key] as Item;
                return i;
            }
        }
    }
}
#endif