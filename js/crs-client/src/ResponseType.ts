export enum ResponseType {
    /**
     * This is a returned when validation failed on a command (Filtering step).
     */
    ValidationError = 'V',
    /**
     * This is a returnd when an error has been raised by the execution of the command, in the command handler. (Execution step).
     */
    InternalErrorResponseType = 'I',
    /**
     * This is returned when the command has successfuly been executed in a synchronous-way, and a result is directly accessible by the client.
     */
    Synchronous = 'S',
    /**
     * This is returned when the execution of the command has been deferred by the pipeline.
     */
    Asynchronous = 'A',
    /**
     * This is returned for any meta command result.
     */
    Meta = 'M'
}
