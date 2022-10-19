namespace Lab1.Common.CompFuncs;

public class ComputationAttrs<R>
{
    public R Result { get; }
    public int Delay { get; }

    public ComputationAttrs(R result, int delay)
    {
        Result = result;
        Delay = delay;
    }
}