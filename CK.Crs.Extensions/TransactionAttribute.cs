using System;
using System.Transactions;

namespace CK.Crs
{
    public class TransactionAttribute : HandlerAttributeBase
    {
        public IsolationLevel IsolationLevel { get; set; }

        public TransactionAttribute()
        {
            IsolationLevel = IsolationLevel.ReadUncommitted;
            Order = -100;
        }

        internal const string Key =  "CK:Infrastructure:Commands:TransactionAttribute";

        public override void OnCommandExecuting( CommandContext ctx )
        {
            TransactionScope scope = BeginTransaction();
            var item = new Item { Scope = scope, ShouldRollback = false };
            ctx.Items.Add( Key, item );
        }

        public override void OnException( CommandContext ctx )
        {
            Item item = Item.GetFrom( ctx );
            item.ShouldRollback = true;
        }

        public override void OnCommandExecuted( CommandContext ctx )
        {
            Item item = Item.GetFrom( ctx );
            if( item != null )
            {
                if( item.ShouldRollback == false )
                {
                    CommitTransaction( item.Scope );
                }
                item.Scope.Dispose();
                item.Scope = null;
            }
        }

        protected virtual TransactionScope BeginTransaction()
        {
            return new TransactionScope(
               TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = IsolationLevel },
               TransactionScopeAsyncFlowOption.Enabled );
        }

        protected virtual void CommitTransaction( TransactionScope scope )
        {
            scope.Complete();
        }

        internal class Item
        {
            public bool ShouldRollback { get; set; }

            public TransactionScope Scope { get; set; }

            internal static Item GetFrom( CommandContext ctx )
            {
                Item i = ctx.Items[Key] as Item;
                return i;
            }
        }
    }
}