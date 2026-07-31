# province.vue 数字经济大屏风格复刻方案

## Context（背景与目标）

用户要求 `province.vue` 参考 bootstrapmb "数字经济产业数据可视化页面"（item 14944）生成**一模一样的功能效果**，同时**保持现有下钻逻辑不变**。经确认复刻范围 = **视觉样式 + 图表类型**（保留健康监测业务数据与下钻逻辑）。

参考模板特征：深蓝渐变背景 + 网格/扫描线装饰；顶部 head.png 标题横幅；左/中/右三栏（左：指标卡+环形图+饼图；中：地图；右：横向柱状图+饼图+折线面积图）；毛玻璃面板 + 青色发光边框 + HUD 装饰角 + titbg.png 标题背景。

当前 province.vue 已是科技风深蓝大屏，结构相近但图表类型不同（KPI卡+排名列表+横向堆叠条形图+仪表盘+横向柱状图）。改造重点：重排布局、替换图表类型、接入图片装饰资源。

## 硬约束（不可破坏）

1. **下钻逻辑**：`drillToCity(name)`(L397)、`goBack()`(L551)、地图点击下钻、`loadMap()`(L998) 原样保留
2. **地图 4 色 visualMap**(L671-678)：#0bc4e9 / #7dd87d / #f5c542 / #e8554f 严禁改动，对应图例 CSS 同步保留
3. **浅色主题适配**：所有新颜色走 `getColors()`，新图片背景在 `.theme-light` 下降 opacity 或隐藏
4. 保留 tabs(5指标)、route 参数联动、主题/时间监听、响应式

## 改造文件

- `e:\5HP_Project\frontend\apps\web-antd\src\views\vision-archive\province.vue`（唯一改动文件）
- 复用图片：`images/head.png` `titbg.png` `dot.png` `leftline.png` `rightline.png` `piebg01.png` `piebg02.png` `mapbg2.png` `lines1-3.png`

## 一、模板重构（template）

### 1. 顶部标题横幅（替换 L22-43 national-header 内部）
- 外层 `.head-banner`，背景 `head.png`；左右贴 `leftline.png`/`rightline.png`；标题两侧放 `dot.png`
- 标题用渐变文字 + 发光；日期时间移入横幅右上角

### 2. 筛选条件上移（原 L55-93 左栏第一个面板 → tabs 下方 `.filter-strip`）
- 压缩为单行紧凑横条（年份/地区 select + 搜索/重置），释放左栏空间

### 3. 左栏 3 面板（原 L94-182：保留 KPI 卡，删 ranking-list，新增 2 图表）
| 面板 | 内容 | ref |
|---|---|---|
| L1 核心数据 | KPI 卡（保留原 kpi-grid 结构与数据） | — |
| L2 学段{metric}率分布 | 环形图 donut | `stageDonutRef` |
| L3 健康等级分布 | 饼图 pie（4色对应地图分段） | `healthLevelPieRef` |

### 4. 右栏 3 面板（原 L227-261：全替换）
| 面板 | 内容 | ref |
|---|---|---|
| R1 城市{metric}率排名 | 横向柱状图（保留 click→drillToCity） | `cityRankBarRef` |
| R2 城乡对比 | 饼图 pie（城区/县乡） | `urbanRuralPieRef` |
| R3 防控效果趋势 | 折线/面积图（多年时间序列） | `trendChartRef` |

### 5. 地图区（保留 + 叠加 mapbg2.png 背景）
- `mapRef`、`map-back-btn`、`map-legend`、`map-deco` 全部保留不动

## 二、脚本改造（script setup）

### 保留不动
`PROVINCE_MAP_CONFIG` `CITY_CODES_MAP` `metricConfig` `currentCityCodes` `drillToCity` `goBack` `getProvinceMapData` `rankingListData` `computeProvinceMetricData` `currentProvinceData` `getMapOption` `loadMap` `initChart` `disposeAll` `resizeAll` `getColors` `makeGrad` `getTooltip` 生命周期与全部 watch

### 删除
`getAgeGenderOption`(L776) `getGaugeOption`(L859) `getInterventionOption`(L903) `handleRankingClick`(L440，逻辑内联到 ECharts click 回调)

### ref 调整
删 `ageGenderChartRef` `urbanGaugeRef` `ruralGaugeRef` `interventionChartRef`；新增 `stageDonutRef` `healthLevelPieRef` `cityRankBarRef` `urbanRuralPieRef` `trendChartRef`（`mapRef` 保留）

### 新增 5 个 option 函数（颜色均走 getColors/makeGrad）
- `getStageDonutOption()`：pie radius['52%','70%']，数据=primary/junior/senior，中心 graphic 显示全省均值
- `getHealthLevelPieOption()`：pie，把 `getProvinceMapData()` 城市按 4 色阈值分桶计数，颜色严格用 #0bc4e9/#7dd87d/#f5c542/#e8554f
- `getCityRankBarOption()`：横向 bar，复用 `rankingListData`，柱色按值分段(同 4 色)，**click 回调直接调 `drillToCity(params.name)`**
- `getUrbanRuralPieOption()`：pie radius['45%','70%']，数据=城区(rate*0.9)/县乡(rate*1.2)
- `getTrendOption()`：双折线(干预前虚线灰/干预后实线主色发光)+面积渐变，x 轴 2022-2026

### renderAllCharts 改写（L979）
```
disposeAll → initChart(stageDonut) → initChart(healthLevelPie)
→ initChart(cityRankBar, click→drillToCity) → initChart(urbanRuralPie)
→ initChart(trend) → if(mapLoaded) initChart(map, click→drillToCity)
```

## 三、样式改造（style）

### 1. 标题横幅 `.head-banner`
- `background: url('./images/head.png') no-repeat center/contain`
- `leftline.png`/`rightline.png` 两侧发光装饰；`dot.png` 标题点缀
- 标题渐变文字 + 流光动画；浅色降亮度

### 2. 面板标题 `.tech-panel-header`
- `background: url('./images/titbg.png') no-repeat left center/auto 100%`
- `panel-bullet` 方块替换为 `dot.png` 装饰点
- 浅色下去掉 titbg 背景，改底部细线

### 3. 毛玻璃面板 `.tech-panel`（保留 + 增强）
- 加 `backdrop-filter: blur(10px)`，背景降为 `rgba(8,18,44,0.55)`
- 保留原 border/box-shadow/::before 流光/panel-corner.br HUD 角
- 右下角叠加 `lines2.png` 纹理(opacity 0.15)
- 新增 `panel-fade-in` 动画，左右列错开 delay

### 4. 地图背景 `.map-area`
- 叠加 `radial-gradient + mapbg2.png + linear-gradient`（opacity 0.4）
- 浅色不叠 mapbg2.png

### 5. 饼图装饰背景
- `.donut-area::before` = `piebg01.png` 旋转动画(20s)
- `.health-pie-area::before` = `piebg02.png` 反向旋转(25s)
- 浅色 opacity 降至 0.2

### 6. 紧凑筛选栏 `.filter-strip`
- 单行 flex，`border-bottom`，搜索/重置按钮靠右

### 7. 删除样式
`.ranking-list` `.ranking-item` `.ranking-bar*` `.bar-level-1~4` `.ranking-empty` `.gauge-row` `.gauge-item`

### 8. 地图顶部装饰条
`.map-col::before` = `lines3.png` 横向装饰

## 四、验证步骤

1. `pnpm dev` 启动，访问 `/vision/province/320000`（江苏）
2. 深色下检查：标题横幅、6 面板布局、5 图表渲染、地图 4 色、装饰图片显示
3. 切换 5 个 tab（vision/oral/mental/weight/bone）→ 全部图表重渲染
4. 切换浅色主题 → 图片降 opacity、文字颜色切换、无视觉冲突
5. **下钻**：点击地图城市 → 跳转 `/vision/city/{code}`；点击右栏城市排名柱条 → 同样下钻；点"返回上级" → 回 `/vision/national`
6. 切换省份路由（route.params.code）→ 地图重新加载、cityList 更新、图表刷新
7. 响应式：缩放窗口至 1200px / 1024px → 布局自适应、图表 resize
8. 控制台无报错（特别检查 echarts graphic/tooltip/回调）
