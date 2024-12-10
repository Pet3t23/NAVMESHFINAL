using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Necesario para utilizar la clase Image

public class Personaje3D : MonoBehaviour
{
    public float TransLayers;

    public Rigidbody rd;
    public float speed;
    public Animator ani;
    public Transform Eje;

    public bool inground;
    private RaycastHit hit;
    public float distance;
    public Vector3 v3;

    public GameObject camara;
    public GameObject arrow;
    public Vector3 v33;
    public GameObject arma;
    public RaycastHit hit2;
    private bool atacando;
    public static Personaje3D me; // Referencia est�tica al jugador

    public float HP_Min; // Salud actual
    public float HP_Max; // Salud m�xima
    public Image barra; // Barra de salud

    // Variables adicionales
    public float tiempoAtaque = 0.5f; // Tiempo entre ataques
    private float ataqueCooldown; // Temporizador para ataques

    // Start is called before the first frame update
    void Start()
    {
        // Inicializamos la referencia est�tica
        if (me == null)
        {
            me = this;
        }

        // Inicializaci�n de la salud
        HP_Min = HP_Max; // Asegura que la salud inicial es igual a la salud m�xima

        // Verifica que Rigidbody no sea nulo
        if (rd == null)
        {
            rd = GetComponent<Rigidbody>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Movimiento del personaje
        MoverPersonaje();

        // Actualiza la barra de salud
        if (barra != null)
        {
            barra.fillAmount = HP_Min / HP_Max; // Se actualiza el valor de la barra
        }

        // Control de ataque
        if (Input.GetMouseButtonDown(0) && ataqueCooldown <= 0)
        {
            Atacar();
            ataqueCooldown = tiempoAtaque; // Reinicia el temporizador
        }

        // Reducir cooldown de ataque
        if (ataqueCooldown > 0)
        {
            ataqueCooldown -= Time.deltaTime;
        }
    }

    // M�todo para mover al personaje
    private void MoverPersonaje()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direccion = new Vector3(horizontal, 0, vertical).normalized;

        if (direccion.magnitude >= 0.1f)
        {
            // Movimiento usando Rigidbody
            Vector3 movimiento = direccion * speed * Time.deltaTime;
            rd.MovePosition(transform.position + movimiento);

            // Rotaci�n hacia la direcci�n del movimiento
            float targetAngle = Mathf.Atan2(direccion.x, direccion.z) * Mathf.Rad2Deg + camara.transform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0, targetAngle, 0);

            // Animaci�n
            if (ani != null)
            {
                ani.SetBool("isRunning", true);
            }
        }
        else
        {
            if (ani != null)
            {
                ani.SetBool("isRunning", false);
            }
        }
    }

    // M�todo para atacar
    private void Atacar()
    {
        if (arma != null && !atacando)
        {
            atacando = true;

            // Ejemplo de animaci�n de ataque
            if (ani != null)
            {
                ani.SetTrigger("Attack");
            }

            // Ejemplo de detecci�n de golpe
            if (Physics.Raycast(arrow.transform.position, arrow.transform.forward, out hit2, distance))
            {
                Debug.Log("Golpeaste: " + hit2.collider.name);
                // Aqu� puedes manejar da�o al enemigo si tiene un script de salud
            }

            atacando = false;
        }
    }

    // M�todo para reducir la salud
    public void SetHP(float damage)
    {
        HP_Min -= damage;
        if (HP_Min < 0) HP_Min = 0; // Evita que la salud sea negativa

        if (HP_Min == 0)
        {
            Debug.Log("Jugador muerto");
            // Aqu� podr�as llamar a un m�todo para manejar la muerte del jugador
        }
    }

    // M�todo para curar al jugador
    public void Curar(float cantidad)
    {
        HP_Min += cantidad;
        if (HP_Min > HP_Max) HP_Min = HP_Max; // Evita que la salud supere el m�ximo
    }
}
