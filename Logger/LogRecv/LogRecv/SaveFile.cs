using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LogRecv
{
    class SaveFile
    {

        public static string GetSaveFolder(string recStr, string recFolderStr)
        {

            string saveSelf = Path.GetDirectoryName(System.Environment.CurrentDirectory);
            string saveDesk = Path.Combine(saveSelf, "日志");//制定存储路径
            string savePath = Path.Combine(saveDesk, string.Format("{0}/{1}", recFolderStr, recStr + ".log"));//获取存储路径及文件名
            string saveParent = Path.GetDirectoryName(savePath);
            string saveParentU1 = Path.GetDirectoryName(saveParent);

            Console.WriteLine(savePath);

            if (!Directory.Exists(saveParent))
            {
                Directory.CreateDirectory(saveParent);
            }

            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }

            return savePath;
        }


        private static List<DirectoryInfo> SortDir(string saveParentU1)
        {
            DirectoryInfo upDir = new DirectoryInfo(saveParentU1);
            var dirs = upDir.GetDirectories().ToList();
            dirs.Sort((x, y) =>
            {
                if (int.Parse(x.Name) >= int.Parse(y.Name))
                    return 1;
                else return -1;
            });

            string frist = Path.Combine(saveParentU1, dirs[0].Name);

            if (Directory.Exists(frist))
                Directory.Delete(frist, true);

            return dirs;
        }
    }
}
