using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class FindMatcher : MonoBehaviour
{
    Board board;

    public List<Gem> matches = new List<Gem>();

    private void Awake()
    {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMatches()
    {
        // xóa list cho mỗi lần update
        matches.Clear();
        for (int x = 0; x < board.width; x++)
        {
            for (int y = 0; y < board.height; y++)
            {
                Gem currentGem = board.allGems[x, y];
                if (board.allGems[x,y] != null)
                {
                    // tìm hàng bị trùng
                    if (x > 0 && x < board.width - 1)
                    {
                        Gem leftGem = board.allGems[x - 1, y];
                        Gem rightGem = board.allGems[x + 1, y];
                        if (leftGem != null & rightGem != null)
                        {
                            if (currentGem.type == leftGem.type && currentGem.type == rightGem.type)
                            {
                                currentGem.isMatched = true;
                                leftGem.isMatched = true;
                                rightGem.isMatched = true;
                                // thêm vào List<Gem>
                                matches.Add(currentGem);
                                matches.Add(rightGem);
                                matches.Add(leftGem);
                            }
                        }
                    }
                    // tìm cột bị trùng
                    if (y > 0 && y < board.height - 1)
                    {
                        Gem belowGem = board.allGems[x, y - 1];
                        Gem aboveGem = board.allGems[x, y + 1];
                        if (belowGem != null & aboveGem != null)
                        {
                            if (currentGem.type == belowGem.type && currentGem.type == aboveGem.type)
                            {
                                currentGem.isMatched = true;
                                belowGem.isMatched = true;
                                aboveGem.isMatched = true;
                                // thêm vào List<Gem>
                                matches.Add(currentGem);
                                matches.Add(aboveGem);
                                matches.Add(belowGem);
                            }
                        }
                    }
                }
            }
        }
        // sử dụng LinQ để xóa những phần tử bị trùng do x giao với y
        if (matches.Count > 0)
        {
            matches = matches.Distinct().ToList();
        }
        CheckBomb();
    }
    void CheckBomb()
    {
        if (matches.Count > 0)
        {
            for (int i = 0; i < matches.Count; i++)
            {
                int x = matches[i].pos.x;
                int y = matches[i].pos.y;
                if (x > 0)
                {
                    if (board.allGems[x - 1, y] != null)
                    {
                        if (board.allGems[x - 1, y].type == Gem.GemType.bomb)
                        {
                            MarkBomb(new Vector2Int(x - 1, y), board.allGems[x - 1, y]);
                        }
                    }
                }
                if (x < board.width - 1)
                {
                    if (board.allGems[x + 1, y] != null)
                    {
                        if (board.allGems[x + 1, y].type == Gem.GemType.bomb)
                        {
                            MarkBomb(new Vector2Int(x + 1, y), board.allGems[x + 1, y]);
                        }
                    }
                }
                if (y > 0)
                {
                    if (board.allGems[x, y - 1] != null)
                    {
                        if (board.allGems[x, y - 1].type == Gem.GemType.bomb)
                        {
                            MarkBomb(new Vector2Int(x, y - 1), board.allGems[x, y - 1]);
                        }
                    }
                }
                if (y < board.height - 1)
                {
                    if (board.allGems[x, y + 1] != null)
                    {
                        if (board.allGems[x, y + 1].type == Gem.GemType.bomb)
                        {
                            MarkBomb(new Vector2Int(x, y + 1), board.allGems[x, y + 1]);
                        }
                    }
                }
            }
        }
    }

    private void MarkBomb(Vector2Int thePos, Gem theBomb)
    {
        for (int x = thePos.x - theBomb.burstRadius; x <= thePos.x + theBomb.burstRadius; x++)
        {
            for (int y = thePos.y - theBomb.burstRadius; y <= thePos.y + theBomb.burstRadius; y++)
            {
                {
                    if (x >= 0 && x < board.width && y >= 0 && y < board.height)
                    {
                        if (board.allGems[x, y] != null)
                        {
                            board.allGems[x, y].isMatched = true;
                            matches.Add(board.allGems[x, y]);
                        }
                    }
                }
            }
        }
        if (matches.Count > 0)
        {
            matches = matches.Distinct().ToList();
        }
    }
}
