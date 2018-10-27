using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CitiesPainter : MonoBehaviour {

    public GameObject city_a;
    public GameObject city_b;

    private LineRenderer line;
    private Transform begin;
    private Transform end;

    private Color startColor = new Color(0, 0.08235294f, 0.6705883f, 1); //blue
    private Color endColor = new Color(0, 0.5294118f, 0.01176471f, 1); //green

    private readonly float lineWidth = .05f;

    // Use this for initialization
    void Start()
    {
        DrawLine(); //debug
    }

    private void DrawLine()
    {
        begin = city_a.transform;
        end = city_b.transform;

        line = GetComponent<LineRenderer>();
        line.SetPosition(0, begin.position);
        line.startWidth = lineWidth;
        line.sortingOrder = 3;
        line.sortingLayerName = "Default";
        line.SetPosition(1, end.position);
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = startColor;
        line.endColor = endColor;
        line.positionCount = 2;
    }

}
