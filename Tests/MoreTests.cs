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
    public async Task ConvertsDoubleToDecimal()
    {
        decimal actual = await
                Please.ConnectTo<SqlServer>()
                      .UsingConfiguredConnectionNamed("my_test")
                      .RunQuery("SELECT CAST(1903484.3252435 AS FLOAT)")
                      .ThenReadAsMultipleParts(o => o.PopulateFields())
                      .ReadSingle<decimal>()
                      .ConfigureAwait(false);
        
        Assert.AreEqual(1903484.3252435, actual);
    }
    
    [Test]
    public async Task GetsOutputParameters()
    {
        SqlResult execution = await
            Please.ConnectTo<SqlServer>()
                .UsingConfiguredConnectionNamed("my_test")
                .AndExecuteStoredProcedure("dbo.InOutReturn")
                .WithParameter<decimal>("@Greg", 4.53M)
                .WithParameter<DateTime>("@Tuila", DateTime.Now)
                .WithParameter("@Samuel", "London Sydney Auckland")
                .WithParameter<int>("@Kings", null)
                .WithOutputParameter<int>("@TheIt")
                .WithCancellationToken(CancellationToken.None)
                .AndReturn()
                .ConfigureAwait(false);

        int? actualValue = execution.GetOutputValue<int>("@TheIt");
        
        Assert.AreEqual(5, execution.Code);
        Assert.NotNull(actualValue);
        Assert.AreEqual(92, actualValue.Value);
    }

    [Test]
    public async Task DoesSimpleButRepeatedExecution()
    {
        SqlServer databaseConnection = Please.ConnectTo<SqlServer>().UsingConfiguredConnectionNamed("my_test");
        
        await
            databaseConnection
                .ExecuteStoredProcedure("dbo.Transaction_Increment")
                .WithParameter<int>("NewValue", 1324)
                .WithCancellationToken(CancellationToken.None)
                .AndReturn()
                .ConfigureAwait(false);

        IResult<int?> actual = await databaseConnection.RunQuery("SELECT NumberValue FROM dbo.TransactionTable").ThenReturn<int>();
        Assert.NotNull(actual);
        Assert.NotNull(actual.Content);
        Assert.AreEqual(1324, actual.Content);
    }
}