using Core.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace TestsShared.Context;
public abstract class TestContextBase : IDisposable
{
    public ITestOutputHelper TestOutputHelper { get; }
    protected readonly CloudNativeDbContext Context;
    private readonly int _rowCountBefore;
    private int _rowCountAfter;

    public TestContextBase(CloudNativeDbContext context, ITestOutputHelper testOutputHelper)
    {
        Context = context;
        TestOutputHelper = testOutputHelper;
        _rowCountBefore = GetRowCount();
    }

    protected int GetRowCount()
    {
        var sql = @"
            CREATE TABLE #counts
            (
                table_name varchar(255),
                row_count int
            )

            EXEC sp_MSForEachTable @command1='INSERT #counts (table_name, row_count) SELECT ''?'', COUNT(*) FROM ?'
            SELECT @row_count = sum(row_count) FROM #counts
            DROP TABLE #counts";

        var rowCount = new SqlParameter("@row_count", System.Data.SqlDbType.Int);
        rowCount.Direction = System.Data.ParameterDirection.Output;
        Context.Database.ExecuteSqlRaw(sql, rowCount);

        return (int)rowCount.Value;
    }

    public abstract void Cleanup();

    private void CleanupInternal()
    {
        Cleanup();
        _rowCountAfter = GetRowCount();

        if (_rowCountAfter != _rowCountBefore)
        {
            Assert.Fail(
                "Test data cleanup failed -check the implementation of 'Cleanup' to ensure all test data has been removed");
        }
    }


    public void Dispose()
    {
        CleanupInternal();
    }
}
