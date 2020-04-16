using System.Collections;
using System.Threading;

public class ThreadedJob
{
    //private bool isDone = true;
    private object handle = new object();
    private Thread thread = null;
    /*public bool IsDone
    {
        get
        {
            bool tmp;
            lock (handle)
            {
                tmp = isDone;
            }
            return tmp;
        }
        set
        {
            lock (handle)
            {
                isDone = value;
            }
        }
    }*/

    public virtual void Start()
    {
        //IsDone = false;

        thread = new Thread(Run);
        thread.Start();
    }

    public virtual void Abort()
    {
        if (thread != null)
        {
            thread.Abort();
        }
    }

    protected virtual void ThreadFunction() { }

    protected virtual void OnFinished() { }

    /*public virtual bool Update()
    {
        if (IsDone)
        {
            OnFinished();
            return true;
        }

        return false;
    }

    IEnumerator WaitFor()
    {
        while (!Update())
        {
            yield return null;
        }
    }*/

    private void Run()
    {
        ThreadFunction();
        //IsDone = true;
        //OnFinished();
    }
}