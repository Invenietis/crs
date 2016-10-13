# Le projet CRS

## Buts et ambitions
* Être un composant minimaliste de traitement des **commandes** côté serveur.
* Fournir une API simple d'utilisation.
* Être le plus explicite possible dans son utilisation.
* Être cross-platform, CRS sur Raspberry PI n'est pas une illusion !

## Définir des commandes

Une *Command* est un object POCO standard, dont l'intitulé est au présent et évoque une action. 
Elle doit contenir sous forme de propriétés, tous les paramètres nécessaires à son bon traitement.

    public class CreateUserCommand
    {
        // The current actor identifier performing the create user action.
        public int ActorId { get;set; }
        
        // The user name to create in the system
        public string UserName { get; set; }
    }
    
Cette commande est traitée par un *CommandHandler*.

    public class CreateUserHandler : CommandHandler<CreateUserCommand>
    {
        public Task DoHandleAsync( ICommandExecutionContext context, CreateUserCommand command )
        {
            // Perform logic...
        }
    }
    
Pour CRS, une *Command* est définie par  :

| Propriété      |     Description    |   Par défaut |  Exemple 
| ------------- |: ------------- | :--------- |
| Nom     |  Nom de la commande qui sera enregistrée en tant que route pour CRS | **CommandType**.*Name* désuffixé de Command | CreateUserCommand -> CreateUser |
| Description | Description de la commande. Utile pour de la documentation. | **String**.*Empty* | |
| CommandType | Type .Net de la commande | null | **CreateUserCommand** |
| HandlerType | Type .Net du Handler | null | **CreateUserHandler** |
| Traits      | Liste de tags séparées par le séparateur par défaut du CKTraitContext de l'application (&vert;) | null | Async&vert;SQL


Pour que CRS fonctionne, il faut enregistrer ses services dans la *IServicesCollection* fournie par ASP.Net Core puis enregistrer les commandes :

    public void ConfigureServices( IServicesCollection services )
    {
        services.AddCommandReceiver( options => 
        {
           options.Registry.Register<CreateUserCommand, CreateUserHandler>(); 
        });
    }
    
 
## Comment s'en servir ?
Deux implémentations Web-host sont fournies en standard :
* OWIN pour ASP.Net 4.6
* ASP.Net Core 1.0

Un host CRS se map sur un chemin de l'URL comme ceci :

    public Configure( IAppBuilder app )
    {
        app.UseCrs( "/api/commands" ); 
    }

Toutes les reqûetes qui atteindront **/api/commands/[...]** seront routés vers ce endpoint CRS.

Plusieurs Crs peuvent cohabiter comme ceci : 

    public Configure( IAppBuilder app )
    {
        app.UseCrs( "/api/commands-v1" ); 
        app.UseCrs( "/api/commands-v2" );
    }
    
Ainsi, la commande précédement créée sera enregistrer en 