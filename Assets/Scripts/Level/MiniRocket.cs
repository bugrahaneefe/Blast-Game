using UnityEngine;

public class MiniRocket: MonoBehaviour
{
    [SerializeField] private Vector3 Direction;
    private float minSpeed = 15f;
    private float maxSpeed = 30f;
    private float lifeTime = 1f;

    private float elapsedTime = 0f;

    private void Start()
    {
        // destroy rocket after a lifetime
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // rocket smoothly get faster and move on its direction
        elapsedTime += Time.deltaTime;
        
        float t = Mathf.Clamp01(elapsedTime / lifeTime);
        float currentSpeed = Mathf.Lerp(minSpeed, maxSpeed, t);
        
        transform.Translate(Direction * currentSpeed * Time.deltaTime, Space.World);
    }
}
