# Analyzer
此系统核心为新闻爬取爬虫

在此基础上包裹了对数据的上传，以及配置文件的同步的外部模块

在前面的基础上实现了自动运行服务，每天自动运行一次。

服务宿主`AutorunService.dll`

普通工头`AnalyzerCrawler.dll`

苦力`Crawler.py`

可手动运行，也可将服务宿主[部署到系统服务中](https://www.cnblogs.com/RainFate/p/12095793.html)。
