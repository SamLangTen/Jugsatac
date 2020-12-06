# 实例一：利用Jugsatac生成报告并配合前端上传至GitHub Pages

本实例会通过Jugsatac生成报告，并通过内置的前端上传到GitHub Pages，实现在线报告浏览。


## 使用方法

0. 准备工作：
    * 准备好密钥对；
    * 配置GitHub使之能通过SSH密钥对访问你的仓库；
    * 按照GitHub Pages配置好域名解析。

1. 设置```config```目录下的文件，其中：
    
    * ```config.json```为Jugsatac的配置文件；
    * ```github_ssh_key_pub.pem```为用于与GitHub连接的SSH公钥；
    * ```github_ssh_key.pem```为私钥。

2. 使用下列命令构建镜像：

```
docker build . -t image_name && \
--build-arg PUBLISH_REPO_URL=仓库Git地址 && \
--build-arg PUBLISH_BRANCH="要推送的分支" && \
--build-arg PUBLISH_CNAME=域名 && \
--build-arg GIT_EMAIL=用于提交的邮箱 && \ 
--build-arg GIT_NAME=用于提交的用户名
```

3. 运行镜像，即可完成推送：

```
docker run image_name
```

本镜像启用了Jugsatac的缓存功能，但缓存保存在容器内，因此如果需保证缓存可用，应重复执行同一容器。或将缓存映射到容器外或卷内：

```
docker volume create image_name_vol
docker run -v image_name_vol:/app/frontend/config/cache --rm image_name
```
