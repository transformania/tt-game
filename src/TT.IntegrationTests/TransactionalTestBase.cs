using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace TT.IntegrationTests
{
    public abstract class TransactionalTestBase : TestBase
    {
        protected TransactionScope Transaction { get; set; }

        public override void SetUp()
        {
            StartTransaction();
        }

        public override void TearDown()
        {
            EndTransaction();
        }

        protected virtual void StartTransaction()
        {
            Transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        }

        protected virtual void EndTransaction()
        {
            Transaction.Dispose();
        }
    }
}
