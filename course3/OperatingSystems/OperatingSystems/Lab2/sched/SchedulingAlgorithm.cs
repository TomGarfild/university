using System.Collections;
using System.Text;

namespace Lab2.sched;

public class SchedulingAlgorithm
{

    public static Results Run(int runtime, int quantum, List<sProcess> processVector, Results result)
    {
        var comptime = 0;
        var size = processVector.Count;
        var completed = 0;
        var resultsFile = "Summary-Processes";
        var orderedProcesses = new List<sProcess>(processVector).OrderBy(p => p.arrivaltime);
        var processQueue = new Queue<sProcess>(processVector.Where(p => p.arrivaltime == 0));

        result.schedulingType = "Interactive (Preemptive)";
        result.schedulingName = "Round Robin";
        try
        {
            using var output = new FileStream(resultsFile, FileMode.Truncate, FileAccess.Write);
            var process = processQueue.Dequeue()!;
            output.Write(Encoding.Default.GetBytes("Process: " + process.index + " registered... (" + process.cputime + " " + process.cpudone + ")\n"));
            var finish = false;
            while (comptime < runtime)
            {
                finish = false;
                if (process.cpudone == process.cputime)
                {
                    completed++;
                    output.Write(Encoding.Default.GetBytes("Process: " + process.index + " completed... (" + process.cputime + " " + process.cpudone + ")\n"));
                    if (completed == size)
                    {
                        result.compuTime = comptime;
                        output.Close();
                        return result;
                    }
                    process = processQueue.Dequeue()!;
                    output.Write(Encoding.Default.GetBytes("Process: " + process.index + " registered... (" + process.cputime + " " + process.cpudone + ")\n"));
                    finish = true;
                }
                if (quantum == process.quantumnext)
                {
                    output.Write(Encoding.Default.GetBytes("Process: " + process.index + " spent quantum... (" + process.cputime + " " + process.cpudone + ")\n"));
                    process.numblocked++;
                    process.quantumnext = 0;
                    processQueue.Enqueue(process);
                    process = processQueue.Dequeue()!;
                    output.Write(Encoding.Default.GetBytes("Process: " + process.index + " registered... (" + process.cputime + " " + process.cpudone + ")\n"));
                    finish = true;
                }
                process.cpudone++;
                process.quantumnext++;
                comptime++;
                foreach (var p in orderedProcesses.Where(o => o.arrivaltime == comptime))
                {
                    processQueue.Enqueue(p);
                    output.Write(Encoding.Default.GetBytes("Process: " + p.index + " arrived at " + comptime + "... (" + p.cputime + " " + p.cpudone + ")\n"));
                }
            }
            if (!finish)
            {
                output.Write(Encoding.Default.GetBytes("Process: " + process.index + " did not finish... (" + process.cputime + " " + process.cpudone + ")\n"));
            }
            output.Close();
        }
        catch (IOException e) { /* Handle exceptions */ }

        result.compuTime = comptime;
        return result;
    }
}
