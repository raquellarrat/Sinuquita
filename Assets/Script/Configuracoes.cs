using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Configuracoes : MonoBehaviour
{
    [Header("Privados")]
    private int pontuacao = 0;   //Pontuação do Player 
    public ParticleSystem sistemaParticulas; // Use o tipo específico de ParticleSystem

    public AudioSource somAudioSource; //De onde sai o som
    public AudioClip somAudioClip; // Proprio Som

    public GameObject[] buracos;    //Lista das caçapas
    public GameObject[] esferas;    //Lista das bolas
    public List<Vector3> posicoesBuracos = new List<Vector3>();  //Lista de posição dos buracos
    List<GameObject> listaEsferas;  //Novo tipo de Lista das bolas

    public int Pontuacao { get { return pontuacao; }} //Função para repassar a pontuação externamente

    void Start(){ //Inicialização da variavel, e das posições dos buracos
        listaEsferas = new List<GameObject>(esferas);
        InicializarPosicoesBuracos();
    }

    void FixedUpdate(){ //Verifica a fisica de colisão de buraco com bola
        VerificarColisaoBolasBuracos();
    }

    void InicializarPosicoesBuracos(){  //Lista de 
        posicoesBuracos.Clear();
        foreach (GameObject buraco in buracos){
            if (buraco != null){
                posicoesBuracos.Add(buraco.transform.position);
            }
        }
    }

    void VerificarColisaoBolasBuracos(){
        List<GameObject> bolasRemover = new List<GameObject>();
        foreach (GameObject bola in listaEsferas){
            foreach (Vector3 posicaoBuraco in posicoesBuracos){
                //Para cada combinação de esfera e bola, verifica se a colisão delas e menor que a distanciaMinima
                float distancia = Vector3.Distance(bola.transform.position, posicaoBuraco);
                if (distancia < 1.5f){
                    //Caso seja verdadeiro, remove a bola da lista, da fisica, incrementa pontuação e ativa particulas
                    bolasRemover.Add(bola);
                    bola.SetActive(false);
                    pontuacao++;
                    AtivarParticulas(posicaoBuraco);
                    somAudioSource.PlayOneShot(somAudioClip); // Substitua somAudioClip pelo seu AudioClip.

                }
            }
        }
        foreach (GameObject bolaRemover in bolasRemover){ //Assim que uma bola for removida, ela entra aqui
            listaEsferas.Remove(bolaRemover);
        }
    }

    void AtivarParticulas(Vector3 position){ //Aciona a particula, e gira o lançador para cima
        ParticleSystem particulas = Instantiate(sistemaParticulas, position, Quaternion.identity);
        particulas.transform.localRotation = Quaternion.Euler(-90f, 0.0f, 0.0f);
        Destroy(particulas.gameObject, sistemaParticulas.main.duration);
    }
}
