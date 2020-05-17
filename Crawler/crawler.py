# coding=utf-8

import datetime
import time
import requests
import csv
import os
import json
from lxml import etree
import re
import pandas as pd
		
headers = {
        'user-agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.129 Safari/537.36',
        'cookie':'tt_webid=6823926440687355406; s_v_web_id=verify_k9w5xdqx_596ksI8M_GwIO_4xNh_9URh_MDxLpYhsFPEk; ttcid=8db4c6f144d2419a8726a73331e91a5224; WEATHER_CITY=%E5%8C%97%E4%BA%AC; __tasessionId=p17mmo1zx1588819197198; SLARDAR_WEB_ID=b93db66a-d7bc-4a21-9b6e-4c2e24609141; tt_webid=6823926440687355406; csrftoken=bec079753b13701f058b75df60faf8bf; tt_scid=HQ8c3xqRbI85KSLgA3YjioyWCIk4oOuTHGDw6VobuSeNh2js4x6j-MZtiIPzrq8Fa01a'
    }

# 读入新闻信息，看是否抓取过
def urlVisited(url):
    try:
        with open('res.csv', 'r',encoding="utf_8_sig") as f:
            reader = csv.reader(f)
            for i in reader:
                if url in i[2]:
                    return True
    except:
        pass
    return False

def spider(data,url,beginPage,endPage):
    news_total=[]

    for key in data:
        for page in range(beginPage,endPage+1):
            fullUrl=url %{'name':key,'page':page}
            
            # 读取页面
            time.sleep(1)
#           
            
            try:
                news_total.extend(load_page(fullUrl,key))
            except Exception as e:
                print(e)
                continue
            

    return news_total

# 新闻列表
def load_page(url,name):
    results=[]
    # 获取数据
    html = requests.get(url,headers=headers)
    content=etree.HTML(html.text)
    content_list=content.xpath('//div[@class="wrap"]/div[@class="main clearfix"]/div[@class="result"]/div[@class="box-result clearfix"]/h2/a/@href')

    for j in content_list:
        # 没有访问过，则爬取新闻详情
        if (urlVisited(j)==False):
            new_url=j

            # 获取新闻详情页
            res=load_link_page(j,name)
            if res["title"] !=[] and res["article"] != "":
                results.append(res)

    return results

# 新闻详情页
def load_link_page(url,name):
    # 读入页面html信息
    html = requests.get(url,headers=headers)
    html.encoding = html.apparent_encoding
    html.raise_for_status()
    content=etree.HTML(html.text)

    # 获取新闻详情
    for news in content:
        paragraph = news.xpath('//div[@class="article"]/p/text()')
        news_content="\n".join(paragraph)
       
        result={
            "museum":name,
            "url":url,
            "title":news.xpath('//div[@class="main-content w1240"]/h1[@class="main-title"]/text()'),
            "time":news.xpath('//div[@class="top-bar-inner clearfix"]/div[@class="date-source"]/span[@class="date"]/text()'),
            "article":news_content
        }
    
    return result

if __name__ == "__main__":
    beginPage=1
    endPage=30
    results=[]

    if not os.path.isfile("res.csv"):
        file = open('res.csv','w')
        file.close()

    # 读入要爬取的关键字和url
    with open("keywords.json","r+") as f:
        data=json.load(f)

    with open("crawlerconfig.json","r+") as f:
        sites=json.load(f)

    # 爬取
    for site in sites:
        results = results + spider(data,site["url"],beginPage,endPage)

    
    # 结果转化为dataframe
    df = pd.DataFrame(results)
    cols=[u'museum',u'url',u'title',u'time',u'article']
    df=df.loc[:,cols]

    df.to_csv("res.csv",mode='a',encoding="utf_8_sig")
    #pd.set_option('display.max_columns', None)
    print(json.dumps(results))
