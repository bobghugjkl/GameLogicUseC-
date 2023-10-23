using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    //改变物理属性，需要先获取属性
    private Rigidbody rb;//在Unity3D里面把相应的Rigidbody拖进去即可
    //旋转摄像机,别忘了这一步都需要回到Unity3D里面赋值
    [SerializeField]
    private Camera cam;



    private Vector3 velocity = Vector3.zero;//设置速度(每秒钟移动距离)
    //旋转分两个一个是摄像机一个是人体
    private Vector3 yRotation = Vector3.zero;//旋转角色
    private Vector3 xRotation = Vector3.zero;//旋转视角
    private float recoilForce = 0f;//后坐力
    //记录一下上一个的位置做差值
    private Vector3 lastFramePosition = Vector3.zero;

    private float eps = 0.01f;//精度，差值在多少范围内被认定相等
    private Vector3 thusterForce = Vector3.zero;//向上的推力
    private float cameraRoationTotal = 0f;//旋转了多少度
    [SerializeField]
    private float cameraRotationLimit = 85f;//最高85度
    private Animator animator;
    private float distToGround = 0f;
    private void Start()
    {
        lastFramePosition = transform.position;
        animator = GetComponentInChildren<Animator>();
        distToGround = GetComponent<Collider>().bounds.extents.y;
    }

    //需要将用户的值赋给velocity需要写一个赋值函数
    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }
    //旋转函数
    public void Rotate(Vector3 _yRotation, Vector3 _xRotation)
    {
        yRotation = _yRotation;
        xRotation = _xRotation;
    }
    //赋值函数，让他们可以获取值
    public void Thrust(Vector3 _thusterForce)
    {
        thusterForce = _thusterForce;
    }
    //施加后坐力API
    public void AddRecoilForce(float newRecoilForce)
    {
        recoilForce += newRecoilForce;
    }
    //旋转
    private void PerformRotation()
    {
        if (recoilForce < 0.1)
        {
            recoilForce = 0f;
        }

        if (yRotation != Vector3.zero||recoilForce >0)
        {
            //旋转角色
            //角度是一个四元素表示方法
            rb.transform.Rotate(yRotation+rb.transform.up* Random.Range(-2f*recoilForce , 2f*recoilForce));

        }
        if(xRotation != Vector3.zero||recoilForce >0)
        {
            //旋转视角
            cameraRoationTotal += xRotation.x - recoilForce;//累计转多少度
            cameraRoationTotal = Mathf.Clamp(cameraRoationTotal, -cameraRotationLimit, cameraRotationLimit);//一个夹逼函数，在最大值和最小值之间
            cam.transform.localEulerAngles = new Vector3(cameraRoationTotal, 0f, 0f);//直接把角度赋过来
        }
        //施加的力先快后慢
        recoilForce *= 0.5f;
    }
    //作用到物体上
    private void PerformMovement()
    {
        if (velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);//获取原来的位置加上速度向量乘时间相邻两次fixupdate的时间
        }
        if(thusterForce != Vector3.zero)
        {
           
            rb.AddForce(thusterForce);//作用Time.fixedDeltaTime秒
            thusterForce = Vector3.zero;//清空
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
                direction = 4;//右后
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
            direction = 3;//右
        }
        else if(right < -eps)
        {
            direction = 7;//左
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
        //如果作用的物体是刚体的话描述物理一般不用public和Update
        //Update()遵循定值的时间，但不一定均衡， FixedUpdate()严格执行，必均匀
        //需要让他朝向velocity方向移动一段距离
        if (IsLocalPlayer)
        {
            PerformMovement();//每一帧都要调用
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
