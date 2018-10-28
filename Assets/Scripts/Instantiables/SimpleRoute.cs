public class SimpleRoute
{
    private string origin;
    private string destination;
    private bool reversed;

    public SimpleRoute(string origin, string destination, bool reversed = false)
    {
        Origin = origin;
        Destination = destination;
        Reversed = reversed;
    }

    public string Origin
    {
        get
        {
            return origin;
        }

        set
        {
            origin = value;
        }
    }

    public string Destination
    {
        get
        {
            return destination;
        }

        set
        {
            destination = value;
        }
    }

    public bool Reversed
    {
        get
        {
            return reversed;
        }

        set
        {
            reversed = value;
        }
    }
}