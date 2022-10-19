using System.Diagnostics;

namespace Lab1.Common.CompFuncs;

public class Configurations
{
    public static Optional<OR> FlatGenericFunc<OR>(OR var0, int var1)
    {
        try
        {
            Thread.Sleep(var1 * 1000);
        }
        catch (ThreadInterruptedException)
        {
            return Optional<OR>.Empty();
        }

        return Optional<OR>.Of(var0);
    }

    public static Optional<Optional<R>> FlatGenericFunc<R>(ComputationAttrs<R> var0)
    {
        return FlatGenericFunc(Optional<R>.OfNullable(var0.Result), var0.Delay);
    }

    public static Optional<R> GenericFunc<R>(Optional<ComputationAttrs<R>> var0)
    {
        return Optional<ComputationAttrs<R>>.OfNullable(var0.OrElseGet(() =>
        {
            //try
            //{
            //    Thread.CurrentThread.Join();
            //}
            //catch (ThreadInterruptedException)
            //{
            //}

            return null;
        })).FlatMap(FlatGenericFunc).OrElseThrow(() => new ThreadInterruptedException());
    }
}