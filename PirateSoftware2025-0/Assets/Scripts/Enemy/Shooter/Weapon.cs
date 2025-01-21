using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject player; // Referencia al jugador
    public GameObject projectilePrefab; // Prefab del proyectil
    public Transform firePoint; // Punto desde donde se dispara
    public float fireRate = 1.5f; // Tiempo entre disparos
    public int velocityproyectile;
    private float nextFireTime; // Tiempo para el pr�ximo disparo
    private void Start()
    {
        player = GameObject.Find("Player");
    }
    private void Update()
    {
        // Calcular la direcci�n hacia el jugador
        Vector3 direction = (player.transform.position - firePoint.position).normalized;

        // Rotar el punto de disparo hacia la direcci�n del jugador
        firePoint.LookAt(player.transform.position);

        // Disparar al jugador
        if (Vector3.Distance(transform.position, player.transform.position) <= GetComponentInParent<Enemy2>().detectionRange &&
            Time.time >= nextFireTime)
        {
            Shoot(direction);
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Shoot(Vector3 direction)
    {
        // Instanciar el proyectil y ajustar su direcci�n
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.velocity = direction * velocityproyectile; // Velocidad del proyectil
    }
}
