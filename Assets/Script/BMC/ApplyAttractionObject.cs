using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 끌어당겨지는 오브젝트
/// </summary>
public class ApplyAttractionObject : MonoBehaviour
{
    Rigidbody2D _rb;
    public List<Attraction> attractionList;
    public float AttractionForce = 100f;
    public float F = 9.81f;

    // 끌어당기는 방향
    public Vector2 AttractionDirection
    {
        get { return GetAttractionForce(); }
    }

    /// <summary>
    /// 끌어당기는 힘
    /// </summary>
    /// <returns></returns>
    Vector2 GetAttractionForce()
    {
        if (attractionList == null || attractionList.Count == 0) return Vector2.zero;

        // 중력의 총합이 0이 되어 표류하지 않도록 하기 위함
        // 다른 행성에 있는데, 중력이 더 큰 행성에 끌려가지 않기 위함
        // 이를 위한 가장 가까운 행성과의 거리, 힘, 방향
        float closetPlanet = 9999999f;
        float closetPlanetForce = 0.0f;
        Vector2 closetPlanetVector = Vector3.zero;

        float objectMass = _rb.mass;
        Vector2 totalForce = Vector2.zero;
        for (int i = 0; i < attractionList.Count; i++)
        {
            float pivotMass = attractionList[i].GetComponent<Rigidbody2D>().mass;                // 행성 질량
            Vector2 distVector = attractionList[i].transform.position - transform.position;      // 행성의 거리 벡터
            float dist = distVector.magnitude;                                                   // 행성과의 거리
            float force = F * pivotMass * objectMass / Mathf.Pow(dist, 2);                       // 만유인력

            // 플레이어 전용
            if (closetPlanet > dist)
            {
                closetPlanet = dist;
                closetPlanetForce = force;
                closetPlanetVector = distVector;
            }

            totalForce += distVector.normalized * force;                                           // 타겟에 적용되는 모든 힘 더하기
        }

        return totalForce.normalized;
    }

    void Awake()
    {
        _rb = transform.GetComponent<Rigidbody2D>();
        attractionList = new List<Attraction>();
    }

    void FixedUpdate()
    {
        // 힘을 받는 방향으로 힘을 가함
        _rb.AddForce(AttractionDirection * (AttractionForce * Time.fixedDeltaTime), ForceMode2D.Impulse);
        StandOnGround();
    }

    public void AddGravityField(Attraction gravityField)
    {
        attractionList.Add(gravityField);
    }

    public void RemoveGravityField(Attraction gravityField)
    {
        attractionList.Remove(gravityField);
    }

    void OnDrawGizmos()
    {
        if (AttractionDirection == Vector2.zero)
            return;

        Debug.DrawRay(transform.position, AttractionDirection * 5f, Color.red);
    }
    public void StandOnGround()
    {
        // 각도 구하기
        Vector2 dist = AttractionDirection - (Vector2)transform.up;
        float angle = Mathf.Atan2(dist.y, dist.x) * Mathf.Rad2Deg;

        // 물체 회전
        transform.rotation = Quaternion.Euler(0f, 0f, angle + 90); // z축 회전
    }

    public bool IsInField()
    {
        return (attractionList.Count > 0) ? true : false;
    }
}
