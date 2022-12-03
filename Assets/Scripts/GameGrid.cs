using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class GameGrid : MonoBehaviour
{
    [SerializeField]
    public int width = 4;
    [SerializeField]
    public int height = 4;
    [SerializeField]
    public int heightMult = 5;
   
    public GameObject alight;
    public GameObject qbitText;
    public GameObject qLabel;

    public List<GameObject> gridObjects = new List<GameObject>();
    public List<List<Complex>> qbits = new List<List<Complex>>();
    public List<int> qPlatformRowNumbers = new List<int>();
    public List<List<int>> qPlatformIndices = new List<List<int>> ();
    public List<int> materialNumbers = new List<int>();
    public List<List<double>> probabilityLists = new List<List<double>>();
    public List<GameObject> qbitTextList = new List<GameObject>();
    public List<List<GameObject>> qProbLabels = new List<List<GameObject>>();

    // Start alled before the first frame update
    void Start()
    {
        var gameGrid = new GameObject("gameGrid").transform;
        gameGrid.position = new UnityEngine.Vector3(0, 0, 0);
        int totalHeight = height * heightMult;

        for (int i = 0; i < totalHeight; i++)
        {
            if ((i + 1) % heightMult == 0)
            {// QPlatformlar 
                List<Complex> twoQbits = new List<Complex>();
                int startingPos = Random.Range(0, 4);
                Complex c00, c01, c10, c11;

                if (startingPos == 0)
                {
                    c00 = new Complex(1, 0);
                    c01 = new Complex(0, 0);
                    c10 = new Complex(1, 0);
                    c11 = new Complex(0, 0);
                } else if(startingPos == 1)
                {
                    c00 = new Complex(0, 0);
                    c01 = new Complex(1, 0);
                    c10 = new Complex(1, 0);
                    c11 = new Complex(0, 0);
                } else if(startingPos == 2)
                {
                    c00 = new Complex(1, 0);
                    c01 = new Complex(0, 0);
                    c10 = new Complex(0, 0);
                    c11 = new Complex(1, 0);
                } else if (startingPos == 3)
                {
                    c00 = new Complex(0, 0);
                    c01 = new Complex(1, 0);
                    c10 = new Complex(0, 0);
                    c11 = new Complex(1, 0);
                }
                twoQbits.Add(c00);
                twoQbits.Add(c01);
                twoQbits.Add(c10);
                twoQbits.Add(c11);
                qbits.Add(twoQbits);
                qPlatformRowNumbers.Add(i);
                List<int> qPlatformIndicesOneRow = new List<int>();
                List<double> probabilityList = new List<double>();
                List<GameObject> oneProbLables = new List<GameObject>();
                for (int j = 0; j < width; j++)
                {//şifalı for=> Qplatformu oluşturur, qplatformun grid içindeki kaçıncı eleman olduğunu listeye atar
                    // şeffaflık, qplatformun isminin ve olasılığının textini yazar
                    GameObject gridObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    gridObject.isStatic = true;
                    gridObject.name = "Qgrid" + j + i;
                    gridObject.transform.position = new UnityEngine.Vector3(j, 0, i);
                    gridObject.transform.localScale = new UnityEngine.Vector3(0.9f, 0.2f, 0.9f);
                    gridObject.transform.parent = gameGrid;

                    qPlatformIndicesOneRow.Add(j + (i * width)); //qPlatform grid listesindeki kaçıncı eleman
                    probabilityList.Add(0.0);

                    MeshRenderer pMeshRenderer = gridObject.GetComponent<MeshRenderer>();
                    Material mat;
                    if (j == startingPos)
                    {
                        mat = Resources.Load<Material>("Materials/1");
                    } else
                    {
                        mat = Resources.Load<Material>("Materials/0");
                    }
                    pMeshRenderer.material = mat;
                    materialNumbers.Add(-1);
                    gridObjects.Add(gridObject);

                    Instantiate(alight);
                    alight.transform.position = new UnityEngine.Vector3(gridObject.transform.position.x, 4, gridObject.transform.position.z);

                    GameObject aQLabel = Instantiate(qLabel);
                    aQLabel.transform.position = new UnityEngine.Vector3(gridObject.transform.position.x - 0.25f, 0.45f, gridObject.transform.position.z + 0.4f);
                    if (j == 0) aQLabel.GetComponent<TextMesh>().text = "|00>";
                    else if (j == 1) aQLabel.GetComponent<TextMesh>().text = "|01>";
                    else if (j == 2) aQLabel.GetComponent<TextMesh>().text = "|10>";
                    else if (j == 3) aQLabel.GetComponent<TextMesh>().text = "|11>";


                    GameObject aProbLabel = Instantiate(qLabel);
                    aProbLabel.transform.position = new UnityEngine.Vector3(gridObject.transform.position.x - 0.3f, 0.18f, gridObject.transform.position.z);
                    oneProbLables.Add(aProbLabel);
                    if (j == startingPos)
                    {
                        aProbLabel.GetComponent<TextMesh>().text = "1";
                    }
                    else
                    {
                        aProbLabel.GetComponent<TextMesh>().text = "0";
                    }

                }
                qProbLabels.Add(oneProbLables);

                GameObject qtext = Instantiate(qbitText);
                qtext.transform.position = new UnityEngine.Vector3(3.55f, 0, i);
                qtext.GetComponent<TextMesh>().text = "";
                qbitTextList.Add(qtext);
                probabilityLists.Add(probabilityList);
                qPlatformIndices.Add(qPlatformIndicesOneRow);
            } else
            { //Düz platformlar
                Material matlilax = Resources.Load<Material>("Materials/X0");
                Material matlilah = Resources.Load<Material>("Materials/H0");
                Material matlilaz = Resources.Load<Material>("Materials/Z0");
                Material matmintx = Resources.Load<Material>("Materials/X1");
                Material matminth = Resources.Load<Material>("Materials/H1");
                Material matmintz = Resources.Load<Material>("Materials/Z1");
                for (int j = 0; j < width; j++)
                { //şifalı forun Q'suz hali 
                    int rand = Random.Range(0, 6);
                    GameObject gridObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    gridObject.name = "grid" + j + i;
                    gridObject.transform.position = new UnityEngine.Vector3(j, 0, i);
                    gridObject.transform.localScale = new UnityEngine.Vector3(0.9f, 0.2f, 0.9f);
                    gridObject.transform.Rotate(0, 180, 0); //fotoğrafın yönü yüzünden
                    
                    MeshRenderer pMeshRenderer = gridObject.GetComponent<MeshRenderer>();
                   
                    if (rand == 0)
                    {
                        pMeshRenderer.material = matlilax;
                        materialNumbers.Add(0);
                    }
                    else if (rand == 1)
                    {
                        pMeshRenderer.material = matlilah;
                        materialNumbers.Add(1);
                    }
                    else if (rand == 2)
                    {
                        pMeshRenderer.material = matlilaz;
                        materialNumbers.Add(2);
                    }
                    else if (rand == 3)
                    {
                        pMeshRenderer.material = matmintx;
                        materialNumbers.Add(3);
                    }
                    else if (rand == 4)
                    {
                        pMeshRenderer.material = matminth;
                        materialNumbers.Add(4);
                    }
                    else if (rand == 5)
                    {
                        pMeshRenderer.material = matmintz;
                        materialNumbers.Add(5);
                    }


                    gridObject.transform.parent = gameGrid;
                    gridObjects.Add(gridObject);


                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
