using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModbusTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //ModbusHelper.ModbusTcpMasterReadHoldingRegisters();
        ushort[] temp = new ushort[2];
        temp[0] = 49808;
        temp[1] = 15733;


        Debug.Log(ModbusHelper.GetSingle(temp[0], temp[1]));

        var test = BitConverter.GetBytes(temp[0]).Concat(BitConverter.GetBytes(temp[1])).ToArray();

        Debug.Log(BitConverter.ToSingle(test, 0));

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
