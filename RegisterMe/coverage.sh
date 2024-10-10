#!/bin/bash

dotnet test --collect:"XPlat Code Coverage"

reports=$(find /Users/ondrejman/RiderProjects/RegisterMe/RegisterMe/tests -name coverage.cobertura.xml | tr '\n' ';')
echo $reports

reportgenerator -reports:$reports -targetdir:coveragereport -assemblyfilters:-WebApi\;-WebGui\;-DAL.Migrations.*\;

open -a safari coveragereport/index.htm
