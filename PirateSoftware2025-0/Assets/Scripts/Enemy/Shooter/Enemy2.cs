using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    public GameObject player; // Referencia al jugador
    public float detectionRange = 15f; // Rango de detecci�n del jugador
    public float moveSpeed = 3f; // Velocidad de movimiento del enemigo
    public Transform weapon; // Referencia al arma

    public List<Transform> patrolPoints; // Puntos de patrullaje
    private int currentPatrolIndex = 0; // �ndice del punto de patrullaje actual

    public Material idleMaterial; // Material cuando el enemigo est� en reposo
    public Material alertMaterial; // Material cuando el enemigo est� persiguiendo
    private Renderer enemyRenderer; // Referencia al Renderer del enemigo

    private Vector3 initialPosition; // Posici�n inicial del enemigo
    private bool returningToStart = false; // Bandera para indicar si est� regresando al punto inicial

    private void Start()
    {
        player = GameObject.Find("Player");
        enemyRenderer = GetComponent<Renderer>();
        initialPosition = transform.position; // Guardar la posici�n inicial del enemigo
        enemyRenderer.material = idleMaterial; // Establecer el material inicial
    }

    private void Update()
    {
        // Detectar si el jugador est� en el rango de visi�n
        if (Vector3.Distance(transform.position, player.transform.position) <= detectionRange)
        {
            returningToStart = false;
            enemyRenderer.material = alertMaterial; // Cambiar al material de alerta

            // Mirar hacia el jugador solo en el eje Y
            Vector3 direction = (player.transform.position - transform.position).normalized;
            Vector3 lookDirection = new Vector3(direction.x, 0, direction.z);
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            // Mover hacia el jugador
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
        else if (returningToStart || patrolPoints.Count == 0)
        {
            ReturnToStart();
        }
        else if(patrolPoints != null && patrolPoints.Any(p => p != null))
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        // Si no hay puntos de patrullaje, salir
        if (patrolPoints.Count == 0) return;

        enemyRenderer.material = idleMaterial; // Cambiar al material de reposo

        // Obtener el punto de patrullaje actual
        Transform patrolPoint = patrolPoints[currentPatrolIndex];

        // Moverse hacia el punto de patrullaje
        Vector3 direction = (patrolPoint.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Mirar hacia el punto de patrullaje solo en el eje Y
        Vector3 lookDirection = new Vector3(direction.x, 0, direction.z);
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        // Comprobar si se ha llegado al punto de patrullaje
        if (Vector3.Distance(transform.position, patrolPoint.position) <= 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
        }
    }

    private void ReturnToStart()
    {
        enemyRenderer.material = idleMaterial; // Cambiar al material de reposo

        // Moverse hacia la posici�n inicial
        Vector3 direction = (initialPosition - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Mirar hacia la posici�n inicial solo en el eje Y
        Vector3 lookDirection = new Vector3(direction.x, 0, direction.z);
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        // Comprobar si se ha llegado a la posici�n inicial
        if (Vector3.Distance(transform.position, initialPosition) <= 0.5f)
        {
            returningToStart = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Dibujar el rango de detecci�n en la escena para visualizaci�n
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
