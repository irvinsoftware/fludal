using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Irvin.Fludal;
using Irvin.Fludal.Postgres;
using Npgsql;
using NUnit.Framework;

namespace Tests;

[TestFixture]
public class PostgresTests
{
    [Test]
    public async Task BasicParameterTest()
    {
        IResult<List<string>> actual =
            await Please.ConnectTo<Postgres>()
                .UsingConfiguredConnectionNamed("postgres")
                .RunQuery("SELECT $1 AS zeb")
                .WithParameter("hello")
                .ThenReadAsList<string>();
        
        Assert.AreEqual("hello", actual.Content.First());
    }

    [Test]
    [Ignore("verify ADO supports what we're trying to do")]
    public async Task SanityCheck()
    {
        object result;
        
        using (CancellationTokenSource cts = new CancellationTokenSource())
        {
            await using (NpgsqlConnection connection = new NpgsqlConnection(Please.GetFrameworkConnectionString("postgres")))
            {
                await using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT $1 AS zeb";
                    NpgsqlParameter parameter = new NpgsqlParameter();
                    parameter.ParameterName = null;
                    parameter.Value = "aloha";
                    command.Parameters.Add(parameter);
                    await connection.OpenAsync(cts.Token);
                    result = await command.ExecuteScalarAsync(cts.Token);
                }
            }
        }

        Assert.AreEqual("aloha", result?.ToString());
    }
}