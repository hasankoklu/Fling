using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PuzzlePieceDisplay : MonoBehaviour
{
    public PuzzlePiece puzzlePiece;

    [HideInInspector]
    bool IsMoving;
    [HideInInspector]
    public Vector3 Speed;

    [Button("Snap The Object")]
    void SnapObject()
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

        transform.position = new Vector3(transform.position.x, 0.14f, transform.position.z);

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

    private Animator _animator;
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.SetInteger("animation", 1);
        _animator.speed = Random.Range(0.8f, 1.2f);

        PuzzleDisplay.instance.currentPuzzlePieceList.Add(puzzlePiece);
    }

    Ray ray;
    RaycastHit hit;
    Vector2 firstMousePosition;
    void Update()
    {

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        #region Touch and send

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    GameSceneDisplay.instance.SelectedObject = this.gameObject;
                    firstMousePosition = Input.mousePosition;
                }
            }
        }

        if (GameSceneDisplay.instance.SelectedObject != null)
            if (GameSceneDisplay.instance.SelectedObject == this.gameObject)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    GameSceneDisplay.instance.LockPanel.SetActive(true);
                    GameSceneDisplay.instance.InfoText.text = "";

                    if (Input.mousePosition.x - firstMousePosition.x > GameSceneDisplay.instance.SlideDetectionDistance)
                    {
                        GameSceneDisplay.instance.InfoText.text = " You slide right ! \n";
                        Speed = new Vector3(GameSceneDisplay.instance.PuzzleMovementSpeed, 0f, 0f);
                        transform.LookAt(transform.position + (Speed * 20f));

                        if (PuzzleDisplay.instance.currentPuzzlePieceList.Where(x => x.GameObject.transform.position.x - 1 == transform.position.x && x.GameObject.transform.position.z == transform.position.z).Count() > 0 || ObstacleDisplay.instance.currentObstaclePieceList.Where(x => x.GameObject.transform.position.x - 1 == transform.position.x && x.GameObject.transform.position.z == transform.position.z).Count() > 0)
                        {
                            StartCoroutine(HitAnimator());
                        }
                        else if (PuzzleDisplay.instance.currentPuzzlePieceList.Where(x => x.GameObject.transform.position.x - 1 > transform.position.x && x.GameObject.transform.position.z == transform.position.z).Count() > 0 || ObstacleDisplay.instance.currentObstaclePieceList.Where(x => x.GameObject.transform.position.x - 1 > transform.position.x && x.GameObject.transform.position.z == transform.position.z).Count() > 0)
                        {
                            StartCoroutine(Movement(Speed));
                        }
                        else
                        {
                            StartCoroutine(HitAnimator());
                            GameSceneDisplay.instance.InfoText.text += "This movement is not available! Try Again!";
                            GameSceneDisplay.instance.LockPanel.SetActive(false);
                        }

                    }
                    else if (Input.mousePosition.x - firstMousePosition.x < -GameSceneDisplay.instance.SlideDetectionDistance)
                    {
                        GameSceneDisplay.instance.InfoText.text = " You slide left ! \n";

                        Speed = new Vector3(-GameSceneDisplay.instance.PuzzleMovementSpeed, 0f, 0f);
                        transform.LookAt(transform.position + (Speed * 20f));

                        if (PuzzleDisplay.instance.currentPuzzlePieceList.Where(x => x.GameObject.transform.position.x == transform.position.x - 1 && x.GameObject.transform.position.z == transform.position.z).Count() > 0 || ObstacleDisplay.instance.currentObstaclePieceList.Where(x => x.GameObject.transform.position.x == transform.position.x - 1 && x.GameObject.transform.position.z == transform.position.z).Count() > 0)
                        {
                            StartCoroutine(HitAnimator());
                        }
                        else if (PuzzleDisplay.instance.currentPuzzlePieceList.Where(x => x.GameObject.transform.position.x < transform.position.x - 1 && x.GameObject.transform.position.z == transform.position.z).Count() > 0 || ObstacleDisplay.instance.currentObstaclePieceList.Where(x => x.GameObject.transform.position.x < transform.position.x - 1 && x.GameObject.transform.position.z == transform.position.z).Count() > 0)
                        {
                            StartCoroutine(Movement(Speed));
                        }
                        else
                        {
                            StartCoroutine(HitAnimator());
                            GameSceneDisplay.instance.InfoText.text += "This movement is not available! Try Again!";
                            GameSceneDisplay.instance.LockPanel.SetActive(false);
                        }
                    }
                    else if (Input.mousePosition.y - firstMousePosition.y > GameSceneDisplay.instance.SlideDetectionDistance)
                    {
                        GameSceneDisplay.instance.InfoText.text = " You slide up ! \n";

                        Speed = new Vector3(0f, 0f, GameSceneDisplay.instance.PuzzleMovementSpeed);
                        transform.LookAt(transform.position + (Speed * 20f));

                        if (PuzzleDisplay.instance.currentPuzzlePieceList.Where(x => x.GameObject.transform.position.z - 1 == transform.position.z && x.GameObject
                            .transform.position.x == transform.position.x).Count() > 0 || ObstacleDisplay.instance.currentObstaclePieceList.Where(x => x.GameObject.transform.position.z - 1 == transform.position.z && x.GameObject
                             .transform.position.x == transform.position.x).Count() > 0)
                        {
                            StartCoroutine(HitAnimator());
                        }
                        else if (PuzzleDisplay.instance.currentPuzzlePieceList.Where(x => x.GameObject.transform.position.z - 1 > transform.position.z &&
                        x.GameObject.transform.position.x == transform.position.x).Count() > 0 || ObstacleDisplay.instance.currentObstaclePieceList.Where(x => x.GameObject.transform.position.z - 1 > transform.position.z && x.GameObject.transform.position.x == transform.position.x).Count() > 0)
                        {
                            StartCoroutine(Movement(Speed));
                        }
                        else
                        {
                            StartCoroutine(HitAnimator());
                            GameSceneDisplay.instance.InfoText.text += "This movement is not available! Try Again!";
                            GameSceneDisplay.instance.LockPanel.SetActive(false);
                        }
                    }
                    else if (Input.mousePosition.y - firstMousePosition.y < -GameSceneDisplay.instance.SlideDetectionDistance)
                    {
                        GameSceneDisplay.instance.InfoText.text = " You slide down ! \n";

                        Speed = new Vector3(0f, 0f, -GameSceneDisplay.instance.PuzzleMovementSpeed);
                        transform.LookAt(transform.position + (Speed * 20f));

                        if (PuzzleDisplay.instance.currentPuzzlePieceList.Where(x => x.GameObject.transform.position.z == transform.position.z - 1 && x.GameObject.transform.position.x == transform.position.x).Count() > 0 || ObstacleDisplay.instance.currentObstaclePieceList.Where(x => x.GameObject.transform.position.z == transform.position.z - 1 && x.GameObject.transform.position.x == transform.position.x).Count() > 0)
                        {
                            StartCoroutine(HitAnimator());
                        }
                        else if (PuzzleDisplay.instance.currentPuzzlePieceList.Where(x => x.GameObject.transform.position.z < transform.position.z - 1 && x.GameObject.transform.position.x == transform.position.x).Count() > 0 || ObstacleDisplay.instance.currentObstaclePieceList.Where(x => x.GameObject.transform.position.z < transform.position.z - 1 && x.GameObject.transform.position.x == transform.position.x).Count() > 0)
                        {
                            StartCoroutine(Movement(Speed));
                        }
                        else
                        {
                            StartCoroutine(HitAnimator());

                            GameSceneDisplay.instance.InfoText.text += "This movement is not available! Try Again!";
                            GameSceneDisplay.instance.LockPanel.SetActive(false);
                        }
                    }
                    GameSceneDisplay.instance.SelectedObject = null;
                }
            }
        #endregion

        if (transform.position.x < 0 || transform.position.z < 0 || transform.position.x > 10 || transform.position.z > 10)
        {
            PuzzleDisplay.instance.currentPuzzlePieceList.Remove(puzzlePiece);

            if (PuzzleDisplay.instance.currentPuzzlePieceList.Count == 1)
            {
                PuzzleDisplay.instance.currentPuzzlePieceList.FirstOrDefault().GameObject.GetComponent<Animator>().SetInteger("animation", 2);
                GameSceneDisplay.instance.FinishPopUpRect.SetActive(true);
            }
        }

    }

    public IEnumerator Movement(Vector3 spd)
    {
        IsMoving = true;

        _animator.SetInteger("animation", 14);

        while (IsMoving)
        {
            transform.Translate(Vector3.Lerp(Vector3.zero, spd, 0.2f), Space.World);
            yield return new WaitForEndOfFrame();
        }



        if (triggedObject.tag == "PuzzlePiece")
        {
            _animator.SetInteger("animation", 1);

            StartCoroutine(triggedObject.GetComponent<PuzzlePieceDisplay>().Movement(spd));
            transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
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
            GameSceneDisplay.instance.LockPanel.SetActive(false);
            triggedObject = other.gameObject;
            IsMoving = false;
            StartCoroutine(WaterAnimator());
        }
        else if (other.tag == "PuzzlePiece")
        {
            GameSceneDisplay.instance.LockPanel.SetActive(false);
            triggedObject = other.gameObject;
            IsMoving = false;
        }
        else if (other.tag == "Obstacle")
        {
            GameSceneDisplay.instance.LockPanel.SetActive(false);
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
}