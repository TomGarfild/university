using Lab1.Common.CompFuncs;
using Lab1.Common.CompFuncs.Basic;
using Microsoft.CodeAnalysis;

namespace Lab1.Common.CompFuncs.Basic;

public class DoubleOps
{
    private static readonly Case<double>[] _cases;

    private static Optional<double> UncheckedF(int var0)
    {
        var var1 = _cases[var0];
        return Configurations.GenericFunc(var1.FAttrs);
    }

    private static Optional<double> UncheckedG(int var0)
    {
        var var1 = _cases[var0];
        return Configurations.GenericFunc(var1.GAttrs);
    }

    public static Optional<double> TrialF(int var0) 
    {
        return Utility.Sandbox(() => UncheckedF(var0));
    }

    public static Optional<double> TrialG(int var0)
    {
        return Utility.Sandbox(() => UncheckedG(var0));
    }
    
    static DoubleOps()
    {
        _cases = new[]
        {
            new Case<double>(new ComputationAttrs<double>(3.0, 1), new ComputationAttrs<double>(5.0, 3)),
            new Case<double>(null, new ComputationAttrs<double>(0.0, 3)),
            new Case<double>(new ComputationAttrs<double>(3.0, 1), null)
        };
    }
}