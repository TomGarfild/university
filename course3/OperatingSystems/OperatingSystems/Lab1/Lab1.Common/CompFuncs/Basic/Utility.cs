namespace Lab1.Common.CompFuncs.Basic;

public class Utility
{
    public static Optional<T> Sandbox<T>(Func<Optional<T>> var0) {
        try
        {
            return var0();
        }
        catch (IndexOutOfRangeException)
        {
            return Optional<T>.Empty();
        }
        catch (ThreadInterruptedException)
        {
            return Optional<T>.Empty();
        }
        catch (Exception var4) {
            Console.WriteLine(var4.StackTrace);
            return Optional<T>.Empty();
        }
    }
}