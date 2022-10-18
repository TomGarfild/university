using Microsoft.CodeAnalysis;
using BasicDoubleOps = Lab1.Common.CompFuncs.Basic.DoubleOps;

namespace Lab1.Common.CompFuncs.Advanced;

public class DoubleOps
{
    public static Optional<Optional<double>> TrialF(int x)
    {
        return new Optional<Optional<double>>(BasicDoubleOps.TrialF(x));
    }

    public static Optional<Optional<double>> TrialG(int x)
    {
        return new Optional<Optional<double>>(BasicDoubleOps.TrialG(x));
    }
}