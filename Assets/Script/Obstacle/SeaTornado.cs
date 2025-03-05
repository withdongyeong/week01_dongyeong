using System.Collections.Generic;
using UnityEngine;

public class SeaTornado : Attraction
{
    [SerializeField] Transform trail;
    [SerializeField] Transform trailPrefab;

    [SerializeField] float angle = 5f;
    [SerializeField] float intervalLine = 0.1f;

    [SerializeField] float lifeTime = 5f;

    private void Update()
    {
        //if (Vector3.Distance(trail.transform.position, transform.position) < 0.5f)
        //{
        //    Destroy(trail.gameObject);
        //    trail = Instantiate(trailPrefab, transform);
        //    transform.position = new Vector3(0, 0, 0);
        //    return;
        //}

        if (transform.position.z < 0.8f)
        {
            trail.transform.RotateAround(transform.position, Vector3.forward, angle);
            trail.position = Vector3.Slerp(trail.transform.position, transform.position, intervalLine * Time.deltaTime);

            transform.Translate(0, 0, 0.1f * Time.deltaTime);
        }
        else
            Disappear();
    }

    // 배를 회오리 중심으로 끌어당기는 방향 반환
    public override Vector3 GetAttractionDirection(ApplyAttractionObject applyObject)
    {
        // 배 -> 회오리 중심
        return (transform.position - applyObject.transform.position).normalized;
    }

    void Disappear()
    {
        if (isExistPlayer)
        {
            isExistPlayer = false;
            Debug.Log("토네이도가 플레이어 체력 감소시킴!");
        }
        Debug.Log("토네이도 소멸!");
        Destroy(gameObject);
    }

    // OnTrigger, OnCollision 추가 금지
}
