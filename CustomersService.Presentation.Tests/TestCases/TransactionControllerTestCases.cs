
using CustomersService.Presentation.Models.Requests;

namespace CustomersService.Presentation.Tests.TestCases
{
    public class TransactionControllerTestCases
    {
        public static IEnumerable<object[]> SimpleTransaction()
        {
            var transaction = new TransactionCreateRequest()
            {
                AccountId = Guid.NewGuid(),
                Amount = 100
            };

            yield return new object[] { transaction };
        }

        public static IEnumerable<object[]> TransferTransaction()
        {
            var transaction = new TransferTransactionCreateRequest()
            {
                FromAccountId = Guid.NewGuid(),
                ToAccountId = Guid.NewGuid(),
                Amount = 100
            };

            yield return new object[] { transaction };
        }
    }
}
