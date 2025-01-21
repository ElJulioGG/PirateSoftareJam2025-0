using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimation : MonoBehaviour
{
    public Transform player;
    public AnimationCurve transitionCurve;
    public float animationDuration = 2f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 playerPosition;
    private Quaternion playerRotation;

    void Start()
    {
        // Guarda la posici�n y rotaci�n inicial de la c�mara
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Guarda la posici�n y rotaci�n del jugador
        playerPosition = player.position + new Vector3(0, 1.5f, -3f); // Ajusta seg�n sea necesario
        playerRotation = Quaternion.LookRotation(player.position - playerPosition);

        // Inicia la corrutina para la animaci�n de la c�mara
        StartCoroutine(AnimateCamera());
    }

    IEnumerator AnimateCamera()
    {
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = transitionCurve.Evaluate(elapsedTime / animationDuration);

            // Interpola la posici�n y rotaci�n de la c�mara
            transform.position = Vector3.Lerp(initialPosition, playerPosition, t);
            transform.rotation = Quaternion.Slerp(initialRotation, playerRotation, t);

            yield return null;
        }

        // Aseg�rate de que la c�mara est� exactamente en la posici�n y rotaci�n final
        transform.position = playerPosition;
        transform.rotation = playerRotation;

        // Aqu� puedes agregar cualquier l�gica adicional que necesites cuando la c�mara termine la animaci�n
        Debug.Log("La c�mara ha terminado la animaci�n y est� en la vista del jugador.");
    }
}

