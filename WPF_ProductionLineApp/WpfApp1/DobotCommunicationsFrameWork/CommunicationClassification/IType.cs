using System;

namespace WpfProductionLineApp.DobotCommunicationsFrameWork
{
    interface IType    //类型类的基类(因为一个id有可能对应两个指令，分别返回不同类型)
    {
        Type GetZeroType();//获取第一个模式的类型
        Type GetOneType();//获取第二个模式的类型
    }
}
