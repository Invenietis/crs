export class Command{
    
    name: string;
    properties: any;
    
    constructor(name: string, properties: {}){
        this.name = name;
        this.properties = properties;
    }
}