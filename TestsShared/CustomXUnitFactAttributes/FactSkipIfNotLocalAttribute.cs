using System.Diagnostics;
using Xunit;

namespace TestsShared.CustomXUnitFactAttributes;
public sealed class FactSkipIfNotLocalAttribute : FactAttribute
{
    public FactSkipIfNotLocalAttribute()
    {
        var isLocal = false;
        SetIsLocal(ref isLocal);
        if (!isLocal)
        {
            Skip = "This test is only run during local development, not as part of CI";
        }
    }

    [Conditional("DEBUG")]
    private void SetIsLocal (ref bool isLocal)
    {
        isLocal = true;
    }
}
