using Core.Data;
using Core.Models;
using Dapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TestsShared.Context;
using TestsShared.Logging;
using TestsShared.Mocks;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests.Database.StoredProcedure;
[Collection("Database collection")]
public class GetAllUsersTests : IDisposable
{
    private const string Prefix = nameof(GetAllUsersTests);
    private readonly CloudNativeDbContext _dbContext;
    protected readonly SharedTestDataContext _sharedTestDataContext;
    private readonly ITestOutputHelper _testOutputHelper;

    public GetAllUsersTests(DatabaseFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _dbContext = fixture.CreateDbContext();
        _testOutputHelper = testOutputHelper;
        _sharedTestDataContext = new SharedTestDataContext(_dbContext, testOutputHelper, Prefix);
        Cleanup();
    }

    public void Dispose()
    {
        Cleanup();
    }

    private void Cleanup()
    {
        _dbContext.Users.ExecuteDelete();
        _sharedTestDataContext.Dispose();
    }

    [Fact]
    public async Task ShouldBeAbleToGetAllUsers()
    {
        using (new TimingLogger(_testOutputHelper, "Test GetAllUsers tests"))
        {
            new UserBuilder()
                .WithName("Test")
                .BuildInto(_dbContext);

            var spResults = await _dbContext.Database.GetDbConnection().QueryAsync<User>(
                "GetAllUsers", commandType: CommandType.StoredProcedure, commandTimeout: 300);

            spResults.Count().Should().Be(1);
        }

    }
}
