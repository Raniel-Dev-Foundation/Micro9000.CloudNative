using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace TestsShared.Logging;
public class TimingLogger : IDisposable
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly string _operationName;
    private Stopwatch _stopwatch;

    public TimingLogger(ITestOutputHelper testOutputHelper, string operationName)
    {
        _testOutputHelper = testOutputHelper;
        _operationName = operationName;

        _stopwatch = new Stopwatch();
        _stopwatch.Start();
    }

    public void Dispose()
    {
        _stopwatch.Stop();
        _testOutputHelper.WriteLine("[TIMING] {0} - completed in {1} ms", _operationName, _stopwatch.ElapsedMilliseconds);
    }
}
