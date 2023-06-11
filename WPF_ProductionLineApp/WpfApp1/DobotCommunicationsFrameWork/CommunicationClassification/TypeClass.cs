using System;

namespace WpfProductionLineApp.DobotCommunicationsFrameWork.CommunicationClassification
{
    class Chars : IType
    {
        public Type GetOneType()
        {
            return typeof(char[]);
        }
        public Type GetZeroType()
        {
            return null;
        }
    }

    class Uint64s : IType
    {
        public Type GetOneType()
        {
            return typeof(ulong[]);
        }
        public Type GetZeroType()
        {
            return null;
        }
    }

    class Uint32s : IType
    {
        public Type GetOneType()
        {
            return typeof(UInt32[]);
        }
        public Type GetZeroType()
        {
            return null;
        }
    }

    class Uint8s : IType
    {
        public Type GetOneType()
        {
            return typeof(byte[]);
        }
        public Type GetZeroType()
        {
            return null;
        }
    }

    class FloatAndUint8 : IType
    {
        public Type GetOneType()
        {
            return typeof(byte[]);
        }
        public Type GetZeroType()
        {
            return typeof(float[]);
        }
    }


    class Floats : IType
    {
        public Type GetOneType()
        {
            return typeof(float[]);
        }
        public Type GetZeroType()
        {
            return null;
        }
    }

    class Uint64AndFloat : IType
    {
        public Type GetOneType()
        {
            return typeof(float[]);
        }
        public Type GetZeroType()
        {
            return typeof(ulong[]);
        }
    }

    class Uint64AndUint8 : IType
    {
        public Type GetOneType()
        {
            return typeof(byte[]);
        }
        public Type GetZeroType()
        {
            return typeof(ulong[]);
        }
    }
}
