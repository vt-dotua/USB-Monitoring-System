from datetime import datetime
from flask import render_template, render_template, flash, redirect, url_for, request, session, g
from webapp import app
from .myforms import *
from .SQLdb import SQLdb
import psycopg2
from psycopg2.extras import DictCursor
from functools import wraps
import hashlib
from itsdangerous import URLSafeTimedSerializer, SignatureExpired
from flask_mail import Mail, Message

def formSQLqueryHostMac(form):
    vInput = {}
    if form.hostname.data:
        vInput['userpc'] = form.hostname.data.strip()
    else:
        vInput['userpc'] = None

    if form.mac.data:
        vInput['mac'] = form.mac.data.strip()
    else:
        vInput['mac'] = None
    return vInput

def fromSQLqueryUSBevent(form):
    vInput = {}
    if form.dateFrom.data:
        vInput['eventdate.>='] = form.dateFrom.data
    else:
        vInput['eventdate.>='] = None
    if form.dateTo.data:
        vInput['eventdate.<='] = form.dateTo.data
    else:
        vInput['eventdate.<='] = None
    if form.timeFrom.data:
        vInput['eventtime.>='] = form.timeFrom.data
    else:
        vInput['eventtime.>='] = None
    if form.timeTo.data:
        vInput['eventtime.<='] = form.timeTo.data
    else:
        vInput['eventtime.<='] = None
    if form.typeEvent.data != 'all':
        vInput['typeevent.='] = form.typeEvent.data.strip()
    else:
        vInput['typeevent.='] = None
    if form.vid.data:
        vInput['vid.='] = form.vid.data.strip()
    else:
        vInput['vid.='] = None
    if form.pid.data:
        vInput['pid.='] = form.pid.data.strip()
    else:
        vInput['pid.='] = None
    if form.sn.data:
        vInput['sn.='] = form.sn.data.strip()
    else:
        vInput['sn.='] = None
    print("vInput : ", vInput)
    return vInput

def login_required(f):
    @wraps(f)
    def decorated_function(*args, **kwargs):
        if not session.get('logged_in'):
            return  redirect(url_for('.login'))
        return f(*args, **kwargs)
    return decorated_function

def login_required_admin(f):
    @wraps(f)
    def decorated_function(*args, **kwargs):
        if not (session.get('logged_in') and session.get('admin')):
            return  redirect(url_for('.login'))
        return f(*args, **kwargs)
    return decorated_function

@app.route('/login', methods=['GET', 'POST'] )
def login( ):
    try:
        db = SQLdb()
    except (Exception, psycopg2.DatabaseError) as error:
        return 'Error404' + str(error)
    form = LoginForm()
    try:
        if form.validate_on_submit():
            user = form.username.data
            password = '\\x' + hashlib.sha256(form.password.data.encode('utf-8')).hexdigest()
            print(password)
            userDict = db.CheckUser(user, password)
            if(len(userDict) == 1):
                userDict = userDict[0]
                if(userDict[0] == user):
                    if(userDict[1] == password):
                        session['logged_in'] = True
                        session['username']  = user
                        session['surname'] = userDict[2]
                        session['name']    = userDict[3]
                        session['admin']   = userDict[4]
                        return redirect('/')
            else :
                error = 'Пароль або логін невірний! Спробуйте знову.'
                return render_template('login.html', form=form, error=error)
    except (Exception, psycopg2.DatabaseError) as error:
            message = "Помилка : " + str(error)
            return 'Erro404: ' + str(error)
    finally:
        if db:
            db.Close()
    return  render_template('login.html', form=form)

@app.route('/logout')
def logout():
    session['logged_in'] = False
    session.pop('logged_in', None)
    session.pop('username',  None)
    session.pop('surname',   None)
    session.pop('name',      None)
    session.pop('admin',     None)
    return redirect('/login')

s = URLSafeTimedSerializer('Thisisasecret!')
@app.route('/ForgotPassword',  methods=['GET', 'POST'])
def ForgotPassword():
   form = MyEmail()
   db = None
   if form.validate_on_submit():
        try:
            db = SQLdb()
            email = form.email.data
            if db.CheckEmail(email):
                message = "Лист для відновлення паролю надіслано!"
                token =s.dumps(email, salt='email-confirm')
                msg = Message('Відновлення паролю!', sender = 'testboss@ukr.net', recipients = [email])
                Link = url_for('confirm_email', token=token, _external=True)
                msg.body = 'Посилання для відновленя паролю: {}'.format(Link)
                mail = Mail()
                mail.send(msg)
            else:
                message = "Такої почтової скриньки не існує!"
        except Exception as inst:
            return "Error404" + str(inst)
        finally:
             if db:
                db.Close()
        return render_template('ForgotPassword.html', form = form, message =  message)
   return render_template('ForgotPassword.html', form = form)

@app.route('/confirm_email/<token>',  methods=['GET', 'POST'])
def confirm_email(token):
    form = ChangePassword()
    try:
        email = s.loads(token ,salt='email-confirm', max_age=3600)
    except :
         return '<h1>Не вдалося відновити повідомлення!</h1>'
    if form.validate_on_submit():
       password = form.password.data
       try:
            db = SQLdb()
       except:
            return "Error404"
       try:
           db.ChangePassword(email, password)
       except (Exception) as error:
           message = "Помилка : " + str(error)
           return render_template('ChangePassword.html', form = form, token=token, message = message)
       finally:
          if db:
                db.Close()
       return redirect('/login')
      
    return render_template('ChangePassword.html', form = form, token=token)

@app.route('/', methods=['GET', 'POST'])
@app.route('/home', methods=['GET', 'POST'])
@login_required
def showuserevent():
    """Renders the about page."""
    db = None
    try:
        db = SQLdb()
    except:
        return 'Erro404'
    form = HostAndMac()
    try:
        if form.validate_on_submit():
            vInput =  formSQLqueryHostMac(form)
            allUsbEvent = db.FindSpecificUserEvent(vInput)
        else:
            allUsbEvent = db.ShowAllUserEvent()
    except (Exception, psycopg2.DatabaseError) as error:
            message = "Помилка : " + str(error)
            return 'Erro404' + str(error)
    finally:
        if db:
            db.Close()
    return render_template(
        'showuserevent.html',
        allUsbEvent=allUsbEvent,
        form=form
    )

@app.route('/usbevent', methods=['GET', 'POST'])
@login_required
def usbevent():
    db  = None
    try:
        form = SearchUsbEvent()
        db = SQLdb() 
        if request.method == 'GET':
            id = int(request.args.get('id'))
            UsbEventWithId = db.FindAllUsbEventWithId(id)            
        if request.method == 'POST':
            vInput =  fromSQLqueryUSBevent(form)
            id = request.form['idpage']
            UsbEventWithId = db.FindSpecificUsbEvents(id,vInput)
    except (Exception, psycopg2.DatabaseError) as error:
            message = "Помилка : " + str(error)
            return 'Erro404: ' + str(error)
    finally:
        if db: 
            db.Close()
    return render_template(
        'usbevent.html',
        listUsbEvent = UsbEventWithId['listUsbEvent'],
        IdHostMac    = UsbEventWithId['IdHostMac'],
        id=id,
        form = form
    )

@app.route('/manageuser', methods=['GET', 'POST'])
@login_required_admin
def manageuser():
    db  = None
    messageAdd    = None
    messageDelete = None
    amountAdmin = 0
    try:
        formAdd    = RegisterForm()
        formDelete = DeleteUserForm()
        db = SQLdb()
        if formAdd.validate_on_submit():
            username = formAdd.username.data
            surname  = formAdd.surname.data
            name     = formAdd.name.data
            ochestvo = formAdd.ochestvo.data
            password = formAdd.password.data
            email    = formAdd.email.data
            if formAdd.adminOrNot.data == 'a': 
                admin = True
            else:
                admin = False
            db.SingUp(username, surname, name, ochestvo, password, email, admin)
            messageAdd = "Користувач створений!"
        if formDelete.validate_on_submit():
            d = request.form
            idToDelete = []
            for k, v in d.items():
                if len(k.split(".")) == 2:
                    idToDelete.append(v)
            if idToDelete:
                db.DeleteUser(idToDelete)
                messageDelete = "Успішно видаленно!"
        allUser = db.GetAllUser()
        for countAdmin in allUser:
            if countAdmin[6]:
                amountAdmin +=1
                
    except (Exception, psycopg2.DatabaseError) as error:
            message = "Помилка : " + str(error)
            return 'Erro404: ' + str(error)
    finally:
        if db: 
            db.Close()

    return render_template(
        'manageuser.html',
        formAdd = formAdd,
        formDelete = formDelete,
        allUser = allUser,
        messageAdd = messageAdd,
        messageDelete = messageDelete,
        amountAdmin = amountAdmin
    )
    
@app.route('/delete_event_main', methods=['GET', 'POST'])
@login_required_admin
def delete_event_main():
    db = None
    try:
        db = SQLdb()
    except:
        return 'Erro404'
    form       = HostAndMac()
    formDelete = deleteUserEventMain()
    try:
        if form.validate_on_submit():
            vInput =  formSQLqueryHostMac(form)
            allUsbEvent = db.FindSpecificUserEvent(vInput)
        else:
            allUsbEvent = db.ShowAllUserEvent()

        if formDelete.validate_on_submit():
            d = request.form
            idToDelete = []
            for k, v in d.items():
                if len(k.split(".")) == 2:
                    idToDelete.append(v)
            if idToDelete:
                db.DeleteHostEvent(idToDelete)
                allUsbEvent = db.ShowAllUserEvent()

    except (Exception, psycopg2.DatabaseError) as error:
            message = "Помилка : " + str(error)
            return 'Erro404' + str(error)
    finally:
        if db:
            db.Close()

    return render_template(
        'delete_main.html',
        allUsbEvent=allUsbEvent,
        formDelete = formDelete,
        form=form
    )

@app.route('/delete_childe', methods=['GET', 'POST'])
@login_required
def delete_childe():
    db  = None
    try:
        form = DeleteUsbEvent()
        db = SQLdb() 
        if request.method == 'GET':
            id = int(request.args.get('id'))            
        if request.method == 'POST':
            id = request.form['idpage']
            dateFrom = form.dateFrom.data
            dateTo   = form.dateTo.data
            db.DeleteHostSpecificEvent(dateFrom, dateTo, id)
        UsbEventWithId = db.FindAllUsbEventWithId(id)
    except (Exception, psycopg2.DatabaseError) as error:
            message = "Помилка : " + str(error)
            return 'Erro404: ' + str(error)
    finally:
        if db: 
            db.Close()

    return render_template(
        'delete_childe.html',
        listUsbEvent = UsbEventWithId['listUsbEvent'],
        IdHostMac    = UsbEventWithId['IdHostMac'],
        id=id,
        form = form
    )

