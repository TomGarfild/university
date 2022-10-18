using Microsoft.CodeAnalysis;

namespace Lab1.Common.CompFuncs;

public class Configurations
{
    public Configurations()
    {
    }

    public static Optional<OR> FlatGenericFunc<OR>(OR var0, int var1)
    {
        try
        {
            Thread.Sleep(var1);
        }
        catch (ThreadInterruptedException)
        {
            return new Optional<OR>();
        }

        return var0;
    }

    public static Optional<Optional<R>> FlatGenericFunc<R>(ComputationAttrs<R> var0)
    {
        return FlatGenericFunc(new Optional<R>(var0.Result), var0.Delay);
    }

    public static Optional<R> GenericFunc<R>(Optional<ComputationAttrs<R>> var0)
    {
        return var0.Value.Result;
    }
}