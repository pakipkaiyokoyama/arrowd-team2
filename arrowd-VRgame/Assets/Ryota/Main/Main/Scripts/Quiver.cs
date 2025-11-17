using UnityEngine;

public class Quiver : MonoBehaviour
{
    public enum ArrowType { Move, Attack }
    public ArrowType arrowType;

    private void OnTriggerEnter(Collider other)
    {
        BowController bow = other.GetComponentInParent<BowController>();
        if (bow != null)
        {
            bow.SetArrowMode(arrowType == ArrowType.Move ?
                             BowController.ArrowMode.Move :
                             BowController.ArrowMode.Attack);
        }
    }
}
