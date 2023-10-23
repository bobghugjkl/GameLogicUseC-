using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    //�ı��������ԣ���Ҫ�Ȼ�ȡ����
    private Rigidbody rb;//��Unity3D�������Ӧ��Rigidbody�Ͻ�ȥ����
    //��ת�����,��������һ������Ҫ�ص�Unity3D���渳ֵ
    [SerializeField]
    private Camera cam;



    private Vector3 velocity = Vector3.zero;//�����ٶ�(ÿ�����ƶ�����)
    //��ת������һ���������һ��������
    private Vector3 yRotation = Vector3.zero;//��ת��ɫ
    private Vector3 xRotation = Vector3.zero;//��ת�ӽ�
    private float recoilForce = 0f;//������
    //��¼һ����һ����λ������ֵ
    private Vector3 lastFramePosition = Vector3.zero;

    private float eps = 0.01f;//���ȣ���ֵ�ڶ��ٷ�Χ�ڱ��϶����
    private Vector3 thusterForce = Vector3.zero;//���ϵ�����
    private float cameraRoationTotal = 0f;//��ת�˶��ٶ�
    [SerializeField]
    private float cameraRotationLimit = 85f;//���85��
    private Animator animator;
    private float distToGround = 0f;
    private void Start()
    {
        lastFramePosition = transform.position;
        animator = GetComponentInChildren<Animator>();
        distToGround = GetComponent<Collider>().bounds.extents.y;
    }

    //��Ҫ���û���ֵ����velocity��Ҫдһ����ֵ����
    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }
    //��ת����
    public void Rotate(Vector3 _yRotation, Vector3 _xRotation)
    {
        yRotation = _yRotation;
        xRotation = _xRotation;
    }
    //��ֵ�����������ǿ��Ի�ȡֵ
    public void Thrust(Vector3 _thusterForce)
    {
        thusterForce = _thusterForce;
    }
    //ʩ�Ӻ�����API
    public void AddRecoilForce(float newRecoilForce)
    {
        recoilForce += newRecoilForce;
    }
    //��ת
    private void PerformRotation()
    {
        if (recoilForce < 0.1)
        {
            recoilForce = 0f;
        }

        if (yRotation != Vector3.zero||recoilForce >0)
        {
            //��ת��ɫ
            //�Ƕ���һ����Ԫ�ر�ʾ����
            rb.transform.Rotate(yRotation+rb.transform.up* Random.Range(-2f*recoilForce , 2f*recoilForce));

        }
        if(xRotation != Vector3.zero||recoilForce >0)
        {
            //��ת�ӽ�
            cameraRoationTotal += xRotation.x - recoilForce;//�ۼ�ת���ٶ�
            cameraRoationTotal = Mathf.Clamp(cameraRoationTotal, -cameraRotationLimit, cameraRotationLimit);//һ���бƺ����������ֵ����Сֵ֮��
            cam.transform.localEulerAngles = new Vector3(cameraRoationTotal, 0f, 0f);//ֱ�ӰѽǶȸ�����
        }
        //ʩ�ӵ����ȿ����
        recoilForce *= 0.5f;
    }
    //���õ�������
    private void PerformMovement()
    {
        if (velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);//��ȡԭ����λ�ü����ٶ�������ʱ����������fixupdate��ʱ��
        }
        if(thusterForce != Vector3.zero)
        {
           
            rb.AddForce(thusterForce);//����Time.fixedDeltaTime��
            thusterForce = Vector3.zero;//���
        }
        

    }
    private void PerformAnimation()
    {
        Vector3 deltaPosition = transform.position - lastFramePosition;
        lastFramePosition = transform.position;

        float forward = Vector3.Dot(deltaPosition, transform.forward);
        float right = Vector3.Dot(deltaPosition, transform.right);
        int direction = 0;
        if(forward > eps)
        {
            direction = 1;
        }
        else if (forward < -eps)
        {
            if(right > eps)
            {
                direction = 4;//�Һ�
            }
            else if(right < -eps)
            {
                direction = 6;
            }
            else
            {
                direction = 5;
            }
        }
        else if(right>eps)
        {
            direction = 3;//��
        }
        else if(right < -eps)
        {
            direction = 7;//��
        }
        if ((!Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f))){
            direction = 8;

        }
        {
            
        }
        if (GetComponent<Player>().IsDead())
        {
            direction = -1;
        }
        animator.SetInteger("direction", direction);
    }


    /*
    private void FixedUpdate()
    {
        //������õ������Ǹ���Ļ���������һ�㲻��public��Update
        //Update()��ѭ��ֵ��ʱ�䣬����һ�����⣬ FixedUpdate()�ϸ�ִ�У��ؾ���
        //��Ҫ��������velocity�����ƶ�һ�ξ���
        if (IsLocalPlayer)
        {
            PerformMovement();//ÿһ֡��Ҫ����
            PerformRotation();
            //PerformAnimation();
        }
        if (IsLocalPlayer)
        {
            PerformAnimation();
        }

    }
    private void Update()
    {
        if (!IsLocalPlayer)
        {
            PerformAnimation();
        }
       
    }*/
    private void FixedUpdate()
    {
        if (IsLocalPlayer)
        {
            PerformMovement();
            PerformRotation();
        }

        if (IsLocalPlayer)
        {
            PerformAnimation();
        }
    }

    private void Update()
    {
        if (!IsLocalPlayer)
        {
            PerformAnimation();
        }
    }

    
}
