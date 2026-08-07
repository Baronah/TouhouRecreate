using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveDataManager : MonoBehaviour
{
    public static SaveDataManager _instance;

    StreamReader streamReader;

    private void Awake()
    {
        _instance = this;
        streamReader = new StreamReader("data.dat");
    }
}
