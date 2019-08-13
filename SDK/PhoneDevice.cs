using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 手机设备类，用于获取设备信息
/// </summary>
public class PhoneDevice {

    /// <summary>
    /// 类单例对象
    /// </summary>
    private static PhoneDevice instance = null;



    /// <summary>
    /// 获取单例的函数接口
    /// </summary>
    /// <returns></returns>
    public static PhoneDevice GetInstance()
    {
        return instance != null ? instance : instance = new PhoneDevice();
    }


    /// <summary>
    /// 获取设备类型
    /// </summary>
    /// <returns>返回设备类型的字符串格式</returns>
    public string GetDeviceType()
    {
        return SystemInfo.deviceType.ToString();
    }


    /// <summary>
    /// 获取设备名称
    /// </summary>
    /// <returns>设备名称字符串</returns>
    public string GetDeviceName()
    {
        return SystemInfo.deviceModel;
    }


    /// <summary>
    /// 获取当前屏幕比率   高/宽
    /// </summary>
    /// <returns>当前屏幕比率浮点值</returns>
    public float GetScreenRadioF()
    {
        return ((Screen.height * 1.0f) / Screen.width); 
    }


    /// <summary>
    /// 获取当前屏幕比率   高/宽
    /// </summary>
    /// <returns>当前屏幕比率的整型值</returns>
    public int GetScreenRadioI()
    {
        return (int)GetScreenRadioF();
    }
    

   

}
