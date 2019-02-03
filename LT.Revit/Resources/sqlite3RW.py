# -*- coding: utf-8 -*-
#python sqlite
# version table
import sqlite3
import os
import argparse
import re

db_file_path = ""
table_name=""
SHOW_SQL = True
gstrPreFix = "_1";

def get_conn(path):
    conn = sqlite3.connect(path)
    if os.path.exists(path) and os.path.isfile(path):
        print('硬盘上面：'+path)
        return conn
    else:
        conn = None
        print("内存上面")
        return sqlite3.connect(':memory:')

def get_cursor(conn):
    if conn is not None:
        return conn.cursor()
    else:
        return get_conn('').cursor()

def drop_table(conn, table):
     '''如果表存在,则删除表，如果表中存在数据的时候，使用该
     方法的时候要慎用！'''
     if table is not None and table != '':
         sql = 'DROP TABLE IF EXISTS ' + table
         if SHOW_SQL:
             print('执行sql:[{}]'.format(sql))
         cu = get_cursor(conn)
         cu.execute(sql)
         conn.commit()
         print('删除数据库表[{}]成功!'.format(table))
         close_all(conn, cu)
     else:
         print('the [{}] is empty or equal None!'.format(sql))
 
def create_table(conn,sql):
     '''创建数据库表：student'''
     if sql is not None and sql != '':
        cu = get_cursor(conn)
        if SHOW_SQL:
            print('执行sql:[{}]'.format(sql))
        cu.execute(sql)
        conn.commit()
        print('创建数据库表[student]成功!')
        close_all(conn, cu)
     else:
        print('the [{}] is empty or equal None!'.format(sql))
 
 ###############################################################
 ####            创建|删除表操作     END
 ###############################################################
def close_all(conn, cu):
     '''关闭数据库游标对象和数据库连接对象'''
     try:
        if cu is not None:
             cu.close()
     finally:
        if cu is not None:
            cu.close()
 
 ###############################################################
 ####            数据库操作CRUD     START
 ###############################################################
 
def save(conn, sql, data):
     '''插入数据'''
     if sql is not None and sql != '':
         if data is not None:
             cu = get_cursor(conn)
             for d in data:
                 if SHOW_SQL:
                     print('执行sql:[{}],参数:[{}]'.format(sql, d))
                 cu.execute(sql, d)
                 conn.commit()
             close_all(conn, cu)
     else:
         print('the [{}] is empty or equal None!'.format(sql))
 
def fetchall(conn, sql):
     '''查询所有数据'''
     if sql is not None and sql != '':
         cu = get_cursor(conn)
         if SHOW_SQL:
             print('执行sql:[{}]'.format(sql))
         cu.execute(sql)
         r = cu.fetchall()
         if len(r) > 0:
             for e in range(len(r)):
                 print(r[e])
     else:
         print('the [{}] is empty or equal None!'.format(sql)) 
 
def fetchone(conn, sql, data):
     '''查询一条数据'''
     if sql is not None and sql != '':
         if data is not None:
             #Do this instead
             d = (data,) 
             cu = get_cursor(conn)
             if SHOW_SQL:
                 print('执行sql:[{}],参数:[{}]'.format(sql, data))
             cu.execute(sql, d)
             r = cu.fetchall()
             if len(r) > 0:
                 for e in range(len(r)):
                     print(r[e])
         else:
             print('the [{}] equal None!'.format(data))
     else:
         print('the [{}] is empty or equal None!'.format(sql))
 
def update(conn, sql, data):
     '''更新数据'''
     if sql is not None and sql != '':
         if data is not None:
             cu = get_cursor(conn)
             for d in data:
                 if SHOW_SQL:
                     print('执行sql:[{}],参数:[{}]'.format(sql, d))
                 cu.execute(sql, d)
                 conn.commit()
             close_all(conn, cu)
     else:
         print('the [{}] is empty or equal None!'.format(sql))
 
def delete(conn, sql, data):
     '''删除数据'''
     if sql is not None and sql != '':
         if data is not None:
             cu = get_cursor(conn)
             for d in data:
                 if SHOW_SQL:
                     print('执行sql:[{}],参数:[{}]'.format(sql, d))
                 cu.execute(sql, d)
                 conn.commit()
             close_all(conn, cu)
     else:
         print('the [{}] is empty or equal None!'.format(sql))
 ###############################################################
 ####            数据库操作CRUD     END
 ###############################################################
 
 
 ###############################################################
 ####            测试操作     START
 ###############################################################
def drop_table_test():
     '''删除数据库表测试'''
     print('删除数据库表测试...')
     conn = get_conn(DB_FILE_PATH)
     drop_table(conn, TABLE_NAME)

def save_test():
     '''保存数据测试...'''
     print('保存数据测试...')
     save_sql = '''INSERT INTO student values (?, ?, ?, ?, ?, ?)'''
     data = [(1, 'Hongten', '男', 20, '广东省广州市', '13423****62'),
             (2, 'Tom', '男', 22, '美国旧金山', '15423****63'),
             (3, 'Jake', '女', 18, '广东省广州市', '18823****87'),
             (4, 'Cate', '女', 21, '广东省广州市', '14323****32')]
     conn = get_conn(DB_FILE_PATH)
     save(conn, save_sql, data)
 
def fetchall_test():
     '''查询所有数据...'''
     print('查询所有数据...')
     fetchall_sql = '''SELECT * FROM student'''
     conn = get_conn(DB_FILE_PATH)
     fetchall(conn, fetchall_sql)
 
def fetchone_test():
     '''查询一条数据...'''
     print('查询一条数据...')
     fetchone_sql = 'SELECT * FROM student WHERE ID = ? '
     data = 1
     conn = get_conn(DB_FILE_PATH)
     fetchone(conn, fetchone_sql, data)
 
def update_test():
     '''更新数据...'''
     print('更新数据...')
     update_sql = 'UPDATE student SET name = ? WHERE ID = ? '
     data = [('HongtenAA', 1),
             ('HongtenBB', 2),
             ('HongtenCC', 3),
             ('HongtenDD', 4)]
     conn = get_conn(DB_FILE_PATH)
     update(conn, update_sql, data)
 
def delete_test():
     '''删除数据...'''
     print('删除数据...')
     delete_sql = 'DELETE FROM student WHERE NAME = ? AND ID = ? '
     data = [('HongtenAA', 1),
             ('HongtenCC', 3)]
     conn = get_conn(DB_FILE_PATH)
     delete(conn, delete_sql, data)
 
 ###############################################################
 ####            测试操作     END
 ###############################################################


def create_table_Coordinate_System():
     '''创建坐标系信息表'''
     create_table_sql = '''CREATE TABLE `COORDINATE_SYSTEM` (
                           `OBJECTID` int(11) NOT NULL,
                           `NAME` varchar(128) DEFAULT NULL,
                           `SEMIMAJORAXIS` real DEFAULT NULL,
                           `INVERSEFLATTENING` real DEFAULT NULL,
                           `SCALEFACTOR` real DEFAULT NULL,
                           `FALSE_EASTING` real DEFAULT NULL,
                           `FALSE_NORTHING` real DEFAULT NULL,
                           `CENTRALMERIDIAN` real DEFAULT NULL,
                            PRIMARY KEY (`OBJECTID`)
                         )'''
     conn = get_conn(DB_FILE_PATH)
     create_table(conn, create_table_sql)


def create_table_AffixTable():
     '''创建属性信息表'''
     create_table_sql = '''CREATE TABLE `AFFIXTABLE'''+gstrPreFix+'''` (
                           `OBJECTID` int(11) NOT NULL,
                           `NAME` varchar(128) DEFAULT NULL,
                           `TYPE` varchar(128) DEFAULT NULL,
                           `AFFIXFILEID` varchar(128) DEFAULT NULL,
                            PRIMARY KEY (`OBJECTID`)
                         )'''
     conn = get_conn(DB_FILE_PATH)
     create_table(conn, create_table_sql)


def create_table_AffixFile():
     '''创建纹理表'''
     create_table_sql = '''CREATE TABLE `AFFIXFILE'''+gstrPreFix+'''` (
                           `OBJECTID` int(11) NOT NULL,
                           `OBJGUID` varchar(128) DEFAULT NULL,
                           `CONTENT` blob DEFAULT NULL,
                            PRIMARY KEY (`OBJECTID`)
                         )'''
     conn = get_conn(DB_FILE_PATH)
     create_table(conn, create_table_sql)


def create_table_ProjectTable():
     '''创建项目表'''
     create_table_sql = '''CREATE TABLE `PROJECT` (
                           `OBJECTID` int(11) NOT NULL,
                           `NAME` varchar(128) DEFAULT NULL,
                           `PREFIXION` integer DEFAULT NULL,
                           `SOURCEID` int(11) DEFAULT -1,
                           `ZIPKEY` varchar(128) DEFAULT NULL,
                           `FOREIGNID` varchar(128) DEFAULT NULL,
                           `DESCRIBE` varchar(128) DEFAULT NULL,
                            PRIMARY KEY (`OBJECTID`)
                         )'''
     conn = get_conn(DB_FILE_PATH)
     create_table(conn, create_table_sql)


def create_table_Textures():
     '''创建纹理表'''
     create_table_sql = '''CREATE TABLE `TEXTURES'''+gstrPreFix+'''` (
                           `OBJECTID` int(11) NOT NULL,
                           `NAME` varchar(128) DEFAULT NULL,
                           `FORMAT` integer DEFAULT NULL,
                           `MAGFILTER` integer DEFAULT NULL,
                           `MINFILTER` integer DEFAULT NULL,
                           `WRAPS` integer DEFAULT NULL,
                           `WRAPT` integer DEFAULT NULL,
                           `IMAGENAME` varchar(128) DEFAULT NULL,
                           `TEXIMGID` varchar(128) DEFAULT NULL,
                            PRIMARY KEY (`OBJECTID`)
                         )'''
     conn = get_conn(DB_FILE_PATH)
     create_table(conn, create_table_sql)


def create_table_TexImg():
     '''创建纹理表'''
     create_table_sql = '''CREATE TABLE `TEXIMG'''+gstrPreFix+'''` (
                           `OBJECTID` int(11) NOT NULL,
                           `OBJGUID` varchar(128) DEFAULT NULL,
                           `CONTENT` blob DEFAULT NULL,
                            PRIMARY KEY (`OBJECTID`)
                         )'''
     conn = get_conn(DB_FILE_PATH)
     create_table(conn, create_table_sql)


def create_table_Tilesettable():
     '''创建分片集表'''
     create_table_sql = '''CREATE TABLE `TILESETTABLE'''+gstrPreFix+'''` (
                           `OBJECTID` int(11) NOT NULL,
                           `PARENT` int(11) DEFAULT NULL,
                           `MINX` real DEFAULT NULL,
                           `MINY` real DEFAULT NULL,
                           `MINZ` real DEFAULT NULL,
                           `MAXX` real DEFAULT NULL,
                           `MAXY` real DEFAULT NULL,
                           `MAXZ` real DEFAULT NULL,
                           `CENTER` varchar(128) DEFAULT NULL,
                           `LASTUPDATETIME` varchar(128) DEFAULT NULL,
                            PRIMARY KEY (`OBJECTID`)
                         )'''
     conn = get_conn(DB_FILE_PATH)
     create_table(conn, create_table_sql)


def create_table_GeoTable():
     '''创建图形信息表'''
     create_table_sql = '''CREATE TABLE `GEOTABLE'''+gstrPreFix+'''` (
                           `OBJECTID` int(11) NOT NULL,
                           `NAME` varchar(128) DEFAULT NULL,
                           `TILESETID` int(11) DEFAULT -1,
                           `MATRIX` varchar(512) DEFAULT NULL,
                           `MESHIDS` text DEFAULT NULL,
                           `BLOCKID` int(11) DEFAULT NULL,
                           `BOX` varchar(256) DEFAULT NULL,
                           `FAMILYID` int(11) DEFAULT NULL,
                            PRIMARY KEY (`OBJECTID`)
                         )'''
     conn = get_conn(DB_FILE_PATH)
     create_table(conn, create_table_sql)


def create_table_GeoBlockTable():
     '''创建实例几何对象表'''
     create_table_sql = '''CREATE TABLE `GEOBLOCK'''+gstrPreFix+'''` (
                           `OBJECTID` int(11) NOT NULL,
                           `NAME` varchar(128) DEFAULT NULL,
                           `DESCRIBE` varchar(128) DEFAULT NULL,
                           `MESHIDS` text DEFAULT NULL,
                            PRIMARY KEY (`OBJECTID`)
                         )'''
     conn = get_conn(DB_FILE_PATH)
     create_table(conn, create_table_sql)


def create_table_FamilyTable():
     '''创建属性信息表'''
     create_table_sql = '''CREATE TABLE `FAMILYTABLE'''+gstrPreFix+'''` (
                           `OBJECTID` int(11) NOT NULL,
                           `NAME` varchar(128) DEFAULT NULL,
                           `PARENTID` int(11) NOT NULL,
                           `DESCRIBE` varchar(128) DEFAULT NULL,
                            PRIMARY KEY (`OBJECTID`)
                         )'''
     conn = get_conn(DB_FILE_PATH)
     create_table(conn, create_table_sql)

def create_table_FmlItemTable():
     '''创建属性信息表'''
     create_table_sql = '''CREATE TABLE `FMYITEMTABLE'''+gstrPreFix+'''` (
                           `OBJECTID` int(11) NOT NULL,
                           `SECTION` varchar(128) DEFAULT NULL,
                           `NAME` varchar(128) DEFAULT NULL,
                           `VALUE` varchar(256) DEFAULT NULL,
                           `UNIT` varchar(128) DEFAULT NULL,
                           `TYPE` int(11) DEFAULT NULL,
                           `FAMILYID` int(11) DEFAULT NULL,
                            PRIMARY KEY (`OBJECTID`)
                         )'''
     conn = get_conn(DB_FILE_PATH)
     create_table(conn, create_table_sql)


def create_table_ProTable():
     '''创建属性信息表'''
     create_table_sql = '''CREATE TABLE `PROTABLE'''+gstrPreFix+'''` (
                           `OBJECTID` int(11) NOT NULL,
                           `SECTION` varchar(128) DEFAULT NULL,
                           `NAME` varchar(128) DEFAULT NULL,
                           `VALUE` varchar(256) DEFAULT NULL,
                           `UNIT` varchar(128) DEFAULT NULL,
                           `TYPE` int(11) DEFAULT NULL,
                           `GEOID` int(11) DEFAULT NULL,
                            PRIMARY KEY (`OBJECTID`)
                         )'''
     conn = get_conn(DB_FILE_PATH)
     create_table(conn, create_table_sql)


def create_table_MeshTable():
     '''创建网格数据表'''
     create_table_sql = '''CREATE TABLE `MESHTABLE'''+gstrPreFix+'''` (
                           `OBJECTID` int(11) NOT NULL,
                           `NAME` varchar(128) DEFAULT NULL,
                           `MATERIALID` int(11) DEFAULT NULL,
                           `ACCESSOR_POSTION` int(11) DEFAULT NULL,
                           `ACCESSOR_INDEX` int(11) DEFAULT NULL,
                           `ACCESSOR_NORMAL` int(11) DEFAULT NULL,
                           `ACCESSOR_TEXCOORD_0` int(11) DEFAULT NULL,
                           `ACCESSOR_TEXCOORD_1` int(11) DEFAULT NULL,
                           `ACCESSOR_TEXCOORD_2` int(11) DEFAULT NULL,
                           `ACCESSOR_TEXCOORD_3` int(11) DEFAULT NULL,
                           `ACCESSOR_TEXCOORD_4` int(11) DEFAULT NULL,
                           `ACCESSOR_TEXCOORD_5` int(11) DEFAULT NULL,
                           `ACCESSOR_TEXCOORD_6` int(11) DEFAULT NULL,
                           `ACCESSOR_TEXCOORD_7` int(11) DEFAULT NULL,
                            PRIMARY KEY (`OBJECTID`)
                         )'''
     conn = get_conn(DB_FILE_PATH)
     create_table(conn, create_table_sql)


def create_table_AccessorTable():
     '''创建访问器表'''
     create_table_sql = '''CREATE TABLE `ACCESSORTABLE'''+gstrPreFix+'''` (
                           `OBJECTID` int(11) NOT NULL,
                           `TYPE` varchar(128) DEFAULT NULL,
                           `BYTEOFFSET` integer DEFAULT NULL,
                           `BYTESTRIDE` integer DEFAULT NULL,
                           `COMPONENTTYPE` integer DEFAULT NULL,
                           `NCOUNT` integer DEFAULT NULL,
                           `BFILEID` varchar(128) DEFAULT NULL,
                            PRIMARY KEY (`OBJECTID`)
                         )'''
     conn = get_conn(DB_FILE_PATH)
     create_table(conn, create_table_sql)

def create_table_BFileTable():
     '''创建文件表'''
     create_table_sql = '''CREATE TABLE `BFILETABLE'''+gstrPreFix+'''` (
                           `OBJECTID` int(11) NOT NULL,
                           `OBJGUID` varchar(128) DEFAULT NULL,
                           `CONTENT` blob DEFAULT NULL,
                            PRIMARY KEY (`OBJECTID`)
                         )'''
     conn = get_conn(DB_FILE_PATH)
     create_table(conn, create_table_sql)


def create_table_MaterialTable():
     '''创建材质信息表'''
     create_table_sql = '''CREATE TABLE `MATERIALTABLE'''+gstrPreFix+'''` (
                           `OBJECTID` int(11) NOT NULL,
                           `NAME` varchar(128) DEFAULT NULL,
                           `TECHDES` varchar(128) DEFAULT NULL,
                           `AMBIENT` varchar(128) DEFAULT NULL,
                           `EMISSION` varchar(128) DEFAULT NULL,
                           `SHININESS` varchar(128) DEFAULT NULL,
                           `SPECULAR` varchar(128) DEFAULT NULL,
                           `DIFFUSE` varchar(128) DEFAULT NULL,
                           `TEXTURE_1` int(11) DEFAULT NULL,
                           `TEXTURE_2` int(11) DEFAULT NULL,
                           `TEXTURE_3` int(11) DEFAULT NULL,
                           `TEXTURE_4` int(11) DEFAULT NULL,
                            PRIMARY KEY (`OBJECTID`)
                         )'''
     conn = get_conn(DB_FILE_PATH)
     create_table(conn, create_table_sql)


def init():
     '''初始化方法'''
     #数据库表名称
     global TABLE_NAME
     TABLE_NAME = 'student'
     #是否打印sql
     global SHOW_SQL
     SHOW_SQL = False
     print('show_sql : {}'.format(SHOW_SQL))


     #向数据库表中插入数据
     create_table_Coordinate_System()
     create_table_AffixTable()
     create_table_AffixFile()
     create_table_ProjectTable()
     create_table_Textures()
     create_table_TexImg()
     create_table_Tilesettable()
     create_table_GeoTable()
     create_table_GeoBlockTable()
     create_table_FamilyTable()
     create_table_FmlItemTable()
     create_table_ProTable()
     create_table_MeshTable()
     create_table_AccessorTable()
     create_table_BFileTable()
     create_table_MaterialTable()
 
def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("filepath")
    args = parser.parse_args()
    dirpath = args.filepath

    global gstrPreFix
    str1 = dirpath;
    strArrs = str1.split(",")
    gstrPreFix = strArrs[0];
    dirpath = strArrs[1];

    # 数据库文件绝句路径
    global DB_FILE_PATH
    DB_FILE_PATH = dirpath
    init()
 
if __name__ == '__main__':
    main()
