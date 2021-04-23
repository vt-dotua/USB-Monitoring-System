create database USBApp;
\c usbapp;

CREATE TABLE all_User (
  Id_user  serial NOT NULL UNIQUE,
  Surname  varchar(40) NOT NULL, 
  Name     varchar(40) NOT NULL,
  Ochestvo varchar(40) NOT NULL, 
  password bytea NOT NULL, 
  email    varchar(100) NOT NULL UNIQUE, 
  login    varchar(50) NOT NULL UNIQUE,  
  admin    bool, 
  PRIMARY KEY (Id_user));

CREATE TABLE Net_info (
  id     int4 NOT NULL, 
  userpc varchar(255) NOT NULL, 
  ip     varchar(255) UNIQUE, 
  mac    varchar(255) UNIQUE);

CREATE TABLE USBEvent (
  userpc    varchar(255) NOT NULL, 
  id        int4 NOT NULL, 
  eventdate date NOT NULL, 
  eventtime time NOT NULL, 
  VID       varchar(255) NOT NULL, 
  PID       varchar(255) NOT NULL, 
  SN        varchar(255) NOT NULL, 
  typeevent varchar(255) NOT NULL);

CREATE TABLE UserEvent (
  userpc varchar(255) NOT NULL, 
  id     serial,
  dmac   varchar(255),  
  PRIMARY KEY (userpc, 
  id));

ALTER TABLE USBEvent ADD CONSTRAINT R_1 FOREIGN KEY (userpc, id) REFERENCES UserEvent (userpc, id);
ALTER TABLE Net_info ADD CONSTRAINT R_2 FOREIGN KEY (userpc, id) REFERENCES UserEvent (userpc, id);

--FUNCTIONS
CREATE or REPLACE FUNCTION insert_usb_event(
 _userpc varchar,
 _dmac varchar,
 _mac varchar,
 _ip varchar,
 _eventdate date,
 _eventtime time,
 _VID varchar,
 _PID varchar,
 _SN varchar,
 _typeevent varchar)
returns int as
$$
declare
userid int;
begin
    IF( select count(*) from UserEvent where userpc = _userpc and dmac = _dmac) = 0
    THEN
       insert INTO UserEvent(userpc, dmac)
            values(_userpc, _dmac);
       select id into userid from userevent u 
            where u.userpc = _userpc and u.dmac = _dmac;
       insert INTO USBEvent(id, userpc, eventdate , eventtime, VID, PID, SN, typeevent) 
            values(userid, _userpc, _eventdate  , _eventtime, _VID, _PID, _SN, _typeevent);
       insert INTO net_info(id, userpc,ip, mac) 
            values(userid, _userpc, _ip, _mac);
       if found THEN
            return 1;
       else return 0;
       end if; 
	ELSE
        select id into userid from userevent u 
            where u.userpc = _userpc and u.dmac = _dmac;
        insert INTO USBEvent(id, userpc, eventdate , eventtime, VID, PID, SN, typeevent) 
            values(userid, _userpc, _eventdate  , _eventtime, _VID, _PID, _SN, _typeevent);
        UPDATE net_info
        SET mac = _mac, ip = _ip
        WHERE id = userid; 
        if found THEN
            return 1;
        else return 0;
        end if;
    END IF;
end 
$$
language plpgsql;

select * from insert_usb_event (
     'ExamplePC',
     'C85B76F76777',
     'C85B76F76777',
     '127.0.0.1',
     '2020-04-21',
     '16:32:25',
     '1001',
     '7002',
     '15T2V7G8I9P0XXKK',
     'connected');

insert into all_user(login, surname, name, ochestvo, password, email,admin)
                values('ADMIN_USER', 'ADMIN_NAME', 'ADMIN_SURNAME', '___', sha256('12345'), 'example@example.com', true);