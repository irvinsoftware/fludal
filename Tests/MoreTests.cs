using System;
using System.Threading;
using System.Threading.Tasks;
using Irvin.Fludal;
using Irvin.Fludal.SqlClient;
using NUnit.Framework;

namespace Tests;

[TestFixture]
public class MoreTests
{
    [Test]
    public async Task GetsOutputParameters()
    {
        SqlResult execution = await
            Please.ConnectTo<SqlServer>()
                .UsingConfiguredConnectionNamed("my_test")
                .AndExecuteStoredProcedure("dbo.InOutReturn")
                .WithParameter<decimal>("@Greg", 4.53M)
                .WithParameter<DateTime>("@Tuila", DateTime.Now)
                .WithStringParameter("@Samuel", "London Sydney Auckland")
                .WithParameter<int>("@Kings", null)
                .WithOutputParameter<int>("@TheIt")
                .WithCancellationToken(CancellationToken.None)
                .Go()
                .ConfigureAwait(false);

        int? actualValue = execution.GetOutputValue<int>("@TheIt");
        
        Assert.AreEqual(5, execution.Code);
        Assert.NotNull(actualValue);
        Assert.AreEqual(92, actualValue.Value);
    }
}