from datetime import datetime, date
from flask_wtf import FlaskForm
from wtforms import StringField, PasswordField, BooleanField, SubmitField, RadioField
from wtforms.fields.html5 import DateTimeLocalField, DateField,EmailField,TimeField 
from wtforms.validators import DataRequired,InputRequired, Email, Length
from wtforms_components import DateRange

class HostAndMac(FlaskForm):
    hostname = StringField("Ім'я юзера")
    mac      = StringField('Mac адреса')
    submit   = SubmitField('Знайти')

class SearchUsbEvent(FlaskForm):
     dateFrom  = DateField("Дата з",  format='%Y-%m-%d')
     dateTo    = DateField("Дата по", format='%Y-%m-%d')
     timeFrom  = TimeField("Час з")
     timeTo    = TimeField("Час до")
     typeEvent = RadioField("Тип події", choices = [('connected', "Під'єднання"),('disconnected',"Від'єднання"),('all','Всі')], default='all')
     vid = StringField("vid")
     pid = StringField('pid')
     sn  = StringField('sn')

class LoginForm(FlaskForm):
    username = StringField("І'мя користувача", validators = [InputRequired(), Length(min=4, max=15)])
    password = PasswordField('Пароль', validators=[InputRequired(), Length(min=4, max=50)])

class RegisterForm(FlaskForm):
    username = StringField("І'мя користувача", validators = [InputRequired(), Length(min=4, max=15)])
    email = EmailField('Email', validators=[InputRequired(), Email()])
    surname = StringField('Призвище', validators = [InputRequired(), Length(min=4, max=15)])
    name = StringField("І'мя", validators = [InputRequired(), Length(min=4, max=15)])
    ochestvo = StringField("Ім'я по батькові", validators = [InputRequired(), Length(min=4, max=15)])
    password = PasswordField('Пароль', validators=[InputRequired(), Length(min=4, max=50)])
    adminOrNot = RadioField("Тип події", choices = [('a', "Admin"),('d',"User")], default='d')
    submit = SubmitField('Додати')
    
class DeleteUserForm(FlaskForm):
    submit = SubmitField('Видалити')

class deleteUserEventMain(FlaskForm):
    submit = SubmitField('Видалити')

class MyEmail(FlaskForm):
    email = EmailField('Email', validators=[InputRequired(), Email()])

class ChangePassword(FlaskForm):
      password = PasswordField('Введіть новий пароль', validators=[InputRequired(), Length(min=4, max=50)])

class DeleteUsbEvent(FlaskForm):
      dateFrom  = DateField("Дата з",  format='%Y-%m-%d')
      dateTo    = DateField("Дата по", format='%Y-%m-%d')
      submit = SubmitField('Видалити')
