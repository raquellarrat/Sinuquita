using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class GerenciadorDeCenas : MonoBehaviour
{
    private GameObject playerObj;
    private bool valorNpc;
    private bool NPC;
    public bool ValorNpc { get { return valorNpc; }} 
    public Controller controller;
    private int PontPlayer;
    private int PontNPC;


    void Update()
    {
        // Verifica se a tecla "Space" foi pressionada na primeira cena
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                valorNpc = false;
                CarregarCenaSinuca();
            }
            else if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                valorNpc = true;
                CarregarCenaSinuca();
            }
        }
        // Verifica se a tecla "K" foi pressionada na cena da sinuca
        if (SceneManager.GetActiveScene().buildIndex == 1){
            PontPlayer = controller.PontPlayer;
            PontNPC = controller.PontNPC;
            NPC = controller.NPC;
            Debug.Log(NPC);
             if(Input.GetKeyDown(KeyCode.K)){
                CarregarCenaInicioComDelay();
            }
            if(NPC == false && PontPlayer == 15){
                valorNpc = false;
                StartCoroutine(CarregarCenaComDelay());
            }
            if(NPC == true && (PontPlayer >= 8 || PontNPC >= 8)){
                valorNpc = true;
                StartCoroutine(CarregarCenaComDelay());
            }
        }
    }

    void CarregarCenaSinuca()
    {
        // Registra o evento SceneManager.sceneLoaded chamando o método interno CenaCarregadaComParametro
        SceneManager.sceneLoaded += CenaCarregadaComParametro;

        // Carrega a segunda cena (índice 1)
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    void CarregarCenaInicioComDelay()
    {
        // Registra o evento SceneManager.sceneLoaded chamando o método interno CenaCarregadaComParametro
        SceneManager.sceneLoaded += CenaCarregadaComParametro;

        // Carrega a primeira cena (índice 0)
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }


    // Método interno para lidar com o SceneManager.sceneLoaded e repassar o parâmetro adicional
    private void CenaCarregadaComParametro(Scene cena, LoadSceneMode modo)
    {
        CenaCarregada(cena, modo, valorNpc);

        // Remove o evento para evitar chamadas adicionais
        SceneManager.sceneLoaded -= CenaCarregadaComParametro;
    }

    // Método original CenaCarregada que agora aceita um parâmetro adicional
    void CenaCarregada(Scene cena, LoadSceneMode modo, bool valorNpc)
    {
        // Garante que o código é executado apenas após a cena da sinuca ser carregada
        if (cena.buildIndex == 1)
        {
            // Busca o objeto chamado "Player" na hierarquia da cena carregada
            playerObj = GameObject.Find("Player");

            // Verifica se o objeto foi encontrado
            if (playerObj != null)
            {
                // Obtém o componente Controller no objeto encontrado
                Controller controller = playerObj.GetComponent<Controller>();

                // Verifica se o componente Controller foi encontrado
                if (controller != null)
                {
                    // Altera a variável npc
                    controller.npc = valorNpc;
                }
                else
                {
                    //Debug.LogWarning("Componente Controller não encontrado no objeto do jogador.");
                }
            }
            else
            {
                Debug.LogWarning("Objeto do jogador não encontrado na cena.");
                PrintNamesOfAllObjectsInScene();
            }
        }
    }

    void PrintNamesOfAllObjectsInScene()
    {
        Debug.Log("Nomes de todos os objetos na cena:");
        GameObject[] allObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject obj in allObjects)
        {
            Debug.Log(obj.name);
        }
    }

    IEnumerator CarregarCenaComDelay()
    {
        // Aguarda o tempo especificado pelo delay
        yield return new WaitForSeconds(3f);
        // Recarrega a cena atual
        CarregarCenaSinuca();
    }
}
