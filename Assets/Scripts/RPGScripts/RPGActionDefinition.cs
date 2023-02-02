using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RPGActionDefinition : MonoBehaviour
{
    [SerializeField] GameObject ui;
    [SerializeField] TMP_Text h;

   
    private void Update()
    {
        if (ui == null && h == null)
        {
            if (gameObject.TryGetComponent<RPGVisualPiece>(out RPGVisualPiece rv))
            {
                if (rv.PieceColor == UnityChess.Side.Black)
                {
                    ui = RPGGameManager.Instance.bui;
                    h  = RPGGameManager.Instance.bh;
                }
                else
                {
                    ui = RPGGameManager.Instance.wui;
                    h = RPGGameManager.Instance.wh;
                }
        }
        }

        h.text = "HP- " + gameObject.GetComponent<Health>().HitPoints;
    }

    public bool InteractWithPiece(int actionValue, GameObject go)
    {
        if (go.TryGetComponent<Health>(out Health h))
        {
            ui.SetActive(false);
            return 0 >= h.Damage(actionValue);
        }

        return false;
    }

    public void uiSetActive()
    {
        ui.SetActive(true);
    }
}
