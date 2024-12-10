using UnityEngine;

public class MagicProjectile : MonoBehaviour
{
    public float damage; // Da�o que causa el proyectil
    public float speed = 10f; // Velocidad del proyectil
    public float lifespan = 5f; // Tiempo de vida del proyectil

    private Vector3 direction;

    void Start()
    {
        // Aqu� podr�as establecer la direcci�n del proyectil hacia el jugador
        direction = transform.forward;
        Destroy(gameObject, lifespan); // Destruir el proyectil despu�s de un tiempo
    }

    void Update()
    {
        // Mover el proyectil en la direcci�n indicada
        transform.Translate(direction * speed * Time.deltaTime);
    }

    // M�todo para establecer el da�o del proyectil
    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    // M�todo para detectar colisiones
    void OnCollisionEnter(Collision collision)
    {
        // Si el proyectil colisiona con el jugador, le causa da�o
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject); // Destruir el proyectil despu�s de impactar
        }
    }
}
