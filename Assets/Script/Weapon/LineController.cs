using UnityEngine;

public class LineController : MonoBehaviour
{
    LineRenderer lr;
    Transform[] points;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }
    
    public void SetUpLine(Transform[] points)
    {
        lr.positionCount = points.Length;
        this.points = points;
    }

    private void Update()
    {
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] != null)
            {
                lr.SetPosition(i, points[i].position);
            }
        }
    }
}


