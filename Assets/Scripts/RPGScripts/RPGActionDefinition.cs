using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGActionDefinition : MonoBehaviour
{
    bool InteractWithPiece(int actionValue, GameObject pieceBoi)
    {
        if (TryGetComponent<Health>(out Health h))
        {
            return 0 <= h.Damage(actionValue);
        }

        return false;
    }
}
