using AutoFixture;
using Core.Data;
using Xunit.Abstractions;

namespace TestsShared.Context;
public class SharedTestDataContext : TestContextBase
{
    private readonly ITestOutputHelper _testOutputHelper;
    public readonly string CorrelationId;
    public string Prefix = nameof(SharedTestDataContext);
    private readonly Fixture _fixture;

    public SharedTestDataContext(CloudNativeDbContext context, ITestOutputHelper testOutputHelper,
        string? correlationPrefix = null) : base (context, testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        Prefix = correlationPrefix ?? Prefix;
        CorrelationId = $"{correlationPrefix ?? Prefix}{RandomData.String()}";
        _fixture = new Fixture();
    }

    public override void Cleanup()
    {
    }
}
