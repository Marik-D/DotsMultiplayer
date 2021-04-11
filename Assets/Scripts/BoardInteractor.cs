using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;

public class BoardInteractor : MonoBehaviour
{
    public GameObject redDotPrefab;
    public GameObject blueDotPrefab;
    public GameObject debugDotPrefab;
    
    public GameObject redCapturePrefab;

    private BoardState _state;
    private SpriteRenderer _spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        var gridSize = _spriteRenderer.size / 0.64f;
        _state = new BoardState((int)gridSize.y, (int)gridSize.x);
        
        SyncBoardState();
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
        for (int i = 0; i < _state.Rows; i++)
        {
            for (int j = 0; j < _state.Cols; j++)
            {
                var cell = _state.Get(i, j);
                if (cell.IsPlaced)
                {
                    var dotLocation = GetOnScreenLocation(i, j);
                    Instantiate(cell.Player == Player.Red ? redDotPrefab : blueDotPrefab, transform.TransformPoint(dotLocation), Quaternion.identity, transform);
                }
            }
        }
        
        foreach (var capture in _state.Captures)
        {
            var obj = Instantiate(redCapturePrefab, transform.position, Quaternion.identity, transform);
            var shape = obj.GetComponent<SpriteShapeController>();
            shape.spline.Clear();
            for (int i = 0; i < capture.Points.Points.Count; i++)
            {
                var (row, col) = capture.Points.Points[i];
                shape.spline.InsertPointAt(i, GetOnScreenLocation(row, col));
            }
        }

        if (_state.Captures.Count > 0)
        {
            var last = _state.Captures.Last();
            var (i, j) = _state.GetPointInside(last.Points);
            
            var dotLocation = GetOnScreenLocation(i, j);
            Instantiate(debugDotPrefab, transform.TransformPoint(dotLocation), Quaternion.identity, transform);
        }
    }

    private Vector2 GetOnScreenLocation(int row, int col)
    {
        return new Vector2(col + 1, row + 1) * 0.64f - _spriteRenderer.size / 2;
    }
}
