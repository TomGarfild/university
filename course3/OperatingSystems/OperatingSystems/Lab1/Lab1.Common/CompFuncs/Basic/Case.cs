using Microsoft.CodeAnalysis;

namespace Lab1.Common.CompFuncs.Basic;

class Case<T>
{
    public Optional<ComputationAttrs<T>> FAttrs { get; }
    public Optional<ComputationAttrs<T>> GAttrs { get; }

    public Case(ComputationAttrs<T> var1, ComputationAttrs<T> var2)
    {
        FAttrs = new Optional<ComputationAttrs<T>>(var1);
        GAttrs = new Optional<ComputationAttrs<T>>(var2);
    }
}