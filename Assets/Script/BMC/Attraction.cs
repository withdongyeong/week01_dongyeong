using UnityEngine;

[RequireComponent(typeof(Collider2D))] // Collider 컴포넌트 없으면 자동으로 추가
public abstract class Attraction : MonoBehaviour
{
    CircleCollider2D _collider;
    protected bool isExistPlayer = false; 

    void Start()
    {
        // 영역 안에 들어왔을 때 감지하기 위함
        _collider = GetComponent<CircleCollider2D>();
        _collider.isTrigger = true;
    }

    /// <summary>
    /// 끌어당기는 방향
    /// </summary>
    /// <param name="attractableObject"></param>
    /// <returns></returns>
    public abstract Vector3 GetAttractionDirection(ApplyAttractionObject attractableObject);

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out ApplyAttractionObject gravityObject))
        {
            isExistPlayer = true;
            gravityObject.AddGravityField(this);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out ApplyAttractionObject gravityObject))
        {
            gravityObject.RemoveGravityField(this);
            isExistPlayer = false;
        }
    }

    void OnDrawGizmos()
    {
        if (_collider != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _collider.radius);
        }
    }
}
