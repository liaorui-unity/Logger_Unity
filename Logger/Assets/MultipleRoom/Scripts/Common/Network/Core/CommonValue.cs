using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fs
{
    /// <summary>
    /// 通用存储类型
    /// NOTE:内部会进行装箱拆箱操作
    /// @author hannibal
    /// @time 2016-5-2
    /// </summary>
    public class CommonValue
    {
        enum CommonValueType
        {
            CommonValueType_None,       //没有类型
            CommonValueType_Char,       //char类型
            CommonValueType_Int,        //int类型
            CommonValueType_Float,      //float类型
            CommonValueType_String,     //string类型
        }
        private CommonValueType m_ValueType;
        public string m_StrValue;

        public CommonValue()
        {

        }

        public CommonValue(System.Object val)
        {
            this.SetValue(val);
        }

        /// <summary>
        ///重置数据
        /// </summary>
        public void Reset()
        {
            m_ValueType = CommonValueType.CommonValueType_None;
            m_StrValue = "";
        }
        #region #设置数据#
        public void SetValue(byte val) { m_StrValue = val.ToString(); m_ValueType = CommonValueType.CommonValueType_Int; }
        public void SetValue(sbyte val) { m_StrValue = val.ToString(); m_ValueType = CommonValueType.CommonValueType_Int; }
        public void SetValue(bool val) { m_StrValue = val ? "1" : "0"; m_ValueType = CommonValueType.CommonValueType_Int; }
        public void SetValue(short val) { m_StrValue = val.ToString(); m_ValueType = CommonValueType.CommonValueType_Int; }
        public void SetValue(ushort val) { m_StrValue = val.ToString(); m_ValueType = CommonValueType.CommonValueType_Int; }
        public void SetValue(int val) { m_StrValue = val.ToString(); m_ValueType = CommonValueType.CommonValueType_Int; }
        public void SetValue(uint val) { m_StrValue = val.ToString(); m_ValueType = CommonValueType.CommonValueType_Int; }
        public void SetValue(long val) { m_StrValue = val.ToString(); m_ValueType = CommonValueType.CommonValueType_Int; }
        public void SetValue(ulong val) { m_StrValue = val.ToString(); m_ValueType = CommonValueType.CommonValueType_Int; }
        public void SetValue(float val) { m_StrValue = val.ToString(); m_ValueType = CommonValueType.CommonValueType_Float; }
        public void SetValue(double val) { m_StrValue = val.ToString(); m_ValueType = CommonValueType.CommonValueType_Float; }
        public void SetValue(string val) { m_StrValue = val; m_ValueType = CommonValueType.CommonValueType_String; }

        public void SetValue(int a, int b)
        {
            byte[] values = new byte[8];
            ByteUtils.UInt32ToByte4((uint)a, out values[0], out  values[1], out  values[2], out  values[3]);
            ByteUtils.UInt32ToByte4((uint)b, out values[4], out values[5], out  values[6], out  values[7]);
            SetValue(System.BitConverter.ToInt64(values, 0));
        }

        public void SetValue(System.Object val)
        {
            if (val == null)
                return;
            switch (val.GetType().ToString())
            {
                case "System.Decimal":
                case "System.Double":
                case "System.Single":
                    m_StrValue = val.ToString(); m_ValueType = CommonValueType.CommonValueType_Float; break;
                case "System.Char":
                    SetValue(System.Convert.ToChar(val)); m_ValueType = CommonValueType.CommonValueType_Char; break;
                case "System.Int16"://short
                case "System.UInt16"://ushort
                case "System.Int32"://int
                case "System.UInt32"://uint
                case "System.Int64"://long
                case "System.UInt64"://ulong
                case "System.Byte":
                case "System.SByte":
                    m_StrValue = val.ToString(); m_ValueType = CommonValueType.CommonValueType_Int; break;
                case "System.String"://string
                    m_StrValue = val as string; m_ValueType = CommonValueType.CommonValueType_String; break;
            }
        }
        #endregion

        #region #数据转换#
        public bool ToBool() { return ToInt64() == 0 ? false : true; }
        public short ToInt16() { return (short)ToInt64(); }
        public ushort ToUInt16() { return (ushort)ToUInt64(); }
        public int ToInt32() { return (int)ToInt64(); }
        public uint ToUInt32() { return (uint)ToUInt64(); }
        public float ToFloat() { return (float)ToDouble(); }
        public override string ToString() { return m_StrValue; }

        public double ToDouble()
        {
            if (string.IsNullOrEmpty(m_StrValue))
                return 0;
            try
            {
                if (m_ValueType == CommonValueType.CommonValueType_Int || m_ValueType == CommonValueType.CommonValueType_Char)
                {
                    return (double)Int64.Parse(m_StrValue);
                }
                if (m_ValueType == CommonValueType.CommonValueType_String)
                {
                    return double.Parse(m_StrValue);
                }
                return double.Parse(m_StrValue);
            }
            catch (Exception)
            {
                Debuger.LogError("ToDouble类型转换错误:" + m_StrValue);
                return 0;
            }
        }

        public long ToInt64()
        {
            if (string.IsNullOrEmpty(m_StrValue))
                return 0;
            try
            {
                if (m_ValueType == CommonValueType.CommonValueType_String)
                {
                    return Int64.Parse(m_StrValue);
                }
                if (m_ValueType == CommonValueType.CommonValueType_Float)
                {
                    return (long)Double.Parse(m_StrValue);
                }
                return Int64.Parse(m_StrValue);
            }
            catch (Exception)
            {
                Debuger.LogError("ToInt64类型转换错误:" + m_StrValue);
                return 0;
            }
        }

        public ulong ToUInt64()
        {
            if (string.IsNullOrEmpty(m_StrValue))
                return 0;
            try
            {
                if (m_ValueType == CommonValueType.CommonValueType_String)
                {
                    return UInt64.Parse(m_StrValue);
                }
                if (m_ValueType == CommonValueType.CommonValueType_Float)
                {
                    return (ulong)Double.Parse(m_StrValue);
                }
                return UInt64.Parse(m_StrValue);
            }
            catch (Exception)
            {
                Debuger.LogError("ToUInt64类型转换错误:" + m_StrValue);
                return 0;
            }
        }
        #endregion
    }
}