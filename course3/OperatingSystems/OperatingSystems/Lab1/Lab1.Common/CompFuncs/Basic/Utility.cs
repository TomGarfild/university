using Microsoft.CodeAnalysis;

namespace Lab1.Common.CompFuncs.Basic;

public class Utility
{
    public static Optional<T> Sandbox<T>(Func<Optional<T>> var0) {
        try {
            return var0();
        } catch (IndexOutOfRangeException) {
            return new Optional<T>();
        } catch (ThreadInterruptedException) {
            throw;
        } catch (Exception var4) {
            Console.WriteLine(var4.StackTrace);
            return new Optional<T>();
        }
    }
}