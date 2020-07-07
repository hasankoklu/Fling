using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
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

    int currentDirection;
    int characterNumber;

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
        if (transform.rotation.eulerAngles.y == 0)
        {
            currentDirection = 0;
        }
        else if (transform.rotation.eulerAngles.y == 90)
        {
            currentDirection = 1;
        }
        else if (transform.rotation.eulerAngles.y == -180)
        {
            currentDirection = 2;
        }
        else if (transform.rotation.eulerAngles.y == -90)
        {
            currentDirection = 3;
        }

    }

    public Animator _animator;
    private void Start()
    {
        characterNumber = UnityEngine.Random.Range(0, AudioDisplay.instance.GrumbleMusicList.Count);
        _animator = GetComponent<Animator>();
        _animator.SetTrigger("Idle");
        _animator.speed = UnityEngine.Random.Range(0.8f, 1.2f);
        //StartCoroutine(GrumbleSound());
        PuzzleDisplay.instance.currentPuzzlePieceList.Add(puzzlePiece);
    }

    private void OnEnable()
    {
        _animator = GetComponent<Animator>();
        _animator.SetTrigger("Idle");
        _animator.speed = UnityEngine.Random.Range(0.8f, 1.2f);

        //PuzzleDisplay.instance.currentPuzzlePieceList.Add(puzzlePiece);
    }

    Ray ray;
    RaycastHit hit;
    Vector2 firstMousePosition;
    void Update()
    {
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
                if (!GetComponent<AudioSource>().isPlaying)
                {
                    GrumbleSound();
                }

                if (Input.GetMouseButtonUp(0))
                {
                    GameSceneDisplay.instance.InfoText.text = "";
                    GameSceneDisplay.instance.LockPanel.SetActive(true);


                    if (Input.mousePosition.x - firstMousePosition.x > GameManager.instance.SlideDetectionDistance)
                    {
                        SlideSound();
                        GameSceneDisplay.instance.InfoText.text = " You slide right ! \n";
                        Speed = new Vector3(GameManager.instance.PuzzleMovementSpeed, 0f, 0f);
                        PuzzleDisplay.instance.myPuzzle.MyStepList.LastOrDefault().movedObjectPos = transform.position;

                        PuzzleDisplay.instance.myPuzzle.MyStepList.LastOrDefault().targetObjectPos = transform.position + Vector3.right;

                        if (PuzzleDisplay.instance.currentPuzzlePieceList.Where(x => x.GameObject.transform.position.x - 1 == transform.position.x && x.GameObject.transform.position.z == transform.position.z).Count() > 0 || ObstacleDisplay.instance.currentObstaclePieceList.Where(x => x.GameObject.transform.position.x - 1 == transform.position.x && x.GameObject.transform.position.z == transform.position.z).Count() > 0)
                        {
                            //StartCoroutine(HitAnimator());
                            WrongSideAnimato(1);
                        }
                        else if (PuzzleDisplay.instance.currentPuzzlePieceList.Where(x => x.GameObject.transform.position.x - 1 > transform.position.x && x.GameObject.transform.position.z == transform.position.z).Count() > 0 || ObstacleDisplay.instance.currentObstaclePieceList.Where(x => x.GameObject.transform.position.x - 1 > transform.position.x && x.GameObject.transform.position.z == transform.position.z).Count() > 0)
                        {
                            transform.LookAt(transform.position + (Speed * 20f));
                            SetParticle();

                            StartCoroutine(Movement(Speed, 1));
                        }
                        else
                        {
                            //StartCoroutine(HitAnimator());
                            WrongSideAnimato(1);
                            GameSceneDisplay.instance.InfoText.text += "This movement is not available! Try Again!";
                        }

                    }
                    else if (Input.mousePosition.x - firstMousePosition.x < -GameManager.instance.SlideDetectionDistance)
                    {
                        SlideSound();
                        GameSceneDisplay.instance.InfoText.text = " You slide left ! \n";
                        Speed = new Vector3(-GameManager.instance.PuzzleMovementSpeed, 0f, 0f);
                        PuzzleDisplay.instance.myPuzzle.MyStepList.LastOrDefault().movedObjectPos = GameManager.instance.SelectedObject.transform.position;

                        PuzzleDisplay.instance.myPuzzle.MyStepList.LastOrDefault().targetObjectPos = transform.position + Vector3.left;

                        if (PuzzleDisplay.instance.currentPuzzlePieceList.Where(x => x.GameObject.transform.position.x == transform.position.x - 1 && x.GameObject.transform.position.z == transform.position.z).Count() > 0 || ObstacleDisplay.instance.currentObstaclePieceList.Where(x => x.GameObject.transform.position.x == transform.position.x - 1 && x.GameObject.transform.position.z == transform.position.z).Count() > 0)
                        {
                            //StartCoroutine(HitAnimator());
                            WrongSideAnimato(3);
                        }
                        else if (PuzzleDisplay.instance.currentPuzzlePieceList.Where(x => x.GameObject.transform.position.x < transform.position.x - 1 && x.GameObject.transform.position.z == transform.position.z).Count() > 0 || ObstacleDisplay.instance.currentObstaclePieceList.Where(x => x.GameObject.transform.position.x < transform.position.x - 1 && x.GameObject.transform.position.z == transform.position.z).Count() > 0)
                        {
                            transform.LookAt(transform.position + (Speed * 20f));
                            SetParticle();

                            StartCoroutine(Movement(Speed, 3));
                        }
                        else
                        {
                            //StartCoroutine(HitAnimator());
                            WrongSideAnimato(3);
                            GameSceneDisplay.instance.InfoText.text += "This movement is not available! Try Again!";
                        }
                    }
                    else if (Input.mousePosition.y - firstMousePosition.y > GameManager.instance.SlideDetectionDistance)
                    {
                        SlideSound();
                        GameSceneDisplay.instance.InfoText.text = " You slide up ! \n";
                        Speed = new Vector3(0f, 0f, GameManager.instance.PuzzleMovementSpeed);
                        PuzzleDisplay.instance.myPuzzle.MyStepList.LastOrDefault().movedObjectPos = GameManager.instance.SelectedObject.transform.position;

                        PuzzleDisplay.instance.myPuzzle.MyStepList.LastOrDefault().targetObjectPos = transform.position + Vector3.forward;

                        if (PuzzleDisplay.instance.currentPuzzlePieceList.Where(x => x.GameObject.transform.position.z - 1 == transform.position.z && x.GameObject
                            .transform.position.x == transform.position.x).Count() > 0 || ObstacleDisplay.instance.currentObstaclePieceList.Where(x => x.GameObject.transform.position.z - 1 == transform.position.z && x.GameObject
                             .transform.position.x == transform.position.x).Count() > 0)
                        {
                            //StartCoroutine(HitAnimator());
                            WrongSideAnimato(0);
                        }
                        else if (PuzzleDisplay.instance.currentPuzzlePieceList.Where(x => x.GameObject.transform.position.z - 1 > transform.position.z &&
                        x.GameObject.transform.position.x == transform.position.x).Count() > 0 || ObstacleDisplay.instance.currentObstaclePieceList.Where(x => x.GameObject.transform.position.z - 1 > transform.position.z && x.GameObject.transform.position.x == transform.position.x).Count() > 0)
                        {
                            transform.LookAt(transform.position + (Speed * 20f));
                            SetParticle();

                            StartCoroutine(Movement(Speed, 0));
                        }
                        else
                        {
                            //StartCoroutine(HitAnimator());
                            WrongSideAnimato(0);
                            GameSceneDisplay.instance.InfoText.text += "This movement is not available! Try Again!";
                        }
                    }
                    else if (Input.mousePosition.y - firstMousePosition.y < -GameManager.instance.SlideDetectionDistance)
                    {
                        SlideSound();
                        GameSceneDisplay.instance.InfoText.text = " You slide down ! \n";
                        Speed = new Vector3(0f, 0f, -GameManager.instance.PuzzleMovementSpeed);
                        PuzzleDisplay.instance.myPuzzle.MyStepList.LastOrDefault().movedObjectPos = GameManager.instance.SelectedObject.transform.position;

                        PuzzleDisplay.instance.myPuzzle.MyStepList.LastOrDefault().targetObjectPos = transform.position + Vector3.back;

                        if (PuzzleDisplay.instance.currentPuzzlePieceList.Where(x => x.GameObject.transform.position.z == transform.position.z - 1 && x.GameObject.transform.position.x == transform.position.x).Count() > 0 || ObstacleDisplay.instance.currentObstaclePieceList.Where(x => x.GameObject.transform.position.z == transform.position.z - 1 && x.GameObject.transform.position.x == transform.position.x).Count() > 0)
                        {
                            //StartCoroutine(HitAnimator());
                            WrongSideAnimato(2);
                        }
                        else if (PuzzleDisplay.instance.currentPuzzlePieceList.Where(x => x.GameObject.transform.position.z < transform.position.z - 1 && x.GameObject.transform.position.x == transform.position.x).Count() > 0 || ObstacleDisplay.instance.currentObstaclePieceList.Where(x => x.GameObject.transform.position.z < transform.position.z - 1 && x.GameObject.transform.position.x == transform.position.x).Count() > 0)
                        {
                            transform.LookAt(transform.position + (Speed * 20f));

                            SetParticle();
                            StartCoroutine(Movement(Speed, 2));
                        }
                        else
                        {
                            //StartCoroutine(HitAnimator());
                            WrongSideAnimato(2);
                            GameSceneDisplay.instance.InfoText.text += "This movement is not available! Try Again!";
                        }
                    }
                    else
                    {

                        GameSceneDisplay.instance.LockPanel.SetActive(false);
                    }
                    GameManager.instance.SelectedObject = null;
                }
            }
        #endregion

    }

    void SetParticle()
    {
        //GameSceneDisplay.instance.WindParticlePrefab.SetActive(false);
        //GameSceneDisplay.instance.WindParticlePrefab.SetActive(true);
        //GameSceneDisplay.instance.WindParticlePrefab.transform.position = transform.position;
        //GameSceneDisplay.instance.WindParticlePrefab.transform.rotation = transform.rotation;
        //GameSceneDisplay.instance.WindParticlePrefab.transform.Rotate(0f, 180f, 0f);
    }

    public IEnumerator Movement(Vector3 spd, int direction)
    {
        IsMoving = true;
        //GameSceneDisplay.instance.LockPanel.SetActive(true);
        //GameSceneDisplay.instance.WindParticlePrefab.SetActive(true);
        //GameSceneDisplay.instance.WindParticlePrefab.transform.position = transform.position;
        //GameSceneDisplay.instance.WindParticlePrefab.transform.rotation = transform.rotation;
        //GameSceneDisplay.instance.WindParticlePrefab.transform.Rotate(0f, 180f, 0f);

        _animator.SetTrigger("Hit");

        currentDirection = direction;
        while (IsMoving)
        {
            transform.Translate(Vector3.Lerp(Vector3.zero, spd, 0.2f), Space.World);
            //transform.Rotate(0f, UnityEngine.Random.Range(5, 10), UnityEngine.Random.Range(5, 10));

            if (transform.position.x < 0 || transform.position.z < 0 || transform.position.x > 10 || transform.position.z > 10)
            {

                transform.Rotate(UnityEngine.Random.Range(5, 10), UnityEngine.Random.Range(5, 10), UnityEngine.Random.Range(5, 10));
                GetComponent<Rigidbody>().useGravity = true;
                if (puzzlePiece.GameObject.tag == "PuzzlePiece")
                {
                    ScreamSound();
                    puzzlePiece.GameObject.tag = "FinishedPuzzle";
                    PuzzleDisplay.instance.currentPuzzlePieceList.Remove(puzzlePiece);

                    PuzzlePieceList ppl = new PuzzlePieceList();
                    ppl.puzzlePieceList = new List<PuzzlePiece>();
                    foreach (GameObject item in GameObject.FindGameObjectsWithTag("PuzzlePiece"))
                    {
                        PuzzlePiece pp = new PuzzlePiece();
                        pp.GameObject = item;
                        pp.position = new Vector3((int)item.transform.position.x, item.transform.position.y, (int)item.transform.position.z);
                        ppl.puzzlePieceList.Add(pp);
                    }
                    PuzzleDisplay.instance.myPuzzle.MyStepList.Add(ppl);
                }

                if (PuzzleDisplay.instance.currentPuzzlePieceList.Count == 1 && GameManager.instance.isGameRunning)
                {
                    int i = UnityEngine.Random.Range(0, AudioDisplay.instance.FinishMusicList.Count);
                    GameManager.instance.GetComponent<AudioSource>().PlayOneShot(AudioDisplay.instance.FinishMusicList[i], 0.4f);
                    PuzzleDisplay.instance.currentPuzzlePieceList.FirstOrDefault().GameObject.GetComponent<Animator>().SetInteger("animation", 2);

                    GameSceneDisplay.instance.FinishPopUpRect.SetActive(true);
                    GameObject.Find("GoToNextLevel-Button").GetComponent<Button>().onClick.AddListener(GameManager.instance.GoToNextLevelButtonClick);
                    PuzzleDisplay.instance.SaveSolution();
                    GameManager.instance.currentLevel++;
                    PlayerPrefs.SetInt("currentLevel", GameManager.instance.currentLevel);

                    GameManager.instance.isGameRunning = false;
                }

            }
            GameSceneDisplay.instance.LockPanel.SetActive(false);
            yield return new WaitForEndOfFrame();
        }

        //gameObject.transform.GetChild(2).gameObject.SetActive(false);

        if (triggedObject.tag == "PuzzlePiece")
        {
            _animator.SetTrigger("Idle");

            StartCoroutine(triggedObject.GetComponent<PuzzlePieceDisplay>().Movement(spd, direction));

            triggedObject.transform.LookAt(transform.position + (spd * 20f));

            transform.position = new Vector3(Mathf.Round(transform.position.x), transform.position.y, Mathf.Round(transform.position.z));
            float sety = transform.position.y;
            //transform.rotation = rotation;
        }
        else if (triggedObject.tag == "Obstacle")
        {
            _animator.SetTrigger("Idle");
            transform.position = new Vector3(Mathf.Round(transform.position.x), transform.position.y, Mathf.Round(transform.position.z));
            //transform.rotation = rotation;
        }
    }

    GameObject triggedObject;
    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.position.x == transform.position.x || other.transform.position.z == transform.position.z)
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
                AudioDisplay.instance.GetComponent<AudioSource>().PlayOneShot(AudioDisplay.instance.OnHitMusic, 1f);
            }
            else if (other.tag == "Obstacle")
            {
                triggedObject = other.gameObject;
                IsMoving = false;
            }
        }
    }

    IEnumerator HitAnimator()
    {
        _animator.SetTrigger("Hit");
        yield return new WaitForSeconds(0.2f);
        //_animator.SetTrigger("Idle");
    }

    IEnumerator WaterAnimator()
    {
        _animator.SetTrigger("Jump");
        yield return new WaitForSeconds(1f);
        //_animator.SetTrigger("Idle");
        transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
    }


    void WrongSideAnimato(int direction)
    {
        Debug.Log("sa : " + (currentDirection - direction));

        GameSceneDisplay.instance.LockPanel.SetActive(false);
        if (Math.Abs(direction - currentDirection) == 0) //forward
        {
            _animator.SetTrigger("isForward");
            Debug.Log("ileri");
        }
        else if (Math.Abs(direction - currentDirection) == 1) // right
        {
            _animator.SetTrigger("isRight");
            Debug.Log("sağ");
        }
        else if (Math.Abs(direction - currentDirection) == 2) // back
        {
            _animator.SetTrigger("isBack");
            Debug.Log("geri");
        }
        else if (Math.Abs(direction - currentDirection) == 3) // left
        {
            _animator.SetTrigger("isLeft");
            Debug.Log("sol");
        }
    }

    void GrumbleSound()
    {
        //GetComponent<AudioSource>().PlayOneShot(AudioDisplay.instance.GrumbleMusicList[characterNumber], UnityEngine.Random.Range(0.5f, 0.6f));
        //yield return new WaitForSeconds(UnityEngine.Random.Range(1.2f, 2.1f));
    }
    void SlideSound()
    {
        //GetComponent<AudioSource>().PlayOneShot(AudioDisplay.instance.SlideMusicList[characterNumber], UnityEngine.Random.Range(0.5f, 0.6f));
        //yield return new WaitForSeconds(UnityEngine.Random.Range(1.2f, 2.1f));
    }
    void ScreamSound()
    {
        //GetComponent<AudioSource>().PlayOneShot(AudioDisplay.instance.ScreamMusicList[UnityEngine.Random.Range(0, AudioDisplay.instance.ScreamMusicList.Count)], UnityEngine.Random.Range(0.5f, 0.6f));
        //yield return new WaitForSeconds(UnityEngine.Random.Range(1.2f, 2.1f));
    }
}