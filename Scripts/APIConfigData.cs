using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "API Data", menuName = "Data/API", order = 1)]
public class APIConfigData : ScriptableObject
{
    public List<com.wao.core.APIDefine> aPIDefines;
}
