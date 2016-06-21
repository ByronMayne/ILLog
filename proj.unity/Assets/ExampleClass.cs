using UnityEngine;
using System.Collections;
using ILLog;

public class ExampleClass : MonoBehaviour
{
  [ILLog]
  public void Start()
  {

  }

  [ILLog]
  public void Awake()
  {

    transform.position = Vector3.zero;
  }

  [ILLog(ILLogTypes.Once)]
  public void Update()
  {

    throw new System.Exception("FUCK");
  }
}
