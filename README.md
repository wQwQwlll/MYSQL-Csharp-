# 基于 MySQL 数据库和 C# 实现的简单视频用户交互界面  
A Simple Video User Interaction Interface Based on MySQL and C#

本项目使用 MySQL 数据库配合 C# 编程语言，实现一个具备基本用户注册、视频上传、评论、点赞、关注等功能的视频平台交互系统。


## 数据库连接配置

如果想要使用本项目，请根据你的实际环境更改数据库连接字符串：

将以下内容：

```csharp
string connStr = "Data Source=localhost;Initial Catalog=videoapp;Integrated Security=True;";
```
改为：
```csharp
string connStr = "Data Source=你的数据库地址;Initial Catalog=你的数据库名称;Integrated Security=True;";
```

## 数据库设计

本项目一共使用如下 6 个表：

---

### 1. 用户表（User）

| 属性名     | 数据类型     | 必填字段 | 允许空值 | 属性描述     | 备注             |
|------------|--------------|----------|----------|--------------|------------------|
| user_id    | INT          | 是       | 否       | 用户编号     | 主码，自增       |
| username   | VARCHAR(50)  | 是       | 否       | 用户名       | 唯一             |
| password   | VARCHAR(100) | 是       | 否       | 登录密码     | 加密存储         |
| email      | VARCHAR(100) | 是       | 否       | 邮箱地址     | 唯一，可用于登录 |
| avatar_url | VARCHAR(200) | 否       | 是       | 用户头像地址 |                  |

---

### 2. 视频表（Video）

| 属性名   | 数据类型     | 必填字段 | 允许空值 | 属性描述     | 备注                        |
|----------|--------------|----------|----------|--------------|-----------------------------|
| video_id | INT          | 是       | 否       | 视频编号     | 主码，自增                 |
| title    | VARCHAR(100) | 是       | 否       | 视频标题     |                             |
| user_id  | INT          | 是       | 否       | 上传者用户ID | 外码，引用 User(user_id)   |

---

### 3. 评论表（Comment）

| 属性名     | 数据类型 | 必填字段 | 允许空值 | 属性描述     | 备注                                          |
|------------|----------|----------|----------|--------------|-----------------------------------------------|
| comment_id | INT      | 是       | 否       | 评论编号     | 主码，自增                                   |
| content    | TEXT     | 是       | 否       | 评论内容     |                                               |
| post_time  | DATETIME | 是       | 否       | 评论时间     | 默认当前时间                                 |
| user_id    | INT      | 是       | 否       | 评论者ID     | 外码，引用 User(user_id)                     |
| video_id   | INT      | 是       | 否       | 所属视频ID   | 外码，引用 Video(video_id)                   |
| parent_id  | INT      | 否       | 是       | 父评论ID     | 外码，引用 Comment(comment_id)，可为 null   |

---

### 4. 点赞表（Like）

| 属性名   | 数据类型 | 必填字段 | 允许空值 | 属性描述    | 备注                        |
|----------|----------|----------|----------|-------------|-----------------------------|
| like_num | INT      | 是       | 否       | 点赞数量    | 主码，自增                 |
| user_id  | INT      | 是       | 否       | 点赞用户ID  | 外码，引用 User(user_id)   |
| video_id | INT      | 是       | 否       | 点赞视频ID  | 外码，引用 Video(video_id) |

---

### 5. 观看记录表（WatchHistory）

| 属性名     | 数据类型 | 必填字段 | 允许空值 | 属性描述  | 备注                        |
|------------|----------|----------|----------|-----------|-----------------------------|
| history_id | INT      | 是       | 否       | 记录编号  | 主码，自增                 |
| user_id    | INT      | 是       | 否       | 用户ID    | 外码，引用 User(user_id)   |
| video_id   | INT      | 是       | 否       | 视频ID    | 外码，引用 Video(video_id) |

---

### 6. 关注表（Follow）

| 属性名       | 数据类型 | 必填字段 | 允许空值 | 属性描述       | 备注                        |
|--------------|----------|----------|----------|----------------|-----------------------------|
| follower_num | INT      | 是       | 否       | 关注数         | 主码，自增                 |
| follower_id  | INT      | 是       | 否       | 关注者ID       | 外码，引用 User(user_id)   |
| followee_id  | INT      | 是       | 否       | 被关注用户ID   | 外码，引用 User(user_id)   |
| follow_time  | DATETIME | 是       | 否       | 关注时间       | 默认当前时间               |


