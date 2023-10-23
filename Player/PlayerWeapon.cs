using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//可以串行化
[Serializable]
public class PlayerWeapon
{
    public string name = "M16";  
    public int damage = 15;
    public float range = 100f;


    public float shootRate = 10f;//一秒十发,<=0为单发
    public float shootCoolDownTime = 0.75f;//为单发模式时冷却时间
    public float recoilForce = 2f;//后坐力
    //将对象引过来
    public GameObject graphics;
}
