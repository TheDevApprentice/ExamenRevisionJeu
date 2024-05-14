using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float rotationSpeed = 10f;
    public float forwardForce = 50f;
    public GameObject smokeParticlesPrefab;

    private Rigidbody rb;
    private Transform helice;
    private bool isJumping = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        helice = transform.Find("Helice");
    }

    void Update()
    {
        // Lorsqu’on appuie sur `Jump`
        if (Input.GetButtonDown("Jump"))
        {
            isJumping = true;
        }

        // Lorsqu’on relâche `Jump`
        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }

        // Si Jump est enfoncé
        if (isJumping)
        {
            // Tourner l'hélice à un rythme proportionnel à l’input
            float input = Input.GetAxis("Jump");
            float heliceRotation = input * rotationSpeed * Time.deltaTime;
            helice.Rotate(0, 0, heliceRotation);

            // Appliquer une force vers l'avant de l’avion
            rb.AddForce((transform.forward * -1) * forwardForce);
        }

        // Contrôles de l'avion
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Orienter l’avion vers le haut ou le bas
        transform.Rotate(Vector3.right, -verticalInput * rotationSpeed * Time.deltaTime);

        // Pencher l’avion vers la gauche ou la droite
        transform.Rotate(Vector3.forward, -horizontalInput * rotationSpeed * Time.deltaTime);
    }
}
