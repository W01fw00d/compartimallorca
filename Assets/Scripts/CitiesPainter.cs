using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CitiesPainter : MonoBehaviour {

    public GameObject city_a;
    public GameObject city_b;

    public GameObject city_c;
    public GameObject city_d;

    public LineRenderer complexRouteLine;
    public LineRenderer simpleRouteLine;

    private Transform begin;
    private Transform end;

    
    private Color complexRouteStartColor = new Color(1, 0.3372549f, 0, 1); //orange
    private Color complexRouteEndColor = new Color(1, 0, 0, 1); //red

    //private Color complexRouteStartColor = new Color(0, 0.08235294f, 0.6705883f, 1); //blue
    //private Color complexRouteEndColor = new Color(0, 0.5294118f, 0.01176471f, 1); //green

        
    private Color simpleRouteStartColor = new Color(0.2745098f, 0.882353f, 0.8196079f, 1); //turquose
    private Color simpleRouteEndColor = new Color(0.03137255f, 0.5254902f, 0.04313726f, 1); //green
    //private Color simpleRouteStartColor = new Color(1, 0, 0.9130435f, 1); //pink
    //private Color simpleRouteEndColor = new Color(1, 0.3377618f, 0, 1); //orange

    // Use this for initialization
    void Start()
    {
        DrawLine(complexRouteLine, city_a, city_b, complexRouteStartColor, complexRouteEndColor); //debug
        DrawLine(simpleRouteLine, city_c, city_d, simpleRouteStartColor, simpleRouteEndColor);
    }

    private void DrawLine(LineRenderer line, GameObject origin, GameObject destination, Color startColor, Color endColor)
    {
        begin = origin.transform;
        end = destination.transform;
        line.SetPosition(0, begin.position);
        //line.sortingOrder = 3;
        //line.sortingLayerName = "Default";
        line.SetPosition(1, end.position);

        line.startWidth = .2f;
        line.endWidth = .02f;

        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = startColor;
        line.endColor = endColor;
        line.positionCount = 2;

    }

}
