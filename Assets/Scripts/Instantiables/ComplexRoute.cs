public class ComplexRoute
{
    private SimpleRoute originRoute;
    private SimpleRoute destinationRoute;

    public ComplexRoute(SimpleRoute originRoute, SimpleRoute destinationRoute)
    {
        OriginRoute = originRoute;
        DestinationRoute = destinationRoute;
    }

    public SimpleRoute OriginRoute
    {
        get
        {
            return originRoute;
        }

        set
        {
            originRoute = value;
        }
    }

    public SimpleRoute DestinationRoute
    {
        get
        {
            return destinationRoute;
        }

        set
        {
            destinationRoute = value;
        }
    }
}