using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class GdtWiringSystem : MonoBehaviour
{

    #region ABOUT

    /* Solution by GameDevTraum
    * Website: https://gamedevtraum.com/en/
    * Channel: https://youtube.com/c/GameDevTraum
    * Check my channel, it has subtitles for most of the videos. In my web you'll find more Solutions, Articles and Assets
    */

    /*
     * This solution is used to automatically generate wires or ropes that hang from two or more points.
     * To use it simply create an Empty GameObject in the hierarchy and add this Script to it, or you can use the prefab that comes inside the package.
     * Adding the Script will automatically create two empty objects as children of the GameObject that has this script, change the names and add a LineRenderer component, which is needed to draw the wire.
     * In the inspector you will be able to modify the thickness parameters, the amount of subdivisions, add a material for the cable and adjust the curvature as you like.
     * If the range of the sliders doesn't allow you to achieve what you want, you can modify it in this Script (the "Range" instructions).
     * To add a new point I recommend that you don't duplicate the children, as it produces errors, instead right click on the parent GameObject and then choose the "Create Empty" option.
     * When adding a new point a new slider for the curvature will appear.
     * The inspector's loopWire bool adds a wire that connects the last point to the first one.
     * Finally the isAnimatedWire bool serves to indicate that the cable should be updated from time to time, in case we want to animate it.
     * I hope you find it useful!
    */

    #endregion

    #region ACERCA

    /* Solución por GameDevTraum
    * Página: https://gamedevtraum.com/es/
    * Canal: https://youtube.com/c/GameDevTraum
    * Suscríbete al canal y visita la página para encontrar más Soluciones, Assets y Artículos
    */

    /*
     * Esta solución sirve para generar automáticamente cables o cuerdas que cuelgan desde dos o más puntos.
     * Para utilizarla simplemente crea un Empty GameObject en la jerarquía y añádele este Script, o puedes usar el prefabricado que viene en el paquete.
     * Al agregar el Script automáticamente se crearán dos objetos vacíos como hijos del GameObject que tiene asignado este script, se modificarán los nombres y se agregará un componente LineRenderer, necesario para dibujar el cable.
     * En el inspector podrás modificar los parámetros espesor, la cantidad de subdivisiones, agregar un material para el cable y ajustar la curvatura a gusto.
     * Si el rango de los Sliders no te permite lograr lo que quieres, puedes modificarlo en este Script (las instrucciones "Range").
     * Para agregar un nuevo punto te recomiendo que no dupliques los hijos, ya que produce errores, en su lugar haz clic derecho en el GameObject padre y luego en la opción "Create Empty".
     * Al agregar un nuevo punto nos aparecerá un nuevo parámetro de curvatura.
     * El bool loopWire del inspector agrega un cable que conecta el último punto con el primero.
     * Por último el bool isAnimatedWire sirve para indicar que el cable se actualice cada cierto tiempo, en caso de que queramos animarlo.
     * Espero que te sea de utilidad!
    */

    #endregion

    [Range(0f,1f)]
    [SerializeField]
    private float wireWidth=0.1f; 
    [SerializeField]
    private Material wireMaterial;
    [SerializeField]
    private bool loopWire;
    [Range(1,15)]
    [SerializeField]
    private int subdivisions=1;
    [Range(-10f,10f)]
    [SerializeField]
    private float[] curvatureParameters;
    [SerializeField]
    private bool isAnimatedWire;

    private LineRenderer lineRenderer;
    private Transform[] positions;

    private void Update()
    {

        #if UNITY_EDITOR

        if (!EditorApplication.isPlaying)
        {
            
            FindReferences();

            ConfigureWire();


            DrawWire();
        }

       
        #endif
    }

    void FixedUpdate()
    {
        if (isAnimatedWire)
        {
            DrawWire();
        }
    }

    #region Wire

    private void FindReferences()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        gameObject.name = "GDT Wire Container";//Rename the Container
        int n = gameObject.transform.childCount;

        while (n < 2)
        {
            GameObject g = new GameObject();
            g.transform.SetParent(gameObject.transform);
            g.transform.position += new Vector3(Random.Range(0f,1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            n++;
        }
       
        if (loopWire)
        {
            positions = new Transform[n + 1];
            for (int i = 0; i < n; i++)
            {
                positions[i] = gameObject.transform.GetChild(i);
            }
            positions[n] = gameObject.transform.GetChild(0);
        }
        else
        {
            positions = new Transform[n];

            for (int i = 0; i < n; i++)
            {
                positions[i] = gameObject.transform.GetChild(i);
            }
        }

        //Rename the Childs
        for (int i = 0; i < n; i++)
        {
            positions[i].name = "Wire Point " + (i + 1).ToString();     
        }

        if (curvatureParameters == null)
        {
            curvatureParameters = new float[n];
        }
        else
        {
            int k = n - 1;
            if (loopWire)
            {
                k = n;
            }

            if (curvatureParameters.Length != k )
            {         
                float[] aux = curvatureParameters;
                curvatureParameters = new float[k];
                int j=0;
                if (aux.Length > k)
                {
                    j = k;
                }
                else
                {
                    j = aux.Length;
                }
                for(int i = 0; i < j; i++)
                {
                    curvatureParameters[i] = aux[i];
                }
            }
        }

    }

    private void ConfigureWire()
    {
        lineRenderer.material = wireMaterial;
        lineRenderer.startWidth = wireWidth;
        lineRenderer.endWidth = wireWidth;

    }

    private void DrawWire()
    {
        int lineVertices = positions.Length;
        if (subdivisions > 1)
        {
            int n = positions.Length;
            
            lineRenderer.positionCount=subdivisions*(n-1);

            int vertexIndex = 0;
            
            //Draw the hanging wire
            for (int k = 0; k < positions.Length-1; k++)
            {
                float distance = Vector3.Distance(positions[k].position, positions[k + 1].position);
                float deltaX = distance/(subdivisions);
               
                float x0 = 0;
                float y0 = positions[k].position.y;
                float x1 = distance/2;
                float y1;
                if (positions[k].position.y < positions[k + 1].position.y)
                {
                    y1 = positions[k].position.y + curvatureParameters[k];
                }
                else
                {
                    y1 = positions[k+1].position.y + curvatureParameters[k];
                }

                float x2 = distance;
                float y2 = positions[k+1].position.y;

                float b=(y2-y0-((x2*x2)*(y1-y0))/(x1*x1))/(x2-(x2*x2/x1));
                float a=(y1-y0-b*x1)/(x1*x1);

                float c=y0;//Always

                for (int i = 0; i < subdivisions; i++)
                {
                    //draw the curve
                    Vector3 pos = Vector3.Lerp(positions[k].position,positions[k+1].position,deltaX*i/distance);
                    pos.y = a * ((deltaX * i) * (deltaX * i)) + b * deltaX * i + c;
                    lineRenderer.SetPosition(vertexIndex, pos);

                    vertexIndex++;
                }
            }

            if (!loopWire)
            {
                lineRenderer.positionCount=lineRenderer.positionCount + 1;
                lineRenderer.SetPosition(vertexIndex, positions[positions.Length - 1].position);
            }
        }
        else
        {
           
            lineRenderer.positionCount=lineVertices;
            for (int i = 0; i < lineVertices; i++)
            {
                lineRenderer.SetPosition(i, positions[i].position);               
            }           
        }
        lineRenderer.loop = loopWire;

    }

    public LineRenderer GetLineRenderer() {
        return lineRenderer;
    }

    #endregion

}
