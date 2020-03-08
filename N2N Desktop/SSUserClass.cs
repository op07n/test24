using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N2N_Desktop
{
    /// <summary>
    /// 这是SS的用户工具类——
    /// </summary>
    public class SSUserClass
    {
        #region Reg类 | 注册表操作
        public class Reg
        {
            /// <summary>
            /// 在用户目录下判断注册表项是否存在
            /// </summary>
            /// <param name="way">注册表项父级路径</param>
            /// <param name="name">注册表项名</param>
            /// <returns>是否存在</returns>
            public static bool IsRegeditItemExist(RegistryKey hkml, string way, string name)
            {
                string[] subkeyNames;
                RegistryKey software = hkml.OpenSubKey(way, true);
                subkeyNames = software.GetSubKeyNames();
                //取得该项下所有子项的名称的序列，并传递给预定的数组中
                foreach (string keyName in subkeyNames) //遍历整个数组
                {
                    if (keyName.Equals(name)) //判断子项的名称
                    {
                        hkml.Close();
                        return true;
                    }
                }
                hkml.Close();
                return false;
            }

            /// <summary>
            /// 在用户目录下创建注册表项
            /// </summary>
            /// <param name="way">注册表项路径</param>
            /// <returns>是否成功</returns>
            public static bool CreateRegItem(RegistryKey key, String way)
            {
                try
                {
                    RegistryKey software = key.CreateSubKey(way);
                }
                catch
                {
                    return false;
                }
                return true;
            }

            /// <summary>
            /// 在用户目录下判断注册表项的键值是否存在
            /// </summary>
            /// <param name="way">注册表项路径</param>
            /// <param name="name">键名</param>
            /// <returns>是否存在</returns>
            public static bool IsRegeditKeyExit(RegistryKey hkml, String way, String name)
            {
                string[] subkeyNames;
                RegistryKey software = hkml.OpenSubKey(way, true);
                subkeyNames = software.GetValueNames();
                //取得该项下所有键值的名称的序列，并传递给预定的数组中
                foreach (string keyName in subkeyNames)
                {
                    if (keyName.Equals(name)) //判断键值的名称
                {
                        hkml.Close();
                        return true;
                    }
                }
                hkml.Close();
                return false;
            }


            /// <summary>
            /// 在用户目录下创建键
            /// </summary>
            /// <param name="way">键的父路径</param>
            /// <param name="name">键名</param>
            /// <param name="value">键值</param>
            /// /// <returns>是否成功</returns>
            public static bool CreateRegKey(RegistryKey key, String way, String name, String value)
            {
                try
                {
                    RegistryKey software = key.OpenSubKey(way, true); //该项必须已存在
                    software.SetValue(name, value);
                }
                catch
                {
                    return false;
                }
                return true;
            }

            /// <summary>
            /// 获取在用户目录下的键值
            /// </summary>
            /// <param name="way">键的父路径</param>
            /// <param name="name">键名</param>
            /// <returns></returns>
            public static String GetRegKey(RegistryKey key, String way,String name)
            {
                try
                {
                    RegistryKey software = key.OpenSubKey(way, true); //该项必须已存在
                    return software.GetValue(name).ToString();
                }
                catch
                {
                    return "Err";
                }
            }
        }
        #endregion

        #region Proc | 进程操作
        public class Proc
        {
            /// <summary>
            /// 获取进程
            /// </summary>
            /// <param name="strProcName">进程名</param>
            /// <returns></returns>
            public static Process GetProc(string strProcName)
            {
                //精确进程名  用GetProcessesByName
                Process[] processList = Process.GetProcesses();
                foreach (Process process in processList)
                {
                    if (process.ProcessName.ToLower() == strProcName.ToLower())
                    {
                        return process;
                    }
                }
                return new Process();
            }

            /// <summary>
            /// 获取进程（模糊）
            /// </summary>
            /// <param name="strProcName">进程名</param>
            /// <param name="noTrue">是否为模糊搜索</param>
            /// <returns></returns>
            public static List<Process> GetProc(string strProcName, bool noTrue)
            {
                //模糊进程名  枚举
                List<Process> processes = new List<Process>();
                foreach (Process p in Process.GetProcesses())
                {
                    if (p.ProcessName.IndexOf(strProcName) > -1)  //第一个字符匹配的话为0，这与VB不同
                    {
                        processes.Add(p);
                    }
                }
                return processes;
            }

            /// <summary>
            /// 结束进程
            /// </summary>
            /// <param name="p">进程</param>
            public static void KillProc(Process p)
            {
                p.Kill();
            }

            /// <summary>
            /// 判断进程是否存在
            /// </summary>
            /// <param name="name">进程名</param>
            /// <returns></returns>
            public static bool HasProc(string name)
            {
                Process[] processList = Process.GetProcesses();
                foreach (Process process in processList)
                {
                    if (process.ProcessName.ToLower() == name.ToLower())
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        #endregion
    }
}
