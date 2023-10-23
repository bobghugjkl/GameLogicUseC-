using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField]
    //�������ֵ
    private int maxHealth = 100;

    [SerializeField]
    //�����״̬
    private Behaviour[] componentsToDisable;
    private bool[] componentsEnabled;
    //collider�Ľ���״��
    private bool colliderEnabled;

    //��ǰ����ֵ,�Զ�ͬ�����д��ڵ�ֵ,��ʼ��
    private NetworkVariable<int> currentHealth = new NetworkVariable<int>();
    private NetworkVariable<bool> isDead = new NetworkVariable<bool>();
    
    //��ʼ��
    public void Setup()
    {
        componentsEnabled = new bool[componentsToDisable.Length];
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsEnabled[i] = componentsToDisable[i].enabled;
        }
        Collider col = GetComponent<Collider>();
        colliderEnabled = col.enabled;

        SetDefaults();

    }
    private void SetDefaults()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = componentsEnabled[i];
        }
        Collider col = GetComponent<Collider>();
        col.enabled = colliderEnabled;
        //ֻ�ڷ��������޸�
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
            isDead.Value = false;
        }
    }
    //�ܵ�����
    public void TakeDamage(int damage)
    {
        if (isDead.Value)
        {
            return;
        }
        currentHealth.Value -= damage;
        if (currentHealth.Value <= 0)
        {
            currentHealth.Value = 0;
            isDead.Value = true;

            if (!IsHost)
            {
                DieOnServer();
            }

            
            DieClientRpc();
        }
    }
    private IEnumerator Respawn()  // �������ӳٲ���
    {
        //�첽����
        yield return new WaitForSeconds(GameManager.Singleton.MatchingSettings.respawnTime);

        SetDefaults();
        GetComponentInChildren<Animator>().SetInteger("direction", 0);
        GetComponent<Rigidbody>().useGravity = true;
        
        //ֻ�ڷ������˸�������
        if (IsLocalPlayer)
        {
            transform.position = new Vector3(0f, 10f, 0f);
        }
    }
    public bool IsDead()
    {
        return isDead.Value;
    }
    private void DieOnServer()
    {
        Die();
    }

    [ClientRpc]
    private void DieClientRpc()
    {
        Die();
    }

    private void Die()
    {
        GetComponentInChildren<Animator>().SetInteger("direction", -1);
        GetComponent<Rigidbody>().useGravity = false;
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
        Collider col = GetComponent<Collider>();
        col.enabled = false;
        //��һ�����߳�ִ�к���
        StartCoroutine(Respawn());
    }

    
    public int GetHealth()
    {
        return currentHealth.Value;
    }
}