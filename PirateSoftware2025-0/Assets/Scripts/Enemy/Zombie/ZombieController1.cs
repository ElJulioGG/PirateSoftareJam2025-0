using UnityEngine;

public class ZombieControlle : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 15f;
    public float attackRange = 2f;
    public float moveSpeed = 2f;
    public float attackCooldown = 4.5f;
    public AudioClip attackSound;
    //
    //public Material attackMaterial;
    //public Material defaultMaterial;
    public float damageAmount = 10f;//da�o del zombie


    private Animator animator;
    private AudioSource audioSource;
    //private Renderer renderer;
    private float lastAttackTime;
    private bool isDead = false;
    private PlayerHealth playerHealth;//referencia al jugador wa

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        //renderer = GetComponent<Renderer>();

       
        //renderer.material = defaultMaterial;
        player = GameObject.Find("Player").transform;
        playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth script not found on player.");
        }
        lastAttackTime = -attackCooldown;
    }

    void Update()
    {
        if (isDead) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            ChasePlayer();
        }
        else
        {
            Idle();
        }
    }

    void ChasePlayer()
    {
        //Debug.Log("Cambiando a estado: Perseguir");
        animator.SetBool("isWalking", true);
        animator.SetBool("isAttacking", false);

        //.material = defaultMaterial;

        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
    }

    void AttackPlayer()
    {
        //Debug.Log("Cambiando a estado: Atacar");
        animator.SetBool("isWalking", false);

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            //animator.SetBool("isAttacking", true);
            lastAttackTime = Time.time;
            PlayAttackSound();
            //ChangeColorToAttack();
            // Aqu� puedes a�adir la l�gica para causar da�o al jugador.
            if (player.tag == "Player" && playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }
        

        else
        {
            animator.SetBool("isAttacking", false);
        }
    }

    void Idle()
    {
        //Debug.Log("Cambiando a estado: Idle");
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", false);

        //renderer.material = defaultMaterial;
    }

    public void Die()
    {
        Debug.Log("Cambiando a estado: Muerte");
        isDead = true;
        animator.SetTrigger("Die");
        // Aqu� puedes a�adir cualquier otra l�gica necesaria cuando el zombie muere.
        Destroy(gameObject, 5f); // Destruye el zombie despu�s de 5 segundos.
    }

    void PlayAttackSound()
    {
        if (attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }



    // Funci�n para dibujar el rango de detecci�n en la escena
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
