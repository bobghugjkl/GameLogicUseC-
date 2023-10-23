using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    //��һ��ע�⣬������Unity3D����ɵ���
    [SerializeField]
    //����һ�������������ٶȣ��������ٶ����󵽶��ٱ�
    private float speed = 5f;
    [SerializeField]
    //���ļ������ù�����unity3d����Ѷ�Ӧ�ļ��ƹ���
    private PlayerController controller;
    //���������
    [SerializeField]
    private float lookSensitivity = 8f;

    [SerializeField]
    private float thrusterForce = 25f;

    //��¼��������ײ���ľ���
    private float distToGround = 0f;
    

    // Start is called before the first frame update
    void Start()
    {
        //һ˲��ִ��һ��
     Cursor.lockState = CursorLockMode.Locked;//��ס���
        //һ��ʼ��ֵ��д��,�ҵ����,����Ļ������ȡ
        //��ȡ����
        distToGround = GetComponent<Collider>().bounds.extents.y;
    }

    // Update is called once per frame
    void Update()
    {
        //ÿ��һ��ͼƬ֮ǰ��Ӧ�ĺ�����ÿһ֡��λ�û����������,�û�����Ҳ���Է����������ȡ
        float xMov = Input.GetAxisRaw("Horizontal");//��ȡˮƽ�����ֵ��ͬʱget���������ֵ����ͨ����unity3d����Edit->project setting->Input Manager->Axes�����ȡ
        float yMov = Input.GetAxisRaw("Vertical");
        //�����ƶ��ٶ�,��һ���������ٶ�x��y���������,vector3��һ����ά����
        Vector3 velocity = (transform.right * xMov + transform.forward * yMov).normalized * speed;//transform�ҵ���Ŀ����transform����������,.normalized��׼�������ٶ�Ϊ1
        controller.Move(velocity);//�����ƶ�,��ֵ

        //��ȡ����ƶ�,����row�Ļ���ƽ��һЩ
        float xMouse = Input.GetAxisRaw("Mouse X");
        float yMouse = Input.GetAxisRaw("Mouse Y");
        //����
        //print(xMouse.ToString() + " " + yMouse.ToString());
        //ˮƽ�ƶ�,x�Ķ����Ӧy,��untiy3D�����ĸ������ɫ����Ҫ�ı仯��
        //��ֱ�仯һ��ȡ��
        Vector3 yRotation = new Vector3(0f, xMouse, 0f) * lookSensitivity;
        Vector3 xRotation = new Vector3(-yMouse,0f, 0f) * lookSensitivity;
        //����ȥ
        controller.Rotate(yRotation, xRotation);

        
        if (Input.GetButton("Jump"))
        {
            //������ײ��⣬����һ������������Ժ���ײ��Ӵ��������
            if (Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f))
                {
                Vector3 force = Vector3.up * thrusterForce;
                //��Ҫ���¸�ֵ
                controller.Thrust(force);//��Ծ
            }
            
        }
       
        
    }
}
