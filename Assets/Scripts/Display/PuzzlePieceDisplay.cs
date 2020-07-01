using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PuzzlePieceDisplay : MonoBehaviour
{
    public PuzzlePiece puzzlePiece;

    [HideInInspector]
    bool IsMoving;
    [HideInInspector]
    public Vector3 Speed;

    [Button("Snap The Object")]
    public void SnapObject()
    {
        puzzlePiece = new PuzzlePiece();
        puzzlePiece.GameObject = gameObject;
        if (puzzlePiece.ScaleX == 0)
        {
            puzzlePiece.ScaleX = 1;
        }
        if (puzzlePiece.ScaleY == 0)
        {
            puzzlePiece.ScaleY = 1;
        }

        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        if ((transform.rotation.z / 90) % 2 == 0) // vertical
        {
            if (puzzlePiece.ScaleX % 2 == 1) // snap x
            {
                transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), transform.position.y, transform.position.z);
            }
            else if (puzzlePiece.ScaleX % 2 == 0)
            {
                transform.position = new Vector3(Mathf.RoundToInt(transform.position.x - 0.5f) + 0.5f, transform.position.y, transform.position.z);
            }

            if (puzzlePiece.ScaleY % 2 == 1) // snap z
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.RoundToInt(transform.position.z));
            }
            else if (puzzlePiece.ScaleY % 2 == 0)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.RoundToInt(transform.position.z - 0.5f) + 0.5f);
            }
        }
        else // horizontal
        {
            if (puzzlePiece.ScaleY % 2 == 1) // snap x
            {
                transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), transform.position.y, transform.position.z);
            }
            else if (puzzlePiece.ScaleY % 2 == 0)
            {
                transform.position = new Vector3(Mathf.RoundToInt(transform.position.x - 0.5f) + 0.5f, transform.position.y, transform.position.z);
            }

            if (puzzlePiece.ScaleX % 2 == 1) // snap z
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.RoundToInt(transform.position.z));
            }
            else if (puzzlePiece.ScaleX % 2 == 0)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.RoundToInt(transform.position.z - 0.5f) + 0.5f);
            }
        }

    }

    [Button("Rotate The Object")]
    void RotateObject()
    {
        transform.Rotate(0f, 90f, 0f);
    }

    private void Awake()
    {
        puzzlePiece.GameObject = gameObject;
    }

    public Animator _animator;
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.SetInteger("animation", 1);
        _animator.speed = Random.Range(0.8f, 1.2f);

        PuzzleDisplay.instance.currentPuzzlePieceList.Add(puzzlePiece);
    }

    private void OnEnable()
    {
        _animator = GetComponent<Animator>();
        _animator.SetInteger("animation", 1);
        _animator.speed = Random.Range(0.8f, 1.2f);

        //PuzzleDisplay.instance.currentPuzzlePieceList.Add(puzzlePiece);
    }

    Ray ray;
    RaycastHit hit;
    Vector2 firstMousePosition;
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.H))
        {
            PuzzleDisplay.instance.LoadSolution();
        }

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (GameSceneDisplay.instance.LockPanel.activeInHierarchy)
            return;

        #region Touch and send

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    GameManager.instance.SelectedObject = this.gameObject;
                    firstMousePosition = Input.mousePosition;
                }
            }
        }

        if (GameManager.instance.SelectedObject != null)
            if (GameManager.instance.SelectedObject == this.gameObject)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    GameSceneDisplay.instance.InfoText.text = "";

                    if (Input.mousePosition.x - firstMousePosition.x > GameManager.instance.SlideDetectionDistance)
                    {
                        GameSceneDisplay.instance.InfoText.text = " You slide right ! \n";
                        Speed = new Vector3(GameManager.instance.PuzzleMovementSpeed, 0f, 0f);

                        if (PuzzleDisplay.instance.currentPuzzlePieceList.Where(x => x.GameObject.transform.position.x - 1 == transform.position.x && x.GameObject.transform.position.z == transform.position.z).Count() > 0 || ObstacleDisplay.instance.currentObstaclePieceList.Where(x => x.GameObject.transform.position.x - 1 == transform.position.x && x.GameObject.transform.position.z == transform.position.z).Count() > 0)
                        {
                            transform.LookAt(transform.position + (Speed * 20f));
                            StartCoroutine(Movement(Speed));
                        }
                        else if (PuzzleDisplay.instance.currentPuzzlePieceList.Where(x => x.GameObject.transform.position.x - 1 > transform.position.x && x.GameObject.transform.position.z == transform.position.z).Count() > 0 || ObstacleDisplay.instance.currentObstaclePieceList.Where(x => x.GameObject.transform.position.x - 1 > transform.position.x && x.GameObject.transform.position.z == transform.position.z).Count() > 0)
                        {
                            transform.LookAt(transform.position + (Speed * 20f));
                            StartCoroutine(Movement(Speed));
                        }
                        else
                        {
                            StartCoroutine(WrongSideAnimato());
                            GameSceneDisplay.instance.InfoText.text += "This movement is not available! Try Again!";
                        }

                    }
                    else if (Input.mousePosition.x - firstMousePosition.x < -GameManager.instance.SlideDetectionDistance)
                    {
                        GameSceneDisplay.instance.InfoText.text = " You slide left ! \n";

                        Speed = new Vector3(-GameManager.instance.PuzzleMovementSpeed, 0f, 0f);

                        if (PuzzleDisplay.instance.currentPuzzlePieceList.Where(x => x.GameObject.transform.position.x == transform.position.x - 1 && x.GameObject.transform.position.z == transform.position.z).Count() > 0 || ObstacleDisplay.instance.currentObstaclePieceList.Where(x => x.GameObject.transform.position.x == transform.position.x - 1 && x.GameObject.transform.position.z == transform.position.z).Count() > 0)
                        {
                            transform.LookAt(transform.position + (Speed * 20f));
                            StartCoroutine(Movement(Speed));
                        }
                        else if (PuzzleDisplay.instance.currentPuzzlePieceList.Where(x => x.GameObject.transform.position.x < transform.position.x - 1 && x.GameObject.transform.position.z == transform.position.z).Count() > 0 || ObstacleDisplay.instance.currentObstaclePieceList.Where(x => x.GameObject.transform.position.x < transform.position.x - 1 && x.GameObject.transform.position.z == transform.position.z).Count() > 0)
                        {
                            transform.LookAt(transform.position + (Speed * 20f));
                            StartCoroutine(Movement(Speed));
                        }
                        else
                        {
                            StartCoroutine(WrongSideAnimato());
                            GameSceneDisplay.instance.InfoText.text += "This movement is not available! Try Again!";
                        }
                    }
                    else if (Input.mousePosition.y - firstMousePosition.y > GameManager.instance.SlideDetectionDistance)
                    {
                        GameSceneDisplay.instance.InfoText.text = " You slide up ! \n";

                        Speed = new Vector3(0f, 0f, GameManager.instance.PuzzleMovementSpeed);

                        if (PuzzleDisplay.instance.currentPuzzlePieceList.Where(x => x.GameObject.transform.position.z - 1 == transform.position.z && x.GameObject
                            .transform.position.x == transform.position.x).Count() > 0 || ObstacleDisplay.instance.currentObstaclePieceList.Where(x => x.GameObject.transform.position.z - 1 == transform.position.z && x.GameObject
                             .transform.position.x == transform.position.x).Count() > 0)
                        {
                            transform.LookAt(transform.position + (Speed * 20f));
                            StartCoroutine(Movement(Speed));
                        }
                        else if (PuzzleDisplay.instance.currentPuzzlePieceList.Where(x => x.GameObject.transform.position.z - 1 > transform.position.z &&
                        x.GameObject.transform.position.x == transform.position.x).Count() > 0 || ObstacleDisplay.instance.currentObstaclePieceList.Where(x => x.GameObject.transform.position.z - 1 > transform.position.z && x.GameObject.transform.position.x == transform.position.x).Count() > 0)
                        {
                            transform.LookAt(transform.position + (Speed * 20f));
                            StartCoroutine(Movement(Speed));
                        }
                        else
                        {
                            StartCoroutine(WrongSideAnimato());
                            GameSceneDisplay.instance.InfoText.text += "This movement is not available! Try Again!";
                        }
                    }
                    else if (Input.mousePosition.y - firstMousePosition.y < -GameManager.instance.SlideDetectionDistance)
                    {
                        GameSceneDisplay.instance.InfoText.text = " You slide down ! \n";

                        Speed = new Vector3(0f, 0f, -GameManager.instance.PuzzleMovementSpeed);

                        if (PuzzleDisplay.instance.currentPuzzlePieceList.Where(x => x.GameObject.transform.position.z == transform.position.z - 1 && x.GameObject.transform.position.x == transform.position.x).Count() > 0 || ObstacleDisplay.instance.currentObstaclePieceList.Where(x => x.GameObject.transform.position.z == transform.position.z - 1 && x.GameObject.transform.position.x == transform.position.x).Count() > 0)
                        {
                            transform.LookAt(transform.position + (Speed * 20f));
                            StartCoroutine(Movement(Speed));
                        }
                        else if (PuzzleDisplay.instance.currentPuzzlePieceList.Where(x => x.GameObject.transform.position.z < transform.position.z - 1 && x.GameObject.transform.position.x == transform.position.x).Count() > 0 || ObstacleDisplay.instance.currentObstaclePieceList.Where(x => x.GameObject.transform.position.z < transform.position.z - 1 && x.GameObject.transform.position.x == transform.position.x).Count() > 0)
                        {
                            transform.LookAt(transform.position + (Speed * 20f));
                            StartCoroutine(Movement(Speed));
                        }
                        else
                        {
                            StartCoroutine(WrongSideAnimato());
                            GameSceneDisplay.instance.InfoText.text += "This movement is not available! Try Again!";
                        }
                    }
                    GameManager.instance.SelectedObject = null;
                }
            }
        #endregion

    }

    public IEnumerator Movement(Vector3 spd)
    {
        IsMoving = true;
        Quaternion rotation = transform.rotation;
        GameSceneDisplay.instance.LockPanel.SetActive(true);

        _animator.SetInteger("animation", 14);

        while (IsMoving)
        {
            transform.Translate(Vector3.Lerp(Vector3.zero, spd, 0.2f), Space.World);
            //transform.Rotate(Random.Range(5, 10), Random.Range(5, 10), Random.Range(5, 10));

            if (transform.position.x < 0 || transform.position.z < 0 || transform.position.x > 10 || transform.position.z > 10)
            {
                GetComponent<Rigidbody>().useGravity = true;
                if (puzzlePiece.GameObject.tag == "PuzzlePiece")
                {
                    puzzlePiece.GameObject.tag = "FinishedPuzzle";
                    PuzzleDisplay.instance.currentPuzzlePieceList.Remove(puzzlePiece);

                    PuzzlePieceList ppl = new PuzzlePieceList();
                    ppl.puzzlePieceList = new List<PuzzlePiece>();
                    foreach (GameObject item in GameObject.FindGameObjectsWithTag("PuzzlePiece"))
                    {
                        PuzzlePiece pp = new PuzzlePiece();
                        pp.GameObject = item;
                        pp.position = item.transform.position;
                        ppl.puzzlePieceList.Add(pp);
                    }
                    PuzzleDisplay.instance.myPuzzle.MyStepList.Add(ppl);
                }

                if (PuzzleDisplay.instance.currentPuzzlePieceList.Count == 1 && GameManager.instance.isGameRunning)
                {
                    int i = Random.Range(0, AudioDisplay.instance.FinishMusicList.Count);
                    GameManager.instance.GetComponent<AudioSource>().PlayOneShot(AudioDisplay.instance.FinishMusicList[i]);
                    PuzzleDisplay.instance.currentPuzzlePieceList.FirstOrDefault().GameObject.GetComponent<Animator>().SetInteger("animation", 2);

                    GameSceneDisplay.instance.FinishPopUpRect.SetActive(true);
                    GameObject.Find("GoToNextLevel-Button").GetComponent<Button>().onClick.AddListener(GameManager.instance.GoToNextLevelButtonClick);
                    PuzzleDisplay.instance.SaveSolution();
                    GameManager.instance.currentLevel++;
                    PlayerPrefs.SetInt("currentLevel", GameManager.instance.currentLevel);

                    GameManager.instance.isGameRunning = false;
                }

                GameSceneDisplay.instance.LockPanel.SetActive(false);
            }

            yield return new WaitForEndOfFrame();
        }

        if (triggedObject.tag == "PuzzlePiece")
        {
            _animator.SetInteger("animation", 1);

            StartCoroutine(triggedObject.GetComponent<PuzzlePieceDisplay>().Movement(spd));

            triggedObject.transform.LookAt(transform.position + (spd * 20f));

            transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
            float sety = transform.position.y;
            //transform.rotation = rotation;
        }
        else if (triggedObject.tag == "Obstacle")
        {
            _animator.SetInteger("animation", 1);
            transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));

        }
    }

    GameObject triggedObject;
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Water")
        {
            triggedObject = other.gameObject;
            IsMoving = false;
            StartCoroutine(WaterAnimator());
        }
        else if (other.tag == "PuzzlePiece")
        {
            triggedObject = other.gameObject;
            IsMoving = false;
            AudioDisplay.instance._audioSource.PlayOneShot(AudioDisplay.instance.onHitMusic, 1f);
        }
        else if (other.tag == "Obstacle")
        {
            triggedObject = other.gameObject;
            IsMoving = false;
        }
    }

    IEnumerator HitAnimator()
    {
        _animator.SetInteger("animation", 7);
        yield return new WaitForSeconds(0.2f);
        _animator.SetInteger("animation", 1);
    }

    IEnumerator WaterAnimator()
    {
        _animator.SetInteger("animation", 4);
        yield return new WaitForSeconds(1f);
        _animator.SetInteger("animation", 1);
        transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
    }

    IEnumerator WrongSideAnimato()
    {
        if (transform.rotation.y / 90 == 0) //forward
        {
            _animator.SetInteger("animation", 100);
            yield return new WaitForSeconds(0.2f);
            _animator.SetInteger("animation", 1);
        }
        else if (transform.rotation.y / 90 == 1)
        {
            _animator.SetInteger("animation", 101);
            yield return new WaitForSeconds(0.2f);
            _animator.SetInteger("animation", 1);
        }
        else if (transform.rotation.y / 90 == 2)
        {
            _animator.SetInteger("animation", 102);
            yield return new WaitForSeconds(0.2f);
            _animator.SetInteger("animation", 1);
        }
        else if (transform.rotation.y / 90 == 3)
        {
            _animator.SetInteger("animation", 103);
            yield return new WaitForSeconds(0.2f);
            _animator.SetInteger("animation", 1);
        }
    }
}