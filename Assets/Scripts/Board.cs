using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width = 7;
    public int height = 7;

    [SerializeField]
    GameObject bgTile;

    [SerializeField]
    // lưu 5 gem
    Gem[] gems;

    public Gem Bomb;
    float bombRate = 5f;

    public int gemSpeed = 8;

    public Gem[,] allGems;

    public FindMatcher matcher;

    public enum BoardState { wait, move };
    public BoardState state = BoardState.move;


    private void Awake()
    {
        matcher = FindObjectOfType<FindMatcher>();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        allGems = new Gem[width, height];
        Setup();
    }

    // Update is called once per frame
    void Update()
    {
        // matcher.FindAllMatches();
        if (Input.GetKey(KeyCode.S))
        {
            ShuffleBoard();
        }
    }



    void Setup()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 pos = new Vector2(x, y);
                GameObject tile = Instantiate(bgTile, pos, Quaternion.identity);
                tile.transform.parent = transform;
                tile.name = $"BG - {x}, {y}";

                // Random.Range(min, max) = sinh giá trị ngẫu nhiên trong đoạn [min, max-1]
                int gemToUse = Random.Range(0, gems.Length);
                Vector2Int gemPos = new Vector2Int(x, y);



                int loopCheck = 0;
                while (MatchAt(gemPos, gems[gemToUse]) && loopCheck < 100)
                {
                    gemToUse = Random.Range(0, gems.Length);
                    loopCheck++;
                }

                GenerateGems(gemPos, gems[gemToUse]);
            }
        }
    }

    void GenerateGems(Vector2Int pos, Gem gem)
    {
        if (Random.Range(0f, 100f) < bombRate)
        {
            gem = Bomb;
        }
        Gem currentGem = Instantiate(gem, new Vector3(pos.x, pos.y + height, 0f), Quaternion.identity);
        currentGem.transform.parent = transform;
        currentGem.name = $"Gem - {pos.x}, {pos.y}";
        currentGem.InitGem(pos, this);
        // lưu currentGem vào mảng allGems
        allGems[pos.x, pos.y] = currentGem;
    }

    bool MatchAt(Vector2Int pos, Gem gem)
    {
        bool result = false;
        if (pos.x > 1)
        {
            if (allGems[pos.x - 1, pos.y].type == gem.type
                && allGems[pos.x - 2, pos.y].type == gem.type)
            {
                result = true;
            }
        }
        if (pos.y > 1)
        {
            if (allGems[pos.x, pos.y - 1].type == gem.type
                && allGems[pos.x, pos.y - 2].type == gem.type)
            {
                result = true;
            }
        }
        return result;
    }

    void DeleteMatchAt(Vector2Int pos)
    {
        if (allGems[pos.x, pos.y] != null && allGems[pos.x, pos.y].isMatched)
        {
            Instantiate(allGems[pos.x, pos.y].destroyOject, new Vector2(pos.x, pos.y), Quaternion.identity);
            // xóa gem mà script đang gắn vào
            Destroy(allGems[pos.x, pos.y].gameObject);
            allGems[pos.x, pos.y] = null;
        }
    }

    public void DeleteAllMatches()
    {
        for (int i = 0; i < matcher.matches.Count; i++)
        {
            DeleteMatchAt(matcher.matches[i].pos);
        }

        StartCoroutine(DownGemCo());
    }

    // Co trong DownGemCo là Coroutine
    IEnumerator DownGemCo()
    {
        yield return new WaitForSeconds(.5f);

        int count = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allGems[x, y] == null)
                {
                    count++;
                }
                else if (count > 0)
                {
                    // chuyển gem
                    allGems[x, y].pos.y -= count;
                    allGems[x, y - count] = allGems[x, y];
                    allGems[x, y] = null;
                }
            }
            count = 0;
        }
        StartCoroutine(FillBoardCo());
    }

    IEnumerator FillBoardCo()
    {
        yield return new WaitForSeconds(.5f);
        RefillBoard();

        yield return new WaitForSeconds(.5f);
        matcher.FindAllMatches();

        yield return new WaitForSeconds(.5f);
        if (matcher.matches.Count > 0)
        {
            DeleteAllMatches();
            
        }
        else
        {
            state = BoardState.move;
        }

    }

    void RefillBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allGems[x, y] == null)
                {
                    int gemToUse = Random.Range(0, gems.Length);
                    GenerateGems(new Vector2Int(x, y), gems[gemToUse]);
                }
            }
        }
    }

    void CheckDupGem()
    {
        List <Gem> foundGems = new List <Gem>();
        //foundGems chứa tất cả Gems đang có trong của số Hierarchy
        foundGems.AddRange(FindObjectsOfType<Gem>());
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (foundGems.Contains(allGems[x, y]))
                {
                    foundGems.Remove(allGems[x, y]);
                }
            }
        }

        // gem đang có trong foundGems bị dup và sẽ bị xóa
        foreach ( Gem g in foundGems)
        {
            Destroy(g);
        }
    }

    private void ShuffleBoard()
    {
        if (state != BoardState.wait)
        {
            state = BoardState.wait;

            List<Gem> gemsFromBoard = new List<Gem>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    gemsFromBoard.Add(allGems[x, y]);
                    allGems[x, y] = null;
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int gemsToUse = Random.Range(0, gemsFromBoard.Count);
                    int loopCount = 0;
                    while (MatchAt(new Vector2Int (x, y), gemsFromBoard[gemsToUse]) && loopCount > 100)
                    {
                        gemsToUse = Random.Range(0, gemsFromBoard.Count);
                        loopCount++;
                    }
                    gemsFromBoard[gemsToUse].InitGem(new Vector2Int(x, y), this);
                    allGems[x, y] = gemsFromBoard[gemsToUse];
                    gemsFromBoard.RemoveAt(gemsToUse);
                }
                
            }
            StartCoroutine(FillBoardCo());
        }
    }

}
