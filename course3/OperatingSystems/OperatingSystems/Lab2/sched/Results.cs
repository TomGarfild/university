namespace Lab2.sched;

public class Results
{
    public string schedulingType;
    public string schedulingName;
    public int compuTime;

    public Results(string schedulingType, string schedulingName, int compuTime)
    {
        this.schedulingType = schedulingType;
        this.schedulingName = schedulingName;
        this.compuTime = compuTime;
    }
}