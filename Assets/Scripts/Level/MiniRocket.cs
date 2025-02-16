using UnityEngine;
using System.Collections;

public class MiniRocket : MonoBehaviour
{
    [SerializeField] private Vector3 Direction;
    private float minSpeed = 15f;
    private float maxSpeed = 30f;
    private float lifeTime = 1f;
    private float elapsedTime = 0f;
    
    private void Start()
    {
        // after a long time execute falling new items into emptied spaces
        StartCoroutine(DestroyAndTriggerFall());
    }

    // move logic of minirockets
    private void Update()
    {
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / lifeTime);
        float currentSpeed = Mathf.Lerp(minSpeed, maxSpeed, t);
        
        transform.Translate(Direction * currentSpeed * Time.deltaTime, Space.World);
    }

    // on box collider triggered - take damage to that item
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<MiniRocket>() != null)
        {
            return;
        }

        Item item = other.GetComponent<Item>();
        if (item != null)
        {
            item.TakeDamage(DamageSource.Rocket);
        }
    }

    private IEnumerator DestroyAndTriggerFall()
    {
        yield return new WaitForSeconds(lifeTime);

        StartCoroutine(BoardManager.Instance.FallExistingItems());
        BoardManager.Instance.CheckGoalsAndMoves();
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
}
