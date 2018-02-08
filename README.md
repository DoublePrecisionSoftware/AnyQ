# AnyQ

A backend-agnostic message queueing library for .NET.

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

## Usage

The core of the functionality in AnyQ is handled by the `JobQueueListener` class.
To create a `JobQueueListener` instance, you need only provide an instance of an `IJobQueueFactory`.  Some backing queue implementations (such as [AnyQ.Queues.Msmq](https://www.nuget.org/packages/AnyQ.Queues.Msmq/)) already provide one for you.

Once you have an instance of `JobQueueListener`, you can then add one or more `JobHandler` instances for handling jobs, or an `IJobHandlerLocator` instance for locating other handlers.

Upon calling `Listen()`, AnyQ will begin sending jobs to your `JobHandlers` for processing.

## API Reference

(see the [Wiki](https://github.com/DoublePrecisionSoftware/AnyQ/wiki))

## Builing AnyQ

Building AnyQ requires Visual Studio 2017.