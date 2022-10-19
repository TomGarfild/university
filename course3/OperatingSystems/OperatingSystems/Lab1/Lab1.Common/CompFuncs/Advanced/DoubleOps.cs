using BasicDoubleOps = Lab1.Common.CompFuncs.Basic.DoubleOps;

namespace Lab1.Common.CompFuncs.Advanced;

public class DoubleOps
{
    public static Optional<Optional<double>> TrialF(int x)
    {
        try
        {
            return Optional<Optional<double>>.Of(BasicDoubleOps.TrialF(x));
        }
        catch (ThreadInterruptedException)
        {
            return Optional<Optional<double>>.Empty();
        }
    }

    public static Optional<Optional<double>> TrialG(int x)
    {
        try
        {
            return Optional<Optional<double>>.Of(BasicDoubleOps.TrialG(x));
        }
        catch (ThreadInterruptedException)
        {
            return Optional<Optional<double>>.Empty();
        }
    }
}