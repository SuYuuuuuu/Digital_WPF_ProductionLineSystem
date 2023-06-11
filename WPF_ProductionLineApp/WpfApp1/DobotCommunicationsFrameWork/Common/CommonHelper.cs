using Google.Protobuf;
using System.IO;
using System.Text;

namespace WpfProductionLineApp.DobotCommunicationsFrameWork.Common
{
    public static class CommonHelper
    {
        /// <summary>
        /// 字节数组转16进制字符串（含空格）
        /// </summary>
        /// <param name="byteDatas"></param>
        /// <returns></returns>
        public static string ToHexStringWithSpaceFromByte(byte[] byteDatas)
        {
            if (byteDatas == null || byteDatas.Length == 0)
            {
                return "";
            }
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < byteDatas.Length; i++)
            {
                builder.Append(string.Format("{0:X2} ", byteDatas[i]));
            }
            return builder.ToString().Trim();
        }

        /// <summary>
        /// 字节数组转16进制字符串(不含空格)
        /// </summary>
        /// <param name="byteDatas"></param>
        /// <returns></returns>
        public static string ToHexStringWithoutSpaceFromByte(byte[] byteDatas)
        {
            if (byteDatas == null || byteDatas.Length == 0)
            {
                return "";
            }
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < byteDatas.Length; i++)
            {
                builder.Append(string.Format("{0:X2}", byteDatas[i]));
            }
            return builder.ToString().Trim();
        }

        //--16进制字符串(含空格)转字节数组----Convert.FromHexString(hexString.Replace(" ",""));
        //---16进制字符串(不含空格)转字节数组-----Convert.FromHexString(hexString);


        public static byte[] Serialize(DobotNonRealTimeData data)
        {
            using (MemoryStream output = new MemoryStream())
            {
                data.WriteTo(output);
                return output.ToArray();
            }

        }


        public static byte[] Serialize(DobotRealTimeData data)
        {
            using (MemoryStream output = new MemoryStream())
            {
                data.WriteTo(output);
                return output.ToArray();
            }

        }

        public static byte[] Serialize(InputSignal data)
        {
            using (MemoryStream output = new MemoryStream())
            {
                data.WriteTo(output);
                return output.ToArray();
            }

        }

        public static byte[] Serialize(OutputSignal data)
        {
            using (MemoryStream output = new MemoryStream())
            {
                data.WriteTo(output);
                return output.ToArray();
            }

        }

        public static DobotNonRealTimeData DeserializeNonRealTimeData(byte[] data)
        {
            DobotNonRealTimeData dobotData = DobotNonRealTimeData.Parser.ParseFrom(data);
            return dobotData;
        }


        public static DobotRealTimeData DeserializeRealTimeData(byte[] data)
        {
            DobotRealTimeData dobotData = DobotRealTimeData.Parser.ParseFrom(data);
            return dobotData;
        }


    }
}
