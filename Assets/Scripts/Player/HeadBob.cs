using UnityEngine;

public class HeadBob : MonoBehaviour
{
    private Player player;
    private Animator anim;

    private bool walking;
    private bool running;

    private void Start()
    {
        player = GetComponentInParent<Player>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        walking = player.walking;
        running = player.running;

        UpdateHeadBob();
    }

    private void UpdateHeadBob()
    {
        anim.SetBool("Walk", walking);
        anim.SetBool("Run", running);
    }
}
