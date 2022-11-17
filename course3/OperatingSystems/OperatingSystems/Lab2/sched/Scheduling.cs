using System.Collections;
using System.Text;

namespace Lab2.sched;

public class Scheduling
{

    private static int processnum = 5;
    private static int meanDev = 1000;
    private static int standardDev = 100;
    private static int runtime = 1000;
    private static ArrayList processVector = new ArrayList();
    private static Results result = new Results("null", "null", 0);
    private static string resultsFile = "Summary-Results";
    private const double e = 1e-6;

    private static void Init(string file)
    {
        string tmp;
        var cputime = 0;
        var ioblocking = 0;
        var X = 0.0;

        try
        {
            //BufferedReader in = new BufferedReader(new FileReader(f));
            using var input = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Read);
            var b = new byte[1024];
            var encoding = new UTF8Encoding(true);
            int readLen;
            while (( readLen = input.Read(b, 0, b.Length)) > 0)
            {
                var line = encoding.GetString(b, 0, readLen);
                if (line.StartsWith("numprocess"))
                {
                    var st = line.Split(' ');
                    processnum = Common.S2I(st[1]);
                }
                if (line.StartsWith("meandev"))
                {
                    var st = line.Split(' ');
                    meanDev = Common.S2I(st[1]);
                }
                if (line.StartsWith("standdev"))
                {
                    var st = line.Split(' ');
                    standardDev = Common.S2I(st[1]);
                }
                if (line.StartsWith("process"))
                {
                    var st = line.Split(' ');
                    ioblocking = Common.S2I(st[1]);
                    X = Common.R1();
                    while (Math.Abs(X + 1.0) < e)
                    {
                        X = Common.R1();
                    }
                    X *= standardDev;
                    cputime = (int)X + meanDev;
                    processVector.Add(new sProcess(cputime, ioblocking, 0, 0, 0));
                }
                if (line.StartsWith("runtime"))
                {
                    var st = line.Split(' ');
                    runtime = Common.S2I(st[1]);
                }
            }
            input.Close();
        }
        catch (IOException e) { /* Handle exceptions */ }
    }

    private static void debug()
    {
        int i = 0;

        Console.WriteLine("processnum " + processnum);
        Console.WriteLine("meandevm " + meanDev);
        Console.WriteLine("standdev " + standardDev);
        int size = processVector.Count;
        for (i = 0; i < size; i++)
        {
            sProcess process = (sProcess)processVector[i]!;
            Console.WriteLine("process " + i + " " + process.cputime + " " + process.ioblocking + " " + process.cpudone + " " + process.numblocked);
        }
        Console.WriteLine("runtime " + runtime);
    }

    private static void Main(string[] args)
    {
        int i = 0;

        if (args.Length != 1)
        {
            Console.WriteLine("Usage: 'java Scheduling <INIT FILE>'");
            Environment.Exit(-1);
        }
        if (!File.Exists(args[0]))
        {
            Console.WriteLine("Scheduling: error, file '" + Path.GetFileName(args[0]) + "' does not exist.");
            Environment.Exit(-1);
        }
        Console.WriteLine("Working...");
        Init(args[0]);
        if (processVector.Count < processnum)
        {
            i = 0;
            while (processVector.Count < processnum)
            {
                var X = Common.R1();
                while (Math.Abs(X + 1.0) < e)
                {
                    X = Common.R1();
                }
                X *= standardDev;
                var cputime = (int)X + meanDev;
                processVector.Add(new sProcess(cputime, i * 100, 0, 0, 0));
                i++;
            }
        }
        result = SchedulingAlgorithm.Run(runtime, processVector, result);
        try
        {
            using var output = new FileStream(resultsFile, FileMode.OpenOrCreate, FileAccess.Write);
            output.Write(Encoding.Default.GetBytes("Scheduling Type: " + result.schedulingType + "\n"));
            output.Write(Encoding.Default.GetBytes("Scheduling Name: " + result.schedulingName + "\n"));
            output.Write(Encoding.Default.GetBytes("Simulation Run Time: " + result.compuTime + "\n"));
            output.Write(Encoding.Default.GetBytes("Mean: " + meanDev + "\n"));
            output.Write(Encoding.Default.GetBytes("Standard Deviation: " + standardDev + "\n"));
            output.Write(Encoding.Default.GetBytes("Process #\tCPU Time\tIO Blocking\tCPU Completed\tCPU Blocked\n"));
            for (i = 0; i < processVector.Count; i++)
            {
                var process = (sProcess)processVector[i]!;
                output.Write(Encoding.Default.GetBytes(i.ToString()));
                if (i < 100)
                {
                    output.Write(Encoding.Default.GetBytes("\t\t"));
                }
                else
                {
                    output.Write(Encoding.Default.GetBytes("\t"));
                }
                output.Write(Encoding.Default.GetBytes(process.cputime.ToString()));
                if (process.cputime < 100)
                {
                    output.Write(Encoding.Default.GetBytes(" (ms)\t\t"));
                }
                else
                {
                    output.Write(Encoding.Default.GetBytes(" (ms)\t"));
                }
                output.Write(Encoding.Default.GetBytes(process.ioblocking.ToString()));
                if (process.ioblocking < 100)
                {
                    output.Write(Encoding.Default.GetBytes(" (ms)\t\t"));
                }
                else
                {
                    output.Write(Encoding.Default.GetBytes(" (ms)\t"));
                }
                output.Write(Encoding.Default.GetBytes(process.cpudone.ToString()));
                if (process.cpudone < 100)
                {
                    output.Write(Encoding.Default.GetBytes(" (ms)\t\t"));
                }
                else
                {
                    output.Write(Encoding.Default.GetBytes(" (ms)\t"));
                }
                output.Write(Encoding.Default.GetBytes(process.numblocked + " times\n"));
            }
            output.Close();
        }
        catch (IOException e) { /* Handle exceptions */ }
        Console.WriteLine("Completed.");
    }
}
