# Welcome to AnyQ

AnyQ is a backend-agnostic message queueing library for .NET.

[![Build status](https://ci.appveyor.com/api/projects/status/e8ioxnx8q26br0uh?svg=true)](https://ci.appveyor.com/project/nibblesnbits/anyq)
[![Coverity status](https://scan.coverity.com/projects/15047/badge.svg)](https://scan.coverity.com/projects/doubleprecisionsoftware-anyq)
[![NuGet version](https://img.shields.io/nuget/v/AnyQ.svg)](https://www.nuget.org/packages/AnyQ/)

## Summary

AnyQ is designed to handle the internals of dealing with Message Queueing, leaving developers able to focus on the actual work to be performed.

## Features

- No external dependencies (except those required by the backing queue).
- Supports any message queue supported by .NET
- Send a job to a queue in 3 lines of code.
- Syncronous or asyncronous execution of jobs.
- Highly extensible
- Event-driven model

## Demo

A simple implemention demo using MSMQ as the backing queue can be found [here](https://github.com/nibblesnbits/AnyQDemo).

## Navigating the Documenation

Please use the left hand navigation to get around. I would suggest taking a look at the [Getting Started Guide](./getting-started) first.