using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    // Start is called before the first frame update

    
    [SerializeField] Board board;
    [SerializeField] int boardZ = -10;
    
    private void Awake()
    {
        board.GetComponent<Board>();
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(board.width / 2, board.height / 2, boardZ);
        //Debug.Log(board.transform.position.x);
        //Debug.Log(board.transform.position.y);
    }
}
