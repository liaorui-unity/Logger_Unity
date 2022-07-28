using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Resources;

namespace fs
{
    /// <summary>  
    /// 读取CSV工具类  
    /// （需求：UTF-8格式）  
    /// </summary>  
    public class CSVColumElement
    {
        private string elementValue;
        public string Value
        {
            get { return elementValue; }
            set { elementValue = value; }
        }

        #region 转换数据类型
        public override string ToString() { return elementValue; }
        public bool ToBool() { return ToInt64() != 0 ? true : false; }
        public byte ToByte() { return (byte)ToInt64(); }
        public sbyte ToSByte() { return (sbyte)ToInt64(); }
        public char ToChar() { return (char)ToInt64(); }
        public short ToInt16() { return (short)ToInt64(); }
        public ushort ToUInt16() { return (ushort)ToInt64(); }
        public int ToInt32() { return (int)ToInt64(); }
        public uint ToUInt32() { return (uint)ToInt64(); }
        public long ToInt64()
        {
            if (string.IsNullOrEmpty(elementValue)) return 0;
            long ret;
            if (long.TryParse(elementValue, out ret))
                return ret;
            else
            {
                Debuger.LogError("ToInt64类型转换错误:" + elementValue);
                return 0;
            }
        }
        public ulong ToUInt64()
        {
            if (string.IsNullOrEmpty(elementValue)) return 0;
            ulong ret;
            if (ulong.TryParse(elementValue, out ret))
                return ret;
            else
            {
                Debuger.LogError("ToUInt64类型转换错误:" + elementValue);
                return 0;
            }
        }
        public float ToFloat()
        {
            if (string.IsNullOrEmpty(elementValue)) return 0;
            float ret;
            if (float.TryParse(elementValue, out ret))
                return ret;
            else
            {
                Debuger.LogError("ToFloat类型转换错误:" + elementValue);
                return 0;
            }
        }
        public double ToDouble()
        {
            if (string.IsNullOrEmpty(elementValue)) return 0;
            double ret;
            if (double.TryParse(elementValue, out ret))
                return ret;
            else
            {
                Debuger.LogError("ToDouble类型转换错误:" + elementValue);
                return 0;
            }
        }
        #endregion

        /// <summary>
        /// 拷贝文件
        /// </summary>
        public CSVColumElement CopyTo()
        {
            CSVColumElement ele = new CSVColumElement();
            ele.Value = elementValue;
            return ele;
        }
    }
    /// <summary>
    /// 读取csv文件
    /// </summary>
    public class CSVLoadData
    {
        private List<List<string>> mDocumentText = null;                     //存储读取的文件信息
        private List<string> mDocumentColumNameList = null;                  //表头名称
        public int mRowNum;                                                  //行数量
        public int mColNum;                                                  //列数量
        public static Encoding mDefaultEncoding = System.Text.Encoding.UTF8; //CSV文件通用UTF8格式

        public void Clear()
        {
            if (mDocumentColumNameList != null)
            {
                mDocumentColumNameList.Clear();
            }
            if (mDocumentText != null)
            {
                mDocumentText.Clear();
            }
        }

        /// <summary>
        /// 加载文件，文本长度<=1时返回false，否则返回true
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool Load(string text)
        {
            if (text.Length <= 1)
                return false;
            //先清除当前所有数据
            mDocumentText = new List<List<string>>();
            mDocumentColumNameList = new List<string>();
            mRowNum = 0;
            mColNum = 0;

            text = text.Replace("\n", "");
            string[] lineArray = text.Split('\r');
            bool rowSkip = false;
            for (int i = 0; i < lineArray.Length; i++)
            {
                string columnText = lineArray[i];
                if (columnText.Length >= 2 && columnText[0] == '/' && columnText[1] == '/')
                    continue;	//如果头两个字符 是  //  代表该行无效
                if (columnText.Length >= 2 && columnText[0] == '\\' && columnText[1] == '\\')
                    continue;	//如果头两个字符 是  \\  代表该行无效
                if (columnText.Length >= 2 && columnText[0] == ',' && columnText[1] == ',')
                    continue;   //如果头两个字符都是‘，’表示该行数据无效
                if (columnText.Length > 2 && columnText[0] == '"' && columnText[1] == '/' && columnText[2] == '/')
                    continue;	//如果头三个字符是"// 也认为该行无效
                if (columnText.Length >= 2 && columnText[0] == '/' && columnText[1] == '*')
                {
                    rowSkip = true;
                    continue;	//如果头三个字符是"// 也认为该行无效
                }
                if (rowSkip && columnText.Length >= 2 && columnText[0] == '*' && columnText[1] == '/')
                {
                    rowSkip = false;
                    continue;	//如果头三个字符是"// 也认为该行无效
                }
                if (rowSkip)
                    continue;

                string[] columnArray = readLine(columnText);
                if (i == 0)
                {
                    mColNum = columnArray.Length;
                    //创建列
                    for (int j = 0; j < columnArray.Length; j++)
                    {
                        mDocumentColumNameList.Add(columnArray[j]);
                    }
                }
                else
                {
                    List<string> tempLineStringList = new List<string>();
                    for (int j = 0; j < columnArray.Length; j++)
                    {
                        tempLineStringList.Add(columnArray[j]);
                    }
                    //最后一行有可能为空
                    if (tempLineStringList.Count == 0 || (tempLineStringList.Count == 1 && string.IsNullOrEmpty(tempLineStringList[0])))
                        continue;
                    mDocumentText.Add(tempLineStringList);
                }
            }
            mRowNum = mDocumentText.Count;
            return true;
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private string[] readLine(string line)
        {
            var builder = new StringBuilder();
            var comma = false;
            var array = line.ToCharArray();
            var values = new List<string>();
            var length = array.Length;
            var index = 0;
            while (index < length)
            {
                var item = array[index++];
                switch (item)
                {
                    case ',':
                        if (comma)
                        {
                            builder.Append(item);
                        }
                        else
                        {
                            values.Add(builder.ToString());
                            builder.Remove(0, builder.Length);
                        }
                        break;
                    case '"':
                        comma = !comma;
                        break;
                    default:
                        builder.Append(item);
                        break;
                }
            }
            if (builder.Length > 0)
                values.Add(builder.ToString());
            return values.ToArray();
        }

        //返回CSV文档行数量
        public int numRows() { return mRowNum; }
        //返回CSV文档列数量
        public int numColumns() { return mColNum; }
        //获取列名索引
        public int getColumnIndex(string columnName)
        {
            return mDocumentColumNameList.IndexOf(columnName);
        }

        //读取第rowIndex行columnIndex列的数据
        static CSVColumElement m_DefaultElement = new CSVColumElement();
        public CSVColumElement getValue(int rowIndex, string columnName)
        {
            m_DefaultElement.Value = getStringValue(rowIndex, columnName);
            return m_DefaultElement;
        }
        public string getStringValue(int rowIndex, string columnName)
        {
            if (rowIndex < 0 || mDocumentText.Count <= rowIndex)
            {
                Debuger.LogWarning("行数超出范围:" + rowIndex);
                return string.Empty;
            }
            List<string> tempStrList = mDocumentText[rowIndex];
            int columnIndex = mDocumentColumNameList.IndexOf(columnName);
            if (columnIndex < 0 || tempStrList.Count <= columnIndex)
            {
                if (columnIndex == -1)
                    Debuger.LogWarning("未查找到列:" + columnName);
                return string.Empty;
            }
            return tempStrList[columnIndex].Trim();
        }

        //读取第rowIndex行columnIndex列的数据
        public CSVColumElement getValue(int rowIndex, int columnIndex)
        {
            m_DefaultElement.Value = getStringValue(rowIndex, columnIndex);
            return m_DefaultElement;
        }
        
        public string getStringValue(int rowIndex, int columnIndex)
        {
            if (rowIndex < 0 || mDocumentText.Count <= rowIndex)
            {
                Debuger.LogWarning("行数超出范围:" + rowIndex);
                return string.Empty;
            }
            List<string> tempStrList = mDocumentText[rowIndex];
            if (columnIndex < 0 || tempStrList.Count <= columnIndex)
            {
                if (columnIndex == -1)
                    Debuger.LogWarning("未查找到列:" + columnIndex);
                return string.Empty;
            }
            return tempStrList[columnIndex].Trim();
        }

        public List<List<string>> GetDocumentText()
        {
            return mDocumentText;
        }
    }

    /// <summary>  
    /// 保存CSV工具类  
    /// （需求：UTF-8格式）  
    /// </summary>  
    public class CSVSaveData
    {
        private int mCurrRowNum = 0; //当前保存的行数量
        private int mCurrColNum = 0;//当前保存的列数量
        private int mMaxRowNum = 0; //当前总共行数量
        private int mMaxColNum = 0;//当前总共列数量
        private StringBuilder mDocumentText = new StringBuilder();

        private string mFilePath = "";

        /// <summary>
        /// 打开已有文件
        /// </summary>
        public bool Open(string full_path)
        {
            mFilePath = full_path;
            if (!File.Exists(full_path)) return false;

            using (StreamReader reader = new StreamReader(full_path, Encoding.UTF8))
            {
                string str = reader.ReadToEnd();
                mDocumentText.Append(str);
            }
            return true;
        }

        /// <summary>
        /// 创建新文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool New(string full_path)
        {
            mFilePath = full_path;
            try
            {
                string path = Path.GetDirectoryName(full_path);
                if (!Directory.Exists(path))
                {
                    FileUtils.CreateDirectory(path);
                }
                else if (File.Exists(full_path))
                {
                    FileUtils.DeleteFile(full_path);
                }
            }
            catch (Exception e)
            {
                Debuger.LogException(e);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 保存
        /// </summary>
        public bool Save()
        {
            if (string.IsNullOrEmpty(mFilePath) || mDocumentText.Length == 0) return false;
            using (StreamWriter writer = new StreamWriter(mFilePath, false, Encoding.UTF8))
            {
                writer.Write(mDocumentText);
            }
            return true;
        }

        /// <summary>
        /// 添加元素
        /// </summary>
        public void cat(string s)
        {
            string str = s.Replace("\"", "\"\"");//替换英文冒号 英文冒号需要换成两个冒号
            if (str.Contains(',') || str.Contains('"') || str.Contains('\r') || str.Contains('\n')) //含逗号 冒号 换行符的需要放到引号中
            {
                str = string.Format("\"{0}\"", str);
            }
            mDocumentText.Append(str);
            mDocumentText.Append(',');
            mCurrColNum++;
        }
        public void cat(byte val) { cat(val.ToString()); }
        public void cat(sbyte val) { cat(val.ToString()); }
        public void cat(char val) { cat(val.ToString()); }
        public void cat(short val) { cat(val.ToString()); }
        public void cat(ushort val) { cat(val.ToString()); }
        public void cat(int val) { cat(val.ToString()); }
        public void cat(uint val) { cat(val.ToString()); }
        public void cat(long val) { cat(val.ToString()); }
        public void cat(ulong val) { cat(val.ToString()); }
        public void cat(decimal val) { cat(val.ToString()); }
        public void cat(float val) { cat(val.ToString()); }
        public void cat(double val) { cat(val.ToString()); }

        /// <summary>
        /// 新行
        /// </summary>
        public void newRow()
        {
            mDocumentText.Append("\r\n");
            mCurrRowNum++;
            if (mCurrColNum >= mMaxColNum)
                mMaxColNum = mCurrColNum;
            mCurrColNum = 0;
            mMaxRowNum = mCurrRowNum;
        }
    }
}