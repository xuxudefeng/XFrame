using NModbus;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;

public class ModbusHelper
{
    /// <summary>
    /// 读取遥信的值
    /// </summary>
    public static void ModbusTcpMasterReadCoils()
    {
        using (TcpClient client = new TcpClient("192.168.40.42", 502))
        {
            var factory = new ModbusFactory();
            IModbusMaster master = factory.CreateMaster(client);

            ushort startAddress = 1;
            ushort numInputs = 2000;
            bool[] coils = master.ReadCoils(1, startAddress, numInputs);

            for (int i = 0; i < numInputs; i++)
            {
                Debug.Log($"Coils {(startAddress + i)}={(coils[i] ? 1 : 0)}");
            }
        }
    }
    /// <summary>
    /// 读取遥测的值
    /// </summary>
    public static void ModbusTcpMasterReadHoldingRegisters()
    {
        using (TcpClient client = new TcpClient("192.168.40.42", 502))
        {
            var factory = new ModbusFactory();
            IModbusMaster master = factory.CreateMaster(client);

            ushort startAddress = 1;
            ushort numberOfPoints = 100 * 2;
            ushort[] ushortArray = master.ReadHoldingRegisters(1, startAddress, numberOfPoints);
            
            var result = ConvertUshortArrayToFloatArray(ushortArray);

            foreach (var item in result)
            {
                Debug.Log(item);
            }
        }
    }


    public static float GetSingle(ushort lowOrderValue, ushort hightOrderValue)
    {
        var test = BitConverter.GetBytes(lowOrderValue).Concat(BitConverter.GetBytes(hightOrderValue)).ToArray();
        return BitConverter.ToSingle(test, 0);
    }
    public static float[] ConvertUshortArrayToFloatArray(ushort[] pValues)
    {
        List<float> floatList = new List<float>();
        for (int i = 0; i < pValues.Length;)
        {
            float temp = ModbusHelper.GetSingle(pValues[i++], pValues[i++]);
            floatList.Add(temp);
        }
        return floatList.ToArray();
    }

    /// <summary>
    /// 获取float类型数据
    /// </summary>
    /// <param name="src"></param>
    /// <param name="start"></param>
    /// <returns></returns>
    public static float GetReal(ushort[] src, int start)
    {
        ushort[] temp = new ushort[2];
        for (int i = 0; i < 2; i++)
        {
            temp[i] = src[i + start];
        }
        byte[] bytesTemp = Ushorts2Bytes(temp);
        float res = BitConverter.ToSingle(bytesTemp, 0);
        return res;
    }

    public static byte[] Ushorts2Bytes(ushort[] src, bool reverse = false)
    {

        int count = src.Length;
        byte[] dest = new byte[count << 1];
        if (reverse)
        {
            for (int i = 0; i < count; i++)
            {
                dest[i * 2] = (byte)(src[i] >> 8);
                dest[i * 2 + 1] = (byte)(src[i] >> 0);
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                dest[i * 2] = (byte)(src[i] >> 0);
                dest[i * 2 + 1] = (byte)(src[i] >> 8);
            }
        }
        return dest;
    }
}
