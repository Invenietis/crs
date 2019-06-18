# CK-Crs

A .NET command execution library, with support for ASP.NET Core (as server) and Typescript (as client).

## Goals and ambitions

* Minimalist server-side component handling **commands**
* Straightforward, simple API
* Explicit as possible in its use

## Core concepts

### Commands

A *command* is an instance of a C# command class (eg. `var myCommand = new CreateUserCommand();`.

If the command finishes with a result (sent back to the caller), this class should implement `ICommand<TResult>`.
Otherwise, this command class does not require any particular interface.

```csharp
public class CreateUserCommand
{
    // The ID of the actor responsible for the Command
    public int ActorId { get;set; }

    // The user name to create in the system
    public string UserName { get; set; }
}
```

This command is handled by a *CommandHandler*.

### Command handlers

After a command has been received by CRS, it it processed by a *command handler*.

This command handler is a class implementing `ICommandHandler<TCommand, TResult>` - or `ICommandHandler<TCommand>` if the command does not have a result.

Command handlers are transient, created every time a command is executed (like ASP.NET Core Controllers), and can implement multiple different commands.

```csharp
public class CreateUserHandler : ICommandHandler<CreateUserCommand>
{
    public Task HandleAsync(
        CreateUserCommand command,
        ICommandContext context
    )
    {
        // Create your user here
    }
}
```

A *Command* has additional metadata properties:

| Property      |     Description    |   Default |  Sample value |
|---|---|---|---|
| Name          |  Command name used for routing | `<namespace>.<commandtypename>` w/o `Command` suffix | `my.namespace.createuser` |
| Description | Command description (for display or documentation) | *(empty string)* | |
| CommandType | Full name of the Command's .NET Type | `null` | `My.Namespace.CreateUserCommand` |
| HandlerType | Full name of the CommandHandler's .NET Type | null | `My.Namespace.CreateUserHandler` |
| Traits      | CKTrait tag list, separated with the default CKTraitContext separator (`|`) | null | `FireForget|SQL` |

## Quick start

### Install Crs with SignalR

* Create or open an ASP.NET Core 2.2 project
* Add the following NuGet packages:
  * CK.Crs.AspNetCore
  * CK.Crs.CommandDiscoverer
  * CK.Crs.Dispatcher
  * CK.Crs.InMemory
  * CK.Crs.SignalR
* In your `Startup.cs`, add the following services and middlewares in `ConfigureServices()`:

```csharp
    public class Startup
    {
        public void ConfigureServices( IServiceCollection services )
        {
            // Note: CORS is required for JS clients. Make sure you have a AddCors() somewhere in your services.
            services.AddCors( /* (...) */ );

            // Note: When using SignalR, make sure you also have AddSignalR().
            services.AddSignalR();

            // These add AmbientValues and CRS services in DI.
            services.AddAmbientValues( ( registration ) => { } );
            services.AddCrsCore( ( registry ) =>
                {
                    // Register your commands/command handlers here.
                    // To register all command handlers/commands from an entire assembly:
                    registry.RegisterHandlers( typeof( CreateUserHandler ).Assembly );
                } )
                .AddBackgroundCommandJobHostedService()
                .AddDispatcher()
                .AddInMemoryReceiver()
                .AddSignalR( ( opts ) =>
                {
                    // The CRS SignalR Hub will be mapped to this URL.
                    opts.CrsHubPath = "/hubs/crs";
                } );
        }

        public void Configure( IApplicationBuilder app )
        {
            // Note: CORS is required for JS clients. Make sure you have a UseCors() somewhere in your pipeline.
            app.UseCors();

            // This adds the CRS API endpoint to the pipeline.
            app.UseCrs( "/api/crs" );

            // Note: The CRS SignalR hub is added at the end of the pipeline, and requires another URL at this point.
        }
    }
```

### Create a command and a command handler

Create a class representing your command:

```csharp
    [CommandFireAndForget]
    [CommandName( "MyDeferredCommand" )]
    public class MyDeferredCommand : ICommand<string>
    {
    }
```

Create a class implementing ICommandHandler:

```csharp
    public class CommandHandler : ICommandHandler<MyDeferredCommand, string>
    {
        public Task<string> HandleAsync( MyDeferredCommand command, ICommandContext context )
        {
            return Task.FromResult( "OK" );
        }
    }
```

Your command *and* command handler are automatically registered if you provide the handler's Assembly in `AddCrsCore()` (`Startup.cs`):

```csharp
    services.AddCrsCore( ( registry ) =>
        {
            registry.RegisterHandlers( typeof( CommandHandler ).Assembly );
        } )
```

Alternatively, you can register it by hand:

```csharp
    services.AddCrsCore( ( registry ) =>
        {
            registry.Register<MyDeferredCommand, string, CommandHandler>();
        } )
```

### Send commands using Javascript

First off, NPM packages are available:
```bash
npm i @signature/crs-client
npm i @signature/crs-client-signalr
```

If you use vanilla JavaScript, an example is available in `js/samples/client-vanilla-js`.

If you're using TypeScript, you can create a class for each of your command types and send it with Promises:

```ts
import { Command, CrsEndpoint, CrsEndpointConfiguration } from '@signature/crs-client';
import { SignalrResponseReceiver } from '@signature/crs-client-signalr';

@Command("MyDeferredCommand")
export class MyDeferredCommand {
}

const endpointConfig: CrsEndpointConfiguration = {
    url: ' http://localhost:5000/api/crs',
    responseReceivers: [
        new SignalrResponseReceiver(
            ' http://localhost:5000/hubs/crs'
        )
    ]
};

const endpoint = new CrsEndpoint(endpointConfig);

async function initializeCrs() {
    // Get metadata (and connect to SignalR with crs-client-signalr)
    let metadata = await endpoint.initialize();

    // Create command
    const command = new MyDeferredCommand();

    // Send command
    const commandResult = await endpoint.send<string>(command);

    // If server metadata change for some reason (eg. new ambient values after re-authentication),
    // you can reload the command metadata.
    metadata = await endpoint.reloadMetadata();

    // If you send any commands using endpoint.send(), CRS will wait until metadata is available
    // before actually sending it.
}

initializeCrs();

```

## CRS Services Configuration

You need to register CRS services into the *IServicesCollection* provided by ASP.Net Core.

First step is to register the **CommandReceiver**.
The **CommandReceiver** will receive all the commands and dispatch them to the rigth CRS Endpoint.
**Commands** need to be registered in this **CommandReceiver** and all commands definition are global to the application.
Registration can bed either with the fluent API or by auto discovering them using the *AutoRegisterSimple* helper.

Last step is to register the **CommandExecutor** services.
The **CommandExecutor** is responsible to... execute the Commands!

```csharp
public void ConfigureServices( IServicesCollection services )
{
    services.AddCommandReceiver( options =>
    {
        options.Registry.Register<CreateUserCommand, CreateUserHandler>();
        o.Registry.AutoRegisterSimple(
            assemblies: new[] { "MyAssembly1", "MyAssembly2" } );
    });
    services.AddCommandExecutor();
}
```

## CRS Endpoint Configuration

A CRS endpoint is mapped on a request path:

```csharp
public Configure( IAppBuilder app, IServiceProvider applicationServices )
{
    app.UseCrs( "/api/commands", applicationServices );
}
```

On the above example, every requests starting with **/api/commands/[...]** will be routed to this CRS endpoint.

Several CRS endpoint could be defined:

```csharp
public Configure( IAppBuilder app, IServiceProvider applicationServices )
{
    app.UseCrs( "/api/commands-v1", applicationServices );
    app.UseCrs( "/api/commands-v2", applicationServices );
}
```

You can hook up the endpoint configuration to change the commands used by an endpoint, and defines specific command description at the endpoint level:

```csharp
app.UseCrs( "/api/target/commands/users", applicationServices, c =>
{
    // Only adds Users related commands to this endpoint and mark them as Async
    // An async command will be handled by a background worker.
    c.AddCommand<CreateUserCommand>().IsAsync();
    c.AddCommand<DeleteUserCommand>().IsAsync();

    // Adds a global filter to every commands received by this endpoint.
    c.AddFilter<AuthorizationFilter>()
} );
```

## CRS Components

Internaly, CRS command processing is controlled by a pipeline of components. **PipelineComponent** are invoked by the CRS endpoint in the order they are configured.
It is the responsibility of a **PipelineComponent** to decide wether it is should be invoked or not, regarding the current **IPipeline** state.

You can customizes the pipeline configuration and add, remove or change the order of **PipelineComponent**:

```csharp
app.UseCrs( "/api/target/commands/users", applicationServices, c =>
{
    // Customizes the CRS Pipeline
    c.Pipeline.Clear()
        .UseMetaComponent()
        .UseCommandRouter()
        .UseJsonCommandBuilder()
        // The ambient values validator is removed from the pipeline
        // because we use a global AuthorizationFilter which is enough to
        // guarantee security of commands.
        // .UseAmbientValuesValidator()
        .UseFilters()
        // This is the customization.
        // Only adds the TaskBased executor injto the pipeline,
        // which is responsible to execute async commands
        .UseTaskBasedCommandExecutor()
        .UseJsonCommandWriter()

        // Inline component
        .Use( pipeline =>
        {
            Console.WriteLine( pipeline.Request.Path );
        });
} );
```

## CRS External Components

If you wish to uses CRS external components, you must provides implementations:

* A command scheduler that should implement **IOperationExecutor&lt;ScheduledCommand&gt;**
* An event publisher that should implement **IOperationExecutor&lt;Event&gt;**
* A command response dispatcher that should implement **ICommandResponseDispatcher**

```csharp
app.UseCrs( "/api/target/commands/users", applicationServices, c =>
{
    c.ExternalComponents.CommandSheduler = new MySchedulerImplementation();
    c.ExternalComponents.EventPublisher = new MyPublisherImplementation();
    c.ExternalComponents.ResponseDispatcher
        = new CK.Crs.SignalR.CrsCommandResponseDispatcher();
} );
```

## TO-DO

* `js/crs-client-signalr`: The `axios` dependency from `js/crs-client` is copied in `package.json` and `webpack.config.ts` until <https://npm.community/t/6-8-0-npm-ci-fails-with-local-dependency/5385/3> is fixed.
