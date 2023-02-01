using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityChess;

public class RPGPiece 
{
    public string name = "";
    public Side Owner { get; protected set; }

    public RPGPiece(string nameToBe, Side NewOwner)
    {
        name = nameToBe;
        Owner = NewOwner;
    }
}
