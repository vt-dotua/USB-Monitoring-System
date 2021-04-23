import psycopg2
import os
from   psycopg2.extras import DictCursor
from   psycopg2.extensions import AsIs

class SQLdb(object):
    """description of class"""
    def __init__(self):
        db_name = os.getenv("DATABASE_NAME")
        db_user = os.getenv("DATABASE_USER")
        db_password = os.getenv("DATABASE_PASSWORD") 
        self.connection = psycopg2.connect(dbname=db_name, user=db_user, 
                        password=db_password, host='db', cursor_factory = DictCursor)
        self.connection.autocommit = True
        self.cursor = self.connection.cursor()

    def Close(self):
        self.cursor.close()
        self.connection.close()

    def CheckEmail(self, email):
        sql = """select 
                    email
                 from all_user
                    where email = %s 
                    """
        self.cursor.execute(sql, (email,))
        return self.cursor.fetchall()

    def ChangePassword(self, email, password):
        sql = """ update all_user 
                  set password = sha256(%s)
                  where email = %s
                  """
       
        self.cursor.execute(sql, (password, email))
        pass

    def CheckUser(self, username, password):
        sql = """select 
                    login,
                    password::varchar(256),
                    surname ,
                    name,
                    admin
                 from all_user
                    where login = %s 
                    and
                    password = %s"""
        self.cursor.execute(sql, (username , password) )
        result = self.cursor.fetchall()
        return result

    def SingUp(self, login, surname, name, ochestvo, password, email,admin):
            SQL = """
            insert into all_user(login, surname, name, ochestvo, password, email,admin)
                values(%s, %s, %s, %s, sha256(%s), %s, %s)
            """
            self.cursor.execute(SQL, (login, surname, name, ochestvo, password, email, admin))
            pass
    
    def GetAllUser(self):
        sql = """select id_user, surname ,
                        name , ochestvo,
                        login , email, admin
                        from all_user"""
        self.cursor.execute(sql)
        result = self.cursor.fetchall()
        return result
    
    def ShowAllUserEvent(self):
        sql = """ select * from(
                    select u.id, u.userpc, n.mac, count(u.id),
                    max(u.eventdate) d,(select max(eventtime) 
                    from usbevent 
            		where id = u.id and eventdate = max(u.eventdate)) as t,
                    n.ip
                    from usbevent u, net_info n
                    where n.id = u.id
                    group by u.id, u.userpc, n.mac, n.ip) as r
                    order by r.d + r.t DESC"""
        self.cursor.execute(sql)
        result = self.cursor.fetchall()
        return result

    def FindSpecificUserEvent(self, vInput):
        vString = ""
        V = []
        for k, v in vInput.items():
             if v:
                vString += "n." +str(k) + "= %s and "
                V.append (v)
             if not V:
                vString +=  " 1=1 and "
        vString = vString[:len(vString) - 4]
        sql = """ select * from(select u.id,
                         u.userpc,
                         n.mac,
                         count(u.id),
                         max(u.eventdate) as d,
                         (select max(eventtime)
                         from usbevent 
                         where id = u.id and eventdate = max(u.eventdate)) as t,
                         n.ip
                         from usbevent u, net_info n
                         where """ + vString + """
                         and n.id = u.id
                         group by u.id, u.userpc, n.mac, n.ip) as r
                         order by r.d + r.t DESC
                         """     
        self.cursor.execute(sql,(V))
        result = self.cursor.fetchall()
        return result

    def FindAllUsbEventWithId(self, id):
        result ={}
        sql_1 = """ select
                          eventdate,
                          eventtime,
                          typeevent,
                          vid,
                          pid,
                          sn
                          from usbevent
                          where id = %s
                          order by eventdate + eventtime DESC;"""
        sql_2 = """
                    select id, userpc, mac, ip 
                        from net_info
                        where id = %s
                """
        self.cursor.execute(sql_1, (id,))
        result['listUsbEvent'] = self.cursor.fetchall()
        self.cursor.execute(sql_2, (id,))
        result['IdHostMac'] = self.cursor.fetchone()
        return result

    def FindSpecificUsbEvents(self,id,vInput):
        result ={}
        vString = ""
        V = []
        for k, v in vInput.items():
             if v:
                rk = k.split('.')
                vString += " "+ rk[0] + rk[1] +  "%s and "
                V.append (v)
             if not V:
                vString +=  ""
        V.append(id)
        print(vString)
        print(V)
        sql_1 = """ select
                          eventdate,
                          eventtime,
                          typeevent,
                          vid,
                          pid,
                          sn
                          from usbevent
                          where""" + vString  + """ id = %s
                          order by eventdate + eventtime DESC;"""

        sql_2 = """
                    select id, userpc, mac, ip 
                        from net_info
                        where id = %s
                """
        self.cursor.execute(sql_1, (V))
        result['listUsbEvent'] = self.cursor.fetchall()
        self.cursor.execute(sql_2, (id,))
        result['IdHostMac'] = self.cursor.fetchone()
        return result

    def DeleteUser(self,idToDelete):
        vString = "("
        for i in idToDelete:
            vString += " %s,"
            
        vString = vString[:len(vString) - 1]
        vString += " )"
        sql = """ delete from all_user where id_user in """ + vString
        self.cursor.execute(sql, (idToDelete))
        pass

    def DeleteHostEvent(self,idToDelete):
        vString = "("
        for i in idToDelete:
            vString += " %s,"
            
        vString = vString[:len(vString) - 1]
        vString += " )"
        sql_usbevent  = """ delete from usbevent  where id in """  + vString
        sql_net =       """ delete from net_info  where id in """  + vString
        sql_userevent = """ delete from userevent where id in """  + vString 
        self.cursor.execute(sql_usbevent,  (idToDelete))
        self.cursor.execute(sql_net,       (idToDelete))
        self.cursor.execute(sql_userevent, (idToDelete))
        pass

    def DeleteHostSpecificEvent(self,DateFrom, DateTo, id):
        sql = """ delete from usbevent where eventdate >= %s and eventdate <= %s and id = %s"""
        self.cursor.execute(sql,  (DateFrom, DateTo,id))
        pass

  
        

        
        

