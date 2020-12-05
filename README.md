# 🛹Jugsatac🛸

**Jugsatac** 全称为 ***J**inan **U**niversity **G**raduated **S**tudent **A**I **2**020 Course's **A**ssignment **C**ollector*，即《用于JNU 2020年研究生人工智能课程的作业收集程序》。该程序用于在指定邮箱拉取指定标题格式的邮件，并将正文提取，生成报告；或下载指定邮箱内指定标题格式的附件。

Jugsatac目前只支持IMAP协议的邮件服务器。

## 使用方法

Jugsatac有两种运行模式：分类和下载。前者用于提取正文并生成报告，后者用于下载指定标题的附件。无论是哪一种都需要使用配置文件。

以下是一个配置文件的模板：
```
{
    "host": "imap.example.com",
    "port": 993,
    "username": "assignments@example.com",
    "password": "你的密码",
    "mailbox": "INBOX",
    "assignments": [
        {
            "name": "作业1",
            "identifierPattern": "-作业1$",
            "submitterPattern": "(?<=\\+?)([\\u4e00-\\u9fa5]{1,20}|[a-zA-Z\\.\\s]{1,20})(?=\\+?)",
            "onlySubject": false,
            "hideSubmitterName": true
        },
        {
            "name": "作业2",
            "identifierPattern": "-作业2$",
            "submitterPattern": "(?<=\\+?)([\\u4e00-\\u9fa5]{1,20}|[a-zA-Z\\.\\s]{1,20})(?=\\+?)",
            "onlySubject": true,
            "hideSubmitterName": false
        }
    ]
}
```

```host```、```port```、```username```、```password```分别为你的邮箱服务器信息。```mailbox```为需要拉取的收件箱，这样的设计可以让用户通过邮件服务器的归档功能，将不同标题特征的邮件归档到不同收件箱，从而加快拉取速度以及提高整洁度。

* ```assignments```属性为列表，规定了多个作业拉取任务。
* ```name```表示作业名称，会生成到报告中。
* ```identifierPattern```为用于识别邮件标题的正则表达式。当邮件标题匹配时该邮件才属于该任务项。
* ```submitterPattern```为用于提取邮件标题内的提交人信息的正则表达式。
* ```onlySubject```指示Jugsatac是否只拉取标题不拉取正文，有时你可能只需要一个是否提交的名单而不是具体内容，可以设置此项加快拉取速度。
* ```hideSubmitterName```指示Jugsatc是否隐去部分人的姓名。只支持中文姓名。

以上模板可以匹配如下标题的邮件：
```
张三+李四-作业1
Takajima Kenji+王五-作业2
```

### 分类模式

如果使用**分类模式**，输入如下命令：
```
Jugsatac -s CONFIG_FILE [-o OUTPUT_FILE]
```
```-s```参数表示读入上文提及的配置文件。```-o```是可选参数，表示生成报告的输出文件，如果不指定则在标准输出中输出报告。

对于第一个邮件，最终导出的报告形如下文所示：
```
names: *三、*四
title：作业1
content：正文
time：邮件到达时间
```

对于第二个邮件，最终导出的报告形如下文所示：
```
names: Takajima Kenji、王五
title：作业2
content：正文
time：邮件到达时间
```

> 以上报告仅为范例，实际生成效果为JSON格式。

分类模式可以缓存已经拉取过的邮件正文，以在下一次拉取时加速。使用```-c CACHE_FILE```参数可启用缓存模式。```CACHE_FILE```为保存的缓存文件，如果文件不存在会自动创建，若文件存在则读入缓存内容。

### 下载模式

如果使用**下载模式**，```assignments```属性内的任务项仅有```name```和```identifierPattern```有效。

输入此命令：
```
Jugsatac -s CONFIG_FILE -o OUTPUT_DIRECTORY
```

此时```-o```参数为必选参数，表示下载附件的存放目录。Jugsatac会在目录内创建以任务项名为名称的子目录，并将匹配的邮件附近下载到子目录中，并将附近修改日期设定为邮件到达日期

## Docker适配

你可以通过Dockerfile构建或使用作者提交的镜像。下面是一些用法：

* 不带缓存的分类模式：
    ```
    docker run -v /path/to/your/config:/app/config.json --rm samlangten/jugsatac -s /app/config.json
    ```

* 带缓存的分类模式：
    ```
    docker run -v /path/to/your/config:/app/config.json -v /path/to/your/cache:/app/cache.json --rm samlangten/jugsatac -s /app/config.json -c /app/cache.json
    ```

* 带缓存的分类模式，输出到文件：
    ```
    docker run -v /path/to/your/config:/app/config.json -v /path/to/your/cache:/app/cache.json -v /path/to/your/output/file:/app/output.json --rm samlangten/jugsatac -s /app/config.json -c /app/cache.json -o /app/output.json
    ```

* 下载模式：
    ```
    docker run -v /path/to/your/config:/app/config.json -v /path/to/download/dir:/app/download --rm samlangten/jugsatac -s /app/config.json -o /app/download -d
    ```