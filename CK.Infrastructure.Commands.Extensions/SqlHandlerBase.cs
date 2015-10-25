//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using CK.Core;
//using CK.Setup;
//using CK.SqlServer;

//namespace CK.Infrastructure.Commands
//{
//    class Cmd
//    {
//        public int ActorId { get; set; }

//        public string Message { get; set; }

//        public ISqlCallContext CallContext { get; set; }
//    }
//    class H : CommandHandler<Cmd>
//    {
//        [InjectContract]
//        public ActivityTable ActivityTable { get; set; }

//        [SqlConnection]
//        protected override Task DoHandleAsync( Cmd command ) => ActivityTable.LogMessageAsync( command );
//    }

//    class ActivityTable
//    {
//        internal Task LogMessageAsync( Cmd command )
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
