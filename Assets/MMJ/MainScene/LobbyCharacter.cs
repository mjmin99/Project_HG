using UnityEngine;

public class LobbyCharacter : MonoBehaviour
{
    private Vector3 target;

    private void Start()
    {
        PickRandomTarget();
    }

    private void Update()
    {
        Move();
    }

    void PickRandomTarget()
    {
        target = new Vector3(Random.Range(-3f, 3f), 0, Random.Range(-3f, 3f));
    }
    void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, 1f * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.1f)
            PickRandomTarget();
    }
}
