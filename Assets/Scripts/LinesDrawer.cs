using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LinesDrawer : MonoBehaviour {

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

    // Use this for initialization
    void Start()
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

    private void DrawLine(LineRenderer line, Vector3[] route, Color startColor, Color endColor)
    {


        //line.sortingOrder = 3;
        //line.sortingLayerName = "Default";

        line.startWidth = .2f;
        line.endWidth = .02f;

        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = startColor;
        line.endColor = endColor;

        line.positionCount = route.Length;
        line.SetPositions(route);
    }

}
