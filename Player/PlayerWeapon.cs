using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//���Դ��л�
[Serializable]
public class PlayerWeapon
{
    public string name = "M16";  
    public int damage = 15;
    public float range = 100f;


    public float shootRate = 10f;//һ��ʮ��,<=0Ϊ����
    public float shootCoolDownTime = 0.75f;//Ϊ����ģʽʱ��ȴʱ��
    public float recoilForce = 2f;//������
    //������������
    public GameObject graphics;
}
