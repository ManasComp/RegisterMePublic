# This Dockerfile contains the image specification of our database
FROM postgres:16-alpine

COPY ./certs/certificates/postgresdb.key /var/lib/postgresql
COPY ./certs/certificates/postgresdb.crt /var/lib/postgresql

COPY ./certs/certificates/myCA.crt /var/lib/postgresql
COPY ./certs/certificates/myCA.crl /var/lib/postgresql

RUN chown 0:70 /var/lib/postgresql/postgresdb.key && chmod 640 /var/lib/postgresql/postgresdb.key
RUN chown 0:70 /var/lib/postgresql/postgresdb.crt && chmod 640 /var/lib/postgresql/postgresdb.crt

RUN chown 0:70 /var/lib/postgresql/myCA.crt && chmod 640 /var/lib/postgresql/myCA.crt
RUN chown 0:70 /var/lib/postgresql/myCA.crl && chmod 640 /var/lib/postgresql/myCA.crl

ENTRYPOINT ["docker-entrypoint.sh"] 

CMD [ "-c", "ssl=on" , "-c", "ssl_cert_file=/var/lib/postgresql/postgresdb.crt", "-c",\
    "ssl_key_file=/var/lib/postgresql/postgresdb.key", "-c",\
    "ssl_ca_file=/var/lib/postgresql/myCA.crt", "-c", "ssl_crl_file=/var/lib/postgresql/myCA.crl" ]