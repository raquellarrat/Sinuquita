using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FisicaUnica : MonoBehaviour
{
    [Header("Valores")]
    private float desaceleracao = 1.25f; // Drag
    private float multi = 1.1f; // Drag
    private float limiteVelocidade = 0.02f; //Definição se esta parado ou não

    private Rigidbody rb;
    private Vector3 lastVelocity; // Att de velocidade

    //public AudioSource somAudioSource; //De onde sai o som
    //public AudioClip somAudioClip; // Proprio Som

    void Start(){
        rb = GetComponent<Rigidbody>();
        //somAudioSource = GetComponent<AudioSource>();
    }

    void Update(){
        // Em movimento, a bola deve perder aceleração, sem movimento, deve parar
        if (rb.velocity.magnitude > limiteVelocidade){
            Vector3 forcaDesaceleracao = -rb.velocity.normalized * desaceleracao;
            rb.AddForce(forcaDesaceleracao, ForceMode.Force);
        }
        else{
            rb.velocity = Vector3.zero;
        }
    }

    void FixedUpdate(){
        lastVelocity = rb.velocity;
    }

    void OnCollisionEnter(Collision collision){
        // Verifique se a colisão ocorreu com uma parede
        if (collision.gameObject.layer == LayerMask.NameToLayer("Parede")){
            Vector3 normal = collision.contacts[0].normal; //Normal da superfície
            lastVelocity = Vector3.Reflect(lastVelocity, normal); // Reflexão de velocidade
            rb.velocity = lastVelocity; //Atualização da velocidade
        }

        // Verifique se a colisão ocorreu com uma outra bola
        if (collision.gameObject.CompareTag("ball")){
            Rigidbody otherRb = collision.gameObject.GetComponent<Rigidbody>(); //Rg da outra esfera

            //Velocidade relativa de ambas
            Vector3 relativeVelocity = otherRb.velocity - rb.velocity;
            Vector3 relativePosition = otherRb.transform.position - transform.position;
            float relativeSpeed = Vector3.Dot(relativeVelocity, relativePosition.normalized);

            //Novos impulsos para cada uma
            Vector3 impulse = (2.0f * relativeSpeed / (rb.mass + otherRb.mass)) * relativePosition.normalized;
            impulse *= multi;

            //Aplica as novas velocidades
            rb.velocity += impulse * otherRb.mass / rb.mass;
            otherRb.velocity -= impulse * rb.mass / otherRb.mass;

            //Gera o som para elas
            //somAudioSource.transform.position = transform.position;
            //somAudioSource.PlayOneShot(somAudioClip); // Substitua somAudioClip pelo seu AudioClip.
        }
    }
}
