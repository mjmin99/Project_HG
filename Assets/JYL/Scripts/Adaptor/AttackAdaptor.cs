using UnityEngine;

public class AttackAdaptor : MonoBehaviour
{
    private EnemyController controller;
    public void Init(EnemyController inputController) => controller = inputController;
    public void Attack() => controller.Attack();
}
