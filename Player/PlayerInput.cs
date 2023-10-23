using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    //加一个注解，让它在Unity3D里面可调试
    [SerializeField]
    //定义一个变量，调试速度，看看把速度扩大到多少倍
    private float speed = 5f;
    [SerializeField]
    //把文件引用拿过来在unity3d里面把对应文件移过来
    private PlayerController controller;
    //鼠标灵敏度
    [SerializeField]
    private float lookSensitivity = 8f;

    [SerializeField]
    private float thrusterForce = 25f;

    //记录中心离碰撞检测的距离
    private float distToGround = 0f;
    

    // Start is called before the first frame update
    void Start()
    {
        //一瞬间执行一次
     Cursor.lockState = CursorLockMode.Locked;//锁住鼠标
        //一开始赋值的写法,找到组件,多个的话会随机取
        //获取长度
        distToGround = GetComponent<Collider>().bounds.extents.y;
    }

    // Update is called once per frame
    void Update()
    {
        //每画一次图片之前对应的函数，每一帧的位置画在里面计算,用户输入也可以放在这里面获取
        float xMov = Input.GetAxisRaw("Horizontal");//获取水平方向的值，同时get函数里面的值可以通过在unity3d里面Edit->project setting->Input Manager->Axes里面获取
        float yMov = Input.GetAxisRaw("Vertical");
        //计算移动速度,是一个向量，速度x，y两方向叠加,vector3是一个三维向量
        Vector3 velocity = (transform.right * xMov + transform.forward * yMov).normalized * speed;//transform找到项目名的transform根据他来改,.normalized标准化，让速度为1
        controller.Move(velocity);//可以移动,传值

        //获取鼠标移动,不加row的话会平滑一些
        float xMouse = Input.GetAxisRaw("Mouse X");
        float yMouse = Input.GetAxisRaw("Mouse Y");
        //调试
        //print(xMouse.ToString() + " " + yMouse.ToString());
        //水平移动,x的动向对应y,看untiy3D里面哪个轴的颜色是主要的变化项
        //竖直变化一般取反
        Vector3 yRotation = new Vector3(0f, xMouse, 0f) * lookSensitivity;
        Vector3 xRotation = new Vector3(-yMouse,0f, 0f) * lookSensitivity;
        //传过去
        controller.Rotate(yRotation, xRotation);

        
        if (Input.GetButton("Jump"))
        {
            //射线碰撞检测，发出一个射线如果可以和碰撞面接触则可以跳
            if (Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f))
                {
                Vector3 force = Vector3.up * thrusterForce;
                //需要重新赋值
                controller.Thrust(force);//跳跃
            }
            
        }
       
        
    }
}
