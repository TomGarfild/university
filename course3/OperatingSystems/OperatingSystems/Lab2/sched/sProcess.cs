namespace Lab2.sched;

public class sProcess
{
    public int cputime;
    public int cpudone;
    public int quantumnext;
    public int numblocked;
    public int index;

    public sProcess(int cputime, int cpudone, int quantumnext, int numblocked, int index)
    {
        this.cputime = cputime;
        this.cpudone = cpudone;
        this.quantumnext = quantumnext;
        this.numblocked = numblocked;
        this.index = index;
    }
}