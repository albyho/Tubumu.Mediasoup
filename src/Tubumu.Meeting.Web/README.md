# Tubumu.Meeting.Web

![截图](http://blog.tubumu.com/postimages/mediasoup-01/004.jpg)

对该项目实现上的介绍，见：[使用 ASP.NET Core 实现 mediasoup 的信令服务器](https://blog.tubumu.com/2020/05/05/mediasoup-01/)。

`Tubumu.Meeting.Web` 是基于 `mediasoup` 实现的视频会议系统 Demo。有别于官方 Demo，本项目有如下特点：

1. 将 mediasoup 服务端的 `Node.js` 模块使用 `.Net Core` 重新进行了实现。
2. 支持三种服务模式：
`Open`: 进入会议室立即发送音视频供其他用户浏览，并浏览其他用户的音视频。
`Pull`: 客户端启动时不主动 Produce; 客户端可根据需要 Pull 对端的支持的音视频进行 Consume，而对端按需 Produce。仅 Pull 者会浏览。其他人要浏览可自行 Pull。
`Invite`: 会议室管理员邀请其他用户发言；其他用户也可以向管理员申请发言。

3. 客户端使用 Vue 实现。

## 一、启动服务端

1. 请打开 `mediasoupsettings.json` 进行两处修改：

- 在配置文件中搜索将 `AnnouncedAddress` ，将其值改为本机的局域网 IP/hostname 或外网的 IP/域名。如果其值为 `null` 则取本机的其中一个 IPv4 的地址。
- 在 `MediasoupStartupSettings.WorkerPath` 节点设置 `mediasoup-worker` 可执行程序的物理路径。 如果引用了 `Tubumu.Mediasoup.Executable` 项目则可将该节点注释掉，该项目包含了编译好的 macOS 和 Windows 版的 mediasoup-worker 。

2. 在 `Tubumu.Meeting.Web` 目录执行 `dotnet run` 或者在 `Vistual Sudio` 打开解决方案启动 `Tubumu.Meeting.Web` 项目。

``` shell
> cd Tubumu.Meeting.Web
> dotnet run
```

> 备注：如果将 MediasoupStartupSettings.WorkerPath 注释，启动时将自动去 "runtimes/{platform}/native" 目录查找 "mediasoup-worker" 。其中 "{platform}" 根据平台分别可以是：win、osx 和 linux。 详见 Worker.cs 文件中 Worker 类的构造函数。

## 二、启动前端

在 [tubumu-meeting-web-client](https://github.com/albyho/Tubumu.Mediasoup/tubumu-meeting-web-client) 安装 Node.js 包并运行。

``` shell
> cd tubumu-meeting-web-client
> yarn install
> yarn serve
```

## 三、打开浏览器

>备注：请使用 Chrome、Firefox 或 Edge 浏览器。

因为没有将前端放入 Web 项目中，并且没有使用正式的 TLS 证书，所以先访问一次 `https://192.168.x.x:9001/` 。提示不安全时请继续访问。

在同一个浏览器用两个标签或者在两台电脑的浏览器上分别打开 `https://192.168.x.x:8080`，然后选择不同的 Peer 。提示不安全时请继续访问；提示访问摄像头和麦克风当然应该允许。

## 关联项目

1. [Tubumu.Utils: 工具类等。](https://github.com/albyho/Tubumu.Mediasoup/Tubumu.Utils)
2. [Tubumu.Libuv: Libuv 封装。](https://github.com/albyho/Tubumu.Mediasoup/Tubumu.Libuv)
3. [Tubumu.Mediasoup: mediasoup 服务端库的 C# 实现。](https://github.com/albyho/Tubumu.Mediasoup/Tubumu.Mediasoup)
4. [Tubumu.Mediasoup.Common: 公共类封装。也可供 WinForm 或 WPF 等使用。](https://github.com/albyho/Tubumu.Mediasoup/Tubumu.Mediasoup.Common)
5. [Tubumu.Mediasoup.AspNetCore: 主要是 Asp.Net Core 服务注册和中间件。](https://github.com/albyho/Tubumu.Mediasoup/Tubumu.Mediasoup.AspNetCore)
6. [Tubumu.Meeting.Server: 会议系统的业务实现。](https://github.com/albyho/Tubumu.Mediasoup/Tubumu.Meeting.Server)
7. [Tubumu.Meeting.Web: 简单的 Asp.Net Core 项目。](https://github.com/albyho/Tubumu.Mediasoup/Tubumu.Meeting.Web)
8. [tubumu-meeting-web-client: 简单的前端实现。](https://github.com/albyho/Tubumu.Mediasoup/tubumu-meeting-web-client)
