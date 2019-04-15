namespace SirePluginDemo
{
    public static class Resources
    {
        #region Code_Format_Definition
        
        // 与脚本语法/工具逻辑相关的变量。请谨慎修改。
        public static readonly string AddressSplitter = "<Address>";
        public static readonly string CodeSplitter = "<Code>";
        public static readonly string ProcessName = "san11pk";

        #endregion

        #region GUI_Strings
        public static readonly string GameStartedStr = "游戏已启动";
        public static readonly string GameNotStartedStr = "游戏未启动";
        public static readonly string AboutAuthorStr = "Written By sjn4048@ZJU";

        public static readonly string ModifyToExeStr =
            "重要提示，请务必阅读。\n将本脚本应用到exe后，未来只需启动修改后的exe即可享受本脚本效果，无需每次启动后打开修改器修改。支持多次积累更新。\n请注意：**这将永久改变exe的内容且不可还原**，请务必做好版本备份";
        public static readonly string ImportantTipStr = "重要提示";
        public static readonly string DuringWaitingStr = "写入中...这需要一段时间，请耐心等待，不要关闭程序。";
        public static readonly string WriteToExeSuccessTextStr = "镜像写入成功！以后只需打开该EXE即可。如果遇到BUG，请务必联系作者反馈，十分感谢。";
        public static readonly string WriteToExeSuccessTitleStr = "EXE写入成功";
        public static readonly string ContactAuthorStr = "mailto:3160105216@zju.edu.cn?subject=[修改工具反馈]";
        public static readonly string WriteToMirrorTipStr = "这是一个高级功能，请务必仔细阅读本说明。\n内存写入指的是将已经做好的内存修改持久化写入EXE。" +
                                                            "这不仅将写入当前的脚本，还将写入对内存的所有修改(SIRE、水军patch、PKME等)。\n在写入完成后，你将无需打开" +
                                                            "任何内存修改器，只需打开修改后的游戏EXE即可。\n要注意的是，由于内存与exe之间的同步有一定复杂性，这是一个测试功能，" +
                                                            "作者不保证修改的绝对稳定性，因此强烈建议备份最原始的游戏EXE\n" +
                                                            "在继续写入前，请保证已经用各路修改器将游戏内存修改到你想持久化保存的状态。是否继续？";

        public static readonly string WriteToMemSuccessTitleStr = "内存写入成功";
        public static readonly string WriteToMemSuccessTextStr = "修改成功。";
        public static readonly string WriteToMemFailStr = "修改成功。";
        public static readonly string ChooseExeFileStr = "请选择一个已有的游戏EXE作为模板";
        public static readonly string ProFeature = "高级功能";
        public static readonly string HelpStr = "在与EXE同目录下新建Initial.txt，存放默认加载的脚本。";

        // backup
        public static readonly string FoundExistBakStr = "检测到了一个已有的备份，是否覆盖？";
        public static readonly string OverwriteBakStr = "覆盖备份";

        public static readonly string BakSuccessStr = "已成功备份至 ";
        public static readonly string BakSuccessTitleStr = "备份成功 ";
        public static readonly string AskForBakStr = "是否备份原EXE？";
        public static readonly string AskForBakTitleStr = "备份提示";
        #endregion

        #region Exception_Messages
        public static readonly string GameNotStartedExMsg = $@"游戏未启动。如已启动，请检查exe文件名是否为{ProcessName}.exe";
        public static readonly string ReadMemExMsg = "读取进程内存时出错";
        public static readonly string InvalidHexExMsg = "检测到了非16进制字符：";
        public static readonly string InvalidScriptExMsg = "The following script in is invalid: ";

        #endregion
    }
}