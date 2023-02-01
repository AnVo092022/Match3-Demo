using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    [HideInInspector]
    public Vector2Int pos;
    public Board board;
    

    Vector2 firstPos, finalPos;
    // lưu vị trí trước khi hoán vị
    Vector2Int prevPos;

    bool mousePressed = false;

    float swipeAngle;

    Gem otherGem;

    public enum GemType { blue, green, purple, red, yellow };
    public GemType type;

    public bool isMatched; // đánh dấu có gem cùng loại

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // đổi vị trí gem trên màn hình đồ họa
        if (Vector2.Distance(transform.position, pos) > 0.011f)
        {
            transform.position = Vector2.Lerp(transform.position, pos, board.gemSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = new Vector3(pos.x, pos.y, 0f);
        }
        if (mousePressed && Input.GetMouseButtonUp(0))
        {
            finalPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePressed = false;

            CalculateAngle();
            ChangeGemPos();
        }
    }
    public void InitGem(Vector2Int thePos, Board theBoard)
    {
        pos = thePos;
        board = theBoard;
    }
    private void OnMouseDown()
    {
        firstPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log(firstPos);
        mousePressed = true;
        
    }

    private void CalculateAngle()
    {
        // distance = sau trừ trước
        if (Vector2.Distance(finalPos, firstPos) > 0.5f)
        {
            swipeAngle = Mathf.Atan2(finalPos.y - firstPos.y, finalPos.x - firstPos.x);
            swipeAngle = swipeAngle * 180 / Mathf.PI;

            Debug.Log(swipeAngle);
        }

    }

    void ChangeGemPos()
    {
        prevPos = pos;

        // board.width -1 = giới hạn -1
        if (swipeAngle < 45 && swipeAngle > -45 && pos.x < board.width - 1)
        {
            otherGem = board.allGems[pos.x + 1, pos.y];
            pos.x++;
            otherGem.pos.x--;
        }
        else if (swipeAngle < 135 && swipeAngle > 45 && pos.y < board.height - 1)
        {
            otherGem = board.allGems[pos.x, pos.y + 1];
            pos.y++;
            otherGem.pos.y--;
        }
        else if (swipeAngle <= -45 && swipeAngle >= -135 && pos.y > 0)
        {
            otherGem = board.allGems[pos.x, pos.y - 1];
            pos.y--;
            otherGem.pos.y++;
        }
        else if ((swipeAngle > 135 || swipeAngle < -135) && pos.x > 0)
        {
            otherGem = board.allGems[pos.x - 1, pos.y];
            pos.x--;
            otherGem.pos.x++;
        }
        //cập nhật lại Gem theo vị trí
        if (otherGem != null)
        {
            board.allGems[pos.x, pos.y] = this;
            board.allGems[otherGem.pos.x, otherGem.pos.y] = otherGem;

            StartCoroutine(CheckMoveCo());
        }

        IEnumerator CheckMoveCo()
        {
            yield return new WaitForSeconds(.5f);

            // kiểm tra gem đã match3
            board.matcher.FindAllMatches();

            if (!isMatched && !otherGem.isMatched)
            {
                otherGem.pos = pos;
                pos = prevPos;

                board.allGems[pos.x, pos.y] = this;
                board.allGems[otherGem.pos.x, otherGem.pos.y] = otherGem;
            }
            else
            {
                board.DeleteAllMatches();
            }
        }
    }
}
