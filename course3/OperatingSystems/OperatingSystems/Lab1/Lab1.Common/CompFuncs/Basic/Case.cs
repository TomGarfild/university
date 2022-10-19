namespace Lab1.Common.CompFuncs.Basic;

class Case<T>
{
    public Optional<ComputationAttrs<T>> FAttrs { get; }
    public Optional<ComputationAttrs<T>> GAttrs { get; }

    public Case(ComputationAttrs<T> var1, ComputationAttrs<T> var2)
    {
        FAttrs = Optional<ComputationAttrs<T>>.OfNullable(var1);
        GAttrs = Optional<ComputationAttrs<T>>.OfNullable(var2);
    }
}