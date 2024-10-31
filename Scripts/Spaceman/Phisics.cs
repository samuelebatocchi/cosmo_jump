using UnityEngine;

/*
 * Questa classe ha lo scopo di:
 * 1) posizionare il personaggio nella corretta posizione di inizio
 * 2) gestire la fisica del movimento tramite una curva di Bezier
 * 3) gestire il comportamento del personaggio al click (si sposta alla parete opposta)
 */

public class Spaceman : MonoBehaviour
{
    Camera cam;
    int spazioDiMovimentoX;
    int posizioneY;
    public float duration; // Durata del movimento
    private float elapsedTime = 0f; // Tempo trascorso
    public float endX;
    public float startX;

    void Start()
    {
        cam = Camera.main;
        spazioDiMovimentoX = Screen.currentResolution.width;    //trovo la risoluzione in pixel dello schermo
        posizioneY = Screen.currentResolution.height / 4;       //trovo la risuluzione in pixel dell'altezza e la divido in 4 (voglio che il personaggio si trovi fisso ad un'altezza del 25% dello schermo)

        s = gameObject.transform;
        s.position = cam.ScreenToWorldPoint(new Vector3(GetSpazioMovimentoX().y, posizioneY, s.position.z));    //setto la posizione del personaggio passando dal dominio dei pixel a quello del WorldPoint
    }




    private Vector2 GetSpazioMovimentoX()       //scelgo entro quali x il personaggio può muoversi (tra il 20% e l'80%) 
    {
        int spazioMin = spazioDiMovimentoX / 5;
        int spazioMax = spazioDiMovimentoX - spazioMin;
        return new Vector2(spazioMin, spazioMax);
    }

    private bool isInPrimaMetà(Vector3 worldp)  //ritorna true se si trova nella parte sinistra dello schermo
    {
        
        Vector3 pixel = cam.WorldToScreenPoint(worldp);
        if (pixel.x >= spazioDiMovimentoX / 2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private bool isInSecondaMetà(Vector3 worldp)    //ritorna true se si trova nella parte destra dello schermo
    {

        Vector3 pixel = cam.WorldToScreenPoint(worldp);
        if (pixel.x < spazioDiMovimentoX / 2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    Transform s;    //dichiaro un oggetto Transform che si riferisce al personaggio
    bool start = false;     //inizializzo il valore booleano che mi dice se ho clickato sul display
    
    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)   //se ho clickato...
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began && !start)  //se ho clickato ma non è in corso lo spostamento del personaggio...
            {
                startX = cam.WorldToScreenPoint(s.position).x;  //inizializzo la posizione attuale come quella di inizio
                elapsedTime = 0f;
                duration = 0.2f;    //durata della transizione




                if (this.isInPrimaMetà(s.position))
                {
                    endX = GetSpazioMovimentoX().x;     //inizializzo la variabile di arrivo
                }

                else if (this.isInSecondaMetà(s.position))
                {
                    endX = GetSpazioMovimentoX().y;     //inizializzo la variabile di arrivo
                }
                start = true;   //il personaggio inizia a muoversi
            }
        }
        if (start)      //se il personaggio si muove...
        {
            elapsedTime += Time.deltaTime;  //quanto tempo passa dall'inizio dell'animazione
            float t = Mathf.Clamp01(elapsedTime / duration);    //definisce la t a cui mi trovo
            float easedT = EaseInSine(t);   

            Vector3 newP = cam.WorldToScreenPoint(s.position);  //creo newP e lo inizializzo uguale alla posizione del personaggio (in pixel)
            newP.x = Mathf.Lerp(startX, endX, easedT);  //assegno a newP il valore x che il personaggio deve avere a un istante t


            if (newP.x >= spazioDiMovimentoX / 2)   //se si trova nella metà di sinistra...
            {
                s.eulerAngles = new Vector3(s.eulerAngles.x, 0, s.eulerAngles.z);   //ruoto il personaggio a y = 0
            }
            else
            {
                s.eulerAngles = new Vector3(s.eulerAngles.x, 180, s.eulerAngles.z); //ruoto il personaggio a y = 180
            }
            s.position = cam.ScreenToWorldPoint(newP);



            // Ferma il movimento quando il tempo è trascorso
            if (elapsedTime >= duration)
            {
                start = false; // Ferma il movimento
            }
            
        }

    }     
        
        // Funzione di easing in sine
        float EaseInSine(float t)
        {
            return 1 - Mathf.Cos((t * Mathf.PI) / 2);
        }
    

}
