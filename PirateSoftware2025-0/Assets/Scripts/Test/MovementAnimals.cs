using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAnimals : MonoBehaviour
{
    public string animalName;
    public float speed;
    public float changeDirectionInterval = 2.0f; // Intervalo de tiempo para cambiar la direcci�n
    private Vector3 direction;
    private float timeSinceChange = 0f;

    void Start()
    {
        ChangeDirection();
    }

    void Update()
    {
        Move(Time.deltaTime);
    }

    public virtual void Move(float time)
    {
        // Incrementa el tiempo desde el �ltimo cambio de direcci�n
        timeSinceChange += time;

        // Cambia la direcci�n si ha pasado el intervalo
        if (timeSinceChange >= changeDirectionInterval)
        {
            ChangeDirection();
            timeSinceChange = 0f;
        }

        // Mueve al animal en la direcci�n actual
        float distance = speed * time;
        transform.Translate(direction * distance, Space.World);
        Debug.Log(animalName + " moved " + distance + " units.");
    }

    private void ChangeDirection()
    {
        // Genera una direcci�n aleatoria en el plano XZ
        float x = Random.Range(-1f, 1f);
        float z = Random.Range(-1f, 1f);
        direction = new Vector3(x, 0, z).normalized; // Normaliza el vector para tener magnitud 1, y mantiene y en 0

        // Rotar el animal para que enfrente la nueva direcci�n
        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(-direction, Vector3.up);
            transform.rotation = toRotation;
        }
    }
}