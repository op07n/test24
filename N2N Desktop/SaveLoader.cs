using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace N2N_Desktop
{
    public static class SaveLoader
    {
        public class N2NConfig
        {
            public string error;
            public string iP;
            public string iPAdditional;
            public bool isDHCP;
            public string severIP;
            public int severPost;
            public string teamName;
            public string teamPassword;
            public bool isUsed;
            public string Name;
            public string UUID;
        }

        public static List<N2NConfig> LoadSave()
        {
            List<N2NConfig> configs = new List<N2NConfig>();
            if (Directory.Exists("Save"))
            {
                if(File.Exists("Save\\Config.ssn2n"))
                {
                    int linepass = 0;
                    N2NConfig config = new N2NConfig
                    {
                        error = "False"
                    };
                    StreamReader sr = new StreamReader("Save\\Config.ssn2n", Encoding.UTF8);
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        switch(linepass)
                        {
                            case 0:
                                {
                                    if (line != "[Start]")
                                    {
                                        configs = new List<N2NConfig>
                                        {
                                            new N2NConfig() { error = "文件错误。" }
                                        };
                                        return configs;
                                    }
                                    config = new N2NConfig
                                    {
                                        error = "False"
                                    };
                                    linepass = 1;
                                }
                                break;
                            case 1:
                                {
                                    config.iP = line;
                                    linepass = 2;
                                }
                                break;
                            case 2:
                                {
                                    config.iPAdditional = line;
                                    linepass = 3;
                                }
                                break;
                            case 3:
                                {
                                    config.isDHCP = bool.Parse(line);
                                    linepass = 4;
                                }
                                break;
                            case 4:
                                {
                                    config.severIP = line;
                                    linepass = 5;
                                }
                                break;
                            case 5:
                                {
                                    config.severPost = int.Parse(line);
                                    linepass = 6;
                                }
                                break;
                            case 6:
                                {
                                    config.teamName = line;
                                    linepass = 7;
                                }
                                break;
                            case 7:
                                {
                                    config.teamPassword = line;
                                    linepass = 8;
                                }
                                break;
                            case 8:
                                {
                                    config.isUsed = bool.Parse(line);
                                    linepass = 9;
                                }
                                break;
                            case 9:
                                {
                                    config.Name = line;
                                    linepass = 10;
                                }
                                break;
                            case 10:
                                {
                                    config.UUID = line;
                                    linepass = 11;
                                }
                                break;
                            case 11:
                                {
                                    if (line != "[End]")
                                    {
                                        configs = new List<N2NConfig>
                                        {
                                            new N2NConfig() { error = "文件错误。" }
                                        };
                                        return configs;
                                    }
                                    configs.Add(config);
                                    linepass = 0;
                                }
                                break;
                        }
                    }
                    if(configs == new List<N2NConfig>())
                    {
                        configs.Add(new N2NConfig() { iP = "new" });
                    }
                    return configs;
                }
            }
            configs.Add(new N2NConfig() { error = "False", iP = "new"});
            return configs;
        }

        public static bool AddSave(N2NConfig config)
        {
            if (!Directory.Exists("Save"))
            {
                Directory.CreateDirectory("Save");
            }
            using (StreamWriter sw = File.AppendText("Save\\Config.ssn2n"))
            {
                sw.WriteLine("[Start]");
                sw.WriteLine(config.iP);
                sw.WriteLine(config.iPAdditional);
                sw.WriteLine(config.isDHCP);
                sw.WriteLine(config.severIP);
                sw.WriteLine(config.severPost);
                sw.WriteLine(config.teamName);
                sw.WriteLine(config.teamPassword);
                sw.WriteLine(config.isUsed);
                sw.WriteLine(config.Name);
                sw.WriteLine(config.UUID);
                sw.WriteLine("[End]");
            }
            return false;
        }
    }
}
