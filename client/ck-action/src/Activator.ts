
export interface Activator{
    
    /**
     * Create a new instance of the given class by resolving its dependencies.
     * @param type The class to instanciate
     */
    activate<T>( type: Function ): T;    
}