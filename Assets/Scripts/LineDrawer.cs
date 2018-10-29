using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineDrawer : MonoBehaviour {

    // Simple
    public GameObject city_a;
    public GameObject city_b;
    public LineRenderer simpleRouteLine;
    private Color simpleRouteStartColor = new Color(0.2745098f, 0.882353f, 0.8196079f, 1); //turquose
    private Color simpleRouteEndColor = new Color(0.03137255f, 0.5254902f, 0.04313726f, 1); //green

    // Complex
    public GameObject city_c;
    public GameObject city_d;
    public GameObject city_e;
    public LineRenderer complexRouteLine;
    private Color complexRouteStartColor = new Color(1, 0.601f, 0, 1); //orange
    private Color complexRouteEndColor = new Color(1, 0, 0, 1); //red

    private Color defaultCityColor = new Color(0.4337398f, 0.4465112f, 0.5377358f, 1); //Grey

    // Use this for initialization
    void Start()
    {
        
    }

    private void DebugDrawLine()
    {
        Vector3[] simpleRoute = new Vector3[] {
            city_a.transform.position,
            city_b.transform.position
        };

        Vector3[] complexRoute = new Vector3[] {
            city_c.transform.position,
            city_d.transform.position,
            city_e.transform.position
        };

        //debug
        PaintCity(city_a.GetComponent<Image>(), simpleRouteStartColor);
        PaintCity(city_b.GetComponent<Image>(), simpleRouteEndColor);
        DrawLine(simpleRouteLine, simpleRoute, simpleRouteStartColor, simpleRouteEndColor);


        PaintCity(city_c.GetComponent<Image>(), complexRouteStartColor);
        PaintCity(city_d.GetComponent<Image>(), complexRouteStartColor);
        PaintCity(city_e.GetComponent<Image>(), complexRouteEndColor);
        DrawLine(complexRouteLine, complexRoute, complexRouteStartColor, complexRouteEndColor);
    }

    private void PaintCity(Image city_image, Color color)
    {
        city_image.color = color;
    }

    public void DrawCarRoute(
        string origin_name,
        string middle_name,
        string destination_name
    ) {
        GameObject origin = GameObject.Find(origin_name);
        GameObject middle = GameObject.Find(middle_name);
        GameObject destination = GameObject.Find(destination_name);

        Vector3[] complexRoute = new Vector3[] {
            origin.transform.position,
            middle.transform.position,
            destination.transform.position
        };

        PaintCity(origin.GetComponent<Image>(), complexRouteStartColor);
        PaintCity(middle.GetComponent<Image>(), complexRouteStartColor);
        PaintCity(destination.GetComponent<Image>(), complexRouteEndColor);
        DrawLine(complexRouteLine, complexRoute, complexRouteStartColor, complexRouteEndColor);
    }

    public void ClearCarRoute(
        string origin_name,
        string middle_name,
        string destination_name
    ) {
        GameObject origin = GameObject.Find(origin_name);
        GameObject middle = GameObject.Find(middle_name);
        GameObject destination = GameObject.Find(destination_name);

        PaintCity(origin.GetComponent<Image>(), defaultCityColor);
        PaintCity(middle.GetComponent<Image>(), defaultCityColor);
        PaintCity(destination.GetComponent<Image>(), defaultCityColor);

        ClearLine(complexRouteLine);
    }

    public void DrawPassengerRoute(
       SimpleRoute simpleRoute
    )
    {
        GameObject origin = GameObject.Find(simpleRoute.Origin);
        GameObject destination = GameObject.Find(simpleRoute.Destination);

        Vector3[] simpleRouteVector = new Vector3[] {
            origin.transform.position,
            destination.transform.position
        };

        PaintCity(origin.GetComponent<Image>(), simpleRouteStartColor);
        PaintCity(destination.GetComponent<Image>(), simpleRouteEndColor);
        DrawLine(simpleRouteLine, simpleRouteVector, simpleRouteStartColor, simpleRouteEndColor);
    }

    public void ClearPassengerRoute(
        SimpleRoute simpleRoute
    )
    {
        GameObject origin = GameObject.Find(simpleRoute.Origin);
        GameObject destination = GameObject.Find(simpleRoute.Destination);

        PaintCity(origin.GetComponent<Image>(), defaultCityColor);
        PaintCity(destination.GetComponent<Image>(), defaultCityColor);

        ClearLine(simpleRouteLine);
    }

    private void DrawLine(LineRenderer line, Vector3[] route, Color startColor, Color endColor)
    {
        line.startWidth = .2f;
        line.endWidth = .02f;

        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = startColor;
        line.endColor = endColor;

        line.positionCount = route.Length;
        line.SetPositions(route);

        line.enabled = true;
    }

    private void ClearLine(LineRenderer line)
    {
        line.enabled = false;
    }

}
