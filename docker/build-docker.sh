#!/usr/bin/env bash
dotnet publish ../src/QuantGrid.App/QuantGrid.App.csproj --os linux --arch x64 -c Release -p:PublishProfile=DefaultContainer