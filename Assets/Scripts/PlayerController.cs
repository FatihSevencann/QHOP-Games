using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Numerics;
using UnityEngine;
using UnityEngine.Networking.Match;
using System;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 0.25f;

    public AudioClip normalAdim;
    public AudioClip ciglikliKazanma;
    public AudioClip kazanma;
    public AudioClip huzuraKavusmak;
    public AudioClip duygusuzKaybetme;

    AudioSource ses;
    UnityEngine.Vector3 targetPosition;
    UnityEngine.Vector3 startPosition;

    public bool moving=false;

    List<GameObject> gameGrid;
    int sectionSize;
    int width;
    int height;
    List<List<Complex>> qbits; // [[Complex sayı 1, complex 2, complex 3, complex4], [c1, c2, c3, c4], ...]
    
    GameGrid gameComponent;
    List<int> qPlatformRowNumbers; // [4, 9, 14, ...]
    List<List<int>> qPlatformIndices;// [[16, 17, 18, 19], [36, 37, 38, 39], ...]
    List<int> materialNums;
    List<List<double>> probabilities;
    List<GameObject> qbitTexts;
    List<List<GameObject>> probLabels;

    public void StartOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    // Start is called before the first frame update
    void Start()
    {
        ses = gameObject.GetComponent<AudioSource>();
        gameComponent = GameObject.Find("Grid Spawner").GetComponent<GameGrid>();
        gameGrid = gameComponent.gridObjects;
        sectionSize = gameComponent.heightMult;
        width = gameComponent.width;
        height = gameComponent.height;
        qbits = gameComponent.qbits;
        qPlatformRowNumbers = gameComponent.qPlatformRowNumbers;
        qPlatformIndices = gameComponent.qPlatformIndices;
        materialNums = gameComponent.materialNumbers;
        probabilities = gameComponent.probabilityLists;
        qbitTexts = gameComponent.qbitTextList;
        probLabels = gameComponent.qProbLabels;
    }
   

    public void OnTapMovementButton(int direction)
    {
        if (direction==1)
        {
            targetPosition = transform.position + UnityEngine.Vector3.forward;
            startPosition = transform.position;
          
            

            moving = true;
        }
        else if (direction==0)
        {
            if (transform.position.z == -1)
            {
                targetPosition = transform.position + UnityEngine.Vector3.left;
                
            }
            else
            {
                targetPosition = transform.position + UnityEngine.Vector3.left + UnityEngine.Vector3.forward;
               
            }
            startPosition = transform.position;
            moving = true;
            if (transform.position.x >= 4 || transform.position.x < 0)
            {
                gameObject.AddComponent<Rigidbody>();
                ses.PlayOneShot(duygusuzKaybetme);
            }
        }
        else if (direction==2)
        {
            if (transform.position.z == -1)
            {
                targetPosition = transform.position + UnityEngine.Vector3.right;
                
            }
            else
            {
                targetPosition = transform.position + UnityEngine.Vector3.right + UnityEngine.Vector3.forward;
               
            }
            startPosition = transform.position;
            moving = true;
            if (transform.position.x >= 4 || transform.position.x < 0)
            {
                gameObject.AddComponent<Rigidbody>();
                ses.PlayOneShot(duygusuzKaybetme);

            }
            //Kaybetme ekranı
        }
    }

    public bool kazandik=false;
    // Update is called once per frame
    void Update()
    {//ÖLÜMCÜL UPDATE
        if (gameObject.transform.position.z == 20)
        {

            GameObject.Find("Canvas/Kazandınız").GetComponent<Text>().enabled = true;
            SceneManager.Scene("winner");
            kazandik = true;
            ses.PlayOneShot(ciglikliKazanma);
            GameObject.Find("Canvas/Text").GetComponent<Text>().enabled = false;
            GameObject.Find("Canvas/Tekrar").SetActive(true);
            GameObject.Destroy(this);
        }
        if (moving)
        {
           
            if (UnityEngine.Vector3.Distance(startPosition, transform.position) > 1f)
            {
                ses.PlayOneShot(normalAdim);
                transform.position = targetPosition;
                moving = false;
                gameObject.GetComponent<Animator>().enabled = true;
                int index = (int)(transform.position.x + (transform.position.z * width));
                if (transform.position.x >= 0 && transform.position.z >= 0 && transform.position.x < width && index >= 0 && index < gameGrid.Count)
                {//Parkurda mısın?
                    if (qPlatformRowNumbers.Contains((int)transform.position.z)) // Player is on a quantum row
                     {
                        for (int i = 0; i < qPlatformRowNumbers.Count; i++) //4 kez dönüyo
                        {
                            if(qPlatformRowNumbers[i] == (int)transform.position.z)
                            {
                                
                                  double probToCheck = probabilities[i][(int)transform.position.x];
                                    System.Random rand = new System.Random();
                                    /**/double rrr = rand.NextDouble();
                                    Material mat1 = Resources.Load<Material>("Materials/1");
                                    Material mat0 = Resources.Load<Material>("Materials/0");
                                    if (probToCheck > rrr) //Neden?
                                    {
                                     ses.PlayOneShot(kazanma);
                                      for (int j = qPlatformIndices[i][0]; j < qPlatformIndices[i].Count + qPlatformIndices[i][0]; j++)
                                        {//?

                                            MeshRenderer meshToChange = gameGrid[j].GetComponent<MeshRenderer>();
                                            if (j == index)
                                            {
                                               
                                                probLabels[i][j - qPlatformIndices[i][0]].GetComponent<TextMesh>().text = "1";
                                                meshToChange.material = mat1;
                                            }
                                            else
                                            {
                                                probLabels[i][j - qPlatformIndices[i][0]].GetComponent<TextMesh>().text = "0";
                                                meshToChange.material = mat0;
                                            }
                                        }
                                        return;
                                    }
                                    else
                                    {
                                        // LOSE
                                        
                                        List<int> possibleIndices = new List<int>();
                                        for(int j = qPlatformIndices[i][0]; j < qPlatformIndices[i].Count + qPlatformIndices[i][0]; j++)
                                        {
                                            if(j != index && probabilities[i][j - qPlatformIndices[i][0]] > 0.001)
                                            {
                                                possibleIndices.Add(j);
                                            }
                                        }
                                        int chosenPlatform = possibleIndices[rand.Next(0, possibleIndices.Count)];
                                        for (int j = qPlatformIndices[i][0]; j < qPlatformIndices[i].Count + qPlatformIndices[i][0]; j++)
                                        {
                                            MeshRenderer meshToChange = gameGrid[j].GetComponent<MeshRenderer>();
                                            if (j == chosenPlatform)
                                            {
                                                meshToChange.material = mat1;
                                                probLabels[i][j - qPlatformIndices[i][0]].GetComponent<TextMesh>().text = "1";
                                            }
                                            else
                                            {
                                                meshToChange.material = mat0;
                                                probLabels[i][j - qPlatformIndices[i][0]].GetComponent<TextMesh>().text = "0";
                                            }
                                        }

                                        GameObject.Find("Player").AddComponent<Rigidbody>();
                                        ses.PlayOneShot(duygusuzKaybetme);
                                        updateProbabilities(getNextQrowIndex());
                                       
                                        return;
                                    }
                                }
                            }
                        }
                       // GameObject platform = gameGrid[index];
                        // platform.GetComponent<Renderer>().material



                        // Calculate gate operations
                        int nextQrowIndex = getNextQrowIndex();
                        Debug.Log("nextQrowIndex is: " + nextQrowIndex);

                        // Apply X1 to next qbits
                        //Debug.Log("before gate");
                        //for (int i = 0; i < 4; i++)
                        //{
                        //    Debug.Log(qbits[0][i].ToString());
                        //}

                      // *  Debug.Log(materialNums[index]);
                      // *  Debug.Log(index);
                        if (materialNums[index] == 0)
                        {
                            X1(qbits[nextQrowIndex]);
                        }
                        else if(materialNums[index] == 1)
                        {
                            H1(qbits[nextQrowIndex]);
                        }
                        else if (materialNums[index] == 2)
                        {
                            Z1(qbits[nextQrowIndex]);
                        }
                        else if (materialNums[index] == 3)
                        {
                            X2(qbits[nextQrowIndex]);
                        }
                        else if (materialNums[index] == 4)
                        {
                            H2(qbits[nextQrowIndex]);
                        }
                        else if (materialNums[index] == 5)
                        {
                            Z2(qbits[nextQrowIndex]);
                        }
                        else if (materialNums[index] == -1)
                        {

                        }
                        else
                        {
                            Debug.LogError("HATA");
                        }

                        UpdateQbitText(nextQrowIndex, qbits[nextQrowIndex]);


                        //Debug.Log("after gate");
                        //for (int i = 0; i < 4; i++)
                        //{
                        //    Debug.Log(qbits[0][i].ToString());
                        //}
                        // Update probabilities of the nex qPlatforms
                        updateProbabilities(nextQrowIndex);

                        // Update labels
                        List<GameObject> fourProbs = probLabels[nextQrowIndex];
                        for (int i = 0; i < fourProbs.Count; i++)
                        {
                            fourProbs[i].GetComponent<TextMesh>().text = probabilities[nextQrowIndex][i].ToString();
                        }
                        
                    
                }
                return;
            }

            transform.position += (targetPosition - startPosition) * moveSpeed * Time.deltaTime;
            return;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            targetPosition = transform.position + UnityEngine.Vector3.forward;
            startPosition = transform.position;
            moving = true;
        } 
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if (transform.position.z == -1)
            {
                targetPosition = transform.position + UnityEngine.Vector3.left;
            } else
            {
                targetPosition = transform.position + UnityEngine.Vector3.left + UnityEngine.Vector3.forward;
            }
            startPosition = transform.position;
            moving = true;
           if(transform.position.x>=4 || transform.position.x<0)
                gameObject.AddComponent<Rigidbody>();
        } 
        else if (Input.GetKeyDown(KeyCode.D))
        {
            if (transform.position.z == -1)
            {
                targetPosition = transform.position + UnityEngine.Vector3.right;
            }
            else
            {
                targetPosition = transform.position + UnityEngine.Vector3.right + UnityEngine.Vector3.forward;
            }
            startPosition = transform.position;
            moving = true;
            if (transform.position.x >= 4 || transform.position.x < 0)
                gameObject.AddComponent<Rigidbody>();
        }
    }

    void updateProbabilities(int nextQRowOrder) // nextQRowOrder can be 0, 1, or 2
    {
        List<Complex> twoQbits = qbits[nextQRowOrder];
        
        double c00sq = Math.Pow(Complex.Abs(twoQbits[0]), 2);       
        double c01sq = Math.Pow(Complex.Abs(twoQbits[1]), 2);
        double c10sq = Math.Pow(Complex.Abs(twoQbits[2]), 2);
        double c11sq = Math.Pow(Complex.Abs(twoQbits[3]), 2);
     // *   Debug.Log("c00sq: " + c00sq + " c01sq: " + c01sq + " c10sq: " + c10sq + " c11sq: " + c11sq);
        double p00 = c10sq * c00sq;
        double p01 = c10sq * c01sq;
        double p10 = c11sq * c00sq;
        double p11 = c11sq * c01sq;
        List<double> oneProbs = new List<double>();
        oneProbs.Add(p00);
        oneProbs.Add(p01);
        oneProbs.Add(p10);
        oneProbs.Add(p11);
        probabilities[nextQRowOrder] = oneProbs;
     // *   Debug.Log("p00: " + p00 + " p01: " + p01 + " p10: " + p10 + " p11: " + p11);
        List<int> indexOfPlatforms = qPlatformIndices[nextQRowOrder];

        Material mat1 = Resources.Load<Material>("Materials/1");
        Material mat0 = Resources.Load<Material>("Materials/0");
        Material mat05 = Resources.Load<Material>("Materials/0.5");
        Material mat025 = Resources.Load<Material>("Materials/0.25");
        for (int i = 0; i < indexOfPlatforms.Count; i++)
        {
            GameObject onePlatform = gameGrid[indexOfPlatforms[i]];
            onePlatform.GetComponent<Collider>().isTrigger = true;
            //gameObject.GetComponent<Rigidbody>().isKinematic = false;
            MeshRenderer pMeshRenderer = onePlatform.GetComponent<MeshRenderer>();
            if(i == 0)
            {
                if(p00 < 0.0001f && p00 > -0.0001f) // if p00 == 0
                {
                    pMeshRenderer.material = mat0;
                } 
                else if (p00 < 0.5001f && p00 > 0.4999f)
                {
                    pMeshRenderer.material = mat05;
                }
                else if (p00 < 0.2501f && p00 > 0.2499f)
                {
                    pMeshRenderer.material = mat025;
                }
                else
                {
                    onePlatform.GetComponent<Collider>().isTrigger = false;

                    pMeshRenderer.material = mat1;
                }
            } else if(i == 1)
            {
                if (p01 < 0.0001f && p01 > -0.0001f) // if p01 == 0
                {
                    pMeshRenderer.material = mat0;
                }
                else if (p01 < 0.5001f && p01 > 0.4999f)
                {
                    pMeshRenderer.material = mat05;
                }
                else if (p01 < 0.2501f && p01 > 0.2499f)
                {
                    pMeshRenderer.material = mat025;
                }
                else
                {
                    onePlatform.GetComponent<Collider>().isTrigger = false;
                    pMeshRenderer.material = mat1;
                }
            } else if (i == 2)
            {
                if (p10 < 0.0001f && p10 > -0.0001f) // if p10 == 0
                {
                    pMeshRenderer.material = mat0;
                }
                else if (p10 < 0.5001f && p10 > 0.4999f)
                {
                    pMeshRenderer.material = mat05;
                }
                else if (p10 < 0.2501f && p10 > 0.2499f)
                {
                    pMeshRenderer.material = mat025;
                }
                else
                {
                    onePlatform.GetComponent<Collider>().isTrigger = false;
                    pMeshRenderer.material = mat1;
                }
            } else if (i == 3)
            {
                if (p11 < 0.0001f && p11 > -0.0001f) // if p11 == 0
                {
                    pMeshRenderer.material = mat0;
                }
                else if (p11 < 0.5001f && p11 > 0.4999f)
                {
                    pMeshRenderer.material = mat05;
                }
                else if (p11 < 0.2501f && p11 > 0.2499f)
                {
                    pMeshRenderer.material = mat025;
                }
                else
                {
                    onePlatform.GetComponent<Collider>().isTrigger = false;
                    pMeshRenderer.material = mat1;
                }
            } else
            {
                Debug.LogError("HATA");
            }
        }
    }

    int getNextQrowIndex() // Returns the which qRow is next. e.g 0, 1, or 2
    {
        for (int i = 0; i < qPlatformRowNumbers.Count; i++)
        {
            if (transform.position.z < qPlatformRowNumbers[i])
            {
                return i;
            }
        }
        return -1; // error or win state
    }

    void X1(List<Complex> twoQubits)
    {
        Complex c00 = twoQubits[0];
        twoQubits[0] = twoQubits[1];
        twoQubits[1] = c00;
    }

    void X2(List<Complex> twoQubits)
    {
        Complex c10 = twoQubits[2];
        twoQubits[2] = twoQubits[3];
        twoQubits[3] = c10;
    }

    void H1(List<Complex> twoQubits)
    {
        Complex r0 = ((twoQubits[0] + twoQubits[1]) / Math.Pow(2, 0.5));
        Complex r1 = ((twoQubits[0] - twoQubits[1]) / Math.Pow(2, 0.5));
        twoQubits[0] = r0;
        twoQubits[1] = r1;
    }
    void H2(List<Complex> twoQubits)
    {
        Complex r0 = ((twoQubits[2] + twoQubits[3]) / Math.Pow(2, 0.5));
        Complex r1 = ((twoQubits[2] - twoQubits[3]) / Math.Pow(2, 0.5));
        twoQubits[2] = r0;
        twoQubits[3] = r1;
    }

    void Z1(List<Complex> twoQubits)
    {
        twoQubits[1] = -twoQubits[1];
    }
    void Z2(List<Complex> twoQubits)
    {
        twoQubits[3] = -twoQubits[3];
    }

    public void moveForward()
    {
        targetPosition = transform.position + UnityEngine.Vector3.forward;
        startPosition = transform.position;
        moving = true;
    }

    void UpdateQbitText(int nextQbitRow, List<Complex> twoQubits)
    {
        Debug.Log(("" + twoQubits[2] + " " + twoQubits[0] + "\n" + twoQubits[3] + " " + twoQubits[1]));
        qbitTexts[nextQbitRow].GetComponent<TextMesh>().text = ("" + formatComplex(twoQubits[2]) + " " + formatComplex(twoQubits[0]) + "\n" + formatComplex(twoQubits[3]) + " " + formatComplex(twoQubits[1]));
    }

    String formatComplex(Complex number)
    {
        if(number.Imaginary < 0.001 && number.Imaginary > -0.001)
        {
            Debug.Log(number.Real);
            if(number.Real < 0.7072 && number.Real > 0.7070)
            {
                return "1/√2";
            }
            else if (number.Real < -0.7070 && number.Real > -0.7072)
            {
                return "-1/√2";
            }
            return number.Real.ToString();
        }
        return number.Real.ToString() + " + " + number.Imaginary.ToString() + "i";
    }
}
