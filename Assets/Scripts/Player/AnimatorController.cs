using UnityEngine;
using Mirror;

public class AnimatorController : NetworkBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        _animator.SetFloat("yInput", Input.GetAxisRaw("Vertical"));
    }
}
