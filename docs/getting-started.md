# Basic Implementation

AnyQ is designed to use in .NET 4.5 and above projects.

## Import the Packages

To use AnyQ in your project, install a [queue implementation](./impl-libs.md) of your choice.  In this example we will be using [AnyQ.Queues.Msmq](https://github.com/DoublePrecisionSoftware/AnyQ.Queues.Msmq).  We will also be using a pre-existing [serializer](./serialization.md).

```powershell
Install-Package AnyQ.Queues.Msmq
Install-Package AnyQ.Formatters.Json
```

## Initialize the Components

In order to set up communication with a message queue, we need to first initialize some components so that AnyQ knows how to properly send and receive messages.

### Creating the Listener

First, we need a payload formatter and a job request serializer.

```cs
// This tells AnyQ how to format the data when inserting it into the queue
var payloadFormatter = new JsonPayloadFormatter();
// This tells AnyQ how to deserialize the data coming back from the queue
var requestSerializer = new JsonRequestSerializer();
```

Now we can provide those dependencies to the `IJobQueueFactory` implementation.

```cs
// AnyQ.Queues.Msmq provides one that interacts with MSMQ for us.
var jobQueueFactory = new AnyQ.Queues.Msmq.MsmqJobQueueFactory(payloadFormatter, requestSerializer);
```

Finally, we can initialize our `JobQueueListener` with the newly created `IJobQueueFactory` implementation.

```cs
var listener = new JobQueueListener(jobQueueFactory);
```

The `JobQueueListener` is now ready to receive messages on the queue, but doesn't what queue to look for those messages on.  That is handled by `JobHandler`s.

### Creating the Job Handler

Job Handlers are where the bulk of the code you write will reside, and are covered in more detail [here](./job-handlers.md).  For now, we will create a simple handler that waits 5 seconds, then outputs the `Payload` passed to it to the console.

```cs
public class Payload {
    public string Message { get; set; }
}

public class ConsoleJobHandler : JobHandler {
    private readonly IPayloadFormatter _payloadFormatter;
    private readonly HandlerConfiguration _handlerConfiguration;

    public ConsoleJobHandler(IPayloadFormatter payloadFormatter, HandlerConfiguration handlerConfiguration) {
        _payloadFormatter = payloadFormatter;
        _handlerConfiguration = handlerConfiguration;
    }

    public override HandlerConfiguration Configuration => _handlerConfiguration;

    public override bool CanProcess(ProcessingRequest request) {
        return request.JobRequest.Type == "console";
    }

    public override async Task ProcessAsync(ProcessingRequest request, CancellationToken cancellationToken) {
        var payload = _payloadFormatter.Read<Payload>(request.JobRequest.Payload);
        Console.WriteLine("Waiting 5 seconds...");
        await Task.Delay(5000);
        Console.WriteLine(payload.Message);
        OnProcessingCompleted(request);  // make sure to call OnProcessingCompleted()
    }
}
```

We can now feed this handler to the listener, specifying the queue the messages will arrive on.

```cs
const string queueId = @".\private$\test";
var handler = new ConsoleJobHandler(payloadFormatter, new AnyQ.Jobs.HandlerConfiguration {
    QueueId = queueId,
    QueueName = "Console Queue"
})

listener.AddHandler(handler);
```

Calling `AddHandler()` on our listener causes it to contact the backing queue and initialize (or update) the queue using the configuration specified in the `HandlerConfiguration` exposed by the `JobHandler`.

Finally, we simply call `Listen()` on the listener to have it start receiving messages.

```cs
listener.Listen();
```

We now have everything we need to send jobs to our queue to be processed.

## Sending Jobs to the Queue

In order to send a job to a queue, we call `SendJob()` on the listener, passing in the identifier for the queue the job will go to, a "type" for the job, a data payload, and a human-readable label.

```cs
listener.SendJob(queueId, "console", new Payload {
    Message = "Hello, AnyQ!"
}, "Test Job 1");
```

Now that we have our listener already listening, the job will be immediately picked up and processed.

## Next Steps

You are now ready to begin writing real [Job Handlers](./job-handlers.md).