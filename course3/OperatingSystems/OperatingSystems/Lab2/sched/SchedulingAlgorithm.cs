using System.Collections;
using System.Text;

namespace Lab2.sched;

public class SchedulingAlgorithm
{

    public static Results Run(int runtime, ArrayList processVector, Results result)
    {
        var i = 0;
        var comptime = 0;
        var currentProcess = 0;
        var previousProcess = 0;
        var size = processVector.Count;
        var completed = 0;
        var resultsFile = "Summary-Processes";

        result.schedulingType = "Batch (Nonpreemptive)";
        result.schedulingName = "First-Come First-Served";
        try
        {
            //BufferedWriter out = new BufferedWriter(new FileWriter(resultsFile));
            //OutputStream out = new FileOutputStream(resultsFile);
            using var output = new FileStream(resultsFile, FileMode.OpenOrCreate, FileAccess.Write);
            var process = (sProcess)processVector[currentProcess]!;
            output.Write(Encoding.Default.GetBytes("Process: " + currentProcess + " registered... (" + process.cputime + " " + process.ioblocking + " " + process.cpudone + ")\n"));
            while (comptime < runtime)
            {
                if (process.cpudone == process.cputime)
                {
                    completed++;
                    output.Write(Encoding.Default.GetBytes("Process: " + currentProcess + " completed... (" + process.cputime + " " + process.ioblocking + " " + process.cpudone + ")\n"));
                    if (completed == size)
                    {
                        result.compuTime = comptime;
                        output.Close();
                        return result;
                    }
                    for (i = size - 1; i >= 0; i--)
                    {
                        process = (sProcess)processVector[i]!;
                        if (process.cpudone < process.cputime)
                        {
                            currentProcess = i;
                        }
                    }
                    process = (sProcess)processVector[currentProcess]!;
                    output.Write(Encoding.Default.GetBytes("Process: " + currentProcess + " registered... (" + process.cputime + " " + process.ioblocking + " " + process.cpudone + ")\n"));
                }
                if (process.ioblocking == process.ionext)
                {
                    output.Write(Encoding.Default.GetBytes("Process: " + currentProcess + " I/O blocked... (" + process.cputime + " " + process.ioblocking + " " + process.cpudone + ")\n"));
                    process.numblocked++;
                    process.ionext = 0;
                    previousProcess = currentProcess;
                    for (i = size - 1; i >= 0; i--)
                    {
                        process = (sProcess)processVector[i]!;
                        if (process.cpudone < process.cputime && previousProcess != i)
                        {
                            currentProcess = i;
                        }
                    }
                    process = (sProcess)processVector[currentProcess]!;
                    output.Write(Encoding.Default.GetBytes("Process: " + currentProcess + " registered... (" + process.cputime + " " + process.ioblocking + " " + process.cpudone + ")\n"));
                }
                process.cpudone++;
                if (process.ioblocking > 0)
                {
                    process.ionext++;
                }
                comptime++;
            }
            output.Close();
        }
        catch (IOException e) { /* Handle exceptions */ }
        result.compuTime = comptime;
        return result;
    }
}
