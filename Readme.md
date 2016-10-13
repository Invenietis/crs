# CRS Project

## Goals and ambitions
* A minimalist server-side component of **commands** handling.
* A straightforward, simple API.
* Explicit as possible in its use.
* Cross-platform

## What are a Command and a Handler ?

A *Command* is a standard POCO, whose title is set in the present tense and evokes an action. 
It must contains, as properties, every parameters needed for its processing:

    public class CreateUserCommand
    {
        // The current actor identifier performing the create user action.
        public int ActorId { get;set; }
        
        // The user name to create in the system
        public string UserName { get; set; }
    }
    
This command is handled by a *CommandHandler*.

    public class CreateUserHandler : CommandHandler<CreateUserCommand>
    {
        public Task DoHandleAsync( 
            ICommandExecutionContext context, 
            CreateUserCommand command )
        {
            // Perform logic...
        }
    }
    
Internaly, for CRS, a *Command* is defined by the following properties:

| Property      |     Description    |   Default |  Sample value 
| ------------- |: ------------- | :--------- |
| Name     |  Command name used for routing | **CommandType**.*Name* (w/o Command suffix, if any)| CreateUserCommand -> CreateUser |
| Description | Command description. Useful for documentation. | **String**.*Empty* | |
| CommandType | Command .Net Type | null | **CreateUserCommand** |
| HandlerType | Handler .Net Type | null | **CreateUserHandler** |
| Traits      | Tag list separated with the default CKTraitContext separator (&vert;) | null | Async&vert;SQL

## CRS Services Configuration
You need to register CRS services into the *IServicesCollection* provided by ASP.Net Core.

First step is to register the **CommandReceiver**.
The **CommandReceiver** will receive all the commands and dispatch them to the rigth CRS Endpoint. 
**Commands** need to be registered in this **CommandReceiver** and all commands definition are global to the application.
Registration can bed either with the fluent API or by auto discovering them using the *AutoRegisterSimple* helper.

Last step is to register the **CommandExecutor** services.
The **CommandExecutor** is responsible to... execute the Commands!

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
    
 
## CRS Endpoint Configuration
* OWIN implementation is shipping in  (Net 4.6). 
* ASP.Net Core implementation should be available quickly.

A CRS endpoint is mapped on a request path:

    public Configure( IAppBuilder app, IServiceProvider applicationServices )
    {
        app.UseCrs( "/api/commands", applicationServices ); 
    }

On the above example, every requests starting with **/api/commands/[...]** will be routed to this CRS endpoint.

Several CRS endpoint could be defined: 

    public Configure( IAppBuilder app, IServiceProvider applicationServices )
    {
        app.UseCrs( "/api/commands-v1", applicationServices ); 
        app.UseCrs( "/api/commands-v2", applicationServices );
    }

You can hook up the endpoint configuration to change the commands used by an endpoint, and defines specific command description at the endpoint level:

    app.UseCrs( "/api/target/commands/users", applicationServices, c =>
    {
        // Only adds Users related commands to this endpoint and mark them as Async
        // An async command will be handled by a background worker.
        c.AddCommand<CreateUserCommand>().IsAsync();
        c.AddCommand<DeleteUserCommand>().IsAsync();

        // Adds a global filter to every commands received by this endpoint.
        c.AddFilter<AuthorizationFilter>()
    } );


## CRS Components

Internaly, CRS command processing is controlled by a pipeline of components. **PipelineComponent** are invoked by the CRS endpoint in the order they are configured.
It is the responsibility of a **PipelineComponent** to decide wether it is should be invoked or not, regarding the current **IPipeline** state. 

You can customizes the pipeline configuration and add, remove or change the order of **PipelineComponent**:

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

## CRS External Components

If you wish to uses CRS external components, you must provides implementations:
* A command scheduler that should implement **IOperationExecutor<ScheduledCommand>**
* An event publisher that should implement **IOperationExecutor<Event>**
* A command response dispatcher that should implement **ICommandResponseDispatcher**


    app.UseCrs( "/api/target/commands/users", applicationServices, c =>
    {
        c.ExternalComponents.CommandSheduler = new MySchedulerImplementation();
        c.ExternalComponents.EventPublisher = new MyPublisherImplementation();
        c.ExternalComponents.ResponseDispatcher 
            = new CK.Crs.SignalR.CrsCommandResponseDispatcher();     
    } );