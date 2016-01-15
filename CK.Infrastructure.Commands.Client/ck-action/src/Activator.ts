export interface Activator{
    activate<T>( type: Function ): T;    
}