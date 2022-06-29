using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Irvin.Extensions.Collections;
using Irvin.Fludal;
using Irvin.Fludal.SqlClient;
using NUnit.Framework;

namespace Tests;

public class ListCaptureTests
{
    private CancellationTokenSource _cancellation;
    
    [SetUp]
    public void RunFirstOnce()
    {
        _cancellation = new CancellationTokenSource();
    }

    [TearDown]
    public void RunAfterEachTest()
    {
        _cancellation.Dispose();
        _cancellation = null;
    }
    
    [Test]
    public async Task CapturesEmitAndReturnCode()
    {
        var actual = await
            Please.ConnectTo<SqlServer>()
                  .UsingConfiguredConnectionNamed("my_test")
                  .AndExecuteStoredProcedure("dbo.[ComplexList]")
                  .WithStringParameter("Super", "whatever")
                  .WithCancellationToken(_cancellation.Token)
                  .ThenReadAsEnumerable<DBModelClass>()
                  .ConfigureAwait(false)
            ;

        int rowCount = 0;
        await foreach (DBModelClass actualItem in actual.Content)
        {
            rowCount++;

            if (rowCount == 1)
            {
                Assert.IsNull(actualItem.A);
            }
            else
            {
                Assert.AreEqual('A', actualItem.A);
            }

            Assert.AreEqual(4, actualItem.B);

            if (rowCount != 2)
            {
                Assert.IsNotNull(actualItem.C);
            }
            else
            {
                Assert.IsNull(actualItem.C);
            }
        }
        Assert.AreEqual(3, rowCount);
        Assert.AreEqual(9, actual.Code);
    }

    [Test]
    public async Task CapturesEmitAndReturnCode_AllAtOnce()
    {
        var actual = await
                Please.ConnectTo<SqlServer>()
                    .UsingConfiguredConnectionNamed("my_test")
                    .AndExecuteStoredProcedure("dbo.[ComplexList]")
                    .WithStringParameter("Super", "whatever")
                    .WithCancellationToken(_cancellation.Token)
                    .ThenReadAsEnumerable<DBModelClass>()
                    .ConfigureAwait(false)
            ;

        List<DBModelClass> actualList = await actual.Content.ToListAsync();

        Assert.AreEqual(9, actual.Code);
        var actualList1 = actualList;
        Assert.AreEqual(3, actualList1.Count);
        Assert.IsNull(actualList1[0].A);
        Assert.AreEqual(4, actualList1[0].B);
        Assert.IsNotNull(actualList1[0].C);
        Assert.AreEqual('A', actualList1[1].A);
        Assert.AreEqual(4, actualList1[1].B);
        Assert.IsNull(actualList1[1].C);
        Assert.AreEqual('A', actualList1[2].A);
        Assert.AreEqual(4, actualList1[2].B);
        Assert.IsNotNull(actualList1[2].C);
    }

    [Test]
    public async Task CapturesEmitAndReturnCode_ForStruct()
    {
        var actual = await
                Please.ConnectTo<SqlServer>()
                      .UsingConnectionString("Data Source=localhost;Integrated Security=SSPI;Initial Catalog=Seal_Test;Application Name=Tests")
                      .AndExecuteStoredProcedure("dbo.[ComplexList]")
                      .WithStringParameter("Super", "whatever")
                      .WithCancellationToken(_cancellation.Token)
                      .ThenReadAsList<DBModelStruct>()
                      .ConfigureAwait(false)
            ;
        
        Assert.AreEqual(9, actual.Code);
        var actualList = actual.Content;
        Assert.AreEqual(3, actualList.Count);
        Assert.IsNull(actualList[0].A);
        Assert.AreEqual(4, actualList[0].B);
        Assert.IsNotNull(actualList[0].C);
        Assert.AreEqual('A', actualList[1].A);
        Assert.AreEqual(4, actualList[1].B);
        Assert.IsNull(actualList[1].C);
        Assert.AreEqual('A', actualList[2].A);
        Assert.AreEqual(4, actualList[2].B);
        Assert.IsNotNull(actualList[2].C);
    }
    
    [Test]
    public async Task CapturesEmitAndReturnCode_ForStructWithProperties()
    {
        var actual = await
                Please.ConnectTo<SqlServer>()
                    .UsingConnectionString("Data Source=localhost;Integrated Security=SSPI;Initial Catalog=Seal_Test;Application Name=Tests")
                    .AndExecuteStoredProcedure("dbo.[ComplexList]")
                    .WithStringParameter("Super", "whatever")
                    .WithCancellationToken(_cancellation.Token)
                    .ThenReadAsList<DBModelStruct2>()
                    .ConfigureAwait(false)
            ;
        
        Assert.AreEqual(9, actual.Code);
        var actualList = actual.Content;
        Assert.AreEqual(3, actualList.Count);
        Assert.IsNull(actualList[0].a);
        Assert.AreEqual(4, actualList[0].B);
        Assert.IsNotNull(actualList[0].C);
        Assert.AreEqual('A', actualList[1].a);
        Assert.AreEqual(4, actualList[1].B);
        Assert.IsNull(actualList[1].C);
        Assert.AreEqual('A', actualList[2].a);
        Assert.AreEqual(4, actualList[2].B);
        Assert.IsNotNull(actualList[2].C);
    }
    
    [Test]
    public async Task CapturesEmitAndReturnCode_ForSimpleRecord()
    {
        var actual = await
                Please.ConnectTo<SqlServer>()
                    .UsingConfiguredConnectionNamed("my_test")
                    .AndExecuteStoredProcedure("dbo.[ComplexList]")
                    .WithStringParameter("Super", "whatever")
                    .WithCancellationToken(_cancellation.Token)
                    .ThenReadAsList<DBModelRecord>()
                    .ConfigureAwait(false)
            ;
        
        Assert.AreEqual(9, actual.Code);
        List<DBModelRecord> actualList = actual.Content;
        Assert.AreEqual(3, actualList.Count);
        Assert.IsNull(actualList[0].A);
        Assert.AreEqual(4, actualList[0].B);
        Assert.IsNotNull(actualList[0].c);
        Assert.AreEqual('A', actualList[1].A);
        Assert.AreEqual(4, actualList[1].B);
        Assert.IsNull(actualList[1].c);
        Assert.AreEqual('A', actualList[2].A);
        Assert.AreEqual(4, actualList[2].B);
        Assert.IsNotNull(actualList[2].c);
    }
    
    [Test]
    public async Task CapturesEmitAndReturnCode_ForInitOnlyRecord()
    {
        var actual = await
                Please.ConnectTo<SqlServer>()
                    .UsingConfiguredConnectionNamed("my_test")
                    .AndExecuteStoredProcedure("dbo.[ComplexList]")
                    .WithStringParameter("Super", "whatever")
                    .WithCancellationToken(_cancellation.Token)
                    .ThenReadAsList<DBModelRecord3>()
                    .ConfigureAwait(false)
            ;
        
        Assert.AreEqual(9, actual.Code);
        var actualList = actual.Content;
        Assert.AreEqual(3, actualList.Count);
        Assert.IsNull(actualList[0].A);
        Assert.AreEqual(4, actualList[0].B);
        Assert.IsNotNull(actualList[0].C);
        Assert.AreEqual('A', actualList[1].A);
        Assert.AreEqual(4, actualList[1].B);
        Assert.IsNull(actualList[1].C);
        Assert.AreEqual('A', actualList[2].A);
        Assert.AreEqual(4, actualList[2].B);
        Assert.IsNotNull(actualList[2].C);
    }
    
    [Test]
    public async Task CapturesEmitAndReturnCode_ForRecord()
    {
        var actual = await
                Please.ConnectTo<SqlServer>()
                    .UsingConfiguredConnectionNamed("my_test")
                    .AndExecuteStoredProcedure("dbo.[ComplexList]")
                    .WithStringParameter("Super", "whatever")
                    .WithCancellationToken(_cancellation.Token)
                    .ThenReadAsList<DBModelRecord2>()
                    .ConfigureAwait(false)
            ;
        
        Assert.AreEqual(9, actual.Code);
        var actualList = actual.Content;
        Assert.AreEqual(3, actualList.Count);
        Assert.IsNull(actualList[0].A);
        Assert.AreEqual(4, actualList[0].b);
        Assert.IsNotNull(actualList[0].C);
        Assert.AreEqual('A', actualList[1].A);
        Assert.AreEqual(4, actualList[1].b);
        Assert.IsNull(actualList[1].C);
        Assert.AreEqual('A', actualList[2].A);
        Assert.AreEqual(4, actualList[2].b);
        Assert.IsNotNull(actualList[2].C);
    }
    
    [Test]
    public async Task CapturesEmitAndReturnCode_ForRecordStruct()
    {
        var actual = await
                Please.ConnectTo<SqlServer>()
                    .UsingConfiguredConnectionNamed("my_test")
                    .AndExecuteStoredProcedure("dbo.[ComplexList]")
                    .WithStringParameter("Super", "whatever")
                    .WithCancellationToken(_cancellation.Token)
                    .ThenReadAsList<DBModelRecordStruct>()
                    .ConfigureAwait(false)
            ;
        
        Assert.AreEqual(9, actual.Code);
        var actualList = actual.Content;
        Assert.AreEqual(3, actualList.Count);
        Assert.IsNull(actualList[0].A);
        Assert.AreEqual(4, actualList[0].B);
        Assert.IsNotNull(actualList[0].C);
        Assert.AreEqual('A', actualList[1].A);
        Assert.AreEqual(4, actualList[1].B);
        Assert.IsNull(actualList[1].C);
        Assert.AreEqual('A', actualList[2].A);
        Assert.AreEqual(4, actualList[2].B);
        Assert.IsNotNull(actualList[2].C);
    }
    
    [Test]
    public async Task CapturesEmitAndReturnCode_ForReadonlyRecordStruct()
    {
        var actual = await
                Please.ConnectTo<SqlServer>()
                    .UsingConfiguredConnectionNamed("my_test")
                    .AndExecuteStoredProcedure("dbo.[ComplexList]")
                    .WithStringParameter("Super", "whatever")
                    .WithCancellationToken(_cancellation.Token)
                    .ThenReadAsList<DBModelRecordStructLocked>()
                    .ConfigureAwait(false)
            ;
        
        Assert.AreEqual(9, actual.Code);
        var actualList = actual.Content;
        Assert.AreEqual(3, actualList.Count);
        Assert.IsNull(actualList[0].A);
        Assert.AreEqual(4, actualList[0].B);
        Assert.IsNotNull(actualList[0].C);
        Assert.AreEqual('A', actualList[1].A);
        Assert.AreEqual(4, actualList[1].B);
        Assert.IsNull(actualList[1].C);
        Assert.AreEqual('A', actualList[2].A);
        Assert.AreEqual(4, actualList[2].B);
        Assert.IsNotNull(actualList[2].C);
    }
    
    private class DBModelClass
    {
        public char? A { get; set; }
        public int B { get; set; }
        public DateTime? C { get; set; }
    }
    
    private struct DBModelStruct
    {
        public char? A;
        public int B;
        public DateTime? C;
    }
    
    private struct DBModelStruct2
    {
        public char? a { get; set; }
        public int B { get; set; }
        public DateTime? C { get; set; }
    }

    private record DBModelRecord(char? A, int B, DateTime? c);
    private record struct DBModelRecordStruct(char? A, int B, DateTime? C);
    private readonly record struct DBModelRecordStructLocked(char? A, int B, DateTime? C);

    private record DBModelRecord2
    {
        public char? A { get; set; }
        public int b { get; set; }
        public DateTime? C { get; set; }
    }
    
    private record DBModelRecord3
    {
        public char? A { get; init; }
        public int B { get; init; }
        public DateTime? C { get; init; }
    }
}