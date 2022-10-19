using BasicDoubleOps = Lab1.Common.CompFuncs.Basic.DoubleOps;

namespace Lab1.Common.CompFuncs.Advanced;

public class DoubleOps
{
    public static Optional<Optional<double?>> TrialF(int x)
    {
        try
        {
            var value = BasicDoubleOps.TrialG(x);
            return Optional<Optional<double?>>.OfNullable(!value.IsPresent() || Random.Shared.Next(0, 2) == 0 ? BasicDoubleOps.TrialF(x) : null);
        }
        catch (ThreadInterruptedException)
        {
            return Optional<Optional<double?>>.Empty();
        }
    }

    public static Optional<Optional<double?>> TrialG(int x)
    {
        try
        {
            var value = BasicDoubleOps.TrialG(x);
            return Optional<Optional<double?>>.OfNullable(!value.IsPresent() || Random.Shared.Next(0, 3) == 0 ? value : null);
        }
        catch (ThreadInterruptedException)
        {
            return Optional<Optional<double?>>.Empty();
        }
    }
}