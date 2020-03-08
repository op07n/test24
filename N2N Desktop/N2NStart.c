/*
自定义ip什么的请修改以下几行，详见注释，不会请百度搜索#define使用
*/
#define IPADRESS "dhcp:192.168.211.100"//自定义本机IP,DHCP请在DHCP服务器IP前加："dhcp:"
#define DHCP "-r"//如果为DHCP请将此项改为："-r",不是DHCP请留空：""。
#define GROUPNAME "dhwpcs"//填写组名
#define PASSWORD "dhwpcs"//填写密码
#define SUPERNODEIP "103.205.253.66"//此项为SuperNode服务器IP(公网)
#define SUPERNODEPORT "10120"//此项为SuperNode服务器端口
#define OTHERARG " -r -A"//其他参数
#include <stdio.h>
#include <stdlib.h>
#include <windows.h>
#include <unistd.h>

int main(int argc, char *argv[]) {
	char str[]="SOFTWARE\\call";
	HKEY ck;
	char buffer[2048];
	char temp[2048];
	char edgedir[2048];
	char tapdir[2048];
	char vbedir[2048];
	char edgearg[2048];
	char startdir[2048];
	char regdir[2048];
	_getcwd(buffer,2048);//获取当前目录
	sprintf(temp,"%s%s",buffer,"\\");//定义目录
	sprintf(edgedir,"%s%s",temp,"edge.exe");//定义edge.exe目录
	sprintf(tapdir,"%s%s",temp,"tap-windows.exe");//定义tap-windows.exe安装包目录
	sprintf(vbedir,"%s%s",temp,"start.vbe");//定义自启动vbe目录	
	sprintf(edgearg,"%s%s%s%s%s%s%s%s%s%s%s%s",DHCP," -a ",IPADRESS," -c ",GROUPNAME," -k ",PASSWORD," -l ",SUPERNODEIP,":",SUPERNODEPORT,OTHERARG);//定义静默启动vbe参数
	sprintf(startdir,"%s%s%s%s%s%s","CreateObject(\"WScript.Shell\").Run \"cmd /c ","\"\"",edgedir,"\"\" ",edgearg,"\",0");//定义静默启动vbe内容
	sprintf(regdir,"%s%s%s","\"",vbedir,"\"");//定义自启动vbe目录
start:
	//下面是通过注册表判断tap-windows是否安装
	if (ERROR_SUCCESS==RegOpenKeyEx(HKEY_LOCAL_MACHINE,"SOFTWARE\\TAP-Windows",0,KEY_ALL_ACCESS,&ck)) {
		if (access(edgedir,0)) {
			//MessageBox(NULL,TEXT(edgedir),TEXT("Test!"),0);//edge.exe的目录检查
			MessageBox(NULL,TEXT("没有找到edge.exe!请完整解压后将edge.exe与本程序放到同一目录下运行!"),TEXT("Error!"),0);
			if (MessageBox(NULL,TEXT("是否前往下载？"),TEXT("提示"),1)==1){
				system("start https://gitee.com/timpaik/n2n/raw/master/res/edge.exe");
			} else {
				MessageBox(NULL,TEXT("请手动下载！"),TEXT("提示"),0);
				exit(1);
			}
		} else {
			ShellExecute(0,"open","edge.exe",edgearg,"",0);//启动n2n
			MessageBox(NULL,TEXT("启动成功！如果想要关闭的话可以到任务管理器的后台找到本程序与edge.exe结束任务即可"),TEXT("内网已接通"),0);
			if (MessageBox(NULL,TEXT("是否添加开机启动项？"),TEXT("n2n开机启动"),1)==1){
				FILE *fp;
				fp=fopen(vbedir,"wb");//在同目录生成start.vbe
				fputs(startdir,fp);//在start.vbe中写入启动参数
				fclose(fp);//关闭文件
				HKEY hKey;//定义注册表项
				int length = strlen(regdir);//防止在后面写入注册表项时长度过短
				RegOpenKeyEx(HKEY_LOCAL_MACHINE, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", 0, KEY_ALL_ACCESS | KEY_WOW64_64KEY, &hKey);//打开注册表
				RegSetValueEx(hKey,"n2nstart",0,REG_EXPAND_SZ,(unsigned char*)regdir,length);//写入注册表项
				RegCloseKey(hKey);//关闭注册表
			}
			exit(0);
		}
	} else if (5==RegOpenKeyEx(HKEY_LOCAL_MACHINE,"SOFTWARE\\TAP-Windows",0,KEY_ALL_ACCESS,&ck)) {//管理员检查
		MessageBox(NULL,TEXT("请检查是否以管理员身份运行！"),TEXT("警告"),0);
		exit(5);
	} else if (2==RegOpenKeyEx(HKEY_LOCAL_MACHINE,"SOFTWARE\\TAP-Windows",0,KEY_ALL_ACCESS,&ck)) {//tap环境检查
		MessageBox(NULL,TEXT("未安装Tap运行环境！正在打开Tap安装包，请关闭所有杀毒软件后安装Tap！"),TEXT("警告"),0);
		if (access(tapdir,0)) {//寻找tap安装包
			MessageBox(NULL,TEXT("没有找到tap-windows.exe!请手动安装tap-windows或复制tap-windows安装包到本目录并重命名为tap-windows.exe后运行!"),TEXT("Error!"),0);
			if (MessageBox(NULL,TEXT("是否前往下载？"),TEXT("提示"),1)==1){
				system("start https://gitee.com/timpaik/n2n/raw/master/res/tap-windows.exe");//打开下载网页
			} else {
				MessageBox(NULL,TEXT("请手动下载！"),TEXT("提示"),0);
			}
			exit(2);
		} else {
			system("tap-windows.exe");//打开安装包
			goto start;
		}
	}
	return 0;
}
