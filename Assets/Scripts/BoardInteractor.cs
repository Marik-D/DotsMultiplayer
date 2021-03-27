using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UIElements;

public class BoardInteractor : MonoBehaviour
{
    public GameObject redDotPrefab;
    public GameObject blueDotPrefab;

    private BoardState _state;
    private SpriteRenderer _spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        var gridSize = _spriteRenderer.size / 0.64f;
        _state = new BoardState((int)gridSize.y, (int)gridSize.x);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            var cursorWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cursorWorldPos.z = 0;

            var cursorLocalPos = transform.InverseTransformPoint(cursorWorldPos);

            var cellIdx = ((Vector2)cursorLocalPos + _spriteRenderer.size / 2) / 0.64f;

            int cellCol = Mathf.RoundToInt(cellIdx.x) - 1;
            int cellRow = Mathf.RoundToInt(cellIdx.y) - 1;

            if (_state.CanPlace(cellRow, cellCol))
            {
                Debug.Log($"Placing at row={cellRow} col={cellCol}");
                _state.Place(cellRow, cellCol);
                
                SyncBoardState();
            } 
        }
    }

    void SyncBoardState()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);                
        }
        for (int i = 0; i < _state.rows; i++)
        {
            for (int j = 0; j < _state.cols; j++)
            {
                var cell = _state.Get(i, j);
                if (cell.IsPlaced)
                {
                    var dotLocation = new Vector2(j + 1, i + 1) * 0.64f - _spriteRenderer.size / 2;
                    Instantiate(cell.Player == Player.Red ? redDotPrefab : blueDotPrefab, transform.TransformPoint(dotLocation), Quaternion.identity, transform);
                }
            }
        }
    }
}
