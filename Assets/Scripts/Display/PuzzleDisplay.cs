using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.IO;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Net.Http.Headers;
using UnityEditor.SceneManagement;

public class PuzzleDisplay : MonoBehaviour
{
    public static PuzzleDisplay instance;

    public static string loginURL = "http://cebur.fun/DinoPuzzle/";

    [HideInInspector]
    public List<PuzzlePiece> currentPuzzlePieceList;

    public Puzzle myPuzzle;
    public Puzzle solution = new Puzzle();
    public PuzzleList solutionList = new PuzzleList();
    public int CurrentStep;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        myPuzzle = new Puzzle();
        myPuzzle.MyStepList = new List<PuzzlePieceList>();

        PuzzlePieceList ppl = new PuzzlePieceList();
        ppl.puzzlePieceList = new List<PuzzlePiece>();
        foreach (GameObject item in GameObject.FindGameObjectsWithTag("PuzzlePiece"))
        {
            PuzzlePiece pp = new PuzzlePiece();
            pp.GameObject = item;
            pp.position = item.transform.position;
            ppl.puzzlePieceList.Add(pp);
        }
        myPuzzle.MyStepList.Add(ppl);
        StartCoroutine(GetLevelSolutions());

    }

    [Button("Arrange This Puzzle")]
    void ArrangeThisPuzzle()
    {
        foreach (GameObject obstacle in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            obstacle.transform.SetParent(GameObject.Find("Obstacle").transform);
        }

        foreach (GameObject puzzlePiece in GameObject.FindGameObjectsWithTag("PuzzlePiece"))
        {
            puzzlePiece.transform.SetParent(GameObject.Find("Puzzle").transform);
        }
        foreach (GameObject puzzleBasePiece in GameObject.FindGameObjectsWithTag("PuzzleBasePiece"))
        {
            puzzleBasePiece.transform.SetParent(GameObject.Find("PuzzleBase").transform);
        }
    }

    [Button("Snap This Puzzle")]
    public void SnapThisPuzzle()
    {
        foreach (GameObject obstacle in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            obstacle.transform.SetParent(GameObject.Find("Obstacle").transform);
            obstacle.GetComponent<ObstaclePieceDisplay>().SnapObject();
        }

        foreach (GameObject puzzlePiece in GameObject.FindGameObjectsWithTag("PuzzlePiece"))
        {
            puzzlePiece.transform.SetParent(GameObject.Find("Puzzle").transform);
            puzzlePiece.GetComponent<PuzzlePieceDisplay>().SnapObject();
        }
    }

    [Button("Clear This Puzzle")]
    void ClearPuzzle()
    {
        foreach (GameObject child in GameObject.FindGameObjectsWithTag("PuzzlePiece"))
        {
            DestroyImmediate(child);
        }
        foreach (GameObject child in GameObject.FindGameObjectsWithTag("PuzzleBasePiece"))
        {
            DestroyImmediate(child);
        }
        foreach (GameObject child in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            DestroyImmediate(child);
        }
        DestroyImmediate(GameObject.FindGameObjectWithTag("Ground"));
    }

    [Button("Save This Puzzle")]
    void SavePuzzle(int level)
    {
        //Debug.Log("open for level design editor");
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), "Assets/Scenes/Levels/Level" + level.ToString() + ".unity");
    }

    [Button("Load This Puzzle")]
    public void LoadPuzzle(int level)
    {
        if (level == 0)
        {
            SceneManager.LoadScene("Assets/Scenes/SampleScene.unity");
        }
        else
        {
            SceneManager.LoadScene("Assets/Scenes/Levels/Level" + level.ToString() + ".unity");
        }

    }

    //<PuzzlePieceDisplay>

    bool isOldSolution;
    bool isTempOldSolution;
    public void SaveSolution()
    {
        isOldSolution = false;

        if (solutionList.puzzleList == null)
        {
            solutionList = new PuzzleList();
            solutionList.puzzleList = new List<Puzzle>();
        }
        if (solutionList.puzzleList != null)
            if (solutionList.puzzleList.Where(x => x.MyStepList.Count == myPuzzle.MyStepList.Count).Count() == 0)
            {
                isOldSolution = false;
            }
            else
            {
                foreach (Puzzle pzl in solutionList.puzzleList.Where(x => x.MyStepList.Count == myPuzzle.MyStepList.Count))
                {
                    isTempOldSolution = true;
                    for (int i = 0; i < myPuzzle.MyStepList.Count; i++)
                    {
                        foreach (PuzzlePiece item in myPuzzle.MyStepList[i].puzzlePieceList)
                        {
                            if (pzl.MyStepList[i].puzzlePieceList.Where(x => x.position.x == item.position.x && x.position.z == item.position.z).Count() == 0)
                            {
                                isTempOldSolution = false;
                            }
                        }
                    }

                    if (isTempOldSolution)
                    {
                        isOldSolution = isTempOldSolution;
                    }
                }
            }

        if (!isOldSolution)
        {
            Debug.Log("it's new solution");
            solutionList.puzzleList.Add(myPuzzle);
            string json = JsonUtility.ToJson(solutionList);
            File.WriteAllText("Assets/Resources/Level" + GameManager.instance.currentLevel + "Solution.txt", json);
        }
        else
        {
            Debug.Log("it's same solution");
        }
    }

    public void LoadSolution()
    {
        int solutionCount = 1;
        TextAsset json = Resources.Load<TextAsset>("Level" + GameManager.instance.currentLevel + "Solution" + solutionCount);

        JsonUtility.FromJsonOverwrite(json.text, solution);

        foreach (PuzzlePiece item in solution.MyStepList.FirstOrDefault().puzzlePieceList)
        {
            item.GameObject = GameObject.FindGameObjectsWithTag("PuzzlePiece").Where(x => x.transform.position == item.position).FirstOrDefault();
        }
    }

    public IEnumerator GetLevelSolutions()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            UnityWebRequest uwr = UnityWebRequest.Get(loginURL + "Level" + GameManager.instance.currentLevel + "Solution.txt");
            yield return uwr.SendWebRequest();
            if (uwr.isNetworkError)
            {
                Debug.Log(uwr.isNetworkError.ToString());
                solutionList = new PuzzleList();
                solutionList.puzzleList = new List<Puzzle>();
            }
            else
            {
                Debug.Log(uwr.downloadHandler.text);

                JsonUtility.FromJsonOverwrite(uwr.downloadHandler.text, solutionList);

                foreach (PuzzlePiece item in solutionList.puzzleList.FirstOrDefault().MyStepList.FirstOrDefault().puzzlePieceList)
                {
                    item.GameObject = GameObject.FindGameObjectsWithTag("PuzzlePiece").Where(x => x.transform.position == item.position).FirstOrDefault();
                }
            }
        }
        else
        {
            Debug.Log("there is no connection");
        }
    }

    public void BackButtonClick()
    {
        if (myPuzzle.MyStepList.Count == 1)
            return;


        if (GameManager.instance.backButtonCounter == 0)
        {
            GameSceneDisplay.instance.InfoPopUpRect.SetActive(true);
            Debug.Log("Watch  video and won the Back gift");
        }

        GameSceneDisplay.instance.LockPanel.SetActive(false);
        //GameSceneDisplay.instance.WindParticlePrefab.SetActive(false);

        myPuzzle.MyStepList.Remove(myPuzzle.MyStepList.LastOrDefault());

        foreach (GameObject item in GameObject.FindGameObjectsWithTag("PuzzlePiece"))
        {
            item.SetActive(false);
        }

        foreach (GameObject item in GameObject.FindGameObjectsWithTag("FinishedPuzzle"))
        {
            item.SetActive(false);
        }

        foreach (PuzzlePiece item in myPuzzle.MyStepList.LastOrDefault().puzzlePieceList)
        {
            item.GameObject.SetActive(true);
            item.GameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            item.GameObject.GetComponent<Rigidbody>().useGravity = false;
            item.GameObject.transform.position = item.position;
            item.GameObject.transform.rotation = Quaternion.identity;

            item.GameObject.tag = "PuzzlePiece";
        }

        currentPuzzlePieceList.Clear();
        foreach (GameObject item in GameObject.FindGameObjectsWithTag("PuzzlePiece"))
        {
            currentPuzzlePieceList.Add(item.GetComponent<PuzzlePieceDisplay>().puzzlePiece);
        }

        GameManager.instance.backButtonCounter--;
        ArrangeThisPuzzle();
    }

    public void SuggestButtonClick()
    {
        StartCoroutine(SuggestButtonClickEnum());
    }


    bool isSolutionOkay;
    int solutionStep;
    GameObject suggestedGO;
    GameObject targetGO;
    public IEnumerator SuggestButtonClickEnum()
    {
        if (solutionList.puzzleList.Count == 0)
        {
            yield return StartCoroutine(GetLevelSolutions());
        }

        List<GameObject> currentppGO = new List<GameObject>();
        currentppGO = GameObject.FindGameObjectsWithTag("PuzzlePiece").ToList();
        int currentPuzzleCount = GameObject.FindGameObjectsWithTag("PuzzlePiece").Count();

        foreach (Puzzle suggestableSolution in solutionList.puzzleList)
        {
            suggestedGO = null;
            targetGO = null;

            for (int i = 0; i < suggestableSolution.MyStepList.Count; i++)
            {
                if (suggestableSolution.MyStepList[i].puzzlePieceList.Count == currentPuzzleCount)
                {
                    isSolutionOkay = true;
                    foreach (GameObject ppGO in currentppGO)
                    {
                        if (suggestableSolution.MyStepList[i].puzzlePieceList.Where(x => (x.position.x == ppGO.transform.position.x && x.position.z == ppGO.transform.position.z)).Count() == 0)
                        {
                            isSolutionOkay = false;
                        }
                        else
                        {
                            suggestableSolution.MyStepList[i].puzzlePieceList.Where(x => (x.position.x == ppGO.transform.position.x && x.position.z == ppGO.transform.position.z)).FirstOrDefault().GameObject = ppGO;
                        }
                    }
                    if (isSolutionOkay)
                    {
                        solutionStep = i;
                    }
                }
            }
            if (isSolutionOkay)
            {
                foreach (PuzzlePiece pp in suggestableSolution.MyStepList[solutionStep].puzzlePieceList)
                {
                    if (suggestableSolution.MyStepList[solutionStep + 1].puzzlePieceList.Where(x => (x.position.x == pp.position.x && x.position.z == pp.position.z)).Count() == 0)
                    {
                        suggestedGO = suggestableSolution.MyStepList[solutionStep].puzzlePieceList.Where(
                            x => x.GameObject.transform.position.x == suggestableSolution.MyStepList[solutionStep].movedObjectPos.x &&
                            x.GameObject.transform.position.z == suggestableSolution.MyStepList[solutionStep].movedObjectPos.z)
                            .FirstOrDefault().GameObject;

                        //targetGO = suggestableSolution.MyStepList[solutionStep].puzzlePieceList.Where(
                        // x => x.GameObject.transform.position.x == suggestableSolution.MyStepList[solutionStep].targetObjectPos.x &&
                        // x.GameObject.transform.position.z == suggestableSolution.MyStepList[solutionStep].targetObjectPos.z)
                        // .FirstOrDefault().GameObject;

                    }
                    else
                    {
                        suggestableSolution.MyStepList[solutionStep + 1].puzzlePieceList.Where(x => (x.position.x == pp.position.x && x.position.z == pp.position.z)).FirstOrDefault().GameObject = pp.GameObject;
                    }
                }
                Debug.Log(suggestedGO.name);
                suggestedGO.transform.LookAt(suggestableSolution.MyStepList[solutionStep].targetObjectPos);
                suggestedGO.GetComponent<PuzzlePieceDisplay>()._animator.SetInteger("animation", 6);
                yield break;
            }
        }
        StartCoroutine(animWaiter(GameObject.Find("Back-Button")));
    }

    IEnumerator animWaiter(GameObject go)
    {
        go.GetComponent<Animation>().enabled = true;
        yield return new WaitForSeconds(3f);
        go.GetComponent<Animation>().enabled = false;
        go.transform.localScale = Vector3.one;
    }
}