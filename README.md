# MineCraftMicrosoftOauthLogin
### Auther:TheColdWorld
## A solution to log in to Minecraft with a Microsoft account(.NET C#)
## 一种关于Minecraft微软账户的解决方案(.NET C#)

## How to use
### Download / 下载
#### First method / 方案1
Copy "/MCOauthLogin/LibraryMain.cs" to your project
将"/MCOauthLogin/LibraryMain.cs"复制到你的项目
#### Second method / 方案2
Go to the [releases](https://github.com/TheColdWorld/MineCraftMicrosoftOauthLogin/releases) page to download the dynamic link library
前往[releases](https://github.com/TheColdWorld/MineCraftMicrosoftOauthLogin/releases)页面下载动态链接库
<br/>
Then add a reference to that dynamic-link library in your project
然后在你的项目中添加该动态链接库的引用
Details can be found [here](https://github.com/TheColdWorld/TheColdWorldConfigEditer)
具体可以在[这里](https://github.com/TheColdWorld/TheColdWorldConfigEditer)查看

### implement / 实现
#### First method(Not recommended) / 方案1(不建议)
Use where you want to use it
在你要使用的地方使用
```c#
using MCOauthLogin

 McProfile mcProfile=new(new Uri(uri));

 LoginCore loginCore = new(Token);
```

#### Second method / 方案2
Use every time you need to instantiate
在你每次需要实例化的时候使用
```c#

 MCOauthLogin.McProfile mcProfile=new(new System.Uri(uri));

 MCOauthLogin.LoginCore loginCore = new(Token);
```

## About issues 关于issues
#### you should have updated the dll to the latest Release version
#### you should have confirmed that there is no problem with my operation
#### then submit issues
#### 确认将 dll 更新到最新版本
#### 确认你的操作没有问题
#### 然后提交issues

#### Or do you have any new features you want
#### 或者你有什么想要的新功能
#### submit issues
#### 提交issues
#### I'll get back to you after deciding if you want to include content
#### 我会在决定是否要加入内容后回复你
