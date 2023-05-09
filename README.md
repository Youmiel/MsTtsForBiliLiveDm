# MsTtsForBiliLiveDm

为 [RE-TTSCat](https://github.com/Elepover/RE-TTSCat) 编写的自定义 TTS 引擎，提供简单的请求 API。

## 使用方法

在插件管理页面设置 TTS 引擎监听的端口号以及使用的语音类型。

在 RE-TTSCat `插件设置 - 高级选项 - 自定义TTS引擎URL` 填入对应的 API, 请求方式为 GET。例如：

```
http://localhost:35468/?text=$TTSTEXT
```

对应端口设置为 35468 时的 API。

![usage.png](img/usage.png)

## 注意事项

微软网页演示的请求次数和频率有限制，请合理调用（比如什么进入直播间就不要说了）

现在程序会随机使用微软 TTS 各个地区的接口，尽量让使用次数多一点，但是每个地区支持的语音种类有区别，建议使用共同支持的几种语音，以下是列表：

```
云希
云扬
云野
晓辰
晓涵
晓墨
晓秋
晓睿
晓双
晓晓
晓萱
晓颜
晓悠
```


## //TODO

- 连接Azure的非演示版本TTS服务
- 提供一些别的TTS API，例如[偷懒工具](https://toolight.cn/media/reading)