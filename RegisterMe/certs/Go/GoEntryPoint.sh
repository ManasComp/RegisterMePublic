#!/bin/bash

cd /certificates
set -e
certstrap init --common-name "${MY_CA}" --passphrase "${PASSWORD}"
cd out
mv * ../
cd ..
rm -rf out