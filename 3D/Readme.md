# Question 3 - 3D

[Question1_start.unitypackage](https://s3-us-west-2.amazonaws.com/secure.notion-static.com/c38e3bcb-d119-4702-b4a7-99de7c81cbf9/Question1_start.unitypackage)

1. Créez un plan dans la scène.
Créez le modèle d’avion ci-dessous.

![Untitled](https://s3-us-west-2.amazonaws.com/secure.notion-static.com/e067d295-b75d-43ca-aa79-b4b97f453463/Untitled.png)

1. Les pièces de l’avion doivent être organisées de manière à rester en 1 morceau.
2. Les couleurs doivent être respectées.
3. L’avion doit être affecté par la gravité.
4. Ajoutez une hélice à l’avion.
5. Lorsqu’on appuie sur `Jump`
    1. L’hélice tourne à un rythme proportionnel à la valeur de l’input.
    2. Une force est appliquée vers l’avant de l’avion.
6. L’axe vertical doit permettre d’orienter l’avion vers le haut ou le bas.
7. L’axe horizontal doit permettre de pencher l’avion vers la gauche ou la droite.
8. Il y a des particules derrière l’avion.
    1. Doit être émis seulement lorsqu’on appuie sur `Jump`.
    2. Doit ressembler à de la fumée.
        1. Gris semi-transparent avec une transition vers le blanc avec transparence complète.
    3. Les particules ne doivent pas se déplacer avec l’avion.
9. Faites un prefab de l’avion.

# Question 3 - 3D - Resultat

```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float rotationSpeed = 10f;
    public float forwardForce = 50f;
    public GameObject smokeParticlesPrefab;
    public Transform smokeParticlesParent; // Ajouter un transform pour le parent des particules

    private Rigidbody rb;
    private Transform helice;
    private bool isJumping = false;
    private GameObject smokeParticlesInstance; // Ajouter une référence à l'instance des particules

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

            if (smokeParticlesInstance == null)
            {
                smokeParticlesInstance = Instantiate(smokeParticlesPrefab, smokeParticlesParent); // Instancier les particules
            }

            smokeParticlesInstance.SetActive(true); // Activer l'émission de particules
        }

        // Lorsqu’on relâche `Jump`
        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
            smokeParticlesInstance.SetActive(false); // Désactiver l'émission de particules
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

```