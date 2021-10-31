using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IMovement
{
    public float speed = 200;
    [Range(0, 1)] public float speedMult = .7f;
    [Space]
    public float torque = 300;
    [Space]
    [Range(0, 1)] public float speedMultInAir = .98f;
    [Range(0, 1)] public float controlInAir = .2f;
    public float jumpForce = 200;
    [Space]
    public Pipe_SoundsPlay Addon_Pipe_SoundsPlay;
    public ClipsCollection Addon_stepSound;
    [Space]
    public GroundDetector Addon_WaterDetector;
    [Range(0, 1)] public float Addon_speedMultInWater = .98f;
    [Range(0, 1)] public float Addon_controlInWater = .2f;
    [Range(0, 1)] public float Addon_jumpForceInWater = .2f;

    Rigidbody Rigidbody;
    GroundDetector GroundDetector;

    bool wasInAir = false;
    Vector4 input;
    public Vector3 CurrentInput => input;

    private void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Rigidbody.centerOfMass = Vector3.zero;
        GroundDetector = GetComponentInChildren<GroundDetector>();
        StartCoroutine(SoundWalking());
    }

    IEnumerator SoundWalking()
    {
        while (true)
        {
            yield return new WaitUntil(() => GroundDetector.onGround && Rigidbody.velocity.sqrMagnitude > 4f);
            if (Addon_Pipe_SoundsPlay != null && Addon_stepSound != null)
                Addon_Pipe_SoundsPlay.AddClip(new PlayClipData(Addon_stepSound, Camera.main.transform.position, Camera.main.transform));
            yield return new WaitForSeconds(.25f);
        }
    }

    void Update()
    {
        if (!PlayerSinglton.PlayerCanMove) return;

        if (wasInAir && GroundDetector.onGround)
        {
            Rigidbody.velocity = new Vector3(Rigidbody.velocity.x, 0, Rigidbody.velocity.z);
        }
        wasInAir = !GroundDetector.onGround;
    }

    void FixedUpdate()
    {
        if (!PlayerSinglton.PlayerCanMove) return;

        var input = CreateInput();

        if (GroundDetector.onGround)
        {
            // move xz
            if (input.x != 0 || input.z != 0)
            {
                var inputXZ = new Vector3(input.x, 0, input.z).normalized;
                var localInputXZ = inputXZ.x * transform.right + inputXZ.z * transform.forward;
                var addMovementXZ = localInputXZ * speed * Time.fixedDeltaTime;
                var newMovementXZ = Rigidbody.velocity + addMovementXZ;
                Rigidbody.velocity = newMovementXZ;
            }
            Rigidbody.velocity = new Vector3(Rigidbody.velocity.x * speedMult, Rigidbody.velocity.y, Rigidbody.velocity.z * speedMult);
            // rotate
            //if (input.w != 0)
            //{
            //    transform.Rotate(Vector3.up, input.w * torque * Time.fixedDeltaTime);
            //    //rb.AddTorque(new Vector3(0, input.w * torque * Time.fixedDeltaTime, 0), ForceMode.Acceleration);
            //}
            // move y
            if (input.y != 0)
            {
                var movementY = input.y * jumpForce;
                Rigidbody.velocity = new Vector3(Rigidbody.velocity.x, movementY, Rigidbody.velocity.z);
            }
        }
        else if (Addon_WaterDetector != null && Addon_WaterDetector.onGround)
        {
            // move xz
            if (input.x != 0 || input.z != 0)
            {
                var inputXZ = new Vector3(input.x, 0, input.z).normalized;
                var localInputXZ = inputXZ.x * transform.right + inputXZ.z * transform.forward;
                var addMovementXZ = localInputXZ * speed * Time.fixedDeltaTime * Addon_controlInWater;
                var newMovementXZ = Rigidbody.velocity + addMovementXZ;
                Rigidbody.velocity = newMovementXZ;
            }
            Rigidbody.velocity = new Vector3(Rigidbody.velocity.x * Addon_speedMultInWater, Rigidbody.velocity.y, Rigidbody.velocity.z * Addon_speedMultInWater);
            // move y
            if (input.y != 0)
            {
                var movementY = input.y * jumpForce * Addon_jumpForceInWater;
                Rigidbody.velocity = new Vector3(Rigidbody.velocity.x, movementY, Rigidbody.velocity.z);
            }
        }
        else
        {
            // move xz
            if (input.x != 0 || input.z != 0)
            {
                var inputXZ = new Vector3(input.x, 0, input.z).normalized;
                var localInputXZ = inputXZ.x * transform.right + inputXZ.z * transform.forward;
                var addMovementXZ = localInputXZ * speed * Time.fixedDeltaTime * controlInAir;
                var newMovementXZ = Rigidbody.velocity + addMovementXZ;
                Rigidbody.velocity = newMovementXZ;
            }
            Rigidbody.velocity = new Vector3(Rigidbody.velocity.x * speedMultInAir, Rigidbody.velocity.y, Rigidbody.velocity.z * speedMultInAir);
        }
    }

    private Vector4 CreateInput()
    {
        input = Vector4.zero;
        if (Input.GetKey(KeyCode.A))
            input -= new Vector4(1, 0, 0, 0);
        if (Input.GetKey(KeyCode.D))
            input += new Vector4(1, 0, 0, 0);
        if (Input.GetKey(KeyCode.W))
            input += new Vector4(0, 0, 1, 0);
        if (Input.GetKey(KeyCode.S))
            input -= new Vector4(0, 0, 1, 0);
        if (Input.GetKey(KeyCode.Q))
            input -= new Vector4(0, 0, 0, 1);
        if (Input.GetKey(KeyCode.E))
            input += new Vector4(0, 0, 0, 1);
        if (Input.GetKey(KeyCode.Space))
            input += new Vector4(0, 1, 0, 0);
        return input;
    }
}

public interface IMovement
{
    Vector3 CurrentInput { get; }
}