using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] Text raycastResultText1; // Turno
    [SerializeField] Text raycastResultText2; // Estado
    [SerializeField] Text raycastResultText3; // PontPlayer
    [SerializeField] Text raycastResultText4; // PontNPC
    public Configuracoes configuracoes; // Passagem para Pontuação
    public GerenciadorDeCenas geren; // Passagem para NPC
    [SerializeField] Transform orientation; //Direção da Camera
    public AudioSource somAudioSource; //De onde sai o som
    public AudioClip somAudioClip; // Proprio Som

    [Header("Valores")]
    [SerializeField] float forçaTacada = 100f; //Força de impulso
    [SerializeField] float tempoMax = 2f; //Carregamento
    [SerializeField] float threshold = 0.5f; //Para ver se esta parado
    [SerializeField] float limiteAltura = 1.02f; //Para travar o Y ou nao
    public bool npc;
    public bool NPC { get { return npc; }}

    [Header("Botoes")]
    public KeyCode shootKey = KeyCode.Space;

    [Header("Private")]
    private float tempoPressionado = 0f; //Tempo apertando espaço
    private Rigidbody rb; //Rigidbody

    //Oponente
    private int numeroRaios = 36;
    private float intervaloAngular = 10f;
    private float comprimentoRaio = 30f;
    private Vector3 direcaoAI;
    private bool aiDirectionRequested = false;
    private int rodada = 1;
    private int Pont;

    private int pontPlayer = 0;
    private int pontNPC = 0;
    public int PontPlayer { get { return pontPlayer; }}
    public int PontNPC { get { return pontNPC; }}

    //Sequencia de Estados
    private MovementState state;
    private enum MovementState
    {
        carregando, 
        parado, 
        movimento,
        oponente 
    }

    void Start(){   //Defini o rigidbody e Estado Parado
        rb = GetComponent<Rigidbody>();
        somAudioSource = GetComponent<AudioSource>();
        state = MovementState.parado;
        Pont = configuracoes.Pontuacao;
    }

    void Update()
    {
        Debug.DrawRay(transform.position, orientation.forward * 10, Color.red);
        Debug.Log("NPC: " + npc.ToString());
        UpdateUI();
        Tacada();
        UpdateState(); 
        pontosCorridos();
        if (Input.GetKeyDown(KeyCode.J) && !aiDirectionRequested)
        {
            aiDirectionRequested = true;
            StartCoroutine(AIdirectionWithDelay());
        }

        if (Input.GetKeyUp(KeyCode.J))
        {
            aiDirectionRequested = false;
        }
    }

    IEnumerator AIdirectionWithDelay() //Responsavel por dar direção e Tacada do NPC
    {
        Vector3 melhorDirecao = Vector3.zero; // Inicializa com uma direção nula
        float menorDistancia = float.MaxValue; // Inicializa com um valor grande
        for (int i = 0; i < numeroRaios; i++)
        {
            float anguloAtual = i * intervaloAngular;
            Vector3 direcaoRaio = Quaternion.Euler(0f, anguloAtual, 0f) * orientation.forward;
            Ray raio = new Ray(transform.position, direcaoRaio);
            RaycastHit hit;

            if (Physics.Raycast(raio, out hit, comprimentoRaio) && hit.collider.CompareTag("ball"))
            {   
                if (hit.distance < menorDistancia)
                {
                    // Atualiza a melhor direção e a menor distância
                    melhorDirecao = direcaoRaio;
                    menorDistancia = hit.distance;
                }
            }
            if (melhorDirecao != Vector3.zero)
            {
                Debug.DrawRay(transform.position, direcaoRaio * hit.distance, Color.green, 1f);
                int numeroAleatorio = Random.Range(50, 101);
                yield return new WaitForSeconds(2f); // Pausa de 2 segundos
                rb.AddForce(melhorDirecao * Random.Range(50, 101), ForceMode.Impulse);
                //Gera o som para elas
                somAudioSource.transform.position = transform.position;
                somAudioSource.PlayOneShot(somAudioClip); // Substitua somAudioClip pelo seu AudioClip.
                aiDirectionRequested = false;
                break;
            }

        }
    }

    void pontosCorridos(){
        if(npc == false)
        {
            pontPlayer = Pont;
        }
        else
        {
            if(Pont != configuracoes.Pontuacao){
                if(rodada % 2 == 1)
                    pontNPC += 1;
                else
                    pontPlayer += 1;
            }
        }
        Pont = configuracoes.Pontuacao;
    }

    bool MesaParada(){
        bool bolaEmMovimento = true;
        GameObject[] bolas = GameObject.FindGameObjectsWithTag("ball");
        foreach (GameObject bola in bolas)
        {
            Rigidbody bolaRB = bola.GetComponent<Rigidbody>();
            if (bolaRB != null && bolaRB.velocity.magnitude > 0.0f)
            {
                // Pelo menos uma bola está se movendo
                bolaEmMovimento = false;
                break;
            }
        }
        return bolaEmMovimento;
    }


    void UpdateState()  //Função responsavel por alterar os estados
    {
        
        //Condição total de estar parado, sempre verificada primeiro
        if(MesaParada() && state != MovementState.oponente && transform.position.y < limiteAltura && 
            (rb.velocity.magnitude < threshold && state != MovementState.carregando)) //Se ele estava se movendo e parou
        { 
            transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
            rb.constraints = RigidbodyConstraints.FreezePositionY;
            rb.velocity = Vector3.zero;
            if(state != MovementState.parado){
                rodada += 1;
            }
            state = MovementState.parado;
        } 

        if(npc == true && rodada % 2 == 1 && state == MovementState.parado){ //Vez do Oponente
            state = MovementState.oponente;
            StartCoroutine(AIdirectionWithDelay());
        }

        //Fora do Pulo, condições para carregar o tiro
        else if (Input.GetKeyDown(shootKey) && rb.velocity.magnitude < threshold && state != MovementState.oponente){
            state = MovementState.carregando;
            tempoPressionado = Time.time;
        }
        //Fora do Pulo e do Carregando, condições para movimento
        else if (rb.velocity.magnitude > threshold){
            state = MovementState.movimento;
        }

    }

    void Tacada(){  //Função para dar a tacada
        if (Input.GetKeyUp(KeyCode.Space) &&  state == MovementState.carregando){
            float chargeDuration = Time.time - tempoPressionado;
            float force = Mathf.Clamp(chargeDuration, 0f, tempoMax) * forçaTacada;
            rb.AddForce(orientation.forward * force, ForceMode.Impulse);

            //Gera o som para elas
            somAudioSource.transform.position = transform.position;
            somAudioSource.PlayOneShot(somAudioClip); // Substitua somAudioClip pelo seu AudioClip.
        }
    }


    void UpdateUI(){    //Mostrar tanto a pontuação atual, quanto o estado do player
        raycastResultText1.text = "Turno: " + rodada.ToString();
        raycastResultText2.text = "Estado: " + state.ToString();
        raycastResultText3.text = "Player: " + pontPlayer.ToString();
        raycastResultText4.text = "NPC: " + pontNPC.ToString();
    }
}
