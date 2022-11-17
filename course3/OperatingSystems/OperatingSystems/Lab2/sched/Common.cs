namespace Lab2.sched;

public class Common
{
    public static int S2I(string s)
    {
        var i = 0;

        try
        {
            i = int.Parse(s.Trim());
        }
        catch (FormatException ex)
        {
            Console.WriteLine(ex.Message);
        }

        return i;
    }

    public static double R1()
    {
        var generator = new Random();
        var U = generator.NextDouble();

        while (U is < 0 or >= 1)
        {
            U = generator.NextDouble();
        }
        var V = generator.NextDouble();
        while (V is < 0 or >= 1)
        {
            V = generator.NextDouble();
        }
        double X = Math.Sqrt(8 / Math.E) * (V - 0.5) / U;
        if (!R2(X, U) || !R3(X, U) || !R4(X, U))
        {
            return -1;
        }

        return X;
    }

    public static bool R2(double X, double U)
    {
        return X * X <= 5 - 4 * Math.Exp(.25) * U;
    }

    public static bool R3(double X, double U)
    {
        return !(X * X >= 4 * Math.Exp(-1.35) / U + 1.4);
    }

    public static bool R4(double X, double U)
    {
        return X * X < -4 * Math.Log(U);
    }
}