# Kino - framework for building Actor networks


[![Build status](https://ci.appveyor.com/api/projects/status/khn5imataa5uw4oj?svg=true)](https://ci.appveyor.com/project/iiwaasnet/kino)
[![NuGet version](https://badge.fury.io/nu/kino.svg)](https://badge.fury.io/nu/kino)


Kino
------------------------
**Kino** is a simple communication framework based on *[Actor model](https://en.wikipedia.org/wiki/Actor_model)*.

Unicast, multicast and direct message addressing, delivery-to-node confirmation, callbacks (routing response message to the *resumption* actor). Rendezvous service provides actors auto-discovery and reduces amount of required configuration. Actors can be single- or multithreaded.

Supported Platforms: .NET 4.7, .NET Core 2.1

What's new
------------------------
Partner Rendezvous clusters can be configured to access actors from other networks.

It is simple!
-------------------------------------
#### Define an Actor:

```csharp
public class MyMessageProcessor : Actor
{
    [MessageHandlerDefinition(typeof (MyMessage))]
    public async Task<IActorResult> MyMessageHandler(IMessage message)
    {
        // method body
    }
}
```
#### Send a Message:

```csharp
// Just create a message you would like to send
IMessage request = Message.CreateFlowStartMessage(new MyMessage());
// Define result message you would like to receive
ICallbackPoint callbackPoint = CallbackPoint.Create<ResultMessage>();
// Now, send the message. No need to know actor address, ID or anything else!
IPromise promise = messageHub.Send(request, callbackPoint);
// Wait for result
ResultMessage result = promise.GetResponse().Result.GetPayload<ResultMessage>();
```

Tell me more!
-------------------------------------
If you are interested in the project, please, read [wiki](https://github.com/iiwaasnet/kino/wiki) for details about the framework, ask related questions or suggest features in chat! [![Join the chat at https://gitter.im/iiwaasnet/kino](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/iiwaasnet/kino?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)


* [Introduction](https://github.com/iiwaasnet/kino/wiki)
* [Messages](https://github.com/iiwaasnet/kino/wiki/Messages)
* [Actors](https://github.com/iiwaasnet/kino/wiki/Actors)
* [ActorHost](https://github.com/iiwaasnet/kino/wiki/ActorHost)
* [MessageHub](https://github.com/iiwaasnet/kino/wiki/MessageHub)
* [MessageRouter](https://github.com/iiwaasnet/kino/wiki/MessageRouter)
* [Rendezvous](https://github.com/iiwaasnet/kino/wiki/Rendezvous)
* [Configuration](https://github.com/iiwaasnet/kino/wiki/Configuration)

kino-based Projects
-------------------------------------
* [LeaseProvider Service](https://github.com/iiwaasnet/kino.LeaseProvider)
 

Powered by **[NetMQ](https://github.com/zeromq/netmq)**
