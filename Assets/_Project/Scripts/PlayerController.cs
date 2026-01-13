using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float turnSpeed = 10f;
    public Animator anim;
    public Transform model;

    CharacterController cc;

    void Start() => cc = GetComponent<CharacterController>();

    void Update()
    {
        Vector3 m = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        cc.Move(m * speed * Time.deltaTime);
        anim.SetFloat("Speed", m.magnitude);

        if (m.sqrMagnitude > 0)
        {
            Quaternion t = Quaternion.LookRotation(m);
            model.localRotation = Quaternion.Slerp(model.localRotation, t, turnSpeed * Time.deltaTime);
        }
    }
}