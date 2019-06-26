import { AxiosInstance } from "axios";
import { ResponseReceiver } from "./ResponseReceiver";
import { MetadataOptions } from "./MetadataOptions";
import { IResponseFormatter } from "./EndpointMetadata";

export interface CrsEndpointConfiguration {
    /**
     * The endpoint URL. Mandatory.
     */
    url: string;

    /**
     * The axios instance used by the command emitter. Optional.
     * If not provided, the global axios instance will be used.
     */
    axiosInstance?: AxiosInstance;

    /**
     * Options for the retrived endpoint metadata. Optional.
     * If not provided, both commands and ambient values will be retrieved.
     */
    metadataOptions?: MetadataOptions;

    /*
     * The command receivers used to process command responses (ie. deferred responses). Optional.
     * If not provided, the default command receivers will be used. Note that this may or may not support deferred responses.
     */
    responseReceivers?: ResponseReceiver[];

    /*
     * A custom response formatter. Optional.
     * You can provide your custom response formatter (eg. PascalCaseResponseFormatter) if
     * you change how response formatting is implemented server-side.
     * If not provided, the DefaultResponseFormatter will be used, and will receive response as-is (assumed in camelCase).
     */
    responseFormatter?: IResponseFormatter;
}
