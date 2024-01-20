using AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsShared.Context;
public static class RandomData
{
    private static readonly Fixture _fixture;

    static RandomData()
    {
        _fixture = new Fixture();
    }

    public static int Int()
    {
        return _fixture.Create<int>();
    }

    public static string String(string? prefix = null, int length = 10)
    {
        return $"{prefix}{_fixture.Create<string>().Substring(0, length)}";
    }
}
