# ExampleLAN
Example for async LAN use C# and async/await

## What i used 
In branch master i used System.Net.Sockets. For avoid CallBack (Net.Sockets use CallBack for async call) i used this construction

```csharp
Task<T> Name()
{
    TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
    socket.BeginSomething(args, iar => 
        {
            socket.EndSomething(iar);
            Socket.SetResult(result);
        }, socket);
    return tcs.Task;    
}
```

But in prifio/SocketForPCL i used [sockethelpers-for-pcl](https://github.com/rdavisau/sockethelpers-for-pcl) for client.
I think, what second variant bettr than first because you can use the ready asyncCall.

##What this do
It's example async chat. You and your friend can write message and can don't wait your companion (like sync chat)
