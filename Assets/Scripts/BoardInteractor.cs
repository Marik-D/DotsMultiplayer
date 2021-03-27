using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BoardInteractor : MonoBehaviour
{
    public GameObject redDotPrefab;
    public GameObject blueDotPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            var cursorWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cursorWorldPos.z = 0;

            
            var cursorLocalPos = transform.InverseTransformPoint(cursorWorldPos);
            var spriteSize = GetComponent<SpriteRenderer>().size;
            var gridSize = spriteSize / 0.64f;

            var cellIdx = ((Vector2)cursorLocalPos + spriteSize / 2) / 0.64f;

            int cellCol = Mathf.RoundToInt(cellIdx.x) - 1;
            int cellRow = Mathf.RoundToInt(cellIdx.y) - 1;

            if (cellCol >= 0 && cellCol < gridSize.x - 2 && cellRow >= 0 && cellRow < gridSize.y - 2)
            {
                Debug.Log($"Placing at row={cellRow} col={cellCol}");
                
                var dotLocation = new Vector2(cellCol + 1, cellRow + 1) * 0.64f - spriteSize / 2;
                
                Instantiate(redDotPrefab, transform.TransformPoint(dotLocation), Quaternion.identity, transform);
            } 
        }
    }
}
