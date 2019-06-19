/**
 * The path to the metadata endpoint, appended to the CRS endpoint URL.
 */
export const MetadataPath = "__meta";

export interface MetadataOptions {
    /**
     * If true, the command list will be included in the received metadata.
     * Defaults to true.
     */
    showCommands?: boolean;

    /**
     * If true, the ambient values will be included in the received metadata.
     * Defaults to true.
     */
    showAmbientValues?: boolean;
}

export const defaultMetadataOptions: MetadataOptions = {
    showAmbientValues: true,
    showCommands: true
};
