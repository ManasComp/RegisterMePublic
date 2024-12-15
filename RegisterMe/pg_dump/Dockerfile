FROM postgres:16

ENV POSTGRES_USER=myuser
ENV POSTGRES_PASSWORD=mypassword
ENV POSTGRES_DB=mydatabase

RUN apt-get update && apt-get install -y gzip

RUN mkdir -p /backups

WORKDIR /backups

CMD ["postgres"]