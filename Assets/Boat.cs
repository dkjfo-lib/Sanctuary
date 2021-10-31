using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
    public GroundDetector PlayerDetector;
    public GroundDetector WaterDetector;
    [Space]
    public Transform PropellerMountPoint;
    public Transform CenterOfMass;
    public float waterY = -3.5f;
    public float waterForce = 100;
    public float waterThickness = .5f;
    public Vector3[] waterAnchors;
    public float ff = .6f;
    public float fff = .25f;
    [Space]
    public float forceForward = 2000;
    public float forceRight = 500;
    public float torque = 300;
    public bool playerInside;
    [Range(0, 1)] public float speedMult = .99f;
    [Range(0, 1)] public float speedMultInAir = .9f;
    [Range(0, 1)] public float speedMultY = .99f;

    Rigidbody Rigidbody;

    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Rigidbody.centerOfMass = CenterOfMass.localPosition;
    }

    private void Update()
    {
        if (playerInside && !PlayerDetector.onGround)
        {
            DisebargPlayer();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            TogglePlayer();
        }
        //if (!playerInside && PlayerDetector.onGround)
        //{
        //    EmbargPlayer();
        //}
    }

    void FixedUpdate()
    {
        if (playerInside && WaterDetector.onGround)
        {
            var input = CreateInput();
            if (input.x != 0)
            {
                var movementInput = new Vector2(input.x, input.y);
                var localInputXZ = input.x * transform.forward * forceForward /*+ movementInput.y * transform.right * forceRight*/;
                var addMovementXZ = localInputXZ * Time.fixedDeltaTime;
                var newMovementXZ = Rigidbody.velocity + addMovementXZ;
                Rigidbody.velocity = newMovementXZ;
                //Rigidbody.AddForceAtPosition(newMovementXZ, PropellerMountPoint.position, ForceMode.Acceleration);
            }
            Rigidbody.AddTorque(new Vector3(0, input.y * torque * Time.fixedDeltaTime, 0), ForceMode.Acceleration);
            Rigidbody.velocity += new Vector3(Rigidbody.velocity.x * speedMult, Rigidbody.velocity.y * speedMultY, Rigidbody.velocity.z * speedMult);
        }
        else
        {
            Rigidbody.velocity = new Vector3(Rigidbody.velocity.x * speedMultInAir, Rigidbody.velocity.y * speedMultY, Rigidbody.velocity.z * speedMultInAir);
        }
    }

    void TogglePlayer()
    {
        if (playerInside)
            DisebargPlayer();
        else
            EmbargPlayer();
    }

    private void DisebargPlayer()
    {
        playerInside = false;
        PlayerSinglton.PlayerCanMove = true;
        PlayerSinglton.PlayerTransform.parent = transform.parent;
        PlayerSinglton.PlayerTransform.up = Vector3.up;
        PlayerSinglton.PlayerRigidbody.isKinematic = false;
        PlayerSinglton.PlayerRigidbody.useGravity = true;
        foreach (var collider in PlayerSinglton.PlayerColliders)
        {
            collider.enabled = true;
        }
        Rigidbody.centerOfMass = CenterOfMass.localPosition;
    }

    private void EmbargPlayer()
    {
        playerInside = true;
        PlayerSinglton.PlayerCanMove = false;
        PlayerSinglton.PlayerTransform.parent = transform;
        PlayerSinglton.PlayerTransform.up = transform.up;
        PlayerSinglton.PlayerRigidbody.isKinematic = true;
        PlayerSinglton.PlayerRigidbody.useGravity = false;
        foreach (var collider in PlayerSinglton.PlayerColliders)
        {
            collider.enabled = false;
        }
        Rigidbody.centerOfMass = CenterOfMass.localPosition;
    }

    private Vector4 CreateInput()
    {
        var input = Vector4.zero;
        if (Input.GetKey(KeyCode.W))
            input += new Vector4(1, 0, 0, 0);
        if (Input.GetKey(KeyCode.S))
            input -= new Vector4(1, 0, 0, 0);
        if (Input.GetKey(KeyCode.D))
            input += new Vector4(0, 1, 0, 0);
        if (Input.GetKey(KeyCode.A))
            input -= new Vector4(0, 1, 0, 0);
        return input;
    }

    private void OnDrawGizmos()
    {
        foreach (var anchor in waterAnchors)
        {
            var anchorPosition = anchor.x * transform.right + anchor.y * transform.up + anchor.z * transform.forward;
            Gizmos.DrawLine(CenterOfMass.position, CenterOfMass.position + anchorPosition);
        }
    }
}
