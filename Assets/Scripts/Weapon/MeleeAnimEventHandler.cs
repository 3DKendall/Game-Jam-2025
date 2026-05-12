using UnityEngine;

public class MeleeAnimEventHandler : MonoBehaviour
{
    public void CheckForHit()
    {
        GetComponentInParent<Weapon>().CheckForHit();
    }

    public void ExcecuteHit()
    {
        GetComponentInParent<Weapon>().ExecuteHit();
    }
}
