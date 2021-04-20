FROM python:3

WORKDIR /app

ADD . /app

RUN pip install uwsgi
RUN pip install -r requirements.txt

CMD ["uwsgi", "app.ini"]